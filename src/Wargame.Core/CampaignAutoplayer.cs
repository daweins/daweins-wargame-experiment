namespace Wargame.Core;

public static class CampaignAutoplayer
{
    private const int LaterMissionBeamWidth = 6;
    private const int LaterMissionUnitPlanLimit = 8;

    public static IReadOnlyList<BattleCommand> ChoosePlayerTurn(BattleState state)
    {
        var scoutRescueTurn = ChooseScoutRescueTurn(state);
        if (scoutRescueTurn.Count > 0)
        {
            return scoutRescueTurn;
        }

        var objectiveCleanupTurn = ChooseObjectiveCleanupTurn(state);
        return objectiveCleanupTurn.Count > 0 ? objectiveCleanupTurn : ChooseBeamPlayerTurn(state);
    }

    public static IReadOnlyList<BattleCommand> ChooseGreedyPlayerTurn(BattleState state)
    {
        var commands = new List<BattleCommand>();
        var planningState = state.Clone();

        while (planningState.Outcome == BattleOutcome.InProgress)
        {
            var unit = ReadyPlayerUnits(planningState).FirstOrDefault();
            if (unit is null)
            {
                break;
            }

            var unitPlan = ChooseUnitPlans(planningState, unit);
            if (unitPlan.Count == 0)
            {
                break;
            }

            commands.AddRange(unitPlan);
            foreach (var command in unitPlan)
            {
                BattleRules.ApplyCommand(planningState, command);
                if (planningState.Outcome != BattleOutcome.InProgress)
                {
                    break;
                }
            }
        }

        return commands;
    }

    public static IReadOnlyList<BattleCommand> ChooseBeamPlayerTurn(BattleState state)
    {
        var beam = new List<PlannedTurn> { new([], state.Clone()) };

        while (beam.Count > 0)
        {
            var expandedAny = false;
            var candidates = new List<PlannedTurn>();

            foreach (var candidate in beam)
            {
                if (candidate.State.Outcome != BattleOutcome.InProgress)
                {
                    candidates.Add(candidate);
                    continue;
                }

                var unit = ReadyPlayerUnits(candidate.State).FirstOrDefault();
                if (unit is null)
                {
                    candidates.Add(candidate);
                    continue;
                }

                var unitPlans = BuildUnitPlans(candidate.State, unit)
                    .OrderByDescending(plan => ScorePlan(candidate.State, plan))
                    .ThenBy(plan => plan.Count)
                    .ThenBy(PlanSortKey)
                    .Take(LaterMissionUnitPlanLimit);

                foreach (var unitPlan in unitPlans)
                {
                    var clone = candidate.State.Clone();
                    foreach (var command in unitPlan)
                    {
                        BattleRules.ApplyCommand(clone, command);
                        if (clone.Outcome != BattleOutcome.InProgress)
                        {
                            break;
                        }
                    }

                    candidates.Add(new([.. candidate.Commands, .. unitPlan], clone));
                    expandedAny = true;
                }
            }

            if (!expandedAny)
            {
                break;
            }

            beam = candidates
                .OrderByDescending(candidate => EvaluateState(candidate.State))
                .ThenBy(candidate => candidate.Commands.Count)
                .ThenBy(candidate => PlanSortKey(candidate.Commands))
                .Take(LaterMissionBeamWidth)
                .ToList();
        }

        return beam
            .OrderByDescending(candidate => ScoreTurnPlan(state, candidate.Commands))
            .ThenBy(candidate => candidate.Commands.Count)
            .ThenBy(candidate => PlanSortKey(candidate.Commands))
            .FirstOrDefault(new PlannedTurn([], state.Clone()))
            .Commands;
    }

