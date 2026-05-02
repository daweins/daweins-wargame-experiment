using Wargame.Core;

(BattleState State, IReadOnlyList<string> Transcript)? aiPlayerDemo = null;

var checks = new List<(string Name, Action Check)>
{
    ("movement range respects terrain and blockers", CheckMovementRange),
    ("terrain changes combat forecast", CheckTerrainForecast),
    ("expanded mission has infantry choices", CheckExpandedMission),
    ("scout rescue updates objective state", CheckScoutRescue),
    ("opening end turn does not defeat player", CheckOpeningEndTurn),
    ("hq capture causes defeat", CheckHqDefeat),
    ("replay hash is deterministic", CheckReplayHash),
    ("enemy ai advances mission pressure", CheckEnemyPressure),
    ("ai player wins first mission", CheckAiPlayerWinsFirstMission),
    ("score reports all categories", CheckScore)
};

foreach (var check in checks)
{
    check.Check();
    Console.WriteLine($"PASS {check.Name}");
}

Console.WriteLine($"{checks.Count} smoke checks passed.");
Console.WriteLine();
Console.WriteLine("AI vs AI first mission winning replay:");
foreach (var line in GetAiPlayerDemo().Transcript)
{
    Console.WriteLine(line);
}

void CheckAiPlayerWinsFirstMission()
{
    var state = GetAiPlayerDemo().State;

    AssertEqual(BattleOutcome.PlayerVictory, state.Outcome, "AI player should win the first mission.");
    AssertTrue(state.ScoutRescued, "AI player should rescue Scout-7 before winning.");
    AssertTrue(state.Turn <= 8, "AI player should win within the prototype target turn range.");
}

static void CheckEnemyPressure()
{
    var state = FirstMissionFactory.Create();
    var startingDistances = state.Units
        .Where(unit => unit.Team == Team.Enemy)
        .Sum(unit => unit.Position.DistanceTo(state.PlayerHq));

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    var endingDistances = state.Units
        .Where(unit => unit.Team == Team.Enemy && unit.IsAlive)
        .Sum(unit => unit.Position.DistanceTo(state.PlayerHq));

    AssertTrue(endingDistances < startingDistances, "Enemy AI did not increase HQ pressure.");
}

static void CheckExpandedMission()
{
    var state = FirstMissionFactory.Create();
    var playerInfantry = state.Units.Count(unit => unit.Team == Team.Player && unit.Type == UnitType.Infantry);
    var enemyUnits = state.Units.Count(unit => unit.Team == Team.Enemy);

    AssertTrue(playerInfantry >= 2, "Expanded mission should give the player more than one infantry decision.");
    AssertTrue(enemyUnits >= 5, "Expanded mission should create a longer fight than the first tiny proof map.");
    AssertEqual(enemyUnits, state.InitialEnemyCount, "Initial enemy count should match the expanded roster.");
}

static void CheckHqDefeat()
{
    var state = FirstMissionFactory.Create();
    var enemy = state.Units.First(unit => unit.Team == Team.Enemy);
    enemy.Position = state.PlayerHq;

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    AssertEqual(BattleOutcome.PlayerDefeat, state.Outcome, "Enemy on HQ should defeat the player.");
}

static void CheckMovementRange()
{
    var state = FirstMissionFactory.Create();
    var infantry = state.Units.First(unit => unit.Id == "Infantry-1");
    var reachableTiles = BattleRules.GetReachableTiles(state, infantry);

    AssertTrue(reachableTiles.Contains(new TileCoord(5, 3)), "Road chokepoint should be reachable.");
    AssertTrue(!reachableTiles.Contains(new TileCoord(0, 0)), "Ridge should be impassable.");
    AssertTrue(!reachableTiles.Contains(new TileCoord(2, 4)), "Occupied allied tile should block movement.");
}

static void CheckOpeningEndTurn()
{
    var state = FirstMissionFactory.Create();

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    AssertEqual(BattleOutcome.InProgress, state.Outcome, "Ending the first turn before acting should warn, not instantly lose.");
    AssertTrue(state.Units.First(unit => unit.Id == state.ScoutId).IsAlive, "Scout-7 should survive the first enemy pressure phase.");
}

static void CheckReplayHash()
{
    var first = PlayOpening();
    var second = PlayOpening();

    AssertEqual(BattleRules.GetStateHash(first), BattleRules.GetStateHash(second), "Same command stream should hash identically.");
}

static void CheckScore()
{
    var state = FirstMissionFactory.Create();
    state.ScoutRescued = true;
    state.EnemyLosses = 2;
    state.Outcome = BattleOutcome.PlayerVictory;

    var score = BattleRules.CalculateScore(state);
    AssertTrue(score.Objective > 0, "Objective score should be present.");
    AssertTrue(score.Speed > 0, "Speed score should be present.");
    AssertTrue(score.Technique > 0, "Technique score should be present.");
    AssertTrue(score.Power > 0, "Power score should be present.");
    AssertTrue(score.Total == score.Objective + score.Speed + score.Technique + score.Power, "Total should sum categories.");
}

