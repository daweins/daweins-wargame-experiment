using System;
using System.IO;
using Wargame.Graphics;

static class TerrainTextureCompositor
{
    private const int Size = 768;
    private const int TileSize = 64;
    private const int RoadTop = 306;
    private const int RoadBottom = 462;
    private const int RoadCoreTop = 328;
    private const int RoadCoreBottom = 438;
    private const int TileRoadTop = 25;
    private const int TileRoadBottom = 39;
    private const int TileRoadCoreTop = 28;
    private const int TileRoadCoreBottom = 36;

    public static int Generate(string repoRoot, string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: terrain-compose <output-directory> <ground-texture.png> <road-texture.png>");
            return 1;
        }

        var outputDirectory = ResolvePath(repoRoot, args[0]);
        var ground = PngReader.ReadRgbaPng(ResolvePath(repoRoot, args[1]));
        var road = PngReader.ReadRgbaPng(ResolvePath(repoRoot, args[2]));
        Directory.CreateDirectory(outputDirectory);

        var canvas = new Canvas(Size, Size, Canvas.Transparent);
        for (var y = 0; y < Size; y++)
        {
            for (var x = 0; x < Size; x++)
            {
                var groundColor = Sample(ground, x, y);
                var roadColor = Sample(road, x, y + 97);

                if (y >= RoadCoreTop && y < RoadCoreBottom)
                {
                    canvas.SetPixel(x, y, DarkenRoad(roadColor));
                }
                else if (y >= RoadTop && y < RoadBottom)
                {
                    var edgeDistance = Math.Min(Math.Abs(y - RoadTop), Math.Abs(y - RoadBottom));
                    var roadWeight = edgeDistance < 14 ? 0.45 : 0.72;
                    canvas.SetPixel(x, y, Blend(groundColor, roadColor, roadWeight));
                }
                else
                {
                    canvas.SetPixel(x, y, groundColor);
                }
            }
        }

