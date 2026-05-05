using System.Text.Json;
using Wargame.Core;

if (args.FirstOrDefault()?.Equals("summarize-playtest-log", StringComparison.OrdinalIgnoreCase) == true)
{
    return RunPlaytestSummaryCommand(args.Skip(1).ToArray());
}

if (args.FirstOrDefault()?.Equals("playtest-ai", StringComparison.OrdinalIgnoreCase) == true)
{
    return RunAiPlaytestCommand(args.Skip(1).ToArray());
}

(BattleState State, IReadOnlyList<string> Transcript)? aiPlayerDemo = null;

var checks = new List<(string Name, Action Check)>
{
    ("movement range respects terrain and blockers", CheckMovementRange),
    ("rivers block and workshops repair", CheckRiversAndWorkshops),
    ("terrain changes combat forecast", CheckTerrainForecast),
    ("expanded mission has infantry choices", CheckExpandedMission),
    ("campaign terrain paths stay connected", CheckCampaignTerrainPaths),
    ("scout rescue updates objective state", CheckScoutRescue),
    ("opening end turn does not defeat player", CheckOpeningEndTurn),
    ("hq capture causes defeat", CheckHqDefeat),
    ("replay hash is deterministic", CheckReplayHash),
    ("replay command stream reproduces expected state", CheckReplayCommandStream),
    ("mission objective state affects replay hash", CheckMissionObjectiveStateHash),
    ("mission two brief explains capture markers", CheckMissionTwoBriefExplainsCaptureMarkers),
    ("enemy ai advances mission pressure", CheckEnemyPressure),
    ("mission two relay and fuel objectives resolve", CheckMissionTwoObjectives),
    ("capture economy awards player income", CheckCaptureEconomy),
    ("field rig resupplies limited ammo", CheckFieldRigResupply),
    ("lock the line power charges and expires", CheckLockTheLinePower),
    ("campaign catalog creates missions one through ten", CheckCampaignCatalogCreatesTenMissions),
    ("campaign missions have valid progression metadata", CheckCampaignProgressionMetadata),
    ("ai player wins first mission", CheckAiPlayerWinsFirstMission),
    ("ai player clears third campaign mission", CheckAiPlayerClearsThirdMission),
    ("ai player clears ninth campaign mission", CheckAiPlayerClearsNinthMission),
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

return 0;

void CheckAiPlayerWinsFirstMission()
{
    var state = GetAiPlayerDemo().State;

    AssertEqual(BattleOutcome.PlayerVictory, state.Outcome, "AI player should win the first mission.");
    AssertTrue(state.ScoutRescued, "AI player should rescue Scout-7 before winning.");
    AssertTrue(state.Turn <= 8, "AI player should win within the prototype target turn range.");
}

static void CheckAiPlayerClearsThirdMission()
{
    CheckAiPlayerClearsMission(3);
}

static void CheckAiPlayerClearsNinthMission()
{
    CheckAiPlayerClearsMission(9);
}

static void CheckAiPlayerClearsMission(int missionNumber)
{
    var state = CampaignMissionFactory.Create(missionNumber);
    var transcript = new List<string>();
    while (state.Outcome == BattleOutcome.InProgress && state.Turn <= 20)
    {
        PlayAiPlayerTurn(state, transcript);
        if (state.Outcome == BattleOutcome.InProgress)
        {
            BattleRules.ApplyCommand(state, BattleCommand.EndTurn());
        }
    }

    AssertEqual(BattleOutcome.PlayerVictory, state.Outcome, $"AI player should clear Mission {missionNumber}.");
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

static void CheckCampaignCatalogCreatesTenMissions()
{
    for (var missionNumber = 1; missionNumber <= CampaignMissionCatalog.FinalMissionNumber; missionNumber++)
    {
        var state = CampaignMissionFactory.Create(missionNumber);
        AssertEqual(missionNumber, state.MissionNumber, $"Mission {missionNumber} should stamp its mission number.");
        AssertTrue(!string.IsNullOrWhiteSpace(state.MissionTitle), $"Mission {missionNumber} should have a title.");
        AssertTrue(state.Units.Any(unit => unit.Team == Team.Player), $"Mission {missionNumber} should include player units.");
        AssertTrue(state.Units.Any(unit => unit.Team == Team.Enemy), $"Mission {missionNumber} should include enemy units.");
        AssertEqual(state.Units.Count(unit => unit.Team == Team.Enemy), state.InitialEnemyCount, $"Mission {missionNumber} enemy count should match metadata.");
    }
}

static void CheckCampaignTerrainPaths()
{
    foreach (var missionNumber in new[] { 3, 4 })
    {
        var state = CampaignMissionFactory.Create(missionNumber);
        foreach (var coord in AllCoords(state))
        {
            var terrain = state.GetTerrain(coord);
            if (terrain == TerrainType.Road)
            {
                AssertTrue(
                    coord.Neighbors().Any(neighbor => state.Contains(neighbor) && state.GetTerrain(neighbor) is TerrainType.Road or TerrainType.Hq or TerrainType.Workshop),
                    $"Mission {missionNumber} road at {coord} should connect to another path tile.");
            }

            if (terrain == TerrainType.River)
            {
                AssertTrue(
                    coord.Neighbors().Any(neighbor => state.Contains(neighbor) && state.GetTerrain(neighbor) is TerrainType.River or TerrainType.Road),
                    $"Mission {missionNumber} river at {coord} should connect to river or bridge tile.");
            }
        }
    }

    var missionThree = CampaignMissionFactory.Create(3);
    AssertEqual(TerrainType.Road, missionThree.GetTerrain(new TileCoord(6, 4)), "Mission 3 should bridge the center river on the convoy road.");
    AssertEqual(TerrainType.River, missionThree.GetTerrain(new TileCoord(6, 3)), "Mission 3 should keep river pressure north of the bridge.");
    AssertEqual(TerrainType.River, missionThree.GetTerrain(new TileCoord(6, 5)), "Mission 3 should keep river pressure south of the bridge.");
}

static IEnumerable<TileCoord> AllCoords(BattleState state)
{
    for (var row = 0; row < state.Height; row++)
    {
        for (var column = 0; column < state.Width; column++)
        {
            yield return new TileCoord(column, row);
        }
    }
}

static void CheckCampaignProgressionMetadata()
{
    for (var missionNumber = 1; missionNumber <= CampaignMissionCatalog.FinalMissionNumber; missionNumber++)
    {
        var brief = CampaignMissionCatalog.GetBrief(missionNumber);
        AssertTrue(brief.Number == missionNumber, $"Mission {missionNumber} brief should be ordered.");
        AssertTrue(brief.Id == $"mission{missionNumber}", $"Mission {missionNumber} brief should use the mission id convention.");
        AssertTrue(!string.IsNullOrWhiteSpace(brief.IntroLine), $"Mission {missionNumber} should have an intro cutscene line.");
        AssertTrue(!string.IsNullOrWhiteSpace(brief.VictoryLine), $"Mission {missionNumber} should have an outro cutscene line.");
    }
}

static void CheckCaptureEconomy()
{
    var state = SecondMissionFactory.Create();
    var relayTech = state.Units.First(unit => unit.Id == "Tech-1");
    relayTech.Position = state.RelayStation;

    BattleRules.ApplyCommand(state, BattleCommand.Wait(relayTech.Id));
    ResetUnitForObjectiveFixture(relayTech);
    BattleRules.ApplyCommand(state, BattleCommand.Wait(relayTech.Id));

    AssertTrue(state.RelaySecured, "Relay should be secured after two capture waits.");
    AssertEqual(1, state.PlayerControlledProperties, "Secured relay should count as one controlled property.");
    AssertEqual(BattleState.PropertyIncomeValue, state.PlayerIncome, "Secured relay should increase player income.");
    AssertEqual(0, state.PlayerFunds, "Capture should not pay funds immediately.");

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    AssertEqual(BattleState.PropertyIncomeValue, state.PlayerFunds, "Next player turn should pay income from controlled properties.");
}

static void CheckFieldRigResupply()
{
    var state = SecondMissionFactory.Create();
    var lancerProfile = BattleRules.GetProfile(UnitType.Lancer);
    var rigProfile = BattleRules.GetProfile(UnitType.FieldRig);
    state.Units.AddRange([
        new UnitState
        {
            Ammo = 1,
            Hp = lancerProfile.MaxHp,
            Id = "Lancer-Test",
            MaxAmmo = 1,
            Position = new TileCoord(3, 3),
            Team = Team.Player,
            Type = UnitType.Lancer
        },
        new UnitState
        {
            Hp = rigProfile.MaxHp,
            Id = "Rig-Test",
            Position = new TileCoord(3, 4),
            Team = Team.Player,
            Type = UnitType.FieldRig
        },
        new UnitState
        {
            Hp = 20,
            Id = "Armor-Target",
            Position = new TileCoord(4, 3),
            Team = Team.Enemy,
            Type = UnitType.Armor
        }
    ]);

    var lancer = state.Units.First(unit => unit.Id == "Lancer-Test");
    var armorTarget = state.Units.First(unit => unit.Id == "Armor-Target");
    var loadedForecast = BattleRules.GetCombatForecast(state, lancer, armorTarget);
    AssertTrue(loadedForecast.ExpectedDamage > 0, "Loaded limited-ammo unit should forecast damage.");

    var firstAttack = BattleRules.ApplyCommand(state, BattleCommand.Attack(lancer.Id, armorTarget.Id));
    AssertTrue(firstAttack.Success, "Loaded limited-ammo unit should be able to attack.");
    AssertEqual(0, lancer.Ammo, "Limited-ammo attack should spend one ammo.");

    ResetUnitForObjectiveFixture(lancer);
    var emptyForecast = BattleRules.GetCombatForecast(state, lancer, armorTarget);
    AssertEqual(0, emptyForecast.ExpectedDamage, "Empty limited-ammo unit should forecast zero damage.");
    AssertTrue(!BattleRules.GetAttackableCoords(state, lancer).Contains(armorTarget.Position), "Empty limited-ammo unit should have no legal attack target.");
    var emptyAttack = BattleRules.ApplyCommand(state, BattleCommand.Attack(lancer.Id, armorTarget.Id));
    AssertTrue(!emptyAttack.Success, "Empty limited-ammo unit should not be able to attack.");

    var resupply = BattleRules.ApplyCommand(state, BattleCommand.Wait("Rig-Test"));
    AssertTrue(resupply.Success, "Field Rig should resupply adjacent limited-ammo unit.");
    AssertEqual(1, lancer.Ammo, "Field Rig should restore one ammo.");

    ResetUnitForObjectiveFixture(lancer);
    var secondAttack = BattleRules.ApplyCommand(state, BattleCommand.Attack(lancer.Id, armorTarget.Id));
    AssertTrue(secondAttack.Success, "Resupplied limited-ammo unit should be able to attack again.");
}

static void CheckLockTheLinePower()
{
    var lowChargeState = CreatePowerFixture();
    lowChargeState.PlayerPowerCharge = BattleState.LockTheLineChargeCost - 1;
    var lowChargeHash = BattleRules.GetStateHash(lowChargeState);
    var lowChargeResult = BattleRules.ApplyCommand(lowChargeState, BattleCommand.ActivatePower(BattleState.LockTheLinePowerId));
    AssertTrue(!lowChargeResult.Success, "Lock The Line should fail below charge cost.");
    AssertEqual(lowChargeHash, BattleRules.GetStateHash(lowChargeState), "Failed power activation should not mutate state.");

    var chargeState = CreatePowerFixture();
    chargeState.ActiveTeam = Team.Enemy;
    var chargeResult = BattleRules.ApplyCommand(chargeState, BattleCommand.Attack("Enemy-Test", "Infantry-Test"));
    AssertTrue(chargeResult.Success, "Enemy attack should apply in power charge fixture.");
    AssertEqual(1, chargeState.PlayerPowerCharge, "Surviving player damage should add one commander charge.");

    var activeState = CreatePowerFixture();
    activeState.PlayerPowerCharge = BattleState.LockTheLineChargeCost;
    var activation = BattleRules.ApplyCommand(activeState, BattleCommand.ActivatePower(BattleState.LockTheLinePowerId));
    AssertTrue(activation.Success, "Lock The Line should activate at full charge.");
    AssertEqual(0, activeState.PlayerPowerCharge, "Lock The Line should spend its charge.");
    AssertTrue(activeState.PlayerLockTheLineActive, "Lock The Line should mark the player power active.");

    var baseline = activeState.Clone();
    baseline.PlayerLockTheLineActive = false;
    baseline.ActiveTeam = Team.Enemy;
    activeState.ActiveTeam = Team.Enemy;
    var enemy = activeState.Units.First(unit => unit.Id == "Enemy-Test");
    var defender = activeState.Units.First(unit => unit.Id == "Infantry-Test");
    var baselineEnemy = baseline.Units.First(unit => unit.Id == enemy.Id);
    var baselineDefender = baseline.Units.First(unit => unit.Id == defender.Id);
    var baselineForecast = BattleRules.GetCombatForecast(baseline, baselineEnemy, baselineDefender);
    var poweredForecast = BattleRules.GetCombatForecast(activeState, enemy, defender);
    AssertEqual(Math.Max(1, baselineForecast.ExpectedDamage - 1), poweredForecast.ExpectedDamage, "Lock The Line should reduce incoming forecast damage by one.");

    var expiration = BattleRules.ApplyCommand(activeState, BattleCommand.EndTurn());
    AssertTrue(expiration.Success, "Ending enemy phase should expire Lock The Line.");
    AssertTrue(!activeState.PlayerLockTheLineActive, "Lock The Line should expire when the next player turn starts.");
}

static BattleState CreatePowerFixture()
{
    var terrain = Enumerable.Repeat(TerrainType.Plain, 25).ToArray();
    var state = new BattleState(5, 5, terrain)
    {
        EnemyHq = new TileCoord(4, 2),
        InitialEnemyCount = 1,
        MissionId = "power-fixture",
        PlayerHq = new TileCoord(0, 2),
        RequiresScoutSurvival = false,
        ScoutId = "none",
        ScoutRescued = true
    };

    state.Units.AddRange([
        new UnitState
        {
            Hp = BattleRules.GetProfile(UnitType.Infantry).MaxHp,
            Id = "Infantry-Test",
            Position = new TileCoord(2, 2),
            Team = Team.Player,
            Type = UnitType.Infantry
        },
        new UnitState
        {
            Hp = BattleRules.GetProfile(UnitType.Infantry).MaxHp,
            Id = "Enemy-Test",
            Position = new TileCoord(3, 2),
            Team = Team.Enemy,
            Type = UnitType.Infantry
        }
    ]);

    return state;
}

static void CheckMovementRange()
{
    var state = FirstMissionFactory.Create();
    var infantry = state.Units.First(unit => unit.Id == "Infantry-1");
    var reachableTiles = BattleRules.GetReachableTiles(state, infantry);

    AssertTrue(reachableTiles.Contains(new TileCoord(5, 3)), "Road chokepoint should be reachable.");
    AssertTrue(!reachableTiles.Contains(new TileCoord(0, 0)), "Ridge should be impassable.");
    AssertTrue(!reachableTiles.Contains(new TileCoord(2, 4)), "Occupied allied tile should block movement.");

    var passThroughState = CreateFriendlyPassThroughFixture();
    var scout = passThroughState.Units.First(unit => unit.Id == "Scout-Test");
    var passThroughTiles = BattleRules.GetReachableTiles(passThroughState, scout);
    AssertTrue(!passThroughTiles.Contains(new TileCoord(1, 1)), "Friendly occupied tile should not be a legal destination.");
    AssertTrue(passThroughTiles.Contains(new TileCoord(2, 1)), "Friendly occupied tile should not block path traversal.");
    AssertTrue(!passThroughTiles.Contains(new TileCoord(4, 1)), "Enemy occupied tile should block path traversal.");

    var stackAttempt = BattleRules.ApplyCommand(passThroughState, BattleCommand.Move(scout.Id, new TileCoord(1, 1)));
    AssertTrue(!stackAttempt.Success, "Move command should reject stacking on a friendly unit.");

    var passThroughMove = BattleRules.ApplyCommand(passThroughState, BattleCommand.Move(scout.Id, new TileCoord(2, 1)));
    AssertTrue(passThroughMove.Success, "Move command should allow a destination beyond a friendly unit.");
}

static void CheckRiversAndWorkshops()
{
    var terrain = Enumerable.Repeat(TerrainType.Plain, 25).ToArray();
    terrain[(2 * 5) + 2] = TerrainType.River;
    terrain[(1 * 5) + 1] = TerrainType.Workshop;
    var state = new BattleState(5, 5, terrain)
    {
        EnemyHq = new TileCoord(4, 2),
        MissionId = "terrain-fixture",
        PlayerHq = new TileCoord(0, 2),
        RequiresScoutSurvival = false,
        ScoutId = "none",
        ScoutRescued = true
    };

    state.Units.Add(new UnitState
    {
        Hp = 5,
        Id = "Infantry-Test",
        Position = new TileCoord(1, 1),
        Team = Team.Player,
        Type = UnitType.Infantry
    });

    var unit = state.Units[0];
    var reachable = BattleRules.GetReachableTiles(state, unit);
    AssertTrue(!reachable.Contains(new TileCoord(2, 2)), "River tiles should be impassable.");

    var repair = BattleRules.ApplyCommand(state, BattleCommand.Wait(unit.Id));
    AssertTrue(repair.Success, "Workshop wait should repair a damaged unit.");
    AssertEqual(8, unit.Hp, "Workshop should repair 3 HP without exceeding max HP.");
}

static BattleState CreateFriendlyPassThroughFixture()
{
    const int width = 5;
    const int height = 3;
    var terrain = Enumerable.Repeat(TerrainType.Ridge, width * height).ToArray();
    for (var column = 0; column < width; column++)
    {
        terrain[(1 * width) + column] = TerrainType.Plain;
    }

    var state = new BattleState(width, height, terrain)
    {
        EnemyHq = new TileCoord(4, 1),
        MissionId = "friendly-pass-through-fixture",
        PlayerHq = new TileCoord(0, 1),
        RequiresScoutSurvival = false,
        ScoutId = "none",
        ScoutRescued = true
    };

    state.Units.AddRange([
        new UnitState
        {
            Hp = BattleRules.GetProfile(UnitType.Scout).MaxHp,
            Id = "Scout-Test",
            Position = new TileCoord(0, 1),
            Team = Team.Player,
            Type = UnitType.Scout
        },
        new UnitState
        {
            Hp = BattleRules.GetProfile(UnitType.Infantry).MaxHp,
            Id = "Friendly-Test",
            Position = new TileCoord(1, 1),
            Team = Team.Player,
            Type = UnitType.Infantry
        },
        new UnitState
        {
            Hp = BattleRules.GetProfile(UnitType.Infantry).MaxHp,
            Id = "Enemy-Test",
            Position = new TileCoord(3, 1),
            Team = Team.Enemy,
            Type = UnitType.Infantry
        }
    ]);

    return state;
}

static void CheckMissionTwoObjectives()
{
    var state = SecondMissionFactory.Create();
    var relayTech = state.Units.First(unit => unit.Id == "Tech-1");
    var fuelEngineer = state.Units.First(unit => unit.Id == "Engineer-1");
    relayTech.Position = state.RelayStation;
    fuelEngineer.Position = state.FuelCache;

    BattleRules.ApplyCommand(state, BattleCommand.Wait(relayTech.Id));
    BattleRules.ApplyCommand(state, BattleCommand.Wait(fuelEngineer.Id));
    ResetUnitForObjectiveFixture(relayTech);
    ResetUnitForObjectiveFixture(fuelEngineer);
    BattleRules.ApplyCommand(state, BattleCommand.Wait(relayTech.Id));
    BattleRules.ApplyCommand(state, BattleCommand.Wait(fuelEngineer.Id));

    AssertTrue(state.RelaySecured, "Mission 2 relay should be secured after two capture waits.");
    AssertTrue(state.FuelSecured, "Mission 2 fuel cache should be secured after two capture waits.");

    foreach (var enemy in state.Units.Where(unit => unit.Team == Team.Enemy))
    {
        enemy.Hp = 0;
    }

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    AssertEqual(BattleOutcome.PlayerVictory, state.Outcome, "Mission 2 should end after both objectives are secured and enemies are gone.");
}

static void CheckMissionTwoBriefExplainsCaptureMarkers()
{
    var brief = CampaignMissionCatalog.GetBrief(2);

    AssertTrue(brief.ObjectiveSummary.Contains("Relay (R)", StringComparison.Ordinal), "Mission 2 objective should define the R marker.");
    AssertTrue(brief.ObjectiveSummary.Contains("Fuel Cache (F)", StringComparison.Ordinal), "Mission 2 objective should define the F marker.");
    AssertTrue(brief.RescueInstruction.Contains("Wait", StringComparison.Ordinal), "Mission 2 instruction should explain that Wait captures objectives.");
    AssertTrue(brief.RescueInstruction.Contains("twice", StringComparison.Ordinal), "Mission 2 instruction should explain the two-wait capture requirement.");
}

static void CheckOpeningEndTurn()
{
    var state = FirstMissionFactory.Create();

    BattleRules.ApplyCommand(state, BattleCommand.EndTurn());

    AssertEqual(BattleOutcome.InProgress, state.Outcome, "Ending the first turn before acting should warn, not instantly lose.");
    AssertTrue(state.Units.First(unit => unit.Id == state.ScoutId).IsAlive, "Scout-7 should survive the first enemy pressure phase.");
}

static void ResetUnitForObjectiveFixture(UnitState unit)
{
    unit.HasActed = false;
    unit.HasMoved = false;
}

static void CheckReplayHash()
{
    var first = PlayOpening();
    var second = PlayOpening();

    AssertEqual(BattleRules.GetStateHash(first), BattleRules.GetStateHash(second), "Same command stream should hash identically.");
}

static void CheckReplayCommandStream()
{
    var expectedState = PlayOpening();
    var initialState = FirstMissionFactory.Create();
    var stream = new ReplayCommandStream(
        FormatVersion: 1,
        RulesVersion: "prototype-2026-05-03",
        SaveSchemaVersion: 1,
        MissionId: initialState.MissionId,
        RandomSeed: initialState.RandomSeed,
        InitialStateHash: BattleRules.GetStateHash(initialState),
        ExpectedFinalStateHash: BattleRules.GetStateHash(expectedState),
        Commands: OpeningCommandStream().Select(ReplayCommand.FromBattleCommand).ToList());

    var json = JsonSerializer.Serialize(stream);
    var deserialized = JsonSerializer.Deserialize<ReplayCommandStream>(json)
        ?? throw new InvalidOperationException("Replay stream did not deserialize.");
    var replayed = FirstMissionFactory.Create();

    AssertEqual(stream.InitialStateHash, BattleRules.GetStateHash(replayed), "Replay initial hash should match the fixture.");
    foreach (var command in deserialized.Commands.Select(command => command.ToBattleCommand()))
    {
        var result = BattleRules.ApplyCommand(replayed, command);
        AssertTrue(result.Success, $"Replay command should apply successfully: {command.Kind} {command.UnitId} {command.Destination}. {result.Message}");
    }

    AssertEqual(deserialized.ExpectedFinalStateHash, BattleRules.GetStateHash(replayed), "Replay command stream should reproduce the expected final state.");
}

static void CheckMissionObjectiveStateHash()
{
    var first = SecondMissionFactory.Create();
    var second = SecondMissionFactory.Create();

    second.RelayCaptureProgress = 1;

    AssertTrue(BattleRules.GetStateHash(first) != BattleRules.GetStateHash(second), "Mission objective progress should affect the state hash.");
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
    foreach (var command in OpeningCommandStream())
    {
        BattleRules.ApplyCommand(state, command);
    }

    return state;
}

static IReadOnlyList<BattleCommand> OpeningCommandStream() =>
[
    BattleCommand.Move("Infantry-1", new TileCoord(5, 3)),
    BattleCommand.Wait("Infantry-1"),
    BattleCommand.Move("Infantry-2", new TileCoord(4, 4)),
    BattleCommand.Wait("Infantry-2"),
    BattleCommand.EndTurn()
];

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
        transcript.Add($"  {CampaignAutoplayer.DescribeCommand(command)} -> {result.Message}");
    }
}

