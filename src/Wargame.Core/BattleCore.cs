using System.Security.Cryptography;
using System.Text;

namespace Wargame.Core;

public enum BattleOutcome
{
    InProgress,
    PlayerVictory,
    PlayerDefeat
}

public enum CommandKind
{
    Move,
    Attack,
    Wait,
    EndTurn
}

public enum Team
{
    Player,
    Enemy
}

public enum TerrainType
{
    Plain,
    Road,
    Cover,
    Hq,
    Ridge
}

public enum UnitType
{
    Infantry,
    Armor,
    Scout
}

public readonly record struct BattleCommand(
    CommandKind Kind,
    string UnitId,
    TileCoord Destination,
    string TargetUnitId)
{
    public static BattleCommand Attack(string unitId, string targetUnitId) =>
        new(CommandKind.Attack, unitId, TileCoord.None, targetUnitId);

    public static BattleCommand EndTurn() => new(CommandKind.EndTurn, string.Empty, TileCoord.None, string.Empty);

    public static BattleCommand Move(string unitId, TileCoord destination) =>
        new(CommandKind.Move, unitId, destination, string.Empty);

    public static BattleCommand Wait(string unitId) => new(CommandKind.Wait, unitId, TileCoord.None, string.Empty);
}

public sealed record CommandResult(bool Success, string Message);

public sealed record CombatForecast(
    int ExpectedDamage,
    int MinimumDamage,
    int MaximumDamage,
    int CounterExpectedDamage,
    int CounterMinimumDamage,
    int CounterMaximumDamage);

public sealed record MissionScore(int Objective, int Speed, int Technique, int Power)
{
    public int Total => Objective + Speed + Technique + Power;
}

public sealed record UnitProfile(UnitType Type, int MaxHp, int Move, int Attack, int Defense);

