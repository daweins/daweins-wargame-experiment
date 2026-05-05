using System;
using System.IO;
using Wargame.Graphics;

static class TerrainPathReviewPacket
{
    private const int SpriteSize = 64;
    private const int BoardColumns = 12;
    private const int BoardRows = 8;

    public static int Generate(string repoRoot, string[] args)
    {
        var outputDirectory = ResolvePath(repoRoot, args.Length > 0 ? args[0] : Path.Combine(
            "game",
            "WargamePrototype",
            "assets",
            "art-handoff",
            "local-review",
            "request11-path-atlas-v1"));
        Directory.CreateDirectory(outputDirectory);

        var spritePath = Path.Combine(repoRoot, "game", "WargamePrototype", "assets", "sprites");
        var paths = PngReader.ReadRgbaPng(Path.Combine(spritePath, "art_paths.png"));
        var terrain = PngReader.ReadRgbaPng(Path.Combine(spritePath, "art_terrain.png"));
        var units = PngReader.ReadRgbaPng(Path.Combine(spritePath, "art_units.png"));

        var atlasPreview = ScaleNearest(paths, paths.Width * 2, paths.Height * 2);
        var atlasPreviewPath = Path.Combine(outputDirectory, "path-atlas-2x-preview.png");
        PngWriter.WriteRgbaPng(atlasPreviewPath, atlasPreview.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, atlasPreviewPath)}");