static IReadOnlyList<BattleCommand> ChooseBestAiTurnPlan(BattleState state) => CampaignAutoplayer.ChoosePlayerTurn(state);

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

static int RunPlaytestSummaryCommand(string[] args)
{
    var inputPath = ParseSummaryInputPath(args);
    if (string.IsNullOrWhiteSpace(inputPath))
    {
        Console.Error.WriteLine("Usage: summarize-playtest-log <path-to-jsonl>");
        return 2;
    }

    if (!File.Exists(inputPath))
    {
        Console.Error.WriteLine($"Playthrough log not found: {inputPath}");
        return 2;
    }

    Console.WriteLine($"Playthrough log: {Path.GetRelativePath(Directory.GetCurrentDirectory(), inputPath)}");
    foreach (var line in File.ReadLines(inputPath))
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        using var document = JsonDocument.Parse(line);
        var root = document.RootElement;
        var eventType = GetString(root, "eventType");
        if (!IsSummaryEvent(eventType) || !root.TryGetProperty("payload", out var payload))
        {
            continue;
        }

        Console.WriteLine(FormatSummaryLine(eventType, payload));
    }

    return 0;
}

static string? ParseSummaryInputPath(IReadOnlyList<string> args)
{
    foreach (var arg in args)
    {
        if (arg.StartsWith("--input=", StringComparison.OrdinalIgnoreCase))
        {
            return arg[8..];
        }

        if (!arg.StartsWith("--", StringComparison.Ordinal))
        {
            return arg;
        }
    }

    return null;
}

