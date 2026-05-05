using System;
using System.Collections.Generic;
using System.IO;
using Wargame.Graphics;

static class TerrainMaskGenerator
{
    private const int Size = 768;
    private static readonly (byte R, byte G, byte B, byte A) Basin = (170, 135, 82, 255);
    private static readonly (byte R, byte G, byte B, byte A) Road = (55, 58, 60, 255);
    private static readonly (byte R, byte G, byte B, byte A) River = (35, 96, 132, 255);
    private static readonly (byte R, byte G, byte B, byte A) Bank = (72, 66, 54, 255);
    private static readonly (byte R, byte G, byte B, byte A) Bridge = (128, 105, 76, 255);

    public static int Generate(string repoRoot, string[] args)
    {
        var outputDirectory = ResolvePath(repoRoot, args.Length > 0 ? args[0] : Path.Combine(
            "game",
            "WargamePrototype",
            "assets",
            "art-handoff",
            "local-candidates",
            "request11-topology-masks-v1"));
        Directory.CreateDirectory(outputDirectory);

        var masks = new Dictionary<string, Canvas>
        {
            ["road-horizontal-mask.png"] = RoadHorizontal(),
            ["road-horizontal-guide.png"] = RoadHorizontalGuide(),
            ["road-horizontal-dirt-guide.png"] = RoadHorizontalDirtGuide(),
            ["road-corner-ne-mask.png"] = RoadCornerNorthEast(),
            ["road-junction-mask.png"] = RoadJunction(),
            ["river-horizontal-mask.png"] = RiverHorizontal(),
            ["river-horizontal-guide.png"] = RiverHorizontalGuide(),
            ["river-corner-ne-mask.png"] = RiverCornerNorthEast(),
            ["bridge-horizontal-road-vertical-river-mask.png"] = BridgeHorizontalRoadVerticalRiver(),
        };

        foreach (var (fileName, canvas) in masks)
        {
            var path = Path.Combine(outputDirectory, fileName);
            PngWriter.WriteRgbaPng(path, canvas.Pixels);
            Console.WriteLine($"Wrote {RelativePath(repoRoot, path)}");
        }

        return 0;
    }

    private static Canvas RoadHorizontal()
    {
        var canvas = Base();
        DrawRoad(canvas, 0, 324, Size, 120);
        return canvas;
    }

    private static Canvas RoadHorizontalGuide()
    {
        var canvas = TexturedBase();
        DrawRoad(canvas, 0, 314, Size, 140);
        canvas.DrawRect(0, 326, Size, 96, (48, 51, 52, 255));
        AddRoadWear(canvas, 326, 422);
        return canvas;
    }

    private static Canvas RoadHorizontalDirtGuide()
    {
        var canvas = TexturedBase();
        canvas.DrawRect(0, 306, Size, 156, (118, 95, 67, 255));
        canvas.DrawRect(0, 324, Size, 112, (142, 116, 78, 255));
        canvas.DrawRect(0, 348, Size, 18, (95, 79, 62, 255));
        canvas.DrawRect(0, 398, Size, 18, (95, 79, 62, 255));
        AddDirtRoadWear(canvas, 324, 436);
        return canvas;
    }

    private static Canvas RoadCornerNorthEast()
    {
        var canvas = Base();
        DrawRoad(canvas, 324, 0, 120, 444);
        DrawRoad(canvas, 324, 324, 444, 120);
        return canvas;
    }

    private static Canvas RoadJunction()
    {
        var canvas = Base();
        DrawRoad(canvas, 0, 324, Size, 120);
        DrawRoad(canvas, 324, 0, 120, Size);
        return canvas;
    }

    private static Canvas RiverHorizontal()
    {
        var canvas = Base();
        DrawRiver(canvas, 0, 300, Size, 168);
        return canvas;
    }

    private static Canvas RiverHorizontalGuide()
    {
        var canvas = TexturedBase();
        DrawRiver(canvas, 0, 286, Size, 196);
        canvas.DrawRect(0, 318, Size, 132, (30, 88, 124, 255));
        AddRiverWear(canvas, 318, 450);
        return canvas;
    }