    private static IReadOnlyList<BattleCommand> ChooseObjectiveCleanupTurn(BattleState state)
    {
        if (state.Units.Any(unit => unit.Team == Team.Enemy && unit.IsAlive))
        {
            return [];
        }

        var incompleteObjectives = IncompleteObjectives(state).ToList();
        if (incompleteObjectives.Count == 0)
        {
            return [];
        }

        var commands = new List<BattleCommand>();
        var planningState = state.Clone();
        var unblockCommands = ChooseObjectiveUnblockCommands(planningState, incompleteObjectives);
        if (unblockCommands.Count > 0)
        {
            return unblockCommands;
        }

        foreach (var unit in ReadyPlayerUnits(planningState).Where(CanCaptureObjective).ToList())
        {
            var target = incompleteObjectives
                .OrderBy(objective => unit.Position.DistanceTo(objective))
                .ThenBy(objective => objective.Y)
                .ThenBy(objective => objective.X)
                .First();

            if (unit.Position == target)
            {
                var wait = BattleCommand.Wait(unit.Id);
                commands.Add(wait);
                BattleRules.ApplyCommand(planningState, wait);
                incompleteObjectives = IncompleteObjectives(planningState).ToList();
                if (incompleteObjectives.Count == 0 || planningState.Outcome != BattleOutcome.InProgress)
                {
                    break;
                }

                continue;
            }

            var destination = BattleRules.GetReachableTiles(planningState, unit)
                .OrderBy(coord => coord.DistanceTo(target))
                .ThenBy(coord => coord.Y)
                .ThenBy(coord => coord.X)
                .FirstOrDefault(unit.Position);
            if (destination == unit.Position || destination.DistanceTo(target) >= unit.Position.DistanceTo(target))
            {
                continue;
            }

            var move = BattleCommand.Move(unit.Id, destination);
            var waitAfterMove = BattleCommand.Wait(unit.Id);
            commands.Add(move);
            BattleRules.ApplyCommand(planningState, move);
            commands.Add(waitAfterMove);
            BattleRules.ApplyCommand(planningState, waitAfterMove);
        }

        return commands;
    }

    private static IReadOnlyList<BattleCommand> ChooseScoutRescueTurn(BattleState state)
    {
        if (!state.RequiresScoutSurvival || state.ScoutRescued)
        {
            return [];
        }

        var scout = state.Units.FirstOrDefault(unit => unit.Id == state.ScoutId && unit.IsAlive);
        if (scout is null)
        {
            return [];
        }

        var rescueTiles = scout.Position.Neighbors()
            .Where(state.Contains)
            .Where(coord => state.GetTerrain(coord) is not TerrainType.Ridge and not TerrainType.River)
            .Where(coord => BattleRules.GetLivingUnitAt(state, coord) is null)
            .ToHashSet();

        foreach (var unit in ReadyPlayerUnits(state).Where(unit => unit.Id != state.ScoutId))
        {
            if (unit.Position.DistanceTo(scout.Position) == 1)
            {
                return [BattleCommand.Wait(unit.Id)];
            }

            var destination = BattleRules.GetReachableTiles(state, unit)
                .Where(rescueTiles.Contains)
                .OrderBy(coord => coord.DistanceTo(scout.Position))
                .ThenBy(coord => coord.Y)
                .ThenBy(coord => coord.X)
                .FirstOrDefault(TileCoord.None);
            if (destination != TileCoord.None)
            {
                return [BattleCommand.Move(unit.Id, destination), BattleCommand.Wait(unit.Id)];
            }
        }

        return [];
    }