static bool IsSummaryEvent(string? eventType) => eventType is
    "playthrough-start" or
    "mission-start" or
    "mission-end" or
    "issue-candidate" or
    "playthrough-end";

static string FormatSummaryLine(string? eventType, JsonElement payload) => eventType switch
{
    "playthrough-start" =>
        $"playthrough-start id={GetString(payload, "playthroughId")} mode={GetString(payload, "mode")} maxTurns={GetInt(payload, "maxTurnsPerMission")} finalMission={GetInt(payload, "finalMissionNumber")}",
    "mission-start" =>
        $"mission-start M{GetInt(payload, "MissionNumber")} {GetString(payload, "MissionTitle")} turn={GetInt(payload, "Turn")} outcome={FormatOutcome(payload)}",
    "mission-end" =>
        $"mission-end M{GetInt(payload, "MissionNumber")} {GetString(payload, "MissionTitle")} outcome={FormatOutcome(payload)} turn={GetInt(payload, "Turn")} playerLosses={GetInt(payload, "PlayerLosses")} enemyLosses={GetInt(payload, "EnemyLosses")} score={FormatScore(payload)} relay={GetBool(payload, "RelaySecured")} fuel={GetBool(payload, "FuelSecured")}",
    "issue-candidate" =>
        $"issue-candidate severity={GetString(payload, "severity")} kind={GetString(payload, "kind")} mission={FormatIssueMission(payload)} summary={GetString(payload, "summary")}",
    "playthrough-end" =>
        $"playthrough-end campaignComplete={GetBool(payload, "campaignComplete")} completedCampaigns={GetInt(payload, "completedCampaigns")}",
    _ => eventType ?? string.Empty
};

