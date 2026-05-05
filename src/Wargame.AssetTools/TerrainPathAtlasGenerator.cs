using System;
using System.IO;
using Wargame.Graphics;

static class TerrainPathAtlasGenerator
{
    private const int SpriteSize = 64;
    private const int Columns = 16;

    public static int Generate(string assetBase)
    {
        var incoming = Path.Combine(assetBase, "art-handoff", "incoming");
        var ground = PngReader.ReadRgbaPng(Path.Combine(incoming, "ChatGPT Image May 4, 2026, 02_22_26 PM.png"));
        var road = PngReader.ReadRgbaPng(Path.Combine(incoming, "ChatGPT Image May 4, 2026, 02_13_43 PM.png"));
        var river = PngReader.ReadRgbaPng(Path.Combine(incoming, "ChatGPT Image May 4, 2026, 02_22_40 PM.png"));

        var atlas = new Canvas(SpriteSize * Columns, SpriteSize * 4, Canvas.Transparent);
        for (var mask = 0; mask < Columns; mask++)
        {
            atlas.CopyNonTransparent(BuildRoadTile(ground, road, mask), mask * SpriteSize, 0);
            atlas.CopyNonTransparent(BuildRiverTile(ground, river, mask), mask * SpriteSize, SpriteSize);
            atlas.CopyNonTransparent(BuildBridgeTile(ground, road, river, mask, verticalRiver: true), mask * SpriteSize, SpriteSize * 2);
            atlas.CopyNonTransparent(BuildBridgeTile(ground, road, river, mask, verticalRiver: false), mask * SpriteSize, SpriteSize * 3);
        }

        var outputPath = Path.Combine(assetBase, "sprites", "art_paths.png");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        PngWriter.WriteRgbaPng(outputPath, atlas.Pixels);
        Console.WriteLine($"Generated path topology atlas at {outputPath}");
        return 0;
    }

    private static Canvas BuildRoadTile(Canvas ground, Canvas road, int mask)
    {
        var tile = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        var effectiveMask = mask == 0 ? 10 : mask;
        for (var y = 0; y < SpriteSize; y++)
        {
            for (var x = 0; x < SpriteSize; x++)
            {
                var groundColor = NormalizeMaterial(SampleTile(ground, x, y, mask * 5, mask * 7), 154, 124, 82, 0.50);
                var roadColor = NormalizeMaterial(SampleTile(road, x, y, mask * 11, mask * 3), 122, 100, 72, 0.76);
                var distance = RoadDistance(x, y, effectiveMask);
                if (distance <= 7)
                {
                    var wear = RoadWear(x, y, mask);
                    tile.SetPixel(x, y, Shift(roadColor, wear - 10));
                }
                else if (distance <= 13)
                {
                    var blend = 1.0 - ((distance - 7) / 6.0);
                    tile.SetPixel(x, y, Blend(groundColor, roadColor, 0.32 + blend * 0.35));
                }
                else
                {
                    tile.SetPixel(x, y, groundColor);
                }
            }
        }

        AddRoadEdgeDefinition(tile, effectiveMask);
        return tile;
    }

    private static Canvas BuildRiverTile(Canvas ground, Canvas river, int mask)
    {
        var tile = new Canvas(SpriteSize, SpriteSize, Canvas.Transparent);
        var effectiveMask = mask == 0 ? 5 : mask;
        for (var y = 0; y < SpriteSize; y++)
        {
            for (var x = 0; x < SpriteSize; x++)
            {
                var groundColor = NormalizeMaterial(SampleTile(ground, x, y, mask * 7, mask * 13), 142, 116, 80, 0.42);
                var waterColor = NormalizeMaterial(SampleTile(river, x, y, mask * 5, mask * 17), 32, 91, 123, 0.82);
                var bankColor = NormalizeMaterial(SampleTile(river, x, y, mask * 19, mask * 23), 76, 70, 58, 0.50);
                var distance = RoadDistance(x, y, effectiveMask);
                if (distance <= 8)
                {
                    var shimmer = RiverWear(x, y, mask);
                    tile.SetPixel(x, y, Shift(waterColor, shimmer));
                }
                else if (distance <= 14)
                {
                    var blend = 1.0 - ((distance - 8) / 6.0);
                    tile.SetPixel(x, y, Blend(groundColor, bankColor, 0.45 + blend * 0.42));
                }
                else
                {
                    tile.SetPixel(x, y, groundColor);
                }
            }
        }

        AddRiverEdgeDefinition(tile, effectiveMask);
        return tile;
    }

    private static Canvas BuildBridgeTile(Canvas ground, Canvas road, Canvas river, int roadMask, bool verticalRiver)
    {
        var tile = BuildRiverTile(ground, river, verticalRiver ? 5 : 10);
        var effectiveRoadMask = roadMask == 0 ? verticalRiver ? 10 : 5 : roadMask;
        for (var y = 0; y < SpriteSize; y++)
        {
            for (var x = 0; x < SpriteSize; x++)
            {
                var distance = RoadDistance(x, y, effectiveRoadMask);
                if (distance > 13)
                {
                    continue;
                }

                var roadColor = NormalizeMaterial(SampleTile(road, x, y, roadMask * 11, roadMask * 3), 132, 108, 76, 0.70);
                if (distance <= 7)
                {
                    tile.SetPixel(x, y, Shift(roadColor, RoadWear(x, y, roadMask) - 6));
                }
                else
                {
                    var bridgeColor = NormalizeMaterial(SampleTile(road, x, y, roadMask * 13, roadMask * 5), 146, 124, 88, 0.46);
                    tile.SetPixel(x, y, Blend(tile.Pixels[y][x], bridgeColor, 0.78));
                }
            }
        }

        AddBridgeDeckLines(tile, verticalRiver, effectiveRoadMask);
        AddRoadEdgeDefinition(tile, effectiveRoadMask);
        return tile;
    }

