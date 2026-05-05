namespace Wargame.Core;

public static class CampaignMissionCatalog
{
    public const int FinalMissionNumber = 10;

    public static CampaignMissionBrief GetBrief(int missionNumber) => missionNumber switch
    {
        1 => new(
            1,
            "mission1",
            "Scout-7 Is Late",
            "A survey camp becomes a defensive line before dawn.",
            "Hold HQ, reach Scout-7, then defeat raiders.",
            "Rescue Scout-7: move infantry or armor to a tile directly next to them.",
            "Venn: We came here to measure rock, not doctrine.",
            "Sloane: Scout-7 is back inside the perimeter. Now the questions get louder.",
            "Venn: Pull the recording. We missed the decision point."),
        2 => new(
            2,
            "mission2",
            "Inventory Adjustment",
            "The relay yard and fuel cache become the next battlefield.",
            "Capture the Relay (R) and Fuel Cache (F), then defeat raiders.",
            "Capture R/F: move a Tech or Engineer onto each marker and Wait there twice.",
            "Priya: Good news, the relay came back up. Bad news, Orison got there first.",
            "Priya: Relay authenticated, fuel accounted for, paperwork alarmingly useful.",
            "Sloane: We do not lose the relay and call it a field lesson."),
        3 => new(
            3,
            "mission3",
            "Pump Road Convoy",
            "A civilian pump station road becomes the only route out.",
            "Break the ambush and clear the convoy road.",
            "Keep the road open. Use armor to hold lanes and scouts to finish damaged raiders.",
            "Yara: The pump crews are moving because we asked. Keep the promise attached.",
            "Yara: Convoy is through. Nobody on the trucks saw the worst of it.",
            "Yara: If the road closes, the crews do not get a second evacuation."),
        4 => new(
            4,
            "mission4",
            "Depot Permission",
            "A field fabricator can turn a depot into a foothold.",
            "Capture the depot node, then rout the Orison screen.",
            "Wait twice with infantry, engineer, or field rig on the marked depot node.",
            "Sloane: We are not stealing a depot. We are interrupting improper custody.",
            "Sloane: The fabricator is ours. Try not to name it something sentimental.",
            "Priya: Without the depot, we are back to counting bolts by flashlight."),
        5 => new(
            5,
            "mission5",
            "Soft Fog",
            "Antennas and false returns hide the next contact.",
            "Secure the antenna and destroy the jammer guard.",
            "Capture the antenna with infantry or engineer, then clear nearby raiders.",
            "Venn: The map is lying politely. I dislike when equipment develops manners.",
            "Venn: Antenna is clean. Sable was listening before Orison arrived.",
            "Venn: Blind is not brave. Blind is just math with missing inputs."),
        6 => new(
            6,
            "mission6",
            "Bridge Fuse",
            "A chokepoint bridge turns retreat into a counteroffensive.",
            "Hold the bridge and defeat the demolition team.",
            "Control the center road. Sappers are fragile but dangerous near support units.",
            "Sloane: If the bridge goes, so does our timetable. I dislike gifts to Orison.",
            "Sloane: Bridge intact. Enemy timetable ruined. Acceptable symmetry.",
            "Sloane: Losing the bridge means fighting the same battle three times worse."),
        7 => new(
            7,
            "mission7",
            "Meridian Line",
            "Fast Meridian raiders test restraint near a settlement grid.",
            "Rout the fast raiders without losing the HQ grid.",
            "Use scouts and armor to screen the HQ while infantry finishes damaged units.",
            "Yara: Those lights are homes, not cover markers. Keep the fight off them.",
            "Yara: Settlement grid is dark but intact. Meridian will remember that.",
            "Yara: If the HQ grid falls, the settlement pays for our hesitation."),
        8 => new(
            8,
            "mission8",
            "Blackout Audit",
            "Treaty auditors arrive during a deliberate blackout.",
            "Secure the audit convoy beacon and clear the blackout force.",
            "Capture the audit beacon, then rout the attackers before the convoy scatters.",
            "Priya: An audit convoy in a blackout. Someone is either brave or very scheduled.",
            "Priya: Beacon restored. The sealed file did not enjoy being found.",
            "Priya: Lose the auditors and the truth becomes a rumor with better lawyers."),
        9 => new(
            9,
            "mission9",
            "Ridge Ghosts",
            "Fogged ridges hide the scan data route out of the basin.",
            "Destroy the jammer line and hold the scan-data exit.",
            "Capture the scan relay, then defeat the ridge guard.",
            "Venn: The ridge returns three maps. I trust the one that scares me least.",
            "Venn: Scan packet escaped. The basin is not doing what the contract says.",
            "Venn: If the packet dies here, so does our only clean evidence."),
        10 => new(
            10,
            "mission10",
            "Refinery Lock",
            "The first act ends at Orison's refinery command node.",
            "Capture the refinery HQ or rout Sloane's opposite number.",
            "Capture the refinery command node and defeat the remaining defenders.",
            "Sloane: This is the part where Orison discovers we can count income too.",
            "Sloane: Refinery node is ours. The broadcast opens Act 2 whether they like it or not.",
            "Sloane: If we fail here, the grid keeps speaking only for Orison."),
        _ => throw new ArgumentOutOfRangeException(nameof(missionNumber), "Campaign missions run from 1 through 10.")
    };
}