static string FormatIssueMission(JsonElement payload)
{
    var missionNumber = GetInt(payload, "MissionNumber") ?? GetInt(payload, "missionNumber");
    var missionTitle = GetString(payload, "MissionTitle");
    return missionNumber is null ? string.Empty : $"M{missionNumber} {missionTitle}";
}

static string FormatOutcome(JsonElement payload)
{
    var outcome = GetInt(payload, "Outcome");
    return outcome is null ? string.Empty : ((BattleOutcome)outcome).ToString();
}

static string FormatScore(JsonElement payload)
{
    if (!payload.TryGetProperty("score", out var score))
    {
        return string.Empty;
    }

    return GetInt(score, "Total")?.ToString() ?? string.Empty;
}

static bool? GetBool(JsonElement element, string propertyName) =>
    element.TryGetProperty(propertyName, out var property) &&
    property.ValueKind is JsonValueKind.True or JsonValueKind.False
        ? property.GetBoolean()
        : null;

static int? GetInt(JsonElement element, string propertyName) =>
    element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
        ? value
        : null;

static string? GetString(JsonElement element, string propertyName) =>
    element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
        ? property.GetString()
        : null;

static int RunAiPlaytestCommand(string[] args)
{
    var options = AiPlaytestOptions.Parse(args);
    Directory.CreateDirectory(options.OutputDirectory);
    var completedCampaigns = 0;
    var generatedLogs = new List<string>();

    for (var runIndex = 1; runIndex <= options.Runs; runIndex++)
    {
        var playthroughId = $"ai-campaign-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{runIndex:D2}";
        var logPath = Path.Combine(options.OutputDirectory, $"{playthroughId}.jsonl");
        generatedLogs.Add(logPath);
        using var logger = new PlaythroughLogWriter(logPath, playthroughId);
        logger.Write("playthrough-start", new
        {
            playthroughId,
            mode = "ai-vs-ai-campaign",
            createdUtc = DateTimeOffset.UtcNow,
            maxTurnsPerMission = options.MaxTurnsPerMission,
            finalMissionNumber = CampaignMissionCatalog.FinalMissionNumber
        });

        var campaignComplete = true;
        for (var missionNumber = 1; missionNumber <= CampaignMissionCatalog.FinalMissionNumber; missionNumber++)
        {
            var state = CampaignMissionFactory.Create(missionNumber);
            RunLoggedAiMission(state, logger, options.MaxTurnsPerMission);
            if (state.Outcome != BattleOutcome.PlayerVictory)
            {
                campaignComplete = false;
                logger.Write("issue-candidate", new
                {
                    severity = "high",
                    kind = "campaign-blocker",
                    missionNumber,
                    state.MissionTitle,
                    state.Outcome,
                    summary = "AI playthrough could not advance past this mission. Review balance, objective prompts, map layout, and autoplayer heuristics."
                });
                break;
            }
        }

        if (campaignComplete)
        {
            completedCampaigns++;
        }

        logger.Write("playthrough-end", new
        {
            playthroughId,
            campaignComplete,
            completedCampaigns,
            generatedUtc = DateTimeOffset.UtcNow
        });
    }

    Console.WriteLine($"Generated {generatedLogs.Count} AI playthrough log(s):");
    foreach (var log in generatedLogs)
    {
        Console.WriteLine(Path.GetRelativePath(Directory.GetCurrentDirectory(), log));
    }

    Console.WriteLine($"Completed campaigns: {completedCampaigns}/{options.Runs}");
    return completedCampaigns == options.Runs ? 0 : 1;
}