static void CheckScoutRescue()
{
    var state = FirstMissionFactory.Create();

    BattleRules.ApplyCommand(state, BattleCommand.Move("Infantry-1", new TileCoord(5, 3)));

    AssertTrue(state.ScoutRescued, "Moving adjacent to the scout should rescue them.");
}

static void CheckTerrainForecast()
{
    var state = FirstMissionFactory.Create();
    var attacker = state.Units.First(unit => unit.Id == "Armor-1");
    var defender = state.Units.First(unit => unit.Id == "Raider-A");
    defender.Hp = 20;
    defender.Position = new TileCoord(6, 5);
    var coverForecast = BattleRules.GetCombatForecast(state, attacker, defender);

    defender.Position = new TileCoord(6, 4);
    var plainForecast = BattleRules.GetCombatForecast(state, attacker, defender);

    AssertTrue(plainForecast.ExpectedDamage > coverForecast.ExpectedDamage, "Cover should reduce expected damage.");
}

static BattleState PlayOpening()
{
    var state = FirstMissionFactory.Create();
    BattleRules.ApplyCommand(state, BattleCommand.Move("Infantry-1", new TileCoord(5, 3)));
    BattleRules.ApplyCommand(state, BattleCommand.Wait("Infantry-1"));
    BattleRules.ApplyCommand(state, BattleCommand.Move("Infantry-2", new TileCoord(4, 4)));
    BattleRules.ApplyCommand(state, BattleCommand.Wait("Infantry-2"));
    BattleRules.ApplyCommand(state, BattleCommand.Move("Armor-1", new TileCoord(6, 5)));
    BattleRules.ApplyCommand(state, BattleCommand.Wait("Armor-1"));
    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());
    return state;
}

 (BattleState State, IReadOnlyList<string> Transcript) GetAiPlayerDemo()
{
    aiPlayerDemo ??= RunAiPlayerDemo();
    return aiPlayerDemo.Value;
}

static (BattleState State, IReadOnlyList<string> Transcript) RunAiPlayerDemo()
{
    var state = FirstMissionFactory.Create();
    var transcript = new List<string>();
    for (var safety = 0; safety < 24 && state.Outcome == BattleOutcome.InProgress; safety++)
    {
        transcript.Add($"Turn {state.Turn} player phase: {SummarizeUnits(state)}");

        PlayAiPlayerTurn(state, transcript);
        if (state.Outcome != BattleOutcome.InProgress)
        {
            break;
        }

        var result = BattleRules.ApplyCommand(state, BattleCommand.EndTurn());
        transcript.Add($"  END TURN: {result.Message}");
        transcript.Add($"  After enemy phase: {SummarizeUnits(state)}");
    }

    var score = BattleRules.CalculateScore(state);
    transcript.Add($"Outcome: {state.Outcome} on turn {state.Turn}. Score {score.Total} (Obj {score.Objective}, Speed {score.Speed}, Technique {score.Technique}, Power {score.Power}).");

    return (state, transcript);
}

static void PlayAiPlayerTurn(BattleState state, List<string> transcript)
{
    var turnPlan = ChooseBestAiTurnPlan(state);
    foreach (var command in turnPlan)
    {
        var result = BattleRules.ApplyCommand(state, command);
        transcript.Add($"  {DescribeCommand(command)} -> {result.Message}");
    }
}

static IReadOnlyList<BattleCommand> ChooseBestAiTurnPlan(BattleState state) => BuildTurnPlans(state)
    .OrderByDescending(plan => ScoreTurnPlan(state, plan))
    .ThenBy(plan => plan.Count)
    .FirstOrDefault([]);

static IEnumerable<IReadOnlyList<BattleCommand>> BuildTurnPlans(BattleState state)
{
    if (state.Outcome != BattleOutcome.InProgress)
    {
        yield return [];
        yield break;
    }

    var unit = ReadyPlayerUnits(state).FirstOrDefault();
    if (unit is null)
    {
        yield return [];
        yield break;
    }

    var unitPlans = BuildUnitPlans(state, unit)
        .OrderByDescending(plan => ScorePlan(state, plan))
        .Take(12)
        .ToList();

    foreach (var unitPlan in unitPlans)
    {
        var clone = state.Clone();
        foreach (var command in unitPlan)
        {
            BattleRules.ApplyCommand(clone, command);
        }

        foreach (var rest in BuildTurnPlans(clone))
        {
            yield return [.. unitPlan, .. rest];
        }
    }
}

static IEnumerable<IReadOnlyList<BattleCommand>> BuildUnitPlans(BattleState state, UnitState unit)
{
    if (unit.Type == UnitType.Scout)
    {
        foreach (var destination in BattleRules.GetReachableTiles(state, unit))
        {
            yield return [BattleCommand.Move(unit.Id, destination), BattleCommand.Wait(unit.Id)];
        }

        yield return [BattleCommand.Wait(unit.Id)];
        yield break;
    }

    if (!unit.HasMoved)
    {
        foreach (var destination in BattleRules.GetReachableTiles(state, unit))
        {
            var movePlan = new[] { BattleCommand.Move(unit.Id, destination) };
            var movedState = state.Clone();
            BattleRules.ApplyCommand(movedState, movePlan[0]);
            var movedUnit = movedState.Units.First(candidate => candidate.Id == unit.Id);
            foreach (var attackPlan in BuildAttackOrWaitPlans(movedState, movedUnit, movePlan))
            {
                yield return attackPlan;
            }
        }
    }

    foreach (var attackPlan in BuildAttackOrWaitPlans(state, unit, []))
    {
        yield return attackPlan;
    }
}