public static class CampaignMissionFactory
{
    public static BattleState Create(int missionNumber)
    {
        var state = missionNumber switch
        {
            1 => FirstMissionFactory.Create(),
            2 => SecondMissionFactory.Create(),
            3 => BuildMission(MissionSpec.Mission3()),
            4 => BuildMission(MissionSpec.Mission4()),
            5 => BuildMission(MissionSpec.Mission5()),
            6 => BuildMission(MissionSpec.Mission6()),
            7 => BuildMission(MissionSpec.Mission7()),
            8 => BuildMission(MissionSpec.Mission8()),
            9 => BuildMission(MissionSpec.Mission9()),
            10 => BuildMission(MissionSpec.Mission10()),
            _ => throw new ArgumentOutOfRangeException(nameof(missionNumber), "Campaign missions run from 1 through 10.")
        };

        ApplyBrief(state, CampaignMissionCatalog.GetBrief(missionNumber));
        state.InitialEnemyCount = state.Units.Count(unit => unit.Team == Team.Enemy);
        return state;
    }

    private static void ApplyBrief(BattleState state, CampaignMissionBrief brief)
    {
        state.MissionNumber = brief.Number;
        state.MissionSubtitle = brief.Subtitle;
        state.ObjectiveSummary = brief.ObjectiveSummary;
        state.RescueInstruction = brief.RescueInstruction;
        state.IntroLine = brief.IntroLine;
        state.VictoryLine = brief.VictoryLine;
        state.DefeatLine = brief.DefeatLine;
    }