static void RunLoggedAiMission(BattleState state, PlaythroughLogWriter logger, int maxTurns)
{
    logger.Write("mission-start", MissionSnapshot(state, "start"));

    while (state.Outcome == BattleOutcome.InProgress && state.Turn <= maxTurns)
    {
        logger.Write("turn-start", MissionSnapshot(state, "before-player-turn"));
        var plan = CampaignAutoplayer.ChoosePlayerTurn(state);
        logger.Write("ai-plan", new
        {
            state.MissionNumber,
            state.MissionTitle,
            state.Turn,
            commandCount = plan.Count,
            scoreBefore = CampaignAutoplayer.EvaluateState(state),
            commands = plan.Select(CampaignAutoplayer.DescribeCommand).ToArray()
        });

        foreach (var command in plan)
        {
            ApplyLoggedCommand(state, logger, command, "player-ai");
            if (state.Outcome != BattleOutcome.InProgress)
            {
                break;
            }
        }

        if (state.Outcome != BattleOutcome.InProgress)
        {
            break;
        }

        ApplyLoggedCommand(state, logger, BattleCommand.EndTurn(), "enemy-ai-phase");
    }

    if (state.Outcome == BattleOutcome.InProgress)
    {
        logger.Write("issue-candidate", new
        {
            severity = "high",
            kind = "turn-limit",
            state.MissionNumber,
            state.MissionTitle,
            state.Turn,
            maxTurns,
            summary = "AI playtest reached the turn limit without a result. Check for stalemate, objective ambiguity, or weak AI heuristics."
        });
    }

    var score = BattleRules.CalculateScore(state);
    logger.Write("mission-end", new
    {
        state.MissionNumber,
        state.MissionTitle,
        state.Outcome,
        state.Turn,
        state.CommandCount,
        state.PlayerLosses,
        state.EnemyLosses,
        state.ScoutRescued,
        state.RelaySecured,
        state.FuelSecured,
        score,
        stateHash = BattleRules.GetStateHash(state),
        units = UnitSnapshots(state)
    });
}

