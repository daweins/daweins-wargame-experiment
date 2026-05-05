// Copyright (c) Microsoft Corporation.
// SPDX-License-Identifier: MIT
namespace Wargame.Graphics;

using System;
using System.Collections.Generic;

/// <summary>
/// Generates SNES/GBDS-style sprite sheets for tactical board terrain and units.
/// </summary>
public static class SpriteGenerator
{
    private const int SpriteSize = 64;

    private static readonly (byte R, byte G, byte B, byte A) Outline = (8, 13, 22, 255);
    private static readonly (byte R, byte G, byte B, byte A) SoftShadow = (8, 12, 20, 92);

    public static Canvas GenerateTerrain()
    {
        var tiles = new List<Canvas>
        {
            TilePlain(),
            TileRoad(),
            TileCover(),
            TileHq(),
            TileRidge(),
        };

        var sheet = new Canvas(SpriteSize * tiles.Count, SpriteSize, Canvas.Transparent);
        for (int i = 0; i < tiles.Count; i++)
            sheet.CopyNonTransparent(tiles[i], i * SpriteSize, 0);
        return sheet;
    }

    public static Canvas GenerateUnits()
    {
        var sheet = new Canvas(SpriteSize * 3, SpriteSize * 2, Canvas.Transparent);
        var teams = new[] { ("player", GetPlayerPalette()), ("enemy", GetEnemyPalette()) };

        for (int row = 0; row < teams.Length; row++)
        {
            var (_, palette) = teams[row];
            var sprites = new List<Canvas>
            {
                UnitInfantry(palette),
                UnitArmor(palette),
                UnitScout(palette),
            };

            for (int col = 0; col < sprites.Count; col++)
                sheet.CopyNonTransparent(sprites[col], col * SpriteSize, row * SpriteSize);
        }

        return sheet;
    }

    public static Canvas GenerateCampaignUnits()
    {
        var factories = new (string Name, Func<Dictionary<string, (byte, byte, byte, byte)>, Canvas> Factory)[]
        {
            ("infantry", UnitInfantry),
            ("armor", UnitArmor),
            ("scout", UnitScout),
            ("engineer", UnitEngineer),
            ("sapper", UnitSapper),
            ("lancer", UnitLancer),
            ("striker", UnitStriker),
            ("field_rig", UnitFieldRig),
            ("siege_breaker", UnitSiegeBreaker),
        };

        var sheet = new Canvas(SpriteSize * factories.Length, SpriteSize * 2, Canvas.Transparent);
        var teams = new[] { ("player", GetPlayerPalette()), ("enemy", GetEnemyPalette()) };

        for (int row = 0; row < teams.Length; row++)
        {
            var (_, palette) = teams[row];
            for (int col = 0; col < factories.Length; col++)
            {
                var sprite = factories[col].Factory(palette);
                sheet.CopyNonTransparent(sprite, col * SpriteSize, row * SpriteSize);
            }
        }

        return sheet;
    }

    public static Canvas GenerateUiIcons()
    {
        var icons = new List<Canvas>
        {
            IconMove(),
            IconAttack(),
            IconWait(),
            IconCapture(),
            IconRepair(),
            IconSupply(),
            IconRescue(),
            IconEndTurn(),
            IconTerrainDefense(),
            IconObjective(),
            IconHqDanger(),
            IconScoutRescued(),
        };

        var sheet = new Canvas(SpriteSize * icons.Count, SpriteSize, Canvas.Transparent);
        for (var index = 0; index < icons.Count; index++)
            sheet.CopyNonTransparent(icons[index], index * SpriteSize, 0);

        return sheet;
    }

    public static Canvas GenerateRuntimeTerrainVariants()
    {
        const int tileWidth = 256;
        const int tileHeight = 128;
        var tiles = new List<Canvas>
        {
            BuildTerrainVariant(TilePlain(), 0),
            BuildTerrainVariant(TilePlain(), 1),
            BuildTerrainVariant(TilePlain(), 2),
            BuildTerrainVariant(TilePlain(), 3),
            BuildTerrainVariant(TilePlain(), 4),
            BuildTerrainVariant(TilePlain(), 5),
            BuildTerrainVariant(TileRoad(), 6),
            RotateTile(BuildTerrainVariant(TileRoad(), 7)),
            BuildRoadJunction(8, north: true, south: true, west: true),
            BuildRoadJunction(9, north: true, south: true, east: true),
            BuildRoadCorner(10, east: true, north: true),
            BuildRoadCorner(11, east: true, north: false, south: true),
            BuildTerrainVariant(TileCover(), 12),
            MirrorTile(BuildTerrainVariant(TileCover(), 13)),
            BuildTerrainVariant(TileCover(), 14),
            BuildTerrainVariant(TileCover(), 15),
            BuildTerrainVariant(TileRidge(), 16),
            MirrorTile(BuildTerrainVariant(TileRidge(), 17)),
            BuildTerrainVariant(TileRidge(), 18),
            BuildTerrainVariant(TileRidge(), 19),
            BuildTerrainVariant(TileHq(), 20),
            BuildTerrainVariant(TileHq(), 21),
            BuildObjectiveTile(22, relay: true),
            BuildObjectiveTile(23, relay: false),
        };

        var sheet = new Canvas(tileWidth * 6, tileHeight * 4, Canvas.Transparent);
        for (var index = 0; index < tiles.Count; index++)
        {
            var scaled = ScaleNearest(tiles[index], tileWidth, tileHeight);
            sheet.CopyNonTransparent(scaled, index % 6 * tileWidth, index / 6 * tileHeight);
        }

        return sheet;
    }

    public static Canvas GenerateTransparentUnitSpriteAtlas()
    {
        const int sourceCell = 128;
        var roles = new[]
        {
            "infantry",
            "armor",
            "scout",
            "engineer",
            "sapper",
            "lancer",
            "striker",
            "field_rig",
            "siege_breaker",
        };

        var source = new Canvas(sourceCell * roles.Length, sourceCell * 2, Canvas.Transparent);
        var teams = new[] { GetPlayerPalette(), GetEnemyPalette() };
        for (var row = 0; row < teams.Length; row++)
        {
            for (var col = 0; col < roles.Length; col++)
            {
                DrawHandoffUnitToken(source, col * sourceCell, row * sourceCell, roles[col], teams[row]);
            }
        }

        return ScaleNearest(source, 2304, 512);
    }

    public static Canvas GenerateTransparentUiIconAtlas()
    {
        var source = GenerateUiIcons();
        var scaled = ScaleNearest(source, 1536, 128);
        var sheet = new Canvas(1536, 256, Canvas.Transparent);
        sheet.CopyNonTransparent(scaled, 0, 64);
        return sheet;
    }

    public static Canvas GenerateActOneOverlayAtlas()
    {
        var icons = new List<Canvas>
        {
            IconHqDanger(),
            IconScoutRescued(),
            IconCapture(),
            IconSupply(),
            IconMove(),
            IconObjective(),
            IconSupply(),
            IconCapture(),
            IconWait(),
            IconTerrainDefense(),
            IconHqDanger(),
            IconWait(),
            IconHqDanger(),
            IconRepair(),
            IconRescue(),
            IconAttack(),
        };

        var source = new Canvas(SpriteSize * icons.Count, SpriteSize, Canvas.Transparent);
        for (var index = 0; index < icons.Count; index++)
            source.CopyNonTransparent(icons[index], index * SpriteSize, 0);

        var scaled = ScaleNearest(source, 2048, 128);
        var sheet = new Canvas(2048, 256, Canvas.Transparent);
        sheet.CopyNonTransparent(scaled, 0, 64);
        return sheet;
    }

    public static Canvas GenerateMissionFourToTenReferencePanels()
    {
        const int panelWidth = 320;
        const int panelHeight = 180;
        var sheet = new Canvas(panelWidth * 4, panelHeight * 2, (8, 12, 18, 255));

        for (var missionNumber = 4; missionNumber <= 10; missionNumber++)
        {
            var panelIndex = missionNumber - 4;
            var originX = panelIndex % 4 * panelWidth;
            var originY = panelIndex / 4 * panelHeight;
            DrawMissionReferencePanel(sheet, originX, originY, panelWidth, panelHeight, missionNumber);
        }

        return sheet;
    }