    private static BattleState BuildMission(MissionSpec spec)
    {
        var width = spec.Width;
        var height = spec.Height;
        var terrain = Enumerable.Repeat(TerrainType.Plain, width * height).ToArray();
        SetRow(terrain, width, 0, TerrainType.Ridge);
        SetRow(terrain, width, height - 1, TerrainType.Ridge);

        foreach (var coord in spec.Rivers ?? [])
        {
            SetTile(terrain, width, height, coord, TerrainType.River);
        }

        foreach (var coord in spec.Roads)
        {
            SetTile(terrain, width, height, coord, TerrainType.Road);
        }

        foreach (var coord in spec.Cover)
        {
            SetTile(terrain, width, height, coord, TerrainType.Cover);
        }

        foreach (var coord in spec.Ridges)
        {
            SetTile(terrain, width, height, coord, TerrainType.Ridge);
        }

        foreach (var coord in spec.Workshops ?? [])
        {
            SetTile(terrain, width, height, coord, TerrainType.Workshop);
        }

        SetTile(terrain, width, height, spec.PlayerHq, TerrainType.Hq);
        SetTile(terrain, width, height, spec.EnemyHq, TerrainType.Hq);
        if (spec.RelayStation != TileCoord.None)
        {
            SetTile(terrain, width, height, spec.RelayStation, TerrainType.Hq);
        }

        if (spec.FuelCache != TileCoord.None)
        {
            SetTile(terrain, width, height, spec.FuelCache, TerrainType.Hq);
        }

        var brief = CampaignMissionCatalog.GetBrief(spec.Number);
        var state = new BattleState(width, height, terrain)
        {
            EnemyHq = spec.EnemyHq,
            FuelCache = spec.FuelCache,
            FuelObjectiveName = spec.FuelObjectiveName,
            MissionId = brief.Id,
            MissionNumber = brief.Number,
            MissionSubtitle = brief.Subtitle,
            MissionTitle = brief.Title,
            ObjectiveSummary = brief.ObjectiveSummary,
            PlayerHq = spec.PlayerHq,
            RandomSeed = spec.RandomSeed,
            RelayStation = spec.RelayStation,
            RelayObjectiveName = spec.RelayObjectiveName,
            RequiresScoutSurvival = false,
            RequiresRoutAfterObjectives = true,
            RescueInstruction = brief.RescueInstruction,
            ScoutId = "Scout-7",
            ScoutRescued = true,
            IntroLine = brief.IntroLine,
            VictoryLine = brief.VictoryLine,
            DefeatLine = brief.DefeatLine
        };

        state.Units.AddRange(spec.PlayerUnits.Select(CreateUnit));
        state.Units.AddRange(spec.EnemyUnits.Select(CreateUnit));
        state.InitialEnemyCount = state.Units.Count(unit => unit.Team == Team.Enemy);
        return state;
    }

    private static UnitState CreateUnit(UnitSpec spec)
    {
        var profile = BattleRules.GetProfile(spec.Type);
        return new UnitState
        {
            Hp = spec.Hp ?? profile.MaxHp,
            Id = spec.Id,
            Position = spec.Position,
            Team = spec.Team,
            Type = spec.Type
        };
    }

    private static void SetRow(TerrainType[] terrain, int width, int row, TerrainType type)
    {
        for (var column = 0; column < width; column++)
        {
            terrain[(row * width) + column] = type;
        }
    }

    private static void SetTile(TerrainType[] terrain, int width, int height, TileCoord coord, TerrainType type)
    {
        if (coord.X < 0 || coord.Y < 0 || coord.X >= width || coord.Y >= height)
        {
            return;
        }

        terrain[(coord.Y * width) + coord.X] = type;
    }

    private sealed record UnitSpec(string Id, Team Team, UnitType Type, TileCoord Position, int? Hp = null);