public readonly record struct TileCoord(int X, int Y)
{
    public static TileCoord None => new(-1, -1);

    public int DistanceTo(TileCoord other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

    public IEnumerable<TileCoord> Neighbors()
    {
        yield return new TileCoord(X + 1, Y);
        yield return new TileCoord(X - 1, Y);
        yield return new TileCoord(X, Y + 1);
        yield return new TileCoord(X, Y - 1);
    }

    public override string ToString() => $"{X},{Y}";
}

public sealed class BattleState
{
    public BattleState(int width, int height, TerrainType[] terrain)
    {
        Width = width;
        Height = height;
        Terrain = terrain;
    }

    public Team ActiveTeam { get; set; } = Team.Player;

    public int CommandCount { get; set; }

    public int EnemyLosses { get; set; }

    public TileCoord EnemyHq { get; init; }

    public int Height { get; }

    public int InitialEnemyCount { get; set; }

    public BattleOutcome Outcome { get; set; } = BattleOutcome.InProgress;

    public TileCoord PlayerHq { get; init; }

    public int PlayerLosses { get; set; }

    public ulong RandomSeed { get; set; } = 0x5eed_2026UL;

    public string ScoutId { get; init; } = "p-scout";

    public bool ScoutRescued { get; set; }

    public TerrainType[] Terrain { get; }

    public int Turn { get; set; } = 1;

    public List<UnitState> Units { get; } = [];

    public int Width { get; }

    public bool IsComplete => Outcome is BattleOutcome.PlayerVictory or BattleOutcome.PlayerDefeat;

    public BattleState Clone()
    {
        var clone = new BattleState(Width, Height, [.. Terrain])
        {
            ActiveTeam = ActiveTeam,
            CommandCount = CommandCount,
            EnemyHq = EnemyHq,
            EnemyLosses = EnemyLosses,
            InitialEnemyCount = InitialEnemyCount,
            Outcome = Outcome,
            PlayerHq = PlayerHq,
            PlayerLosses = PlayerLosses,
            RandomSeed = RandomSeed,
            ScoutId = ScoutId,
            ScoutRescued = ScoutRescued,
            Turn = Turn
        };

        clone.Units.AddRange(Units.Select(unit => unit.Clone()));
        return clone;
    }

    public bool Contains(TileCoord coord) => coord.X >= 0 && coord.Y >= 0 && coord.X < Width && coord.Y < Height;

    public TerrainType GetTerrain(TileCoord coord) => Terrain[(coord.Y * Width) + coord.X];
}

public sealed class UnitState
{
    public required string Id { get; init; }

    public bool HasActed { get; set; }

    public bool HasMoved { get; set; }

    public int Hp { get; set; }

    public TileCoord Position { get; set; }

    public required Team Team { get; init; }

    public required UnitType Type { get; init; }

    public bool IsAlive => Hp > 0;

    public UnitState Clone() => new()
    {
        HasActed = HasActed,
        HasMoved = HasMoved,
        Hp = Hp,
        Id = Id,
        Position = Position,
        Team = Team,
        Type = Type
    };
}

public static class BattleRules
{
    private static readonly IReadOnlyDictionary<UnitType, UnitProfile> Profiles = new Dictionary<UnitType, UnitProfile>
    {
        [UnitType.Infantry] = new(UnitType.Infantry, 10, 3, 5, 1),
        [UnitType.Armor] = new(UnitType.Armor, 14, 4, 7, 3),
        [UnitType.Scout] = new(UnitType.Scout, 8, 5, 4, 0)
    };

    public static CommandResult ApplyCommand(BattleState state, BattleCommand command)
    {
        if (state.Outcome != BattleOutcome.InProgress)
        {
            return new CommandResult(false, "The mission is already over.");
        }

        return command.Kind switch
        {
            CommandKind.Move => ApplyMove(state, command.UnitId, command.Destination),
            CommandKind.Attack => ApplyAttack(state, command.UnitId, command.TargetUnitId),
            CommandKind.Wait => ApplyWait(state, command.UnitId),
            CommandKind.EndTurn => ApplyEndTurn(state),
            _ => new CommandResult(false, "Unknown command.")
        };
    }

    public static MissionScore CalculateScore(BattleState state)
    {
        var objective = state.Outcome == BattleOutcome.PlayerVictory ? 70 : 10;
        objective += state.ScoutRescued ? 25 : 0;
        objective += state.Outcome != BattleOutcome.PlayerDefeat ? 5 : 0;
        objective = Math.Clamp(objective, 0, 100);

        var speed = Math.Clamp(100 - Math.Max(0, state.Turn - 6) * 12, 0, 100);
        var technique = Math.Clamp(100 - state.PlayerLosses * 30, 0, 100);
        var power = state.InitialEnemyCount == 0
            ? 0
            : Math.Clamp((int)Math.Round(state.EnemyLosses * 100.0 / state.InitialEnemyCount), 0, 100);

        return new MissionScore(objective, speed, technique, power);
    }

    public static IReadOnlyList<TileCoord> GetAttackableCoords(BattleState state, UnitState attacker)
    {
        if (!CanAct(state, attacker))
        {
            return [];
        }

        return attacker.Position.Neighbors()
            .Where(state.Contains)
            .Where(coord => GetLivingUnitAt(state, coord)?.Team != attacker.Team)
            .Where(coord => GetLivingUnitAt(state, coord) is not null)
            .OrderBy(coord => coord.Y)
            .ThenBy(coord => coord.X)
            .ToList();
    }

    public static CombatForecast GetCombatForecast(BattleState state, UnitState attacker, UnitState defender)
    {
        var expectedDamage = ExpectedDamage(state, attacker, defender);
        var counterExpectedDamage = CanCounterAttack(attacker, defender)
            ? ExpectedDamage(state, defender, attacker)
            : 0;

        return new CombatForecast(
            expectedDamage,
            Math.Clamp(expectedDamage - 3, 1, defender.Hp),
            Math.Clamp(expectedDamage + 3, 1, defender.Hp),
            counterExpectedDamage,
            counterExpectedDamage == 0 ? 0 : Math.Clamp(counterExpectedDamage - 3, 1, attacker.Hp),
            counterExpectedDamage == 0 ? 0 : Math.Clamp(counterExpectedDamage + 3, 1, attacker.Hp));
    }

    public static UnitState? GetLivingUnitAt(BattleState state, TileCoord coord) =>
        state.Units.FirstOrDefault(unit => unit.IsAlive && unit.Position == coord);

    public static UnitProfile GetProfile(UnitType type) => Profiles[type];

    public static IReadOnlyList<TileCoord> GetReachableTiles(BattleState state, UnitState unit)
    {
        if (!CanMove(state, unit))
        {
            return [];
        }

        var profile = GetProfile(unit.Type);
        var frontier = new Queue<TileCoord>();
        var remainingMove = new Dictionary<TileCoord, int> { [unit.Position] = profile.Move };
        frontier.Enqueue(unit.Position);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            var currentRemaining = remainingMove[current];

            foreach (var neighbor in current.Neighbors().Where(state.Contains))
            {
                if (IsBlockedForMovement(state, unit, neighbor))
                {
                    continue;
                }

                var cost = GetMoveCost(state.GetTerrain(neighbor));
                if (cost > currentRemaining)
                {
                    continue;
                }

                var nextRemaining = currentRemaining - cost;
                if (remainingMove.TryGetValue(neighbor, out var knownRemaining) && knownRemaining >= nextRemaining)
                {
                    continue;
                }

                remainingMove[neighbor] = nextRemaining;
                frontier.Enqueue(neighbor);
            }
        }

        return remainingMove.Keys.OrderBy(coord => coord.Y).ThenBy(coord => coord.X).ToList();
    }

    public static string GetStateHash(BattleState state)
    {
        var builder = new StringBuilder();
        builder.Append($"turn={state.Turn};team={state.ActiveTeam};outcome={state.Outcome};scout={state.ScoutRescued};seed={state.RandomSeed};");
        foreach (var unit in state.Units.OrderBy(unit => unit.Id, StringComparer.Ordinal))
        {
            builder.Append($"{unit.Id}:{unit.Team}:{unit.Type}:{unit.Hp}:{unit.Position.X}:{unit.Position.Y}:{unit.HasMoved}:{unit.HasActed};");
        }

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()));
        return Convert.ToHexString(bytes);
    }

    public static bool IsScoutStranded(BattleState state, UnitState unit) =>
        unit.Id == state.ScoutId && !state.ScoutRescued;

    private static CommandResult ApplyAttack(BattleState state, string unitId, string targetUnitId)
    {
        var attacker = FindUnit(state, unitId);
        var defender = FindUnit(state, targetUnitId);
        if (attacker is null || defender is null || !attacker.IsAlive || !defender.IsAlive)
        {
            return new CommandResult(false, "No valid target.");
        }

        if (!CanAct(state, attacker) || attacker.Team != state.ActiveTeam || defender.Team == attacker.Team)
        {
            return new CommandResult(false, "That unit cannot attack now.");
        }

        if (attacker.Position.DistanceTo(defender.Position) != 1)
        {
            return new CommandResult(false, "Targets must be adjacent for this prototype.");
        }

        var attackDamage = RollDamage(state, attacker, defender);
        ApplyDamage(state, defender, attackDamage);
        var message = $"{attacker.Id} hits {defender.Id} for {attackDamage}.";

        if (defender.IsAlive && CanCounterAttack(attacker, defender))
        {
            var counterDamage = RollDamage(state, defender, attacker);
            ApplyDamage(state, attacker, counterDamage);
            message += $" Counterfire deals {counterDamage}.";
        }

        attacker.HasActed = true;
        state.CommandCount++;
        TryAutoRescue(state);
        CheckObjectives(state);
        return new CommandResult(true, message);
    }

    private static CommandResult ApplyEndTurn(BattleState state)
    {
        state.CommandCount++;
        if (state.ActiveTeam == Team.Player)
        {
            ResetTeam(state, Team.Enemy);
            state.ActiveTeam = Team.Enemy;
            RunEnemyTurn(state);

            if (state.Outcome == BattleOutcome.InProgress)
            {
                state.Turn++;
                ResetTeam(state, Team.Player);
                state.ActiveTeam = Team.Player;
                TryAutoRescue(state);
                CheckObjectives(state);
            }

            return new CommandResult(true, "Enemy pressure phase resolved.");
        }

        state.Turn++;
        ResetTeam(state, Team.Player);
        state.ActiveTeam = Team.Player;
        CheckObjectives(state);
        return new CommandResult(true, "Player turn begins.");
    }

    private static CommandResult ApplyMove(BattleState state, string unitId, TileCoord destination)
    {
        var unit = FindUnit(state, unitId);
        if (unit is null || !unit.IsAlive)
        {
            return new CommandResult(false, "No valid unit selected.");
        }

        if (!CanMove(state, unit) || unit.Team != state.ActiveTeam)
        {
            return new CommandResult(false, "That unit cannot move now.");
        }

        var reachableTiles = GetReachableTiles(state, unit);
        if (!reachableTiles.Contains(destination))
        {
            return new CommandResult(false, "That tile is out of range.");
        }

        unit.Position = destination;
        unit.HasMoved = true;
        state.CommandCount++;
        TryAutoRescue(state);
        CheckObjectives(state);
        return new CommandResult(true, $"{unit.Id} moves to {destination}.");
    }

    private static CommandResult ApplyWait(BattleState state, string unitId)
    {
        var unit = FindUnit(state, unitId);
        if (unit is null || !unit.IsAlive || unit.Team != state.ActiveTeam || unit.HasActed)
        {
            return new CommandResult(false, "That unit cannot wait now.");
        }

        unit.HasActed = true;
        state.CommandCount++;
        TryAutoRescue(state);
        CheckObjectives(state);
        return new CommandResult(true, $"{unit.Id} holds position.");
    }

    private static bool CanAct(BattleState state, UnitState unit) =>
        unit.IsAlive && !unit.HasActed && !IsScoutStranded(state, unit);

    private static bool CanCounterAttack(UnitState attacker, UnitState defender) =>
        defender.Position.DistanceTo(attacker.Position) == 1 && defender.Type != UnitType.Scout;

    private static bool CanMove(BattleState state, UnitState unit) =>
        unit.IsAlive && !unit.HasMoved && !unit.HasActed && !IsScoutStranded(state, unit);

    private static void CheckObjectives(BattleState state)
    {
        if (state.Outcome != BattleOutcome.InProgress)
        {
            return;
        }

        var scout = FindUnit(state, state.ScoutId);
        if (scout is null || !scout.IsAlive)
        {
            state.Outcome = BattleOutcome.PlayerDefeat;
            return;
        }

        if (state.Units.Any(unit => unit.IsAlive && unit.Team == Team.Enemy && unit.Position == state.PlayerHq))
        {
            state.Outcome = BattleOutcome.PlayerDefeat;
            return;
        }

        var enemiesAlive = state.Units.Any(unit => unit.IsAlive && unit.Team == Team.Enemy);
        if (state.ScoutRescued && !enemiesAlive)
        {
            state.Outcome = BattleOutcome.PlayerVictory;
        }
    }

    private static int ExpectedDamage(BattleState state, UnitState attacker, UnitState defender)
    {
        var attackerProfile = GetProfile(attacker.Type);
        var defenderProfile = GetProfile(defender.Type);
        var matchup = GetMatchupModifier(attacker.Type, defender.Type);
        var terrainDefense = GetTerrainDefense(state.GetTerrain(defender.Position));
        var healthScale = Math.Max(1, attacker.Hp) / 4;
        return Math.Clamp(attackerProfile.Attack + healthScale + matchup - defenderProfile.Defense - terrainDefense, 1, defender.Hp);
    }

    private static UnitState? FindUnit(BattleState state, string unitId) =>
        state.Units.FirstOrDefault(unit => string.Equals(unit.Id, unitId, StringComparison.Ordinal));

    private static TileCoord FindEnemyTarget(BattleState state)
    {
        var scout = FindUnit(state, state.ScoutId);
        if (scout is { IsAlive: true } && !state.ScoutRescued)
        {
            return scout.Position;
        }

        return state.PlayerHq;
    }

    private static UnitState? FindPlayerTargetInRange(BattleState state, UnitState enemy) =>
        enemy.Position.Neighbors()
            .Select(coord => GetLivingUnitAt(state, coord))
            .Where(unit => unit is { Team: Team.Player })
            .Where(unit => !IsScoutProtectedByOpeningGrace(state, unit!))
            .OrderBy(unit => unit!.Id == state.ScoutId ? 0 : 1)
            .ThenBy(unit => unit!.Hp)
            .FirstOrDefault();

    private static int GetMatchupModifier(UnitType attacker, UnitType defender) => (attacker, defender) switch
    {
        (UnitType.Armor, UnitType.Infantry) => 2,
        (UnitType.Infantry, UnitType.Scout) => 1,
        (UnitType.Scout, UnitType.Infantry) => 1,
        (UnitType.Infantry, UnitType.Armor) => -2,
        (UnitType.Scout, UnitType.Armor) => -3,
        _ => 0
    };

    private static int GetMoveCost(TerrainType terrain) => terrain switch
    {
        TerrainType.Road => 1,
        TerrainType.Plain => 1,
        TerrainType.Hq => 1,
        TerrainType.Cover => 2,
        TerrainType.Ridge => 99,
        _ => 1
    };

    private static int GetTerrainDefense(TerrainType terrain) => terrain switch
    {
        TerrainType.Cover => 2,
        TerrainType.Hq => 3,
        _ => 0
    };

    private static bool IsBlockedForMovement(BattleState state, UnitState movingUnit, TileCoord coord)
    {
        if (state.GetTerrain(coord) == TerrainType.Ridge)
        {
            return true;
        }

        var occupant = GetLivingUnitAt(state, coord);
        return occupant is not null && occupant.Id != movingUnit.Id;
    }

    private static bool IsScoutProtectedByOpeningGrace(BattleState state, UnitState unit) =>
        state.Turn == 1 && IsScoutStranded(state, unit);

    private static ulong NextRandom(BattleState state)
    {
        state.RandomSeed = (state.RandomSeed * 6364136223846793005UL) + 1442695040888963407UL;
        return state.RandomSeed;
    }

    private static void ApplyDamage(BattleState state, UnitState defender, int damage)
    {
        if (!defender.IsAlive)
        {
            return;
        }

        defender.Hp = Math.Max(0, defender.Hp - damage);
        if (defender.Hp > 0)
        {
            return;
        }

        if (defender.Team == Team.Player)
        {
            state.PlayerLosses++;
        }
        else
        {
            state.EnemyLosses++;
        }
    }

    private static void ResetTeam(BattleState state, Team team)
    {
        foreach (var unit in state.Units.Where(unit => unit.Team == team && unit.IsAlive))
        {
            unit.HasActed = false;
            unit.HasMoved = false;
        }
    }

    private static int RollDamage(BattleState state, UnitState attacker, UnitState defender)
    {
        var expectedDamage = ExpectedDamage(state, attacker, defender);
        var variance = (int)(NextRandom(state) % 3) - 1;
        var rareRoll = (int)(NextRandom(state) % 100);
        var rareModifier = rareRoll < 5 ? 2 : rareRoll >= 95 ? -2 : 0;
        return Math.Clamp(expectedDamage + variance + rareModifier, 1, defender.Hp);
    }

    private static void RunEnemyTurn(BattleState state)
    {
        foreach (var enemy in state.Units.Where(unit => unit.IsAlive && unit.Team == Team.Enemy).OrderBy(unit => unit.Id, StringComparer.Ordinal))
        {
            if (state.Outcome != BattleOutcome.InProgress)
            {
                return;
            }

            var adjacentTarget = FindPlayerTargetInRange(state, enemy);
            if (adjacentTarget is not null)
            {
                _ = ApplyAttack(state, enemy.Id, adjacentTarget.Id);
                continue;
            }

            var targetCoord = FindEnemyTarget(state);
            var bestMove = GetReachableTiles(state, enemy)
                .Where(coord => coord == enemy.Position || GetLivingUnitAt(state, coord) is null)
                .OrderBy(coord => coord.DistanceTo(targetCoord))
                .ThenBy(coord => state.GetTerrain(coord) == TerrainType.Cover ? 0 : 1)
                .ThenBy(coord => coord.Y)
                .ThenBy(coord => coord.X)
                .FirstOrDefault(enemy.Position);

            if (bestMove != enemy.Position)
            {
                _ = ApplyMove(state, enemy.Id, bestMove);
            }

            if (enemy.Position == state.PlayerHq)
            {
                state.Outcome = BattleOutcome.PlayerDefeat;
                return;
            }

            adjacentTarget = FindPlayerTargetInRange(state, enemy);
            if (adjacentTarget is not null && !enemy.HasActed)
            {
                _ = ApplyAttack(state, enemy.Id, adjacentTarget.Id);
            }
        }

        CheckObjectives(state);
    }

    private static void TryAutoRescue(BattleState state)
    {
        if (state.ScoutRescued)
        {
            return;
        }

        var scout = FindUnit(state, state.ScoutId);
        if (scout is not { IsAlive: true })
        {
            return;
        }

        var rescuerAdjacent = state.Units.Any(unit =>
            unit.IsAlive &&
            unit.Team == Team.Player &&
            unit.Id != state.ScoutId &&
            unit.Position.DistanceTo(scout.Position) == 1);

        if (rescuerAdjacent)
        {
            state.ScoutRescued = true;
        }
    }
}