static void ApplyLoggedCommand(BattleState state, PlaythroughLogWriter logger, BattleCommand command, string actor)
{
    var beforeHash = BattleRules.GetStateHash(state);
    var beforeUnits = UnitSnapshots(state);
    var result = BattleRules.ApplyCommand(state, command);
    logger.Write("command", new
    {
        actor,
        state.MissionNumber,
        state.MissionTitle,
        state.Turn,
        command = CampaignAutoplayer.DescribeCommand(command),
        command.Kind,
        command.UnitId,
        command.Destination,
        command.TargetUnitId,
        result.Success,
        result.Message,
        beforeHash,
        afterHash = BattleRules.GetStateHash(state),
        beforeUnits,
        afterUnits = UnitSnapshots(state),
        objectives = ObjectiveSnapshot(state),
        state.Outcome
    });
}

static object MissionSnapshot(BattleState state, string phase) => new
{
    phase,
    state.MissionNumber,
    state.MissionId,
    state.MissionTitle,
    state.ObjectiveSummary,
    state.Turn,
    state.CommandCount,
    state.Outcome,
    state.ActiveTeam,
    stateHash = BattleRules.GetStateHash(state),
    evaluation = CampaignAutoplayer.EvaluateState(state),
    objectives = ObjectiveSnapshot(state),
    units = UnitSnapshots(state)
};