static IEnumerable<IReadOnlyList<BattleCommand>> BuildAttackOrWaitPlans(BattleState state, UnitState unit, IReadOnlyList<BattleCommand> prefix)
{
    foreach (var targetCoord in BattleRules.GetAttackableCoords(state, unit))
    {
        var target = BattleRules.GetLivingUnitAt(state, targetCoord);
        if (target is not null)
        {
            yield return [.. prefix, BattleCommand.Attack(unit.Id, target.Id)];
        }
    }

    yield return [.. prefix, BattleCommand.Wait(unit.Id)];
}

static int ScorePlan(BattleState state, IReadOnlyList<BattleCommand> plan)
{
    var before = EvaluateState(state);
    var clone = state.Clone();
    foreach (var command in plan)
    {
        BattleRules.ApplyCommand(clone, command);
    }

    return EvaluateState(clone) - before;
}

static int ScoreTurnPlan(BattleState state, IReadOnlyList<BattleCommand> plan)
{
    var clone = state.Clone();
    foreach (var command in plan)
    {
        BattleRules.ApplyCommand(clone, command);
    }

    if (clone.Outcome == BattleOutcome.InProgress)
    {
        BattleRules.ApplyCommand(clone, BattleCommand.EndTurn());
    }

    return EvaluateState(clone);
}

static int EvaluateState(BattleState state)
{
    if (state.Outcome == BattleOutcome.PlayerVictory)
    {
        return 100_000;
    }

    if (state.Outcome == BattleOutcome.PlayerDefeat)
    {
        return -100_000;
    }

    var scout = state.Units.First(unit => unit.Id == state.ScoutId);
    var playerHp = state.Units.Where(unit => unit.Team == Team.Player && unit.IsAlive).Sum(unit => unit.Hp);
    var enemyHp = state.Units.Where(unit => unit.Team == Team.Enemy && unit.IsAlive).Sum(unit => unit.Hp);
    var closestRescuerDistance = state.Units
        .Where(unit => unit.Team == Team.Player && unit.IsAlive && unit.Id != state.ScoutId)
        .Select(unit => unit.Position.DistanceTo(scout.Position))
        .DefaultIfEmpty(99)
        .Min();
    var closestEnemyDistance = state.Units
        .Where(unit => unit.Team == Team.Enemy && unit.IsAlive)
        .Select(enemy => state.Units
            .Where(player => player.Team == Team.Player && player.IsAlive)
            .Select(player => enemy.Position.DistanceTo(player.Position))
            .DefaultIfEmpty(99)
            .Min())
        .DefaultIfEmpty(0)
        .Min();
    var closestEnemyToScout = state.Units
        .Where(unit => unit.Team == Team.Enemy && unit.IsAlive)
        .Select(unit => unit.Position.DistanceTo(scout.Position))
        .DefaultIfEmpty(8)
        .Min();

    return
        (state.ScoutRescued ? 2_500 : -closestRescuerDistance * 80) +
        (scout.IsAlive ? closestEnemyToScout * 120 : -4_000) +
        (state.EnemyLosses * 1_200) -
        (state.PlayerLosses * 1_500) +
        (playerHp * 35) -
        (enemyHp * 55) -
        (closestEnemyDistance * 20) -
        (state.Turn * 15);
}

static IEnumerable<UnitState> ReadyPlayerUnits(BattleState state) => state.Units
    .Where(unit => unit.Team == Team.Player && unit.IsAlive && !unit.HasActed && !BattleRules.IsScoutStranded(state, unit))
    .OrderBy(unit => unit.Type == UnitType.Armor ? 0 : unit.Type == UnitType.Infantry ? 1 : 2)
    .ThenBy(unit => unit.Id, StringComparer.Ordinal);

static string DescribeCommand(BattleCommand command) => command.Kind switch
{
    CommandKind.Move => $"MOVE {command.UnitId} to {command.Destination}",
    CommandKind.Attack => $"ATTACK {command.UnitId} -> {command.TargetUnitId}",
    CommandKind.Wait => $"WAIT {command.UnitId}",
    CommandKind.EndTurn => "END TURN",
    _ => command.Kind.ToString()
};

static string SummarizeUnits(BattleState state)
{
    var units = state.Units
        .Where(unit => unit.IsAlive)
        .OrderBy(unit => unit.Team)
        .ThenBy(unit => unit.Id, StringComparer.Ordinal)
        .Select(unit => $"{unit.Id}:{unit.Hp}@{unit.Position}");
    return string.Join(", ", units);
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"{message} Expected: {expected}. Actual: {actual}.");
    }
}

static void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