    // Terrain Tiles

    private static Canvas TilePlain()
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, (166, 132, 84, 255));
        canvas.DrawRect(0, 0, SpriteSize, 18, (181, 145, 92, 255));
        canvas.DrawRect(0, 46, SpriteSize, 18, (139, 108, 74, 255));

        foreach (var (col, row, width) in new[] { (7, 13, 14), (35, 20, 17), (16, 43, 15), (47, 50, 9), (27, 31, 11) })
        {
            canvas.DrawRect(col, row, width, 2, (102, 80, 58, 255));
            canvas.DrawRect(col + 2, row + 2, Math.Max(2, width - 5), 2, (202, 168, 111, 255));
        }

        foreach (var (col, row) in new[] { (11, 31), (38, 9), (52, 38), (25, 55) })
        {
            canvas.DrawRect(col, row, 5, 3, (91, 78, 70, 255));
            canvas.DrawRect(col + 1, row - 1, 3, 2, (205, 178, 131, 255));
        }

        DrawTileBorder(canvas, (205, 166, 103, 255), (105, 80, 57, 255));
        return canvas;
    }

    private static Canvas TileRoad()
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, (150, 119, 76, 255));
        canvas.DrawRect(0, 7, SpriteSize, 50, (65, 68, 72, 255));
        canvas.DrawRect(0, 9, SpriteSize, 4, (99, 101, 104, 255));
        canvas.DrawRect(0, 53, SpriteSize, 4, (33, 36, 43, 255));
        canvas.DrawRect(5, 7, 4, 50, (205, 155, 54, 255));
        canvas.DrawRect(55, 7, 4, 50, (205, 155, 54, 255));

        foreach (var row in new[] { 11, 27, 43 })
        {
            canvas.DrawRect(29, row, 6, 10, (210, 210, 198, 255));
            canvas.DrawRect(30, row + 1, 4, 8, (138, 141, 145, 255));
        }

        canvas.DrawRect(14, 15, 9, 2, (42, 45, 52, 255));
        canvas.DrawRect(40, 36, 12, 2, (42, 45, 52, 255));
        DrawTileBorder(canvas, (111, 111, 112, 255), (30, 33, 40, 255));
        return canvas;
    }

    private static Canvas TileCover()
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, (158, 126, 83, 255));
        canvas.DrawRect(0, 46, SpriteSize, 18, (121, 93, 66, 255));
        canvas.DrawRect(7, 34, 50, 17, SoftShadow);
        DrawCrate(canvas, 8, 24, 24, 22, (78, 82, 88, 255), (139, 145, 151, 255));
        DrawCrate(canvas, 31, 18, 24, 27, (62, 67, 75, 255), (119, 126, 136, 255));
        DrawCrate(canvas, 20, 39, 34, 12, (46, 52, 62, 255), (98, 106, 116, 255));
        canvas.DrawRect(12, 41, 38, 4, (212, 163, 54, 255));
        canvas.DrawRect(15, 43, 8, 2, (33, 36, 43, 255));
        canvas.DrawRect(27, 43, 8, 2, (33, 36, 43, 255));
        canvas.DrawRect(39, 43, 8, 2, (33, 36, 43, 255));
        DrawTileBorder(canvas, (198, 162, 103, 255), (95, 72, 54, 255));
        return canvas;
    }

    private static Canvas TileHq()
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, (143, 114, 79, 255));
        canvas.DrawRect(7, 47, 50, 10, (46, 47, 52, 255));
        canvas.DrawRect(10, 22, 44, 28, Outline);
        canvas.DrawRect(13, 18, 38, 30, (111, 113, 113, 255));
        canvas.DrawRect(18, 13, 28, 8, (166, 168, 164, 255));
        canvas.DrawRect(23, 8, 18, 7, (76, 82, 91, 255));
        canvas.DrawRect(27, 29, 11, 18, (38, 42, 49, 255));
        canvas.DrawRect(16, 23, 31, 4, (210, 212, 203, 255));
        canvas.DrawRect(41, 9, 4, 18, (41, 45, 51, 255));
        canvas.DrawRect(32, 6, 10, 4, (84, 92, 100, 255));
        canvas.DrawRect(36, 10, 5, 3, (91, 216, 226, 255));
        canvas.DrawRect(15, 51, 34, 4, (211, 158, 48, 255));

        DrawTileBorder(canvas, (197, 166, 113, 255), (76, 58, 48, 255));
        return canvas;
    }

    private static Canvas TileRidge()
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, (42, 44, 50, 255));
        canvas.DrawPolygon(new List<(int, int)> { (4, 54), (18, 22), (33, 54) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (14, 55), (37, 10), (61, 55) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (9, 51), (18, 28), (30, 51) }, (73, 73, 78, 255));
        canvas.DrawPolygon(new List<(int, int)> { (21, 51), (37, 17), (56, 51) }, (90, 91, 98, 255));
        canvas.DrawPolygon(new List<(int, int)> { (18, 28), (24, 38), (13, 39) }, (141, 144, 151, 255));
        canvas.DrawPolygon(new List<(int, int)> { (37, 17), (44, 31), (30, 31) }, (164, 165, 168, 255));
        canvas.DrawRect(6, 52, 7, 4, (28, 30, 36, 255));
        canvas.DrawRect(47, 49, 9, 5, (28, 30, 36, 255));
        canvas.DrawRect(0, 53, SpriteSize, 8, (17, 25, 38, 255));

        DrawTileBorder(canvas, (94, 97, 106, 255), (22, 25, 32, 255));
        return canvas;
    }

    private static void DrawCrate(Canvas canvas, int x, int y, int width, int height, (byte R, byte G, byte B, byte A) body, (byte R, byte G, byte B, byte A) highlight)
    {
        canvas.DrawRect(x - 2, y - 2, width + 4, height + 4, Outline);
        canvas.DrawRect(x, y, width, height, body);
        canvas.DrawRect(x + 2, y + 2, width - 4, 3, highlight);
        canvas.DrawRect(x + 4, y + 6, 3, height - 9, (38, 42, 49, 255));
        canvas.DrawRect(x + width - 7, y + 6, 3, height - 9, (38, 42, 49, 255));
        canvas.DrawRect(x + 8, y + height - 6, width - 16, 3, (37, 41, 48, 255));
    }

    // Units

    private static Canvas UnitInfantry(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawEllipse(32, 12, 13, 7, Outline);
        canvas.DrawRect(20, 12, 25, 8, Outline);
        canvas.DrawEllipse(32, 11, 10, 5, palette["light"]);
        canvas.DrawRect(22, 13, 21, 6, palette["mid"]);
        canvas.DrawRect(26, 18, 13, 7, (221, 229, 218, 255));
        canvas.DrawRect(27, 22, 11, 3, (63, 81, 95, 255));
        canvas.DrawPolygon(new List<(int, int)> { (16, 25), (47, 25), (44, 47), (20, 47) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (20, 25), (44, 25), (41, 45), (23, 45) }, palette["mid"]);
        canvas.DrawRect(24, 28, 16, 14, palette["dark"]);
        canvas.DrawRect(22, 26, 20, 4, palette["light"]);
        canvas.DrawRect(10, 29, 11, 18, Outline);
        canvas.DrawRect(13, 30, 7, 15, palette["deep"]);
        canvas.DrawRect(43, 25, 9, 20, Outline);
        canvas.DrawRect(45, 25, 5, 18, (222, 230, 218, 255));
        canvas.DrawRect(50, 19, 5, 9, Outline);
        canvas.DrawRect(51, 20, 3, 6, palette["accent"]);
        canvas.DrawRect(21, 45, 10, 12, Outline);
        canvas.DrawRect(34, 45, 10, 12, Outline);
        canvas.DrawRect(23, 45, 7, 10, palette["dark"]);
        canvas.DrawRect(36, 45, 7, 10, palette["dark"]);
        canvas.DrawRect(17, 56, 15, 5, Outline);
        canvas.DrawRect(34, 56, 15, 5, Outline);
        canvas.DrawRect(25, 33, 14, 4, palette["accent"]);
        return canvas;
    }

    private static Canvas UnitArmor(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawRect(6, 38, 52, 14, Outline);
        canvas.DrawRect(9, 35, 47, 13, palette["deep"]);
        canvas.DrawRect(12, 33, 40, 10, palette["dark"]);
        canvas.DrawPolygon(new List<(int, int)> { (15, 30), (42, 24), (51, 36), (12, 38) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (18, 30), (40, 26), (47, 35), (16, 36) }, palette["mid"]);
        canvas.DrawRect(24, 18, 19, 9, Outline);
        canvas.DrawRect(27, 16, 13, 9, palette["light"]);
        canvas.DrawRect(41, 27, 20, 6, Outline);
        canvas.DrawRect(43, 27, 17, 4, (226, 234, 224, 255));
        canvas.DrawRect(16, 36, 30, 5, palette["light"]);
        canvas.DrawRect(20, 29, 19, 4, palette["accent"]);

        foreach (var col in new[] { 14, 25, 36, 47 })
        {
            canvas.DrawEllipse(col, 50, 6, 6, Outline);
            canvas.DrawEllipse(col, 50, 3, 3, (210, 220, 214, 255));
        }

        canvas.DrawRect(11, 42, 43, 4, (231, 240, 231, 255));
        canvas.DrawRect(9, 52, 46, 3, (38, 49, 62, 255));
        return canvas;
    }

    private static Canvas UnitScout(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawPolygon(new List<(int, int)> { (6, 42), (21, 26), (48, 27), (59, 40), (47, 49), (15, 50) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (10, 41), (23, 29), (46, 30), (55, 39), (44, 46), (17, 47) }, palette["dark"]);
        canvas.DrawPolygon(new List<(int, int)> { (21, 25), (45, 24), (51, 32), (18, 32) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (24, 24), (43, 25), (48, 31), (20, 31) }, palette["mid"]);
        canvas.DrawPolygon(new List<(int, int)> { (32, 15), (47, 22), (43, 28), (25, 25) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (34, 17), (43, 22), (39, 25), (29, 23) }, palette["light"]);
        canvas.DrawRect(48, 33, 11, 5, (226, 234, 224, 255));
        canvas.DrawRect(14, 39, 32, 5, palette["accent"]);
        canvas.DrawEllipse(18, 51, 7, 7, Outline);
        canvas.DrawEllipse(46, 51, 7, 7, Outline);
        canvas.DrawEllipse(18, 51, 4, 4, (212, 222, 216, 255));
        canvas.DrawEllipse(46, 51, 4, 4, (212, 222, 216, 255));
        canvas.DrawRect(12, 32, 8, 6, palette["light"]);
        canvas.DrawRect(53, 39, 5, 3, palette["light"]);
        return canvas;
    }

    private static Canvas UnitEngineer(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawRect(19, 8, 25, 13, Outline);
        canvas.DrawRect(22, 7, 19, 12, palette["light"]);
        canvas.DrawRect(20, 16, 23, 5, palette["mid"]);
        canvas.DrawPolygon(new List<(int, int)> { (16, 24), (46, 24), (44, 47), (19, 47) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (20, 24), (43, 24), (40, 45), (22, 45) }, palette["dark"]);
        canvas.DrawRect(23, 27, 18, 4, palette["light"]);
        canvas.DrawRect(12, 29, 10, 17, Outline);
        canvas.DrawRect(14, 31, 7, 14, (92, 105, 92, 255));
        canvas.DrawRect(43, 25, 10, 20, Outline);
        canvas.DrawRect(46, 28, 4, 16, (226, 219, 168, 255));
        canvas.DrawRect(49, 39, 9, 6, Outline);
        canvas.DrawRect(50, 39, 7, 4, palette["accent"]);
        canvas.DrawRect(23, 45, 9, 12, Outline);
        canvas.DrawRect(35, 45, 9, 12, Outline);
        canvas.DrawRect(25, 45, 6, 10, palette["mid"]);
        canvas.DrawRect(37, 45, 6, 10, palette["mid"]);
        canvas.DrawRect(18, 56, 15, 5, Outline);
        canvas.DrawRect(35, 56, 15, 5, Outline);
        canvas.DrawRect(26, 33, 12, 4, palette["accent"]);
        return canvas;
    }

    private static Canvas UnitSapper(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawEllipse(32, 12, 13, 7, Outline);
        canvas.DrawRect(21, 13, 22, 7, palette["dark"]);
        canvas.DrawRect(25, 9, 14, 4, palette["mid"]);
        canvas.DrawPolygon(new List<(int, int)> { (16, 24), (47, 24), (45, 47), (19, 47) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (20, 24), (44, 24), (41, 45), (22, 45) }, palette["mid"]);
        canvas.DrawRect(24, 26, 16, 5, palette["light"]);
        canvas.DrawRect(12, 29, 10, 17, Outline);
        canvas.DrawRect(14, 31, 7, 14, palette["deep"]);
        canvas.DrawRect(43, 24, 10, 21, Outline);
        canvas.DrawRect(47, 17, 4, 27, (226, 222, 178, 255));
        canvas.DrawRect(50, 14, 6, 9, Outline);
        canvas.DrawRect(27, 31, 13, 11, (48, 40, 38, 255));
        canvas.DrawRect(28, 32, 11, 4, (247, 203, 92, 255));
        canvas.DrawRect(29, 38, 9, 3, (247, 203, 92, 255));
        canvas.DrawRect(22, 45, 9, 12, Outline);
        canvas.DrawRect(35, 45, 9, 12, Outline);
        canvas.DrawRect(24, 45, 6, 10, palette["dark"]);
        canvas.DrawRect(37, 45, 6, 10, palette["dark"]);
        canvas.DrawRect(18, 56, 15, 5, Outline);
        canvas.DrawRect(35, 56, 15, 5, Outline);
        return canvas;
    }

    private static Canvas UnitLancer(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawEllipse(32, 12, 12, 6, Outline);
        canvas.DrawRect(22, 9, 20, 11, palette["mid"]);
        canvas.DrawRect(26, 9, 12, 4, palette["light"]);
        canvas.DrawPolygon(new List<(int, int)> { (17, 25), (46, 25), (44, 47), (20, 47) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (21, 25), (43, 25), (40, 45), (23, 45) }, palette["dark"]);
        canvas.DrawRect(24, 27, 16, 4, palette["light"]);
        canvas.DrawRect(6, 25, 52, 6, Outline);
        canvas.DrawRect(8, 24, 47, 4, (226, 233, 222, 255));
        canvas.DrawRect(53, 20, 8, 10, Outline);
        canvas.DrawRect(54, 21, 5, 7, palette["accent"]);
        canvas.DrawRect(10, 32, 12, 11, palette["deep"]);
        canvas.DrawRect(42, 29, 10, 17, palette["deep"]);
        canvas.DrawRect(24, 45, 9, 12, Outline);
        canvas.DrawRect(35, 45, 9, 12, Outline);
        canvas.DrawRect(26, 45, 6, 10, palette["mid"]);
        canvas.DrawRect(37, 45, 6, 10, palette["mid"]);
        canvas.DrawRect(19, 56, 15, 5, Outline);
        canvas.DrawRect(35, 56, 15, 5, Outline);
        canvas.DrawRect(27, 34, 11, 4, palette["accent"]);
        return canvas;
    }

    private static Canvas UnitStriker(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawPolygon(new List<(int, int)> { (6, 42), (20, 28), (48, 26), (60, 38), (48, 48), (15, 50) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (10, 40), (22, 31), (45, 29), (55, 38), (46, 45), (17, 47) }, palette["deep"]);
        canvas.DrawPolygon(new List<(int, int)> { (20, 26), (41, 22), (52, 32), (17, 34) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (23, 26), (39, 24), (47, 31), (20, 32) }, palette["mid"]);
        canvas.DrawPolygon(new List<(int, int)> { (30, 16), (44, 22), (42, 28), (24, 25) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 18), (40, 22), (37, 25), (29, 23) }, palette["light"]);
        canvas.DrawRect(49, 33, 11, 5, palette["accent"]);
        canvas.DrawRect(13, 38, 33, 5, palette["mid"]);
        canvas.DrawRect(17, 29, 7, 6, palette["light"]);

        foreach (var col in new[] { 17, 31, 46 })
        {
            canvas.DrawEllipse(col, 51, 6, 6, Outline);
            canvas.DrawEllipse(col, 51, 3, 3, (212, 222, 216, 255));
        }

        canvas.DrawRect(55, 38, 4, 3, palette["light"]);
        return canvas;
    }

    private static Canvas UnitFieldRig(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawRect(8, 35, 50, 16, Outline);
        canvas.DrawRect(11, 32, 44, 16, palette["deep"]);
        canvas.DrawRect(15, 23, 30, 14, Outline);
        canvas.DrawRect(18, 21, 24, 14, palette["mid"]);
        canvas.DrawRect(22, 24, 14, 5, palette["light"]);
        canvas.DrawRect(43, 26, 11, 13, (83, 92, 82, 255));
        canvas.DrawRect(45, 19, 5, 15, palette["accent"]);
        canvas.DrawRect(49, 17, 9, 5, Outline);
        canvas.DrawRect(49, 16, 8, 4, (228, 220, 169, 255));
        canvas.DrawRect(13, 37, 16, 8, (78, 91, 77, 255));
        canvas.DrawRect(31, 37, 11, 8, (107, 83, 55, 255));
        canvas.DrawRect(32, 39, 9, 3, (239, 194, 92, 255));

        foreach (var col in new[] { 17, 31, 46 })
        {
            canvas.DrawEllipse(col, 51, 6, 6, Outline);
            canvas.DrawEllipse(col, 51, 3, 3, (212, 222, 216, 255));
        }

        canvas.DrawRect(13, 33, 40, 4, palette["light"]);
        return canvas;
    }

    private static Canvas UnitSiegeBreaker(Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var canvas = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        DrawCommonShadow(canvas);
        canvas.DrawRect(3, 38, 58, 15, Outline);
        canvas.DrawRect(6, 34, 52, 15, palette["deep"]);
        canvas.DrawRect(10, 31, 45, 13, palette["dark"]);
        canvas.DrawPolygon(new List<(int, int)> { (15, 24), (43, 20), (55, 35), (11, 37) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (19, 24), (42, 22), (50, 34), (15, 35) }, palette["mid"]);
        canvas.DrawRect(23, 16, 21, 9, Outline);
        canvas.DrawRect(27, 13, 14, 8, palette["light"]);
        canvas.DrawRect(41, 23, 22, 7, Outline);
        canvas.DrawRect(43, 22, 18, 5, (226, 233, 222, 255));
        canvas.DrawRect(58, 20, 5, 8, palette["accent"]);
        canvas.DrawRect(16, 35, 31, 5, palette["light"]);
        canvas.DrawRect(22, 28, 19, 4, palette["accent"]);
        canvas.DrawRect(7, 45, 50, 5, (231, 240, 231, 255));
        canvas.DrawRect(7, 52, 50, 3, (38, 49, 62, 255));

        foreach (var col in new[] { 12, 22, 32, 42, 52 })
        {
            canvas.DrawEllipse(col, 51, 5, 5, Outline);
            canvas.DrawEllipse(col, 51, 3, 3, (210, 220, 214, 255));
        }

        canvas.DrawRect(13, 39, 41, 3, (246, 199, 86, 255));
        return canvas;
    }

    private static void DrawHandoffUnitToken(Canvas canvas, int x, int y, string role, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        canvas.DrawEllipse(x + 64, y + 111, 46, 10, SoftShadow);
        canvas.DrawRect(x + 12, y + 114, 104, 4, (6, 10, 18, 96));

        switch (role)
        {
            case "infantry":
                DrawHandoffInfantry(canvas, x, y, palette, rifle: true, pack: false, tool: false);
                break;
            case "armor":
                DrawHandoffArmor(canvas, x, y, palette, heavy: false);
                break;
            case "scout":
                DrawHandoffScout(canvas, x, y, palette, openTop: true);
                break;
            case "engineer":
                DrawHandoffInfantry(canvas, x, y, palette, rifle: false, pack: true, tool: true);
                break;
            case "sapper":
                DrawHandoffInfantry(canvas, x, y, palette, rifle: false, pack: true, tool: false);
                DrawExplosivePack(canvas, x + 78, y + 68, palette);
                break;
            case "lancer":
                DrawHandoffInfantry(canvas, x, y, palette, rifle: false, pack: false, tool: false);
                DrawLauncher(canvas, x + 27, y + 44, palette);
                break;
            case "striker":
                DrawHandoffScout(canvas, x, y, palette, openTop: false);
                canvas.DrawRect(x + 84, y + 62, 22, 8, palette["accent"]);
                canvas.DrawRect(x + 101, y + 59, 10, 12, Outline);
                break;
            case "field_rig":
                DrawHandoffFieldRig(canvas, x, y, palette);
                break;
            case "siege_breaker":
                DrawHandoffArmor(canvas, x, y, palette, heavy: true);
                break;
        }

        DrawTokenGlints(canvas, x, y, palette["accent"]);
    }

    private static void DrawHandoffInfantry(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette, bool rifle, bool pack, bool tool)
    {
        canvas.DrawEllipse(x + 61, y + 24, 23, 13, Outline);
        canvas.DrawEllipse(x + 61, y + 21, 18, 10, palette["light"]);
        canvas.DrawRect(x + 39, y + 25, 45, 10, Outline);
        canvas.DrawRect(x + 43, y + 27, 37, 7, palette["mid"]);
        canvas.DrawRect(x + 51, y + 35, 22, 13, (218, 229, 222, 255));
        canvas.DrawRect(x + 53, y + 42, 18, 5, (56, 68, 82, 255));

        if (pack)
        {
            canvas.DrawRect(x + 26, y + 47, 22, 38, Outline);
            canvas.DrawRect(x + 30, y + 51, 15, 30, (76, 87, 82, 255));
            canvas.DrawRect(x + 33, y + 56, 9, 5, palette["accent"]);
        }

        canvas.DrawPolygon(new List<(int, int)> { (31, 48), (88, 48), (82, 91), (37, 91) }.Offset(x, y), Outline);
        canvas.DrawPolygon(new List<(int, int)> { (38, 50), (82, 50), (76, 87), (43, 87) }.Offset(x, y), palette["mid"]);
        canvas.DrawRect(x + 45, y + 55, 31, 24, palette["dark"]);
        canvas.DrawRect(x + 42, y + 51, 39, 8, palette["light"]);
        canvas.DrawRect(x + 50, y + 67, 22, 7, palette["accent"]);

        canvas.DrawRect(x + 21, y + 55, 18, 36, Outline);
        canvas.DrawRect(x + 25, y + 59, 10, 28, palette["deep"]);
        canvas.DrawRect(x + 82, y + 53, 18, 38, Outline);
        canvas.DrawRect(x + 86, y + 57, 10, 30, palette["deep"]);

        canvas.DrawRect(x + 39, y + 88, 18, 25, Outline);
        canvas.DrawRect(x + 67, y + 88, 18, 25, Outline);
        canvas.DrawRect(x + 43, y + 90, 11, 20, palette["dark"]);
        canvas.DrawRect(x + 70, y + 90, 11, 20, palette["dark"]);
        canvas.DrawRect(x + 30, y + 111, 31, 8, Outline);
        canvas.DrawRect(x + 65, y + 111, 31, 8, Outline);

        if (rifle)
        {
            canvas.DrawRect(x + 85, y + 45, 33, 8, Outline);
            canvas.DrawRect(x + 88, y + 46, 27, 5, (226, 233, 222, 255));
            canvas.DrawRect(x + 109, y + 34, 8, 18, Outline);
            canvas.DrawRect(x + 111, y + 36, 4, 13, palette["accent"]);
        }

        if (tool)
        {
            canvas.DrawRect(x + 88, y + 48, 9, 42, Outline);
            canvas.DrawRect(x + 91, y + 51, 4, 35, (226, 219, 168, 255));
            canvas.DrawRect(x + 92, y + 43, 24, 10, Outline);
            canvas.DrawRect(x + 95, y + 45, 18, 5, palette["accent"]);
        }
    }

    private static void DrawHandoffArmor(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette, bool heavy)
    {
        var bodyTop = heavy ? 50 : 58;
        var bodyHeight = heavy ? 47 : 39;
        canvas.DrawRect(x + 10, y + bodyTop + 24, 108, 24, Outline);
        canvas.DrawRect(x + 15, y + bodyTop + 18, 98, bodyHeight, palette["deep"]);
        canvas.DrawRect(x + 23, y + bodyTop + 10, 82, 24, palette["dark"]);
        canvas.DrawPolygon(new List<(int, int)> { (30, bodyTop), (83, bodyTop - 10), (108, bodyTop + 17), (20, bodyTop + 21) }.Offset(x, y), Outline);
        canvas.DrawPolygon(new List<(int, int)> { (36, bodyTop + 1), (80, bodyTop - 5), (99, bodyTop + 15), (28, bodyTop + 17) }.Offset(x, y), palette["mid"]);
        canvas.DrawRect(x + 48, y + bodyTop - 26, 39, 18, Outline);
        canvas.DrawRect(x + 54, y + bodyTop - 31, 27, 18, palette["light"]);
        canvas.DrawRect(x + 80, y + bodyTop - 12, heavy ? 46 : 35, heavy ? 11 : 9, Outline);
        canvas.DrawRect(x + 84, y + bodyTop - 11, heavy ? 39 : 30, heavy ? 7 : 5, (226, 233, 222, 255));
        canvas.DrawRect(x + 31, y + bodyTop + 18, 64, 8, palette["light"]);
        canvas.DrawRect(x + 42, y + bodyTop + 5, 41, 7, palette["accent"]);

        var wheelCount = heavy ? 6 : 5;
        for (var index = 0; index < wheelCount; index++)
        {
            var wheelX = x + 21 + index * (heavy ? 17 : 20);
            canvas.DrawEllipse(wheelX, y + bodyTop + 49, 10, 10, Outline);
            canvas.DrawEllipse(wheelX, y + bodyTop + 49, 5, 5, (210, 220, 214, 255));
        }

        if (heavy)
        {
            canvas.DrawRect(x + 110, y + bodyTop - 13, 10, 13, palette["accent"]);
            canvas.DrawRect(x + 16, y + bodyTop + 38, 96, 8, (231, 240, 231, 255));
        }
    }

    private static void DrawHandoffScout(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette, bool openTop)
    {
        canvas.DrawPolygon(new List<(int, int)> { (10, 83), (36, 50), (94, 52), (118, 78), (96, 100), (30, 101) }.Offset(x, y), Outline);
        canvas.DrawPolygon(new List<(int, int)> { (18, 80), (41, 57), (89, 59), (109, 78), (91, 94), (34, 96) }.Offset(x, y), palette["dark"]);
        canvas.DrawPolygon(new List<(int, int)> { (38, 48), (86, 42), (108, 62), (31, 66) }.Offset(x, y), Outline);
        canvas.DrawPolygon(new List<(int, int)> { (44, 48), (82, 46), (97, 61), (38, 63) }.Offset(x, y), palette["mid"]);
        canvas.DrawPolygon(new List<(int, int)> { (60, 24), (91, 42), (84, 56), (47, 49) }.Offset(x, y), Outline);
        canvas.DrawPolygon(new List<(int, int)> { (64, 29), (83, 41), (77, 49), (55, 45) }.Offset(x, y), palette["light"]);
        canvas.DrawRect(x + 95, y + 68, 22, 9, (226, 234, 224, 255));
        canvas.DrawRect(x + 25, y + 82, 67, 9, palette["accent"]);

        if (openTop)
        {
            canvas.DrawRect(x + 31, y + 62, 18, 13, palette["light"]);
        }
        else
        {
            canvas.DrawRect(x + 28, y + 60, 24, 14, Outline);
            canvas.DrawRect(x + 32, y + 63, 16, 8, palette["light"]);
        }

        foreach (var wheelX in new[] { x + 35, x + 94 })
        {
            canvas.DrawEllipse(wheelX, y + 104, 14, 14, Outline);
            canvas.DrawEllipse(wheelX, y + 104, 8, 8, (212, 222, 216, 255));
            canvas.DrawEllipse(wheelX, y + 104, 3, 3, palette["accent"]);
        }
    }

    private static void DrawHandoffFieldRig(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        canvas.DrawRect(x + 12, y + 74, 105, 33, Outline);
        canvas.DrawRect(x + 18, y + 67, 93, 35, palette["deep"]);
        canvas.DrawRect(x + 28, y + 46, 58, 31, Outline);
        canvas.DrawRect(x + 34, y + 42, 47, 30, palette["mid"]);
        canvas.DrawRect(x + 42, y + 49, 29, 10, palette["light"]);
        canvas.DrawRect(x + 85, y + 52, 23, 27, (83, 92, 82, 255));
        canvas.DrawRect(x + 91, y + 26, 10, 37, palette["accent"]);
        canvas.DrawRect(x + 99, y + 24, 19, 9, Outline);
        canvas.DrawRect(x + 99, y + 22, 17, 6, (228, 220, 169, 255));
        canvas.DrawRect(x + 25, y + 78, 34, 17, (78, 91, 77, 255));
        canvas.DrawRect(x + 63, y + 78, 24, 17, (107, 83, 55, 255));
        canvas.DrawRect(x + 65, y + 82, 19, 6, (239, 194, 92, 255));

        foreach (var wheelX in new[] { x + 34, x + 65, x + 96 })
        {
            canvas.DrawEllipse(wheelX, y + 107, 12, 12, Outline);
            canvas.DrawEllipse(wheelX, y + 107, 6, 6, (212, 222, 216, 255));
        }
    }

    private static void DrawLauncher(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        canvas.DrawRect(x, y, 74, 11, Outline);
        canvas.DrawRect(x + 4, y + 2, 65, 6, (226, 233, 222, 255));
        canvas.DrawRect(x + 66, y - 8, 16, 19, Outline);
        canvas.DrawRect(x + 70, y - 5, 9, 12, palette["accent"]);
    }

    private static void DrawExplosivePack(Canvas canvas, int x, int y, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        canvas.DrawRect(x, y, 24, 22, Outline);
        canvas.DrawRect(x + 4, y + 4, 16, 14, (48, 40, 38, 255));
        canvas.DrawRect(x + 6, y + 6, 12, 5, (247, 203, 92, 255));
        canvas.DrawRect(x + 8, y + 14, 8, 4, palette["accent"]);
    }

    private static void DrawTokenGlints(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 13, y + 14, 12, 3, accent);
        canvas.DrawRect(x + 13, y + 14, 3, 12, accent);
        canvas.DrawRect(x + 103, y + 14, 12, 3, accent);
        canvas.DrawRect(x + 112, y + 14, 3, 12, accent);
    }

    private static List<(int, int)> Offset(this List<(int X, int Y)> points, int offsetX, int offsetY)
    {
        var shifted = new List<(int, int)>(points.Count);
        foreach (var (pointX, pointY) in points)
            shifted.Add((pointX + offsetX, pointY + offsetY));
        return shifted;
    }

    // UI Icons

    private static Canvas IconMove()
    {
        var canvas = IconCanvas();
        canvas.DrawRect(18, 41, 22, 7, Outline);
        canvas.DrawRect(18, 41, 20, 5, (102, 224, 229, 255));
        canvas.DrawRect(34, 25, 7, 22, Outline);
        canvas.DrawRect(34, 27, 5, 19, (102, 224, 229, 255));
        canvas.DrawPolygon(new List<(int, int)> { (27, 28), (38, 12), (49, 28) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (31, 28), (38, 17), (45, 28) }, (130, 242, 244, 255));
        canvas.DrawRect(14, 50, 6, 5, (102, 224, 229, 255));
        canvas.DrawRect(25, 50, 6, 5, (102, 224, 229, 255));
        canvas.DrawRect(36, 50, 6, 5, (102, 224, 229, 255));
        return canvas;
    }

    private static Canvas IconAttack()
    {
        var canvas = IconCanvas();
        canvas.DrawEllipse(32, 32, 24, 24, Outline);
        canvas.DrawEllipse(32, 32, 20, 20, (222, 226, 219, 255));
        canvas.DrawEllipse(32, 32, 12, 12, (32, 39, 49, 255));
        canvas.DrawRect(29, 6, 6, 17, Outline);
        canvas.DrawRect(29, 41, 6, 17, Outline);
        canvas.DrawRect(6, 29, 17, 6, Outline);
        canvas.DrawRect(41, 29, 17, 6, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 21), (39, 31), (50, 32), (39, 37), (34, 48), (28, 37), (17, 32), (28, 28) }, (236, 93, 55, 255));
        return canvas;
    }

    private static Canvas IconWait()
    {
        var canvas = IconCanvas();
        canvas.DrawRect(20, 8, 24, 7, Outline);
        canvas.DrawRect(20, 49, 24, 7, Outline);
        canvas.DrawRect(24, 14, 16, 36, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (27, 17), (37, 17), (34, 29), (30, 29) }, (225, 231, 222, 255));
        canvas.DrawPolygon(new List<(int, int)> { (30, 35), (34, 35), (38, 47), (26, 47) }, (104, 224, 229, 255));
        canvas.DrawRect(25, 29, 14, 4, (71, 79, 90, 255));
        return canvas;
    }

    private static Canvas IconCapture()
    {
        var canvas = IconCanvas();
        canvas.DrawRect(19, 12, 5, 39, Outline);
        canvas.DrawRect(21, 14, 2, 36, (215, 222, 213, 255));
        canvas.DrawPolygon(new List<(int, int)> { (24, 14), (50, 21), (24, 31) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (26, 17), (45, 22), (26, 28) }, (102, 224, 229, 255));
        canvas.DrawEllipse(29, 50, 19, 7, Outline);
        canvas.DrawEllipse(29, 50, 15, 5, (218, 166, 52, 255));
        return canvas;
    }

    private static Canvas IconRepair()
    {
        var canvas = IconCanvas();
        canvas.DrawPolygon(new List<(int, int)> { (16, 12), (24, 10), (32, 18), (22, 28), (18, 24), (24, 18) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (18, 13), (23, 12), (29, 18), (22, 25), (20, 23), (26, 17) }, (224, 224, 215, 255));
        canvas.DrawRect(27, 27, 24, 8, Outline);
        canvas.DrawRect(29, 28, 20, 5, (224, 224, 215, 255));
        canvas.DrawPolygon(new List<(int, int)> { (37, 42), (49, 24), (53, 34), (43, 53) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (40, 42), (47, 29), (50, 34), (43, 48) }, (236, 183, 58, 255));
        return canvas;
    }

    private static Canvas IconSupply()
    {
        var canvas = IconCanvas();
        DrawCrate(canvas, 14, 27, 36, 24, (63, 68, 75, 255), (132, 140, 148, 255));
        canvas.DrawRect(28, 32, 8, 15, (91, 216, 226, 255));
        canvas.DrawRect(22, 38, 20, 8, (91, 216, 226, 255));
        canvas.DrawPolygon(new List<(int, int)> { (32, 8), (44, 22), (37, 22), (37, 29), (27, 29), (27, 22), (20, 22) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 12), (40, 21), (35, 21), (35, 27), (29, 27), (29, 21), (24, 21) }, (102, 224, 229, 255));
        return canvas;
    }

    private static Canvas IconRescue()
    {
        var canvas = IconCanvas();
        canvas.DrawEllipse(34, 17, 8, 8, Outline);
        canvas.DrawEllipse(34, 17, 5, 5, (102, 224, 229, 255));
        canvas.DrawRect(26, 26, 16, 15, Outline);
        canvas.DrawRect(28, 27, 12, 12, (102, 224, 229, 255));
        canvas.DrawRect(13, 37, 28, 9, Outline);
        canvas.DrawRect(15, 38, 24, 6, (226, 229, 219, 255));
        canvas.DrawRect(39, 31, 12, 20, Outline);
        canvas.DrawRect(41, 32, 8, 16, (226, 229, 219, 255));
        return canvas;
    }

    private static Canvas IconEndTurn()
    {
        var canvas = IconCanvas();
        canvas.DrawEllipse(30, 32, 20, 20, Outline);
        canvas.DrawEllipse(30, 32, 16, 16, (102, 224, 229, 255));
        canvas.DrawRect(30, 12, 18, 12, Canvas.Transparent);
        canvas.DrawPolygon(new List<(int, int)> { (42, 22), (56, 32), (42, 42) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (44, 26), (51, 32), (44, 38) }, (102, 224, 229, 255));
        canvas.DrawRect(48, 15, 5, 34, Outline);
        canvas.DrawRect(49, 16, 3, 32, (226, 229, 219, 255));
        return canvas;
    }

    private static Canvas IconTerrainDefense()
    {
        var canvas = IconCanvas();
        canvas.DrawEllipse(32, 46, 22, 7, Outline);
        canvas.DrawEllipse(32, 45, 18, 5, (94, 99, 100, 255));
        canvas.DrawPolygon(new List<(int, int)> { (32, 10), (48, 18), (44, 38), (32, 48), (20, 38), (16, 18) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 14), (44, 20), (41, 36), (32, 43), (23, 36), (20, 20) }, (102, 224, 229, 255));
        return canvas;
    }

    private static Canvas IconObjective()
    {
        var canvas = IconCanvas();
        canvas.DrawPolygon(new List<(int, int)> { (32, 8), (53, 32), (32, 56), (11, 32) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 14), (47, 32), (32, 50), (17, 32) }, (236, 183, 58, 255));
        canvas.DrawEllipse(32, 32, 6, 6, Outline);
        canvas.DrawEllipse(32, 32, 3, 3, (246, 230, 120, 255));
        DrawIconCorners(canvas, (102, 224, 229, 255));
        return canvas;
    }

    private static Canvas IconHqDanger()
    {
        var canvas = IconCanvas();
        canvas.DrawRect(13, 34, 38, 17, Outline);
        canvas.DrawRect(16, 31, 32, 19, (65, 68, 72, 255));
        canvas.DrawRect(21, 25, 22, 7, (116, 120, 120, 255));
        canvas.DrawPolygon(new List<(int, int)> { (32, 10), (52, 45), (12, 45) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 16), (46, 41), (18, 41) }, (224, 91, 55, 255));
        canvas.DrawRect(30, 24, 5, 11, (240, 226, 202, 255));
        canvas.DrawRect(30, 37, 5, 4, (240, 226, 202, 255));
        return canvas;
    }

    private static Canvas IconScoutRescued()
    {
        var canvas = IconCanvas();
        canvas.DrawEllipse(27, 17, 12, 9, Outline);
        canvas.DrawEllipse(27, 17, 9, 6, (102, 224, 229, 255));
        canvas.DrawRect(16, 26, 23, 21, Outline);
        canvas.DrawRect(19, 27, 17, 18, (73, 84, 96, 255));
        canvas.DrawEllipse(44, 39, 14, 14, Outline);
        canvas.DrawEllipse(44, 39, 10, 10, (102, 224, 229, 255));
        canvas.DrawPolygon(new List<(int, int)> { (38, 39), (42, 45), (52, 31), (55, 35), (43, 50), (34, 41) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (39, 39), (42, 43), (52, 31), (53, 35), (43, 47), (36, 40) }, (225, 235, 226, 255));
        return canvas;
    }

    private static Canvas IconCanvas() => new(SpriteSize, SpriteSize, Canvas.Transparent);

    private static Canvas BuildTerrainVariant(Canvas baseTile, int variant)
    {
        var canvas = Clone(baseTile);
        var warm = (byte)(185 - variant % 3 * 12);
        for (var index = 0; index < 5; index++)
        {
            var x = 7 + (variant * 11 + index * 13) % 49;
            var y = 8 + (variant * 7 + index * 9) % 45;
            canvas.DrawRect(x, y, 5 + index % 4, 2, (warm, 145, 92, 255));
            canvas.DrawRect(x + 1, y + 2, 3 + index % 3, 1, (92, 76, 62, 255));
        }

        if (variant % 4 == 0)
            canvas.DrawRect(44, 15, 3, 3, (84, 190, 197, 255));
        return canvas;
    }

    private static Canvas BuildRoadJunction(int variant, bool north, bool south, bool west = false, bool east = false)
    {
        var canvas = BuildTerrainVariant(TilePlain(), variant);
        if (north)
            DrawRoadSegment(canvas, 25, 0, 14, 39, vertical: true);
        if (south)
            DrawRoadSegment(canvas, 25, 25, 14, 39, vertical: true);
        if (west)
            DrawRoadSegment(canvas, 0, 25, 39, 14, vertical: false);
        if (east)
            DrawRoadSegment(canvas, 25, 25, 39, 14, vertical: false);
        canvas.DrawRect(25, 25, 14, 14, (65, 68, 72, 255));
        DrawTileBorder(canvas, (111, 111, 112, 255), (30, 33, 40, 255));
        return canvas;
    }

    private static Canvas BuildRoadCorner(int variant, bool east, bool north, bool south = false)
    {
        var canvas = BuildTerrainVariant(TilePlain(), variant);
        canvas.DrawRect(25, 25, 14, 39, (65, 68, 72, 255));
        canvas.DrawRect(25, 25, east ? 39 : 14, 14, (65, 68, 72, 255));
        if (north)
            canvas.DrawRect(25, 0, 14, 39, (65, 68, 72, 255));
        if (south)
            canvas.DrawRect(25, 25, 14, 39, (65, 68, 72, 255));
        canvas.DrawRect(25, 25, 39, 3, (99, 101, 104, 255));
        canvas.DrawRect(25, 36, 39, 3, (33, 36, 43, 255));
        canvas.DrawRect(25, 0, 3, 39, (99, 101, 104, 255));
        canvas.DrawRect(36, 0, 3, 64, (33, 36, 43, 255));
        DrawTileBorder(canvas, (111, 111, 112, 255), (30, 33, 40, 255));
        return canvas;
    }

    private static void DrawRoadSegment(Canvas canvas, int x, int y, int width, int height, bool vertical)
    {
        canvas.DrawRect(x, y, width, height, (65, 68, 72, 255));
        if (vertical)
        {
            canvas.DrawRect(x + 2, y, 2, height, (99, 101, 104, 255));
            canvas.DrawRect(x + width - 4, y, 2, height, (33, 36, 43, 255));
        }
        else
        {
            canvas.DrawRect(x, y + 2, width, 2, (99, 101, 104, 255));
            canvas.DrawRect(x, y + height - 4, width, 2, (33, 36, 43, 255));
        }
    }

    private static Canvas BuildObjectiveTile(int variant, bool relay)
    {
        var canvas = BuildTerrainVariant(TilePlain(), variant);
        DrawCrate(canvas, 12, 36, 40, 15, (54, 60, 68, 255), (126, 136, 144, 255));
        if (relay)
        {
            canvas.DrawRect(29, 14, 6, 30, Outline);
            canvas.DrawRect(31, 10, 2, 36, (215, 222, 213, 255));
            canvas.DrawEllipse(32, 12, 13, 5, (91, 216, 226, 255));
            canvas.DrawEllipse(32, 12, 8, 3, (18, 84, 162, 255));
        }
        else
        {
            canvas.DrawRect(20, 24, 24, 22, Outline);
            canvas.DrawRect(23, 22, 18, 22, (95, 78, 49, 255));
            canvas.DrawRect(25, 25, 14, 5, (236, 183, 58, 255));
            canvas.DrawRect(28, 34, 8, 4, (91, 216, 226, 255));
        }

        return canvas;
    }

    private static void DrawMissionReferencePanel(Canvas canvas, int x, int y, int width, int height, int missionNumber)
    {
        var palette = missionNumber switch
        {
            4 => ((byte)33, (byte)42, (byte)50, (byte)255, (byte)219, (byte)168, (byte)67, (byte)255),
            5 => ((byte)25, (byte)38, (byte)46, (byte)255, (byte)91, (byte)216, (byte)226, (byte)255),
            6 => ((byte)24, (byte)32, (byte)43, (byte)255, (byte)226, (byte)91, (byte)55, (byte)255),
            7 => ((byte)36, (byte)42, (byte)35, (byte)255, (byte)140, (byte)224, (byte)118, (byte)255),
            8 => ((byte)12, (byte)18, (byte)29, (byte)255, (byte)104, (byte)224, (byte)229, (byte)255),
            9 => ((byte)34, (byte)38, (byte)49, (byte)255, (byte)176, (byte)164, (byte)236, (byte)255),
            _ => ((byte)43, (byte)33, (byte)33, (byte)255, (byte)236, (byte)183, (byte)58, (byte)255),
        };

        var baseColor = (palette.Item1, palette.Item2, palette.Item3, palette.Item4);
        var accent = (palette.Item5, palette.Item6, palette.Item7, palette.Item8);
        canvas.DrawRect(x, y, width, height, baseColor);
        canvas.DrawRect(x, y + height - 42, width, 42, (16, 20, 26, 255));
        canvas.DrawRect(x, y, width, 4, accent);
        canvas.DrawRect(x, y + height - 4, width, 4, (5, 8, 13, 255));
        canvas.DrawRect(x, y, 4, height, accent);
        canvas.DrawRect(x + width - 4, y, 4, height, (5, 8, 13, 255));

        for (var index = 0; index < 7; index++)
        {
            var cloudX = x + 18 + index * 42;
            var cloudY = y + 22 + index % 3 * 8;
            canvas.DrawRect(cloudX, cloudY, 36, 5, (255, 255, 255, 18));
        }

        switch (missionNumber)
        {
            case 4:
                DrawFabricatorReference(canvas, x, y, accent);
                break;
            case 5:
                DrawAntennaReference(canvas, x, y, accent);
                break;
            case 6:
                DrawBridgeReference(canvas, x, y, accent);
                break;
            case 7:
                DrawSettlementReference(canvas, x, y, accent);
                break;
            case 8:
                DrawBlackoutReference(canvas, x, y, accent);
                break;
            case 9:
                DrawFogRidgeReference(canvas, x, y, accent);
                break;
            case 10:
                DrawRefineryReference(canvas, x, y, accent);
                break;
        }

        for (var marker = 0; marker < missionNumber - 3; marker++)
        {
            canvas.DrawRect(x + 18 + marker * 10, y + height - 22, 6, 6, accent);
        }
    }

    private static void DrawFabricatorReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 42, y + 116, 236, 16, (65, 68, 72, 255));
        DrawCrate(canvas, x + 48, y + 88, 42, 30, (72, 80, 90, 255), (142, 152, 164, 255));
        DrawCrate(canvas, x + 230, y + 84, 46, 34, (95, 78, 49, 255), (236, 183, 58, 255));
        canvas.DrawRect(x + 126, y + 50, 76, 68, Outline);
        canvas.DrawRect(x + 134, y + 58, 60, 58, (84, 92, 100, 255));
        canvas.DrawRect(x + 146, y + 68, 36, 10, accent);
        canvas.DrawRect(x + 158, y + 28, 12, 34, Outline);
        canvas.DrawRect(x + 162, y + 22, 72, 8, accent);
        canvas.DrawRect(x + 224, y + 30, 6, 34, (226, 229, 219, 255));
    }

    private static void DrawAntennaReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 24, y + 104, 270, 34, (42, 46, 52, 255));
        for (var band = 0; band < 5; band++)
            canvas.DrawRect(x + 20 + band * 54, y + 54 + band % 2 * 13, 76, 12, (210, 220, 225, 34));
        canvas.DrawRect(x + 150, y + 34, 10, 86, Outline);
        canvas.DrawRect(x + 154, y + 30, 3, 92, (215, 222, 213, 255));
        canvas.DrawEllipse(x + 155, y + 42, 58, 13, accent);
        canvas.DrawEllipse(x + 155, y + 42, 32, 7, (18, 84, 162, 255));
        canvas.DrawEllipse(x + 92, y + 112, 22, 14, Outline);
        canvas.DrawEllipse(x + 222, y + 112, 22, 14, Outline);
        canvas.DrawRect(x + 88, y + 108, 8, 8, accent);
        canvas.DrawRect(x + 218, y + 108, 8, 8, accent);
    }

    private static void DrawBridgeReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 10, y + 98, 300, 36, (24, 70, 96, 255));
        canvas.DrawRect(x + 24, y + 84, 272, 28, (80, 75, 68, 255));
        for (var pillar = 0; pillar < 6; pillar++)
        {
            canvas.DrawRect(x + 42 + pillar * 44, y + 72, 24, 54, Outline);
            canvas.DrawRect(x + 46 + pillar * 44, y + 76, 16, 46, (130, 128, 118, 255));
        }
        canvas.DrawRect(x + 38, y + 72, 244, 8, accent);
        canvas.DrawRect(x + 134, y + 118, 22, 14, (226, 91, 55, 255));
        canvas.DrawRect(x + 184, y + 118, 22, 14, (226, 91, 55, 255));
    }

    private static void DrawSettlementReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 20, y + 116, 280, 16, (62, 66, 61, 255));
        for (var index = 0; index < 5; index++)
        {
            var houseX = x + 34 + index * 52;
            var houseY = y + 78 + index % 2 * 12;
            canvas.DrawPolygon(new List<(int, int)> { (houseX, houseY + 18), (houseX + 22, houseY), (houseX + 44, houseY + 18) }, Outline);
            canvas.DrawPolygon(new List<(int, int)> { (houseX + 5, houseY + 18), (houseX + 22, houseY + 5), (houseX + 39, houseY + 18) }, accent);
            canvas.DrawRect(houseX + 6, houseY + 18, 32, 28, (70, 78, 72, 255));
            canvas.DrawRect(houseX + 18, houseY + 26, 8, 20, (20, 26, 30, 255));
        }
        canvas.DrawRect(x + 146, y + 42, 28, 82, Outline);
        canvas.DrawRect(x + 154, y + 36, 12, 88, (96, 110, 98, 255));
        canvas.DrawEllipse(x + 160, y + 34, 26, 8, accent);
    }

    private static void DrawBlackoutReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 20, y + 116, 280, 14, (25, 29, 38, 255));
        for (var column = 0; column < 6; column++)
        {
            canvas.DrawRect(x + 40 + column * 42, y + 46, 24, 72, Outline);
            canvas.DrawRect(x + 44 + column * 42, y + 50, 16, 66, (36, 43, 56, 255));
            if (column % 2 == 0)
                canvas.DrawRect(x + 47 + column * 42, y + 56, 10, 7, accent);
        }
        canvas.DrawRect(x + 92, y + 96, 56, 22, Outline);
        canvas.DrawRect(x + 96, y + 99, 48, 16, (226, 229, 219, 255));
        canvas.DrawRect(x + 198, y + 88, 42, 30, Outline);
        canvas.DrawRect(x + 202, y + 92, 34, 22, (54, 60, 68, 255));
        canvas.DrawRect(x + 210, y + 98, 18, 5, accent);
    }

    private static void DrawFogRidgeReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawPolygon(new List<(int, int)> { (20, 126), (78, 48), (136, 126) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (32, 122), (78, 58), (124, 122) }, (84, 87, 96, 255));
        canvas.DrawPolygon(new List<(int, int)> { (110, 128), (182, 34), (256, 128) }, Outline);
        canvas.DrawPolygon(new List<(int, int)> { (124, 122), (182, 48), (242, 122) }, (96, 98, 108, 255));
        for (var band = 0; band < 4; band++)
            canvas.DrawRect(x + 24 + band * 18, y + 54 + band * 20, 260 - band * 28, 8, (225, 231, 238, 42));
        canvas.DrawRect(x + 224, y + 88, 18, 38, Outline);
        canvas.DrawRect(x + 228, y + 92, 10, 32, accent);
        canvas.DrawEllipse(x + 233, y + 84, 20, 6, accent);
    }

    private static void DrawRefineryReference(Canvas canvas, int x, int y, (byte R, byte G, byte B, byte A) accent)
    {
        canvas.DrawRect(x + 22, y + 118, 276, 14, (48, 42, 38, 255));
        for (var stack = 0; stack < 4; stack++)
        {
            var stackX = x + 54 + stack * 54;
            canvas.DrawRect(stackX, y + 44 - stack % 2 * 10, 24, 80 + stack % 2 * 10, Outline);
            canvas.DrawRect(stackX + 4, y + 48 - stack % 2 * 10, 16, 72 + stack % 2 * 10, (74, 76, 78, 255));
            canvas.DrawRect(stackX + 7, y + 54, 10, 8, accent);
        }
        canvas.DrawRect(x + 192, y + 84, 72, 34, Outline);
        canvas.DrawRect(x + 198, y + 90, 60, 22, (96, 78, 56, 255));
        canvas.DrawRect(x + 208, y + 96, 40, 6, accent);
        canvas.DrawEllipse(x + 96, y + 94, 34, 22, Outline);
        canvas.DrawEllipse(x + 96, y + 94, 26, 16, (126, 50, 44, 255));
        canvas.DrawRect(x + 84, y + 86, 24, 5, accent);
    }

    private static Canvas Clone(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        canvas.CopyNonTransparent(source, 0, 0);
        return canvas;
    }

    private static Canvas MirrorTile(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
            for (var col = 0; col < source.Width; col++)
                canvas.SetPixel(source.Width - 1 - col, row, source.Pixels[row][col]);
        return canvas;
    }

    private static Canvas RotateTile(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
            for (var col = 0; col < source.Width; col++)
                canvas.SetPixel(source.Height - 1 - row, col, source.Pixels[row][col]);
        return canvas;
    }

    private static Canvas ScaleNearest(Canvas source, int width, int height)
    {
        var output = new Canvas(width, height, Canvas.Transparent);
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                var sourceX = Math.Min(source.Width - 1, col * source.Width / width);
                var sourceY = Math.Min(source.Height - 1, row * source.Height / height);
                output.SetPixel(col, row, source.Pixels[sourceY][sourceX]);
            }
        }

        return output;
    }

    private static void DrawIconCorners(Canvas canvas, (byte R, byte G, byte B, byte A) color)
    {
        canvas.DrawRect(10, 10, 8, 3, color);
        canvas.DrawRect(10, 10, 3, 8, color);
        canvas.DrawRect(46, 10, 8, 3, color);
        canvas.DrawRect(51, 10, 3, 8, color);
        canvas.DrawRect(10, 51, 8, 3, color);
        canvas.DrawRect(10, 46, 3, 8, color);
        canvas.DrawRect(46, 51, 8, 3, color);
        canvas.DrawRect(51, 46, 3, 8, color);
    }

    // Helpers

    private static void DrawCommonShadow(Canvas canvas)
    {
        canvas.DrawEllipse(32, 54, 22, 6, SoftShadow);
    }

    private static void DrawTileBorder(Canvas canvas, (byte R, byte G, byte B, byte A) topColor, (byte R, byte G, byte B, byte A) bottomColor)
    {
        canvas.DrawRect(0, 0, SpriteSize, 2, topColor);
        canvas.DrawRect(0, SpriteSize - 3, SpriteSize, 3, bottomColor);
        canvas.DrawRect(0, 0, 2, SpriteSize, topColor);
        canvas.DrawRect(SpriteSize - 2, 0, 2, SpriteSize, bottomColor);
    }

    private static Dictionary<string, (byte, byte, byte, byte)> GetPlayerPalette() => new()
    {
        { "light", (215, 240, 255, 255) },
        { "mid", (59, 166, 238, 255) },
        { "dark", (18, 84, 162, 255) },
        { "accent", (97, 240, 248, 255) },
        { "deep", (10, 42, 95, 255) },
    };

    private static Dictionary<string, (byte, byte, byte, byte)> GetEnemyPalette() => new()
    {
        { "light", (255, 218, 190, 255) },
        { "mid", (235, 83, 60, 255) },
        { "dark", (138, 31, 44, 255) },
        { "accent", (255, 176, 75, 255) },
        { "deep", (82, 18, 34, 255) },
    };
}