static object ObjectiveSnapshot(BattleState state) => new
{
    state.RequiresScoutSurvival,
    state.ScoutId,
    state.ScoutRescued,
    state.RelayStation,
    state.RelayCaptureProgress,
    state.RelaySecured,
    state.FuelCache,
    state.FuelCaptureProgress,
    state.FuelSecured,
    enemiesAlive = state.Units.Count(unit => unit.Team == Team.Enemy && unit.IsAlive)
};

static IReadOnlyList<object> UnitSnapshots(BattleState state) => state.Units
    .OrderBy(unit => unit.Team)
    .ThenBy(unit => unit.Id, StringComparer.Ordinal)
    .Select(unit => new
    {
        unit.Id,
        unit.Team,
        unit.Type,
        unit.Hp,
        unit.Position,
        unit.HasMoved,
        unit.HasActed,
        unit.IsAlive
    })
    .Cast<object>()
    .ToList();

sealed class PlaythroughLogWriter : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };
    private readonly StreamWriter _writer;
    private int _sequence;

    public PlaythroughLogWriter(string path, string playthroughId)
    {
        PlaythroughId = playthroughId;
        _writer = new StreamWriter(File.Create(path));
    }

    private string PlaythroughId { get; }

    public void Dispose() => _writer.Dispose();

    public void Write(string eventType, object payload)
    {
        var record = new
        {
            sequence = ++_sequence,
            eventType,
            playthroughId = PlaythroughId,
            timestampUtc = DateTimeOffset.UtcNow,
            payload
        };
        _writer.WriteLine(JsonSerializer.Serialize(record, JsonOptions));
        _writer.Flush();
    }
}