    private sealed record MissionSpec(
        int Number,
        ulong RandomSeed,
        TileCoord PlayerHq,
        TileCoord EnemyHq,
        TileCoord RelayStation,
        string RelayObjectiveName,
        TileCoord FuelCache,
        string FuelObjectiveName,
        IReadOnlyList<TileCoord> Roads,
        IReadOnlyList<TileCoord> Cover,
        IReadOnlyList<TileCoord> Ridges,
        IReadOnlyList<UnitSpec> PlayerUnits,
        IReadOnlyList<UnitSpec> EnemyUnits,
        int Width = 12,
        int Height = 8,
        IReadOnlyList<TileCoord>? Rivers = null,
        IReadOnlyList<TileCoord>? Workshops = null)
    {
        private static readonly TileCoord[] SpineRoads = MergePaths(
            Path(new(0, 3), new(11, 3)),
            Path(new(3, 3), new(3, 2), new(8, 2)),
            Path(new(3, 3), new(3, 5), new(8, 5)));

        public static MissionSpec Mission3() => new(
            3,
            0x2026_0513_0003UL,
            new TileCoord(0, 4),
            new TileCoord(15, 4),
            TileCoord.None,
            "Convoy road",
            TileCoord.None,
            "Pump station",
            MergePaths(
                Path(new(0, 4), new(15, 4)),
                Path(new(3, 4), new(3, 2), new(10, 2)),
                Path(new(4, 4), new(4, 7), new(11, 7)),
                Path(new(10, 4), new(10, 2))),
            [new(5, 6), new(7, 3), new(10, 5), new(12, 2), new(13, 6)],
            [new(2, 8), new(11, 1), new(12, 8)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 4)),
                new("Scout-7", Team.Player, UnitType.Scout, new TileCoord(2, 6), 7),
                new("Lancer-1", Team.Player, UnitType.Lancer, new TileCoord(1, 5)),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(2, 3))
            ],
            [
                new("Ambush-A", Team.Enemy, UnitType.Infantry, new TileCoord(8, 4), 8),
                new("Road-Wolf", Team.Enemy, UnitType.Scout, new TileCoord(10, 2), 7),
                new("Lancer-A", Team.Enemy, UnitType.Lancer, new TileCoord(10, 5), 8),
                new("Bulwark", Team.Enemy, UnitType.Armor, new TileCoord(13, 4), 10),
                new("Sapper-A", Team.Enemy, UnitType.Sapper, new TileCoord(12, 7))
            ],
            16,
            10,
            Path(new(6, 1), new(6, 8)),
            [new(3, 6)]);

        public static MissionSpec Mission4() => new(
            4,
            0x2026_0514_0004UL,
            new TileCoord(0, 4),
            new TileCoord(14, 4),
            new TileCoord(11, 3),
            "Depot node",
            TileCoord.None,
            "Fabricator",
            MergePaths(
                Path(new(0, 4), new(14, 4)),
                Path(new(3, 4), new(3, 6), new(11, 6)),
                Path(new(5, 4), new(5, 2), new(11, 2)),
                Path(new(11, 2), new(11, 4))),
            [new(5, 1), new(7, 3), new(12, 5)],
            [new(2, 8), new(13, 1)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 4)),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(2, 5)),
                new("Field-Rig", Team.Player, UnitType.FieldRig, new TileCoord(1, 6)),
                new("Scout-7", Team.Player, UnitType.Scout, new TileCoord(2, 2), 8)
            ],
            [
                new("Depot-Guard", Team.Enemy, UnitType.Armor, new TileCoord(11, 4), 11),
                new("Picket-A", Team.Enemy, UnitType.Infantry, new TileCoord(10, 2), 8),
                new("Picket-B", Team.Enemy, UnitType.Infantry, new TileCoord(12, 5), 8),
                new("Sapper-A", Team.Enemy, UnitType.Sapper, new TileCoord(9, 6)),
                new("Striker-A", Team.Enemy, UnitType.Striker, new TileCoord(13, 4), 8)
            ],
            15,
            10,
            Path(new(6, 1), new(6, 8)),
            [new(4, 6)]);

        private static TileCoord[] MergePaths(params IReadOnlyList<TileCoord>[] paths)
        {
            var merged = new List<TileCoord>();
            var seen = new HashSet<TileCoord>();
            foreach (var path in paths)
            {
                foreach (var coord in path)
                {
                    if (seen.Add(coord))
                    {
                        merged.Add(coord);
                    }
                }
            }

            return [.. merged];
        }

        private static TileCoord[] Path(params TileCoord[] waypoints)
        {
            if (waypoints.Length == 0)
            {
                return [];
            }

            var path = new List<TileCoord> { waypoints[0] };
            var seen = new HashSet<TileCoord> { waypoints[0] };
            for (var index = 1; index < waypoints.Length; index++)
            {
                foreach (var coord in Connect(waypoints[index - 1], waypoints[index]).Skip(1))
                {
                    if (seen.Add(coord))
                    {
                        path.Add(coord);
                    }
                }
            }

            return [.. path];
        }

        private static IEnumerable<TileCoord> Connect(TileCoord start, TileCoord end)
        {
            var current = start;
            yield return current;

            while (current.X != end.X)
            {
                current = current with { X = current.X + Math.Sign(end.X - current.X) };
                yield return current;
            }

            while (current.Y != end.Y)
            {
                current = current with { Y = current.Y + Math.Sign(end.Y - current.Y) };
                yield return current;
            }
        }

        public static MissionSpec Mission5() => CaptureMission(
            5,
            0x2026_0515_0005UL,
            new TileCoord(9, 2),
            "Antenna",
            TileCoord.None,
            "Jammer",
            [new(4, 1), new(5, 2), new(6, 4), new(7, 2), new(8, 5), new(9, 5)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Tech-2", Team.Player, UnitType.Infantry, new TileCoord(1, 4)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Scout-7", Team.Player, UnitType.Scout, new TileCoord(2, 5), 7),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(2, 4))
            ],
            [
                new("Sable-Echo", Team.Enemy, UnitType.Scout, new TileCoord(7, 2), 7),
                new("Jammer-Guard", Team.Enemy, UnitType.Infantry, new TileCoord(9, 3), 9),
                new("Sapper-A", Team.Enemy, UnitType.Sapper, new TileCoord(8, 5)),
                new("Striker-A", Team.Enemy, UnitType.Striker, new TileCoord(10, 2), 9),
                new("Bulwark", Team.Enemy, UnitType.Armor, new TileCoord(10, 4), 10)
            ]);

        public static MissionSpec Mission6() => RoutMission(
            6,
            0x2026_0516_0006UL,
            [new(4, 1), new(5, 2), new(6, 2), new(5, 5), new(6, 5), new(8, 4)],
            [new(5, 1), new(6, 1), new(5, 6), new(6, 6)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Tech-2", Team.Player, UnitType.Infantry, new TileCoord(1, 4)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Lancer-1", Team.Player, UnitType.Lancer, new TileCoord(2, 4)),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(1, 5))
            ],
            [
                new("Demo-A", Team.Enemy, UnitType.Sapper, new TileCoord(8, 2)),
                new("Demo-B", Team.Enemy, UnitType.Sapper, new TileCoord(8, 5)),
                new("Guard-A", Team.Enemy, UnitType.Infantry, new TileCoord(7, 3), 8),
                new("Guard-B", Team.Enemy, UnitType.Infantry, new TileCoord(9, 4), 8),
                new("Bulwark", Team.Enemy, UnitType.Armor, new TileCoord(10, 3), 11)
            ]);

        public static MissionSpec Mission7() => RoutMission(
            7,
            0x2026_0517_0007UL,
            [new(3, 1), new(4, 2), new(6, 2), new(7, 4), new(9, 5)],
            [new(5, 1), new(6, 5), new(8, 1)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Tech-2", Team.Player, UnitType.Infantry, new TileCoord(1, 4)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Scout-7", Team.Player, UnitType.Scout, new TileCoord(2, 5), 8),
                new("Striker-1", Team.Player, UnitType.Striker, new TileCoord(2, 2))
            ],
            [
                new("Meridian-A", Team.Enemy, UnitType.Striker, new TileCoord(8, 2), 8),
                new("Meridian-B", Team.Enemy, UnitType.Striker, new TileCoord(9, 5), 8),
                new("Raider-A", Team.Enemy, UnitType.Scout, new TileCoord(7, 3), 7),
                new("Raider-B", Team.Enemy, UnitType.Infantry, new TileCoord(10, 4), 8),
                new("Lancer-A", Team.Enemy, UnitType.Lancer, new TileCoord(9, 2), 8)
            ]);

        public static MissionSpec Mission8() => CaptureMission(
            8,
            0x2026_0518_0008UL,
            new TileCoord(8, 5),
            "Audit beacon",
            TileCoord.None,
            "Convoy",
            [new(4, 2), new(5, 2), new(6, 1), new(7, 4), new(8, 4), new(9, 5)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(1, 4)),
                new("Field-Rig", Team.Player, UnitType.FieldRig, new TileCoord(2, 5)),
                new("Lancer-1", Team.Player, UnitType.Lancer, new TileCoord(2, 2))
            ],
            [
                new("Blackout-A", Team.Enemy, UnitType.Sapper, new TileCoord(7, 5)),
                new("Blackout-B", Team.Enemy, UnitType.Striker, new TileCoord(8, 3), 9),
                new("Guard-A", Team.Enemy, UnitType.Infantry, new TileCoord(9, 4), 8),
                new("Guard-B", Team.Enemy, UnitType.Infantry, new TileCoord(10, 2), 8),
                new("Bulwark", Team.Enemy, UnitType.Armor, new TileCoord(10, 5), 11)
            ]);

        public static MissionSpec Mission9() => CaptureMission(
            9,
            0x2026_0519_0009UL,
            new TileCoord(9, 2),
            "Scan relay",
            TileCoord.None,
            "Data packet",
            [new(4, 1), new(5, 2), new(6, 3), new(7, 3), new(8, 4), new(9, 5)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Scout-7", Team.Player, UnitType.Scout, new TileCoord(2, 5), 8),
                new("Lancer-1", Team.Player, UnitType.Lancer, new TileCoord(2, 4)),
                new("Engineer-1", Team.Player, UnitType.Engineer, new TileCoord(1, 4))
            ],
            [
                new("Ridge-A", Team.Enemy, UnitType.Lancer, new TileCoord(7, 2), 9),
                new("Ridge-B", Team.Enemy, UnitType.Infantry, new TileCoord(8, 5), 8),
                new("Jammer-A", Team.Enemy, UnitType.Sapper, new TileCoord(9, 3)),
                new("Striker-A", Team.Enemy, UnitType.Striker, new TileCoord(10, 4), 9),
                new("Siege-A", Team.Enemy, UnitType.SiegeBreaker, new TileCoord(10, 2), 12)
            ]);

        public static MissionSpec Mission10() => CaptureMission(
            10,
            0x2026_0520_0010UL,
            new TileCoord(10, 3),
            "Refinery HQ",
            TileCoord.None,
            "Refinery",
            [new(3, 1), new(4, 2), new(5, 2), new(6, 4), new(7, 5), new(8, 2), new(9, 4)],
            [
                new("Tech-1", Team.Player, UnitType.Infantry, new TileCoord(1, 2)),
                new("Armor-1", Team.Player, UnitType.Armor, new TileCoord(2, 3)),
                new("Lancer-1", Team.Player, UnitType.Lancer, new TileCoord(2, 4)),
                new("Striker-1", Team.Player, UnitType.Striker, new TileCoord(1, 4)),
                new("Field-Rig", Team.Player, UnitType.FieldRig, new TileCoord(2, 5)),
                new("Siege-1", Team.Player, UnitType.SiegeBreaker, new TileCoord(1, 5))
            ],
            [
                new("Sloane-Guard", Team.Enemy, UnitType.SiegeBreaker, new TileCoord(10, 3), 14),
                new("Bulwark-A", Team.Enemy, UnitType.Armor, new TileCoord(8, 3), 12),
                new("Lancer-A", Team.Enemy, UnitType.Lancer, new TileCoord(9, 2), 9),
                new("Striker-A", Team.Enemy, UnitType.Striker, new TileCoord(9, 5), 9),
                new("Sapper-A", Team.Enemy, UnitType.Sapper, new TileCoord(7, 4)),
                new("Guard-A", Team.Enemy, UnitType.Infantry, new TileCoord(10, 5), 9)
            ]);

        private static MissionSpec RoutMission(
            int number,
            ulong randomSeed,
            IReadOnlyList<TileCoord> cover,
            IReadOnlyList<TileCoord> ridges,
            IReadOnlyList<UnitSpec> playerUnits,
            IReadOnlyList<UnitSpec> enemyUnits) => new(
                number,
                randomSeed,
                new TileCoord(0, 3),
                new TileCoord(11, 3),
                TileCoord.None,
                "Objective",
                TileCoord.None,
                "Objective",
                SpineRoads,
                cover,
                ridges,
                playerUnits,
                enemyUnits);

        private static MissionSpec CaptureMission(
            int number,
            ulong randomSeed,
            TileCoord relayStation,
            string relayObjectiveName,
            TileCoord fuelCache,
            string fuelObjectiveName,
            IReadOnlyList<TileCoord> cover,
            IReadOnlyList<UnitSpec> playerUnits,
            IReadOnlyList<UnitSpec> enemyUnits) => new(
                number,
                randomSeed,
                new TileCoord(0, 3),
                new TileCoord(11, 3),
                relayStation,
                relayObjectiveName,
                fuelCache,
                fuelObjectiveName,
                SpineRoads,
                cover,
                [],
                playerUnits,
                enemyUnits);
    }
}