public static class FirstMissionFactory
{
    public static BattleState Create()
    {
        const int width = 12;
        const int height = 8;
        var terrain = Enumerable.Repeat(TerrainType.Plain, width * height).ToArray();
        SetRow(terrain, width, 0, TerrainType.Ridge);
        SetRow(terrain, width, 7, TerrainType.Ridge);

        foreach (var coord in new[]
        {
            new TileCoord(1, 3), new TileCoord(2, 3), new TileCoord(3, 3), new TileCoord(4, 3),
            new TileCoord(5, 3), new TileCoord(6, 3), new TileCoord(7, 3), new TileCoord(8, 3),
            new TileCoord(9, 3), new TileCoord(10, 3), new TileCoord(2, 4), new TileCoord(3, 4),
            new TileCoord(4, 4), new TileCoord(5, 4), new TileCoord(6, 4), new TileCoord(7, 4),
            new TileCoord(8, 4), new TileCoord(9, 4), new TileCoord(10, 4), new TileCoord(5, 2),
            new TileCoord(6, 2), new TileCoord(7, 2), new TileCoord(7, 5), new TileCoord(8, 5),
            new TileCoord(9, 5), new TileCoord(10, 5)
        })
        {
            terrain[(coord.Y * width) + coord.X] = TerrainType.Road;
        }

        foreach (var coord in new[]
        {
            new TileCoord(4, 2), new TileCoord(4, 5), new TileCoord(6, 5), new TileCoord(8, 2),
            new TileCoord(8, 5), new TileCoord(9, 4)
        })
        {
            terrain[(coord.Y * width) + coord.X] = TerrainType.Cover;
        }

        var playerHq = new TileCoord(1, 3);
        var enemyHq = new TileCoord(10, 3);
        terrain[(playerHq.Y * width) + playerHq.X] = TerrainType.Hq;
        terrain[(enemyHq.Y * width) + enemyHq.X] = TerrainType.Hq;

        var state = new BattleState(width, height, terrain)
        {
            EnemyHq = enemyHq,
            InitialEnemyCount = 5,
            PlayerHq = playerHq,
            RandomSeed = 0x16b1_7a11_2026UL,
            ScoutId = "Scout-7"
        };

        state.Units.AddRange([
            CreateUnit("Infantry-1", Team.Player, UnitType.Infantry, new TileCoord(2, 3)),
            CreateUnit("Infantry-2", Team.Player, UnitType.Infantry, new TileCoord(2, 4)),
            CreateUnit("Armor-1", Team.Player, UnitType.Armor, new TileCoord(3, 5)),
            CreateUnit("Scout-7", Team.Player, UnitType.Scout, new TileCoord(5, 2), hp: 6),
            CreateUnit("Raider-A", Team.Enemy, UnitType.Infantry, new TileCoord(8, 3), hp: 8),
            CreateUnit("Raider-B", Team.Enemy, UnitType.Infantry, new TileCoord(9, 2), hp: 8),
            CreateUnit("Bulwark", Team.Enemy, UnitType.Armor, new TileCoord(10, 4), hp: 9),
            CreateUnit("Skirmisher", Team.Enemy, UnitType.Scout, new TileCoord(7, 5), hp: 7),
            CreateUnit("Picket", Team.Enemy, UnitType.Infantry, new TileCoord(10, 5), hp: 8)
        ]);

        return state;
    }

    private static UnitState CreateUnit(string id, Team team, UnitType type, TileCoord position, int? hp = null)
    {
        var profile = BattleRules.GetProfile(type);
        return new UnitState
        {
            Hp = hp ?? profile.MaxHp,
            Id = id,
            Position = position,
            Team = team,
            Type = type
        };
    }

    private static void SetRow(TerrainType[] terrain, int width, int row, TerrainType type)
    {
        for (var column = 0; column < width; column++)
        {
            terrain[(row * width) + column] = type;
        }
    }
}