    private static double RoadDistance(int x, int y, int mask)
    {
        var hasNorth = (mask & 1) != 0;
        var hasEast = (mask & 2) != 0;
        var hasSouth = (mask & 4) != 0;
        var hasWest = (mask & 8) != 0;
        var distance = DistanceToRect(x, y, 23, 23, 18, 18);
        if (hasNorth)
        {
            distance = Math.Min(distance, DistanceToRect(x, y, 23, 0, 18, 32));
        }

        if (hasEast)
        {
            distance = Math.Min(distance, DistanceToRect(x, y, 32, 23, 32, 18));
        }

        if (hasSouth)
        {
            distance = Math.Min(distance, DistanceToRect(x, y, 23, 32, 18, 32));
        }

        if (hasWest)
        {
            distance = Math.Min(distance, DistanceToRect(x, y, 0, 23, 32, 18));
        }

        return distance;
    }

    private static double DistanceToRect(int x, int y, int rectX, int rectY, int width, int height)
    {
        var dx = Math.Max(Math.Max(rectX - x, 0), x - (rectX + width - 1));
        var dy = Math.Max(Math.Max(rectY - y, 0), y - (rectY + height - 1));
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static void AddRoadEdgeDefinition(Canvas tile, int mask)
    {
        for (var y = 0; y < SpriteSize; y++)
        {
            for (var x = 0; x < SpriteSize; x++)
            {
                var distance = RoadDistance(x, y, mask);
                if (distance is > 7 and <= 8.2)
                {
                    tile.SetPixel(x, y, (88, 71, 52, 255));
                }
            }
        }
    }

    private static void AddRiverEdgeDefinition(Canvas tile, int mask)
    {
        for (var y = 0; y < SpriteSize; y++)
        {
            for (var x = 0; x < SpriteSize; x++)
            {
                var distance = RoadDistance(x, y, mask);
                if (distance is > 8 and <= 9.2)
                {
                    tile.SetPixel(x, y, (42, 57, 62, 255));
                }
            }
        }
    }

    private static void AddBridgeDeckLines(Canvas tile, bool verticalRiver, int roadMask)
    {
        for (var index = 0; index < 5; index++)
        {
            var offset = 18 + index * 7;
            if (verticalRiver)
            {
                for (var y = 23; y < 41; y++)
                {
                    if (RoadDistance(offset, y, roadMask) <= 8)
                    {
                        tile.SetPixel(offset, y, (92, 76, 58, 255));
                    }
                }
            }
            else
            {
                for (var x = 23; x < 41; x++)
                {
                    if (RoadDistance(x, offset, roadMask) <= 8)
                    {
                        tile.SetPixel(x, offset, (92, 76, 58, 255));
                    }
                }
            }
        }
    }

    private static int RoadWear(int x, int y, int seed)
    {
        var hash = Hash(x / 3 + seed * 19, y / 2 + seed * 31);
        if ((y is >= 28 and <= 30) || (y is >= 35 and <= 37))
        {
            return -10;
        }

        return hash % 13 - 6;
    }

    private static int RiverWear(int x, int y, int seed)
    {
        var hash = Hash(x / 4 + seed * 29, y / 3 + seed * 37);
        return hash % 19 - 7;
    }

    private static (byte R, byte G, byte B, byte A) SampleTile(Canvas canvas, int x, int y, int offsetX, int offsetY)
    {
        var sourceX = Math.Clamp((x * canvas.Width / SpriteSize + offsetX * canvas.Width / SpriteSize) % canvas.Width, 0, canvas.Width - 1);
        var sourceY = Math.Clamp((y * canvas.Height / SpriteSize + offsetY * canvas.Height / SpriteSize) % canvas.Height, 0, canvas.Height - 1);
        return canvas.Pixels[sourceY][sourceX];
    }

    private static (byte R, byte G, byte B, byte A) NormalizeMaterial(
        (byte R, byte G, byte B, byte A) color,
        byte baseR,
        byte baseG,
        byte baseB,
        double strength)
    {
        var luminance = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) - 128;
        var detail = Math.Clamp(luminance * strength, -36, 36);
        return ((byte)Math.Clamp(baseR + detail, 0, 255),
            (byte)Math.Clamp(baseG + detail, 0, 255),
            (byte)Math.Clamp(baseB + detail, 0, 255),
            255);
    }

    private static (byte R, byte G, byte B, byte A) Shift((byte R, byte G, byte B, byte A) color, int amount) =>
        ((byte)Math.Clamp(color.R + amount, 0, 255),
            (byte)Math.Clamp(color.G + amount, 0, 255),
            (byte)Math.Clamp(color.B + amount, 0, 255),
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
}