    private static IReadOnlyList<BattleCommand> ChooseObjectiveUnblockCommands(
        BattleState state,
        IReadOnlyList<TileCoord> incompleteObjectives)
    {
        foreach (var capturer in ReadyPlayerUnits(state).Where(CanCaptureObjective))
        {
            var target = incompleteObjectives
                .OrderBy(objective => capturer.Position.DistanceTo(objective))
                .ThenBy(objective => objective.Y)
                .ThenBy(objective => objective.X)
                .First();

            var bestDestination = BattleRules.GetReachableTiles(state, capturer)
                .OrderBy(coord => coord.DistanceTo(target))
                .ThenBy(coord => coord.Y)
                .ThenBy(coord => coord.X)
                .FirstOrDefault(capturer.Position);
            if (bestDestination.DistanceTo(target) < capturer.Position.DistanceTo(target))
            {
                continue;
            }

            var blocker = ReadyPlayerUnits(state)
                .Where(unit => unit.Id != capturer.Id)
                .Where(unit => unit.Position.DistanceTo(capturer.Position) == 1)
                .Where(unit => unit.Position.DistanceTo(target) < capturer.Position.DistanceTo(target))
                .OrderBy(unit => unit.Position.DistanceTo(target))
                .ThenBy(unit => unit.Id, StringComparer.Ordinal)
                .FirstOrDefault();
            if (blocker is null)
            {
                continue;
            }

            var blockerDestination = BattleRules.GetReachableTiles(state, blocker)
                .Where(coord => coord != blocker.Position)
                .OrderByDescending(coord => coord.DistanceTo(target))
                .ThenBy(coord => coord.Y)
                .ThenBy(coord => coord.X)
                .FirstOrDefault(TileCoord.None);
            if (blockerDestination == TileCoord.None)
            {
                continue;
            }

            return [BattleCommand.Move(blocker.Id, blockerDestination), BattleCommand.Wait(blocker.Id)];
        }

        return [];
    }

    private static IEnumerable<TileCoord> IncompleteObjectives(BattleState state)
    {
        if (state.RelayStation != TileCoord.None && !state.RelaySecured)
        {
            yield return state.RelayStation;
        }

        if (state.FuelCache != TileCoord.None && !state.FuelSecured)
        {
            yield return state.FuelCache;
        }
    }

    public static IReadOnlyList<BattleCommand> ChooseUnitPlans(BattleState state, UnitState unit) => BuildUnitPlans(state, unit)
        .OrderByDescending(plan => ScorePlan(state, plan))
        .Take(12)
        .FirstOrDefault([]);

    public static string DescribeCommand(BattleCommand command) => command.Kind switch
    {
        CommandKind.ActivatePower => $"ACTIVATE POWER {command.UnitId}",
        CommandKind.Move => $"MOVE {command.UnitId} to {command.Destination}",
        CommandKind.Attack => $"ATTACK {command.UnitId} -> {command.TargetUnitId}",
        CommandKind.Wait => $"WAIT {command.UnitId}",
        CommandKind.EndTurn => "END TURN",
        _ => command.Kind.ToString()
    };

    public static int EvaluateState(BattleState state)
    {
        if (state.Outcome == BattleOutcome.PlayerVictory)
        {
            return 100_000;
        }

        if (state.Outcome == BattleOutcome.PlayerDefeat)
        {
            return -100_000;
        }

        var playerUnits = state.Units.Where(unit => unit.Team == Team.Player && unit.IsAlive).ToList();
        var enemyUnits = state.Units.Where(unit => unit.Team == Team.Enemy && unit.IsAlive).ToList();
        var playerHp = playerUnits.Sum(unit => unit.Hp);
        var enemyHp = enemyUnits.Sum(unit => unit.Hp);
        var closestEnemyDistance = enemyUnits
            .Select(enemy => playerUnits
                .Select(player => enemy.Position.DistanceTo(player.Position))
                .DefaultIfEmpty(99)
                .Min())
            .DefaultIfEmpty(0)
            .Min();

        var score =
            (state.EnemyLosses * 1_200) -
            (state.PlayerLosses * 1_500) +
            (playerHp * 35) -
            (enemyHp * 55) -
            (closestEnemyDistance * 20) -
            (state.Turn * 15);

        score += ScoreScoutState(state, playerUnits, enemyUnits);
        score += ScoreObjectiveState(state, playerUnits);
        return score;
    }

    public static IEnumerable<UnitState> ReadyPlayerUnits(BattleState state) => state.Units
        .Where(unit => unit.Team == Team.Player && unit.IsAlive && !unit.HasActed && !BattleRules.IsScoutStranded(state, unit))
        .OrderBy(unit => ObjectiveUnitPriority(state, unit))
        .ThenBy(unit => unit.Type == UnitType.Armor ? 0 : unit.Type == UnitType.Lancer ? 1 : unit.Type == UnitType.SiegeBreaker ? 2 : unit.Type == UnitType.Infantry ? 3 : 4)
        .ThenBy(unit => unit.Id, StringComparer.Ordinal);