        AddRoadDefinition(canvas);
        var path = Path.Combine(outputDirectory, "road-horizontal-composite.png");
        PngWriter.WriteRgbaPng(path, canvas.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, path)}");

        var tile = ComposeTileScale(ground, road);
        var tilePath = Path.Combine(outputDirectory, "road-horizontal-composite-64.png");
        PngWriter.WriteRgbaPng(tilePath, tile.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, tilePath)}");

        var pixelPreview = ScaleNearest(tile, Size, Size);
        var previewPath = Path.Combine(outputDirectory, "road-horizontal-composite-pixel-preview.png");
        PngWriter.WriteRgbaPng(previewPath, pixelPreview.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, previewPath)}");
        return 0;
    }

    public static int GenerateFromDirectRoadCandidate(string repoRoot, string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: terrain-salvage-road <output-directory> <direct-road-candidate.png>");
            return 1;
        }

        var outputDirectory = ResolvePath(repoRoot, args[0]);
        var source = PngReader.ReadRgbaPng(ResolvePath(repoRoot, args[1]));
        Directory.CreateDirectory(outputDirectory);

        var tile = new Canvas(TileSize, TileSize, Canvas.Transparent);
        for (var y = 0; y < TileSize; y++)
        {
            for (var x = 0; x < TileSize; x++)
            {
                var groundColor = SampleRect(source, x, y, 72, 72, 246, 246);
                var roadColor = SampleRect(source, x, y, 48, 350, 270, 88);
                var normalizedGround = NormalizeMaterial(groundColor, (byte)166, (byte)132, (byte)84, 0.82);
                var normalizedRoad = NormalizeMaterial(roadColor, (byte)138, (byte)116, (byte)82, 0.78);

                if (y >= TileRoadCoreTop && y < TileRoadCoreBottom)
                {
                    tile.SetPixel(x, y, DarkenRoad(normalizedRoad));
                }
                else if (y >= TileRoadTop && y < TileRoadBottom)
                {
                    var edgeDistance = Math.Min(Math.Abs(y - TileRoadTop), Math.Abs(y - TileRoadBottom));
                    var roadWeight = edgeDistance < 2 ? 0.34 : 0.68;
                    tile.SetPixel(x, y, Blend(normalizedGround, normalizedRoad, roadWeight));
                }
                else
                {
                    tile.SetPixel(x, y, normalizedGround);
                }
            }
        }

        for (var x = 0; x < TileSize; x++)
        {
            tile.SetPixel(x, TileRoadTop, (116, 88, 57, 255));
            tile.SetPixel(x, TileRoadBottom - 1, (116, 88, 57, 255));
        }

        var tilePath = Path.Combine(outputDirectory, "road-horizontal-salvage-64.png");
        PngWriter.WriteRgbaPng(tilePath, tile.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, tilePath)}");

        var preview = ScaleNearest(tile, Size, Size);
        var previewPath = Path.Combine(outputDirectory, "road-horizontal-salvage-pixel-preview.png");
        PngWriter.WriteRgbaPng(previewPath, preview.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, previewPath)}");
        return 0;
    }

    private static Canvas ComposeTileScale(Canvas ground, Canvas road)
    {
        var canvas = new Canvas(TileSize, TileSize, Canvas.Transparent);
        for (var y = 0; y < TileSize; y++)
        {
            for (var x = 0; x < TileSize; x++)
            {
                var groundColor = SampleTile(ground, x, y, 9, 5);
                var roadColor = SampleTile(road, x, y, 17, 21);
                var normalizedGround = NormalizeMaterial(groundColor, (byte)166, (byte)132, (byte)84, 0.55);
                var normalizedRoad = NormalizeMaterial(roadColor, (byte)118, (byte)103, (byte)82, 0.65);

                if (y >= TileRoadCoreTop && y < TileRoadCoreBottom)
                {
                    canvas.SetPixel(x, y, DarkenRoad(normalizedRoad));
                }
                else if (y >= TileRoadTop && y < TileRoadBottom)
                {
                    var edgeDistance = Math.Min(Math.Abs(y - TileRoadTop), Math.Abs(y - TileRoadBottom));
                    var roadWeight = edgeDistance < 2 ? 0.48 : 0.76;
                    canvas.SetPixel(x, y, Blend(normalizedGround, normalizedRoad, roadWeight));
                }
                else
                {
                    canvas.SetPixel(x, y, normalizedGround);
                }
            }
        }

        for (var x = 0; x < TileSize; x++)
        {
            canvas.SetPixel(x, TileRoadTop, (92, 74, 55, 255));
            canvas.SetPixel(x, TileRoadBottom - 1, (92, 74, 55, 255));
        }

        return canvas;
    }

    private static (byte R, byte G, byte B, byte A) NormalizeMaterial(
        (byte R, byte G, byte B, byte A) color,
        byte baseR,
        byte baseG,
        byte baseB,
        double strength)
    {
        var luminance = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) - 128;
        var detail = Math.Clamp(luminance * strength, -34, 34);
        return ((byte)Math.Clamp(baseR + detail, 0, 255),
            (byte)Math.Clamp(baseG + detail, 0, 255),
            (byte)Math.Clamp(baseB + detail, 0, 255),
            255);
    }

    private static (byte R, byte G, byte B, byte A) Sample(Canvas canvas, int x, int y)
    {
        var sourceX = Math.Clamp(x * canvas.Width / Size, 0, canvas.Width - 1);
        var sourceY = Math.Clamp(y * canvas.Height / Size, 0, canvas.Height - 1);
        return canvas.Pixels[sourceY][sourceX];
    }

    private static (byte R, byte G, byte B, byte A) SampleTile(Canvas canvas, int x, int y, int offsetX, int offsetY)
    {
        var sourceX = Math.Clamp((x * canvas.Width / TileSize + offsetX * canvas.Width / TileSize) % canvas.Width, 0, canvas.Width - 1);
        var sourceY = Math.Clamp((y * canvas.Height / TileSize + offsetY * canvas.Height / TileSize) % canvas.Height, 0, canvas.Height - 1);
        return canvas.Pixels[sourceY][sourceX];
    }

    private static (byte R, byte G, byte B, byte A) SampleRect(Canvas canvas, int x, int y, int rectX, int rectY, int rectWidth, int rectHeight)
    {
        var sourceX = Math.Clamp(rectX + x * rectWidth / TileSize, 0, canvas.Width - 1);
        var sourceY = Math.Clamp(rectY + y * rectHeight / TileSize, 0, canvas.Height - 1);
        return canvas.Pixels[sourceY][sourceX];
    }

    private static Canvas ScaleNearest(Canvas source, int width, int height)
    {
        var scaled = new Canvas(width, height, Canvas.Transparent);
        for (var y = 0; y < height; y++)
        {
            var sourceY = y * source.Height / height;
            for (var x = 0; x < width; x++)
            {
                var sourceX = x * source.Width / width;
                scaled.SetPixel(x, y, source.Pixels[sourceY][sourceX]);
            }
        }

        return scaled;
    }

    private static (byte R, byte G, byte B, byte A) DarkenRoad((byte R, byte G, byte B, byte A) color) =>
        ((byte)Math.Clamp(color.R * 0.72, 0, 255),
            (byte)Math.Clamp(color.G * 0.70, 0, 255),
            (byte)Math.Clamp(color.B * 0.66, 0, 255),
            color.A);

    private static (byte R, byte G, byte B, byte A) Blend(
        (byte R, byte G, byte B, byte A) left,
        (byte R, byte G, byte B, byte A) right,
        double rightWeight)
    {
        var leftWeight = 1.0 - rightWeight;
        return ((byte)Math.Clamp(left.R * leftWeight + right.R * rightWeight, 0, 255),
            (byte)Math.Clamp(left.G * leftWeight + right.G * rightWeight, 0, 255),
            (byte)Math.Clamp(left.B * leftWeight + right.B * rightWeight, 0, 255),
            255);
    }

    private static void AddRoadDefinition(Canvas canvas)
    {
        for (var x = 0; x < Size; x++)
        {
            canvas.SetPixel(x, RoadTop, (87, 70, 52, 255));
            canvas.SetPixel(x, RoadTop + 1, (107, 85, 59, 255));
            canvas.SetPixel(x, RoadBottom - 2, (107, 85, 59, 255));
            canvas.SetPixel(x, RoadBottom - 1, (87, 70, 52, 255));
        }
    }

    private static string ResolvePath(string repoRoot, string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(repoRoot, path));

    private static string RelativePath(string repoRoot, string path) =>
        Path.GetRelativePath(repoRoot, path).Replace(Path.DirectorySeparatorChar, '/');
}