        var bridgePreview = BuildBridgePreview(paths, terrain, units);
        var bridgePreviewPath = Path.Combine(outputDirectory, "path-bridge-readability.png");
        PngWriter.WriteRgbaPng(bridgePreviewPath, bridgePreview.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, bridgePreviewPath)}");

        var board = BuildBoardReview(paths, terrain, units);
        var boardPath = Path.Combine(outputDirectory, "path-board-readability.png");
        PngWriter.WriteRgbaPng(boardPath, board.Pixels);
        Console.WriteLine($"Wrote {RelativePath(repoRoot, boardPath)}");
        return 0;
    }

    private static Canvas BuildBoardReview(Canvas paths, Canvas terrain, Canvas units)
    {
        var board = new Canvas(BoardColumns * SpriteSize, BoardRows * SpriteSize, (10, 15, 21, 255));
        var pathMasks = new int[,]
        {
            { 0, 0, 0, 4, 0, 0, 0, 6, 8, 10, 10, 12 },
            { 0, 2, 10, 15, 8, 0, 0, 5, 0, 0, 0, 5 },
            { 0, 0, 0, 5, 0, 2, 10, 15, 8, 0, 0, 5 },
            { 10, 10, 10, 13, 0, 0, 0, 5, 0, 0, 0, 5 },
            { 0, 0, 0, 5, 0, 0, 0, 3, 10, 10, 10, 9 },
            { 0, 2, 10, 15, 8, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 5, 0, 0, 2, 10, 10, 8, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        for (var row = 0; row < BoardRows; row++)
        {
            for (var col = 0; col < BoardColumns; col++)
            {
                var mask = pathMasks[row, col];
                Canvas tile;
                if (mask != 0)
                {
                    var isRiver = col >= 7 || (row == 4 && col >= 7);
                    tile = Crop(paths, mask * SpriteSize, isRiver ? SpriteSize : 0, SpriteSize, SpriteSize);
                }
                else
                {
                    var terrainIndex = (col + row * 3) % 5 switch
                    {
                        0 => 0,
                        1 => 5,
                        2 => 6,
                        3 => 7,
                        _ => 8
                    };
                    tile = Crop(terrain, terrainIndex * SpriteSize, 0, SpriteSize, SpriteSize);
                }

                board.CopyNonTransparent(tile, col * SpriteSize, row * SpriteSize);
            }
        }

        PlaceUnit(board, units, 1, 2, 2, 0);
        PlaceUnit(board, units, 4, 3, 1, 0);
        PlaceUnit(board, units, 8, 2, 2, 1);
        PlaceUnit(board, units, 10, 4, 4, 1);
        DrawOutline(board, 3 * SpriteSize, SpriteSize, SpriteSize, SpriteSize, (246, 200, 95, 255), 3);
        DrawOutline(board, 8 * SpriteSize, 4 * SpriteSize, SpriteSize, SpriteSize, (56, 199, 255, 255), 3);
        DrawObjectiveMarker(board, 3, 3, (246, 200, 95, 255));
        DrawObjectiveMarker(board, 9, 3, (56, 199, 255, 255));
        return board;
    }

    private static Canvas BuildBridgePreview(Canvas paths, Canvas terrain, Canvas units)
    {
        var board = new Canvas(6 * SpriteSize, 3 * SpriteSize, (10, 15, 21, 255));
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 6; col++)
            {
                board.CopyNonTransparent(Crop(terrain, 0, 0, SpriteSize, SpriteSize), col * SpriteSize, row * SpriteSize);
            }
        }

        board.CopyNonTransparent(Crop(paths, 5 * SpriteSize, SpriteSize, SpriteSize, SpriteSize), 2 * SpriteSize, 0);
        board.CopyNonTransparent(Crop(paths, 10 * SpriteSize, SpriteSize * 2, SpriteSize, SpriteSize), 2 * SpriteSize, SpriteSize);
        board.CopyNonTransparent(Crop(paths, 5 * SpriteSize, SpriteSize, SpriteSize, SpriteSize), 2 * SpriteSize, SpriteSize * 2);
        board.CopyNonTransparent(Crop(paths, 10 * SpriteSize, 0, SpriteSize, SpriteSize), SpriteSize, SpriteSize);
        board.CopyNonTransparent(Crop(paths, 10 * SpriteSize, 0, SpriteSize, SpriteSize), 3 * SpriteSize, SpriteSize);

        board.CopyNonTransparent(Crop(paths, 10 * SpriteSize, SpriteSize, SpriteSize, SpriteSize), 0, SpriteSize);
        board.CopyNonTransparent(Crop(paths, 5 * SpriteSize, SpriteSize * 3, SpriteSize, SpriteSize), SpriteSize, SpriteSize);
        board.CopyNonTransparent(Crop(paths, 10 * SpriteSize, SpriteSize, SpriteSize, SpriteSize), SpriteSize * 2, SpriteSize);
        PlaceUnit(board, units, 2, 1, 1, 0);
        DrawOutline(board, 2 * SpriteSize, SpriteSize, SpriteSize, SpriteSize, (246, 200, 95, 255), 3);
        return board;
    }

    private static void PlaceUnit(Canvas board, Canvas units, int col, int row, int unitIndex, int unitRow)
    {
        var unit = Crop(units, unitIndex * SpriteSize, unitRow * SpriteSize, SpriteSize, SpriteSize);
        var x = col * SpriteSize;
        var y = row * SpriteSize;
        board.DrawRect(x + 9, y + 47, 46, 10, (5, 9, 16, 150));
        board.CopyNonTransparent(unit, x, y);
        board.DrawRect(x + 8, y + 53, 48, 5, (37, 15, 20, 255));
        board.DrawRect(x + 10, y + 54, 32, 3, (81, 207, 95, 255));
    }

    private static void DrawObjectiveMarker(Canvas board, int col, int row, (byte R, byte G, byte B, byte A) color)
    {
        var x = col * SpriteSize + 42;
        var y = row * SpriteSize + 8;
        board.DrawRect(x, y, 14, 14, (5, 9, 16, 220));
        board.DrawRect(x + 3, y + 3, 8, 8, color);
    }

    private static void DrawOutline(Canvas canvas, int x, int y, int width, int height, (byte R, byte G, byte B, byte A) color, int lineWidth)
    {
        canvas.DrawRect(x, y, width, lineWidth, color);
        canvas.DrawRect(x, y + height - lineWidth, width, lineWidth, color);
        canvas.DrawRect(x, y, lineWidth, height, color);
        canvas.DrawRect(x + width - lineWidth, y, lineWidth, height, color);
    }

    private static Canvas Crop(Canvas source, int x, int y, int width, int height)
    {
        var result = new Canvas(width, height, Canvas.Transparent);
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                var sourceX = Math.Clamp(x + col, 0, source.Width - 1);
                var sourceY = Math.Clamp(y + row, 0, source.Height - 1);
                result.SetPixel(col, row, source.Pixels[sourceY][sourceX]);
            }
        }

        return result;
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

    private static string ResolvePath(string repoRoot, string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(repoRoot, path));

    private static string RelativePath(string repoRoot, string path) =>
        Path.GetRelativePath(repoRoot, path).Replace(Path.DirectorySeparatorChar, '/');
}