    private static IEnumerable<IReadOnlyList<BattleCommand>> BuildTurnPlans(BattleState state)
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

    private static IEnumerable<IReadOnlyList<BattleCommand>> BuildUnitPlans(BattleState state, UnitState unit)
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

    private static IEnumerable<IReadOnlyList<BattleCommand>> BuildAttackOrWaitPlans(BattleState state, UnitState unit, IReadOnlyList<BattleCommand> prefix)
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

    private static int ObjectiveUnitPriority(BattleState state, UnitState unit)
    {
        if (!CanCaptureObjective(unit))
        {
            return 5;
        }

        if (state.RelayStation != TileCoord.None && !state.RelaySecured)
        {
            return 0;
        }

        if (state.FuelCache != TileCoord.None && !state.FuelSecured)
        {
            return 1;
        }

        return 4;
    }

    private static int ScorePlan(BattleState state, IReadOnlyList<BattleCommand> plan)
    {
        var before = EvaluateState(state);
        var clone = state.Clone();
        foreach (var command in plan)
        {
            BattleRules.ApplyCommand(clone, command);
        }

        return EvaluateState(clone) - before;
    }

    private static int ScoreTurnPlan(BattleState state, IReadOnlyList<BattleCommand> plan)
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

    private static int ScoreScoutState(BattleState state, IReadOnlyList<UnitState> playerUnits, IReadOnlyList<UnitState> enemyUnits)
    {
        if (!state.RequiresScoutSurvival)
        {
            return 0;
        }

        var scout = state.Units.FirstOrDefault(unit => unit.Id == state.ScoutId);
        if (scout is null)
        {
            return 0;
        }

        var closestRescuerDistance = playerUnits
            .Where(unit => unit.Id != state.ScoutId)
            .Select(unit => unit.Position.DistanceTo(scout.Position))
            .DefaultIfEmpty(99)
            .Min();
        var closestEnemyToScout = enemyUnits
            .Select(unit => unit.Position.DistanceTo(scout.Position))
            .DefaultIfEmpty(8)
            .Min();

        return
            (state.ScoutRescued ? 2_500 : -closestRescuerDistance * 80) +
            (scout.IsAlive ? closestEnemyToScout * 120 : -4_000);
    }

    private static int ScoreObjectiveState(BattleState state, IReadOnlyList<UnitState> playerUnits)
    {
        var score = 0;
        if (state.RelayStation != TileCoord.None)
        {
            score += state.RelaySecured ? 4_000 : state.RelayCaptureProgress * 1_200;
            score -= ClosestObjectiveDistance(state, playerUnits, state.RelayStation) * 100;
        }

        if (state.FuelCache != TileCoord.None)
        {
            score += state.FuelSecured ? 4_000 : state.FuelCaptureProgress * 1_200;
            score -= ClosestObjectiveDistance(state, playerUnits, state.FuelCache) * 100;
        }

        return score;
    }

    private static int ClosestObjectiveDistance(BattleState state, IReadOnlyList<UnitState> playerUnits, TileCoord objective) => playerUnits
        .Where(CanCaptureObjective)
        .Select(unit => unit.Position.DistanceTo(objective))
        .DefaultIfEmpty(state.Width + state.Height)
        .Min();

    private static bool CanCaptureObjective(UnitState unit) => unit.Type is UnitType.Infantry or UnitType.Engineer or UnitType.FieldRig;

    private static string PlanSortKey(IReadOnlyList<BattleCommand> plan) => string.Join('|', plan.Select(CommandSortKey));

    private static string CommandSortKey(BattleCommand command) => command.Kind switch
    {
        CommandKind.Move => $"0:{command.UnitId}:{command.Destination.X:D2}:{command.Destination.Y:D2}",
        CommandKind.Attack => $"1:{command.UnitId}:{command.TargetUnitId}",
        CommandKind.Wait => $"2:{command.UnitId}",
        CommandKind.EndTurn => "3",
        _ => $"9:{command.Kind}:{command.UnitId}"
    };

    private sealed record PlannedTurn(IReadOnlyList<BattleCommand> Commands, BattleState State);
}