    private static Canvas RiverCornerNorthEast()
    {
        var canvas = Base();
        DrawRiver(canvas, 300, 0, 168, 468);
        DrawRiver(canvas, 300, 300, 468, 168);
        return canvas;
    }

    private static Canvas BridgeHorizontalRoadVerticalRiver()
    {
        var canvas = Base();
        DrawRiver(canvas, 300, 0, 168, Size);
        DrawRoad(canvas, 0, 324, Size, 120);
        canvas.DrawRect(260, 294, 248, 180, Bridge);
        canvas.DrawRect(0, 334, Size, 100, Road);
        return canvas;
    }

    private static Canvas Base() => new(Size, Size, Basin);

    private static Canvas TexturedBase()
    {
        var canvas = Base();
        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var value = Hash(col, row) % 17 - 8;
                canvas.SetPixel(col, row, Shift(Basin, value));
            }
        }

        for (var index = 0; index < 220; index++)
        {
            var x = Hash(index, 17) % Size;
            var y = Hash(index, 41) % Size;
            var width = 3 + Hash(index, 73) % 11;
            var color = Shift((118, 95, 67, 255), Hash(index, 91) % 15 - 7);
            canvas.DrawRect(x, y, width, 2, color);
        }

        return canvas;
    }

    private static void DrawRoad(Canvas canvas, int x, int y, int width, int height) =>
        canvas.DrawRect(x, y, width, height, Road);

    private static void DrawRiver(Canvas canvas, int x, int y, int width, int height)
    {
        canvas.DrawRect(x - 24, y - 24, width + 48, height + 48, Bank);
        canvas.DrawRect(x, y, width, height, River);
    }

    private static void AddRoadWear(Canvas canvas, int top, int bottom)
    {
        for (var index = 0; index < 150; index++)
        {
            var x = Hash(index, 101) % Size;
            var y = top + Hash(index, 131) % (bottom - top);
            var width = 5 + Hash(index, 149) % 21;
            var color = index % 3 == 0
                ? ((byte)72, (byte)72, (byte)68, (byte)255)
                : ((byte)36, (byte)38, (byte)39, (byte)255);
            canvas.DrawRect(x, y, width, 2, color);
        }

        canvas.DrawRect(0, top, Size, 4, (76, 66, 54, 255));
        canvas.DrawRect(0, bottom - 4, Size, 4, (76, 66, 54, 255));
    }

    private static void AddDirtRoadWear(Canvas canvas, int top, int bottom)
    {
        for (var index = 0; index < 170; index++)
        {
            var x = Hash(index, 307) % Size;
            var y = top + Hash(index, 331) % (bottom - top);
            var width = 6 + Hash(index, 353) % 24;
            var color = index % 3 == 0
                ? ((byte)174, (byte)145, (byte)93, (byte)255)
                : ((byte)90, (byte)73, (byte)55, (byte)255);
            canvas.DrawRect(x, y, width, 2, color);
        }
    }

    private static void AddRiverWear(Canvas canvas, int top, int bottom)
    {
        for (var index = 0; index < 120; index++)
        {
            var x = Hash(index, 211) % Size;
            var y = top + Hash(index, 227) % (bottom - top);
            var width = 7 + Hash(index, 251) % 25;
            var color = index % 2 == 0
                ? ((byte)73, (byte)139, (byte)164, (byte)255)
                : ((byte)19, (byte)70, (byte)106, (byte)255);
            canvas.DrawRect(x, y, width, 2, color);
        }
    }

    private static (byte R, byte G, byte B, byte A) Shift((byte R, byte G, byte B, byte A) color, int amount) =>
        ((byte)Math.Clamp(color.R + amount, 0, 255),
            (byte)Math.Clamp(color.G + amount, 0, 255),
            (byte)Math.Clamp(color.B + amount, 0, 255),
            color.A);

    private static int Hash(int x, int y)
    {
        unchecked
        {
            var hash = x * 73856093 ^ y * 19349663;
            hash ^= hash >> 13;
            hash *= 83492791;
            return Math.Abs(hash);
        }
    }

    private static string ResolvePath(string repoRoot, string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(repoRoot, path));

    private static string RelativePath(string repoRoot, string path) =>
        Path.GetRelativePath(repoRoot, path).Replace(Path.DirectorySeparatorChar, '/');
}