sealed record AiPlaytestOptions(int Runs, int MaxTurnsPerMission, string OutputDirectory)
{
    public static AiPlaytestOptions Parse(IReadOnlyList<string> args)
    {
        var runs = 1;
        var maxTurns = 30;
        var outputDirectory = Path.Combine(".copilot-tracking", "agentic", "runs", "playthrough-logs");

        foreach (var arg in args)
        {
            if (arg.StartsWith("--runs=", StringComparison.OrdinalIgnoreCase) && int.TryParse(arg[7..], out var parsedRuns))
            {
                runs = Math.Max(1, parsedRuns);
            }
            else if (arg.StartsWith("--max-turns=", StringComparison.OrdinalIgnoreCase) && int.TryParse(arg[12..], out var parsedTurns))
            {
                maxTurns = Math.Max(1, parsedTurns);
            }
            else if (arg.StartsWith("--output=", StringComparison.OrdinalIgnoreCase))
            {
                outputDirectory = arg[9..];
            }
        }

        return new AiPlaytestOptions(runs, maxTurns, outputDirectory);
    }
}

sealed record ReplayCommandStream(
    int FormatVersion,
    string RulesVersion,
    int SaveSchemaVersion,
    string MissionId,
    ulong RandomSeed,
    string InitialStateHash,
    string ExpectedFinalStateHash,
    IReadOnlyList<ReplayCommand> Commands);

sealed record ReplayCommand(string Kind, string? UnitId, ReplayTileCoord? Destination, string? TargetUnitId)
{
    public static ReplayCommand FromBattleCommand(BattleCommand command) => command.Kind switch
    {
        CommandKind.ActivatePower => new(command.Kind.ToString(), command.UnitId, null, null),
        CommandKind.Move => new(command.Kind.ToString(), command.UnitId, ReplayTileCoord.FromTileCoord(command.Destination), null),
        CommandKind.Attack => new(command.Kind.ToString(), command.UnitId, null, command.TargetUnitId),
        CommandKind.Wait => new(command.Kind.ToString(), command.UnitId, null, null),
        CommandKind.EndTurn => new(command.Kind.ToString(), null, null, null),
        _ => throw new InvalidOperationException($"Unsupported replay command kind: {command.Kind}.")
    };

    public BattleCommand ToBattleCommand()
    {
        var kind = Enum.Parse<CommandKind>(Kind, ignoreCase: true);
        return kind switch
        {
            CommandKind.ActivatePower => BattleCommand.ActivatePower(Require(UnitId, nameof(UnitId))),
            CommandKind.Move => BattleCommand.Move(Require(UnitId, nameof(UnitId)), Destination?.ToTileCoord() ?? TileCoord.None),
            CommandKind.Attack => BattleCommand.Attack(Require(UnitId, nameof(UnitId)), Require(TargetUnitId, nameof(TargetUnitId))),
            CommandKind.Wait => BattleCommand.Wait(Require(UnitId, nameof(UnitId))),
            CommandKind.EndTurn => BattleCommand.EndTurn(),
            _ => throw new InvalidOperationException($"Unsupported replay command kind: {Kind}.")
        };
    }

    private static string Require(string? value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"Replay command is missing {name}.")
        : value;
}

sealed record ReplayTileCoord(int X, int Y)
{
    public static ReplayTileCoord FromTileCoord(TileCoord coord) => new(coord.X, coord.Y);

    public TileCoord ToTileCoord() => new(X, Y);
}
