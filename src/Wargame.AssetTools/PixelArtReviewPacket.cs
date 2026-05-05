using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Wargame.Graphics;

static class PixelArtReviewPacket
{
    private const int TileSize = 64;
    private const int SheetWidth = 1280;
    private const int SheetHeight = 800;

    private static readonly (byte R, byte G, byte B, byte A) Backdrop = (12, 16, 24, 255);
    private static readonly (byte R, byte G, byte B, byte A) Panel = (26, 32, 44, 255);
    private static readonly (byte R, byte G, byte B, byte A) PanelLight = (48, 58, 72, 255);
    private static readonly (byte R, byte G, byte B, byte A) Warning = (218, 169, 64, 255);

    public static int Generate(string repoRoot, string[] args)
    {
        var outputDirectory = ResolvePath(repoRoot, args.FirstOrDefault() ?? Path.Combine(
            "game",
            "WargamePrototype",
            "assets",
            "art-handoff",
            "local-review",
            DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss")));
        Directory.CreateDirectory(outputDirectory);

        var unitSources = DefaultUnitSources(repoRoot);
        var cutsceneSources = DefaultCutsceneSources(repoRoot);
        var missingSources = unitSources.Concat(cutsceneSources).Where(source => !File.Exists(source.Path)).ToList();
        if (missingSources.Count > 0)
        {
            foreach (var missing in missingSources)
                Console.Error.WriteLine($"Missing source: {RelativePath(repoRoot, missing.Path)}");
            return 1;
        }

        var unitSheetPath = Path.Combine(outputDirectory, "unit-board-readability.png");
        var cutsceneSheetPath = Path.Combine(outputDirectory, "cutscene-contact-sheet.png");
        PngWriter.WriteRgbaPng(unitSheetPath, BuildUnitBoard(unitSources).Pixels);
        PngWriter.WriteRgbaPng(cutsceneSheetPath, BuildCutsceneSheet(cutsceneSources).Pixels);

        var manifest = new ReviewPacketManifest(
            DateTimeOffset.UtcNow,
            RelativePath(repoRoot, unitSheetPath),
            RelativePath(repoRoot, cutsceneSheetPath),
            unitSources.Select(source => source.ToManifestEntry(repoRoot)).ToList(),
            cutsceneSources.Select(source => source.ToManifestEntry(repoRoot)).ToList(),
            [
                "Generated crops are review proxies, not runtime-ready atlases.",
                "Accept unit art only if silhouettes remain readable at 64x64 over busy terrain.",
                "Mission 1 rescue remains a guided sketch or image-to-image candidate, not a keeper."
            ]);
        var manifestPath = Path.Combine(outputDirectory, "manifest.json");
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine($"Wrote {RelativePath(repoRoot, unitSheetPath)}");
        Console.WriteLine($"Wrote {RelativePath(repoRoot, cutsceneSheetPath)}");
        Console.WriteLine($"Wrote {RelativePath(repoRoot, manifestPath)}");
        return 0;
    }

    public static int GenerateCandidateReview(string repoRoot, string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: candidate-review <output-directory> <candidate-image> [candidate-image...]");
            return 1;
        }

        var outputDirectory = ResolvePath(repoRoot, args[0]);
        Directory.CreateDirectory(outputDirectory);
        var sources = args.Skip(1).Select((path, index) => new ReviewSource(
            $"Candidate {index + 1}",
            ResolvePath(repoRoot, path),
            (86, 178, 222, 255),
            [new(0.00, 0.00, 1.00, 1.00)])).ToList();

        var missingSources = sources.Where(source => !File.Exists(source.Path)).ToList();
        if (missingSources.Count > 0)
        {
            foreach (var missing in missingSources)
                Console.Error.WriteLine($"Missing source: {RelativePath(repoRoot, missing.Path)}");
            return 1;
        }

        var unitSheetPath = Path.Combine(outputDirectory, "candidate-board-readability.png");
        PngWriter.WriteRgbaPng(unitSheetPath, BuildUnitBoard(sources).Pixels);

        var manifest = new CandidateReviewManifest(
            DateTimeOffset.UtcNow,
            RelativePath(repoRoot, unitSheetPath),
            sources.Select(source => source.ToManifestEntry(repoRoot)).ToList(),
            [
                "Generated proxy crops are review evidence, not final runtime assets.",
                "Accept candidates only if silhouettes remain readable at 64x64 over representative terrain.",
                "Reject candidates that rely on cards, shadows, labels, or non-keyable backgrounds."
            ]);
        var manifestPath = Path.Combine(outputDirectory, "manifest.json");
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine($"Wrote {RelativePath(repoRoot, unitSheetPath)}");
        Console.WriteLine($"Wrote {RelativePath(repoRoot, manifestPath)}");
        return 0;
    }

    public static int PrepareImg2ImgSource(string repoRoot, string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: prepare-img2img-source <output-directory> <candidate-image> [candidate-image...]");
            return 1;
        }

        var outputDirectory = ResolvePath(repoRoot, args[0]);
        Directory.CreateDirectory(outputDirectory);
        List<PreparedSourceManifestEntry> entries = [];

        foreach (var sourceArgument in args.Skip(1))
        {
            var sourcePath = ResolvePath(repoRoot, sourceArgument);
            if (!File.Exists(sourcePath))
            {
                Console.Error.WriteLine($"Missing source: {RelativePath(repoRoot, sourcePath)}");
                return 1;
            }

            var prepared = PrepareSourceForImg2Img(PngReader.ReadRgbaPng(sourcePath));
            var outputName = Path.GetFileNameWithoutExtension(sourcePath) + "-prepared.png";
            var outputPath = Path.Combine(outputDirectory, outputName);
            PngWriter.WriteRgbaPng(outputPath, prepared.Pixels);
            entries.Add(new PreparedSourceManifestEntry(
                RelativePath(repoRoot, sourcePath),
                RelativePath(repoRoot, outputPath)));
            Console.WriteLine($"Wrote {RelativePath(repoRoot, outputPath)}");
        }

        var manifestPath = Path.Combine(outputDirectory, "manifest.json");
        var manifest = new PreparedSourceManifest(DateTimeOffset.UtcNow, entries);
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Wrote {RelativePath(repoRoot, manifestPath)}");
        return 0;
    }

    private static Canvas BuildUnitBoard(IReadOnlyList<ReviewSource> sources)
    {
        var canvas = new Canvas(SheetWidth, SheetHeight, Backdrop);
        canvas.DrawRect(0, 0, SheetWidth, 92, Panel);
        canvas.DrawRect(0, 92, SheetWidth, 2, Warning);
        DrawSignalBars(canvas, 24, 26, 20, Warning);
        DrawSignalBars(canvas, 1180, 26, 20, Warning);

        var terrain = SpriteGenerator.GenerateTerrain();
        var crops = BuildUnitCrops(sources).ToList();
        var cropIndex = 0;
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 16; col++)
            {
                var x = 32 + col * 74;
                var y = 112 + row * 74;
                CopyRegion(canvas, terrain, (col + row) % 5 * TileSize, 0, TileSize, TileSize, x, y);
                DrawTileFrame(canvas, x, y);

                if ((row + col) % 3 == 0 && cropIndex < crops.Count)
                {
                    DrawUnitPlate(canvas, x + 5, y + 38);
                    CopyNonTransparent(canvas, crops[cropIndex], x + 6, y + 5);
                    cropIndex++;
                }
            }
        }

        var previewX = 864;
        var previewY = 118;
        canvas.DrawRect(previewX - 14, previewY - 14, 348, 532, Panel);
        canvas.DrawRect(previewX - 10, previewY - 10, 340, 524, PanelLight);
        cropIndex = 0;
        foreach (var crop in crops.Take(18))
        {
            var col = cropIndex % 3;
            var row = cropIndex / 3;
            var x = previewX + col * 106;
            var y = previewY + row * 82;
            canvas.DrawRect(x - 4, y - 4, 72, 72, Backdrop);
            CopyNonTransparent(canvas, crop, x, y);
            cropIndex++;
        }

        return canvas;
    }

    private static Canvas BuildCutsceneSheet(IReadOnlyList<ReviewSource> sources)
    {
        var canvas = new Canvas(SheetWidth, SheetHeight, Backdrop);
        canvas.DrawRect(0, 0, SheetWidth, 58, Panel);
        canvas.DrawRect(0, 58, SheetWidth, 2, Warning);
        DrawSignalBars(canvas, 24, 18, 14, Warning);
        DrawSignalBars(canvas, 1200, 18, 14, Warning);

        for (var index = 0; index < sources.Count; index++)
        {
            var source = PngReader.ReadRgbaPng(sources[index].Path);
            var crop = CropToAspect(source, 16.0 / 9.0);
            var scaled = ScaleNearest(crop, 560, 315);
            var x = 60 + index % 2 * 610;
            var y = 88 + index / 2 * 348;
            canvas.DrawRect(x - 8, y - 8, 576, 331, PanelLight);
            CopyOpaque(canvas, scaled, x, y);
            DrawCornerTicks(canvas, x, y, 560, 315, sources[index].RatingColor);
        }

        return canvas;
    }

    private static IEnumerable<Canvas> BuildUnitCrops(IReadOnlyList<ReviewSource> sources)
    {
        foreach (var source in sources)
        {
            var image = PngReader.ReadRgbaPng(source.Path);
            foreach (var crop in source.Crops)
            {
                var sourceCrop = CropFraction(image, crop.X, crop.Y, crop.Width, crop.Height);
                yield return MakeSpriteProxy(sourceCrop);
            }
        }
    }

    private static Canvas MakeSpriteProxy(Canvas crop)
    {
        var keyed = KeyOutFlatBackground(crop);
        var trimmed = TrimTransparent(keyed);
        var scaled = ScaleToFitNearest(trimmed, 52, 52);
        var output = new Canvas(TileSize, TileSize, Canvas.Transparent);
        output.DrawEllipse(32, 53, 24, 7, (5, 8, 14, 126));
        output.DrawRect(4, 4, 56, 56, (8, 12, 20, 255));
        output.DrawRect(6, 6, 52, 52, (20, 25, 34, 255));
        CopyNonTransparent(output, scaled, 6 + (52 - scaled.Width) / 2, 6 + (52 - scaled.Height) / 2);
        DrawCornerTicks(output, 4, 4, 56, 56, Warning);
        return output;
    }

    private static Canvas PrepareSourceForImg2Img(Canvas source)
    {
        var keyed = KeyOutFlatBackground(source);
        var cleaned = RemoveGroundAndShadowArtifacts(keyed);
        var trimmed = TrimTransparent(cleaned);
        var scaled = ScaleToFitNearest(trimmed, 610, 610);
        var output = new Canvas(768, 768, (255, 0, 255, 255));
        CopyNonTransparent(output, scaled, (768 - scaled.Width) / 2, (768 - scaled.Height) / 2);
        return output;
    }

    private static Canvas RemoveGroundAndShadowArtifacts(Canvas source)
    {
        var output = new Canvas(source.Width, source.Height, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                var pixel = source.Pixels[row][col];
                if (pixel.A == 0 || IsBasinGroundOrShadow(pixel))
                    continue;
                output.SetPixel(col, row, pixel);
            }
        }

        return output;
    }

    private static Canvas KeyOutFlatBackground(Canvas source)
    {
        var output = new Canvas(source.Width, source.Height, Canvas.Transparent);
        var samples = new[]
        {
            source.Pixels[0][0],
            source.Pixels[0][source.Width - 1],
            source.Pixels[source.Height - 1][0],
            source.Pixels[source.Height - 1][source.Width - 1],
        };
        var key = samples
            .GroupBy(pixel => (pixel.R / 8, pixel.G / 8, pixel.B / 8))
            .OrderByDescending(group => group.Count())
            .First()
            .First();

        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                var pixel = source.Pixels[row][col];
                if (ColorDistance(pixel, key) <= 46 || IsNearWhite(pixel) || IsNearMagenta(pixel))
                    continue;
                output.SetPixel(col, row, pixel);
            }
        }

        return output;
    }

    private static Canvas TrimTransparent(Canvas source)
    {
        var minX = source.Width;
        var minY = source.Height;
        var maxX = -1;
        var maxY = -1;
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                if (source.Pixels[row][col].A == 0)
                    continue;
                minX = Math.Min(minX, col);
                minY = Math.Min(minY, row);
                maxX = Math.Max(maxX, col);
                maxY = Math.Max(maxY, row);
            }
        }

        if (maxX < minX || maxY < minY)
            return source;
        return Crop(source, minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private static Canvas ScaleToFitNearest(Canvas source, int maxWidth, int maxHeight)
    {
        var scale = Math.Min(maxWidth / (double)source.Width, maxHeight / (double)source.Height);
        var width = Math.Max(1, (int)Math.Round(source.Width * scale));
        var height = Math.Max(1, (int)Math.Round(source.Height * scale));
        return ScaleNearest(source, width, height);
    }

    private static Canvas CropFraction(Canvas source, double x, double y, double width, double height)
    {
        var sourceX = Clamp((int)Math.Round(source.Width * x), 0, source.Width - 1);
        var sourceY = Clamp((int)Math.Round(source.Height * y), 0, source.Height - 1);
        var sourceWidth = Clamp((int)Math.Round(source.Width * width), 1, source.Width - sourceX);
        var sourceHeight = Clamp((int)Math.Round(source.Height * height), 1, source.Height - sourceY);
        return Crop(source, sourceX, sourceY, sourceWidth, sourceHeight);
    }

    private static Canvas CropToAspect(Canvas source, double aspect)
    {
        var width = source.Width;
        var height = source.Height;
        var currentAspect = width / (double)height;
        if (currentAspect > aspect)
        {
            width = (int)Math.Round(height * aspect);
        }
        else
        {
            height = (int)Math.Round(width / aspect);
        }

        var x = (source.Width - width) / 2;
        var y = (source.Height - height) / 2;
        return Crop(source, x, y, width, height);
    }

    private static Canvas Crop(Canvas source, int x, int y, int width, int height)
    {
        var output = new Canvas(width, height, Canvas.Transparent);
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                output.SetPixel(col, row, source.Pixels[y + row][x + col]);
            }
        }

        return output;
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

    private static void CopyRegion(Canvas target, Canvas source, int sourceX, int sourceY, int width, int height, int targetX, int targetY)
    {
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                target.SetPixel(targetX + col, targetY + row, source.Pixels[sourceY + row][sourceX + col]);
            }
        }
    }

    private static void CopyOpaque(Canvas target, Canvas source, int targetX, int targetY)
    {
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                var pixel = source.Pixels[row][col];
                target.SetPixel(targetX + col, targetY + row, (pixel.R, pixel.G, pixel.B, 255));
            }
        }
    }

    private static void CopyNonTransparent(Canvas target, Canvas source, int targetX, int targetY)
    {
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                var pixel = source.Pixels[row][col];
                if (pixel.A != 0)
                    target.SetPixel(targetX + col, targetY + row, pixel);
            }
        }
    }

    private static void DrawTileFrame(Canvas canvas, int x, int y)
    {
        canvas.DrawRect(x, y, TileSize, 2, (210, 216, 210, 52));
        canvas.DrawRect(x, y, 2, TileSize, (210, 216, 210, 52));
        canvas.DrawRect(x, y + TileSize - 2, TileSize, 2, (6, 8, 12, 88));
        canvas.DrawRect(x + TileSize - 2, y, 2, TileSize, (6, 8, 12, 88));
    }

    private static void DrawUnitPlate(Canvas canvas, int x, int y)
    {
        canvas.DrawEllipse(x + 27, y + 12, 26, 9, (3, 7, 12, 138));
        canvas.DrawRect(x + 4, y + 5, 45, 7, (224, 178, 68, 255));
    }

    private static void DrawCornerTicks(Canvas canvas, int x, int y, int width, int height, (byte R, byte G, byte B, byte A) color)
    {
        canvas.DrawRect(x, y, 28, 4, color);
        canvas.DrawRect(x, y, 4, 28, color);
        canvas.DrawRect(x + width - 28, y, 28, 4, color);
        canvas.DrawRect(x + width - 4, y, 4, 28, color);
        canvas.DrawRect(x, y + height - 4, 28, 4, color);
        canvas.DrawRect(x, y + height - 28, 4, 28, color);
        canvas.DrawRect(x + width - 28, y + height - 4, 28, 4, color);
        canvas.DrawRect(x + width - 4, y + height - 28, 4, 28, color);
    }

    private static void DrawSignalBars(Canvas canvas, int x, int y, int scale, (byte R, byte G, byte B, byte A) color)
    {
        canvas.DrawRect(x, y + scale * 2, scale, scale, color);
        canvas.DrawRect(x + scale + 4, y + scale, scale, scale * 2, color);
        canvas.DrawRect(x + scale * 2 + 8, y, scale, scale * 3, color);
    }

    private static int ColorDistance((byte R, byte G, byte B, byte A) first, (byte R, byte G, byte B, byte A) second) =>
        Math.Abs(first.R - second.R) + Math.Abs(first.G - second.G) + Math.Abs(first.B - second.B);

    private static bool IsNearWhite((byte R, byte G, byte B, byte A) pixel) =>
        pixel.R >= 238 && pixel.G >= 238 && pixel.B >= 238;

    private static bool IsNearMagenta((byte R, byte G, byte B, byte A) pixel) =>
        pixel.R >= 220 && pixel.G <= 70 && pixel.B >= 200;

    private static bool IsBasinGroundOrShadow((byte R, byte G, byte B, byte A) pixel)
    {
        var brownOrdered = pixel.R >= pixel.G && pixel.G >= pixel.B;
        var basinDust = brownOrdered && pixel.R >= 58 && pixel.G >= 42 && pixel.B <= 150 && pixel.R >= pixel.B + 20;
        var paleSand = pixel.R >= 135 && pixel.G >= 105 && pixel.B <= 100;
        return basinDust || paleSand;
    }

    private static List<ReviewSource> DefaultUnitSources(string repoRoot) =>
    [
        TokenSource(repoRoot, "Kestrel Field Tech token v4 seed 57700", "token-kestrel-field-tech-sdxl-nerijs-v4", 57700, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Field Tech token v4 seed 57701", "token-kestrel-field-tech-sdxl-nerijs-v4", 57701, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Field Tech token v4 seed 57702", "token-kestrel-field-tech-sdxl-nerijs-v4", 57702, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Field Tech token v4 seed 57703", "token-kestrel-field-tech-sdxl-nerijs-v4", 57703, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Utility Armor token v3 seed 57800", "token-kestrel-utility-armor-sdxl-nerijs-v3", 57800, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Utility Armor token v3 seed 57801", "token-kestrel-utility-armor-sdxl-nerijs-v3", 57801, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Utility Armor token v3 seed 57802", "token-kestrel-utility-armor-sdxl-nerijs-v3", 57802, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Utility Armor token v3 seed 57803", "token-kestrel-utility-armor-sdxl-nerijs-v3", 57803, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Survey Scout token v3 seed 57900", "token-kestrel-survey-scout-sdxl-nerijs-v3", 57900, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Survey Scout token v3 seed 57901", "token-kestrel-survey-scout-sdxl-nerijs-v3", 57901, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Survey Scout token v3 seed 57902", "token-kestrel-survey-scout-sdxl-nerijs-v3", 57902, (86, 178, 222, 255)),
        TokenSource(repoRoot, "Kestrel Survey Scout token v3 seed 57903", "token-kestrel-survey-scout-sdxl-nerijs-v3", 57903, (86, 178, 222, 255)),
        new(
            "Kestrel Field Tech token v3 seed 57400",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-field-tech-sdxl-nerijs-v3", "token-kestrel-field-tech-sdxl-nerijs-v3_57400_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Field Tech token v3 seed 57401",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-field-tech-sdxl-nerijs-v3", "token-kestrel-field-tech-sdxl-nerijs-v3_57401_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Field Tech token v3 seed 57402",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-field-tech-sdxl-nerijs-v3", "token-kestrel-field-tech-sdxl-nerijs-v3_57402_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Field Tech token v3 seed 57403",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-field-tech-sdxl-nerijs-v3", "token-kestrel-field-tech-sdxl-nerijs-v3_57403_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Utility Armor token v2 seed 57500",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-utility-armor-sdxl-nerijs-v2", "token-kestrel-utility-armor-sdxl-nerijs-v2_57500_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Utility Armor token v2 seed 57501",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-utility-armor-sdxl-nerijs-v2", "token-kestrel-utility-armor-sdxl-nerijs-v2_57501_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Utility Armor token v2 seed 57502",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-utility-armor-sdxl-nerijs-v2", "token-kestrel-utility-armor-sdxl-nerijs-v2_57502_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Survey Scout token v2 seed 57600",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-survey-scout-sdxl-nerijs-v2", "token-kestrel-survey-scout-sdxl-nerijs-v2_57600_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Survey Scout token v2 seed 57601",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-survey-scout-sdxl-nerijs-v2", "token-kestrel-survey-scout-sdxl-nerijs-v2_57601_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Survey Scout token v2 seed 57602",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-survey-scout-sdxl-nerijs-v2", "token-kestrel-survey-scout-sdxl-nerijs-v2_57602_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Field Tech token v2 reference",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-field-tech-sdxl-nerijs-v2", "token-kestrel-field-tech-sdxl-nerijs-v2_57100_00001_.png")),
            (218, 169, 64, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Utility Armor token v1 reference",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-utility-armor-sdxl-nerijs-v1", "token-kestrel-utility-armor-sdxl-nerijs-v1_57200_00001_.png")),
            (218, 169, 64, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Kestrel Survey Scout token v1 reference",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "token-kestrel-survey-scout-sdxl-nerijs-v1", "token-kestrel-survey-scout-sdxl-nerijs-v1_57300_00001_.png")),
            (218, 169, 64, 255),
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]),
        new(
            "Field tech SDXL front/back",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "field-tech-sdxl-nerijs", "field-tech-sdxl-nerijs_47100_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.04, 0.06, 0.44, 0.88),
                new(0.54, 0.06, 0.40, 0.88),
            ]),
        new(
            "AT lancer SDXL",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "at-lancer", "at-lancer_43300_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.40, 0.05, 0.42, 0.90),
            ]),
        new(
            "Vehicle roster v4",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "vehicle-roster-sdxl-nerijs-v4", "vehicle-roster-sdxl-nerijs-v4_53000_00001_.png")),
            (74, 190, 112, 255),
            [
                new(0.07, 0.16, 0.24, 0.30),
                new(0.38, 0.16, 0.24, 0.30),
                new(0.68, 0.16, 0.24, 0.30),
                new(0.07, 0.53, 0.24, 0.30),
                new(0.38, 0.53, 0.24, 0.30),
                new(0.68, 0.53, 0.24, 0.30),
            ]),
        new(
            "Infantry roster v4",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "infantry-roster-sdxl-nerijs-v4", "infantry-roster-sdxl-nerijs-v4_53100_00001_.png")),
            (86, 178, 222, 255),
            [
                new(0.08, 0.14, 0.20, 0.34),
                new(0.30, 0.14, 0.20, 0.34),
                new(0.52, 0.14, 0.20, 0.34),
                new(0.74, 0.14, 0.20, 0.34),
                new(0.08, 0.52, 0.20, 0.34),
                new(0.30, 0.52, 0.20, 0.34),
                new(0.52, 0.52, 0.20, 0.34),
                new(0.74, 0.52, 0.20, 0.34),
            ]),
    ];

    private static ReviewSource TokenSource(
        string repoRoot,
        string name,
        string folder,
        int seed,
        (byte R, byte G, byte B, byte A) ratingColor) =>
        new(
            name,
            ResolvePath(repoRoot, Path.Combine(
                "game",
                "WargamePrototype",
                "assets",
                "art-handoff",
                "local-candidates",
                folder,
                $"{folder}_{seed}_00001_.png")),
            ratingColor,
            [
                new(0.00, 0.00, 1.00, 1.00),
            ]);

    private static List<ReviewSource> DefaultCutsceneSources(string repoRoot) =>
    [
        new(
            "Mission 1 intro cinematic v4",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "mission1-intro-cinematic-sdxl-nerijs-v4", "mission1-intro-cinematic-sdxl-nerijs-v4_55000_00001_.png")),
            (74, 190, 112, 255),
            []),
        new(
            "Mission 2 relay cinematic v4",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "mission2-relay-cinematic-sdxl-nerijs-v4", "mission2-relay-cinematic-sdxl-nerijs-v4_55200_00001_.png")),
            (74, 190, 112, 255),
            []),
        new(
            "Mission 3 pump cinematic v5",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "mission3-pump-cinematic-sdxl-nerijs-v5", "mission3-pump-cinematic-sdxl-nerijs-v5_56300_00001_.png")),
            (74, 190, 112, 255),
            []),
        new(
            "Mission 1 rescue cinematic v5",
            ResolvePath(repoRoot, Path.Combine("game", "WargamePrototype", "assets", "art-handoff", "local-candidates", "mission1-rescue-cinematic-sdxl-nerijs-v5", "mission1-rescue-cinematic-sdxl-nerijs-v5_56100_00001_.png")),
            (218, 169, 64, 255),
            []),
    ];

    private static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

    private static string ResolvePath(string repoRoot, string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(repoRoot, path));

    private static string RelativePath(string repoRoot, string path) =>
        Path.GetRelativePath(repoRoot, path).Replace(Path.DirectorySeparatorChar, '/');
}

sealed record ReviewSource(
    string Name,
    string Path,
    (byte R, byte G, byte B, byte A) RatingColor,
    IReadOnlyList<ReviewCrop> Crops)
{
    public ReviewSourceManifestEntry ToManifestEntry(string repoRoot) => new(
        Name,
        System.IO.Path.GetRelativePath(repoRoot, Path).Replace(System.IO.Path.DirectorySeparatorChar, '/'),
        Crops.Count);
}

sealed record ReviewCrop(double X, double Y, double Width, double Height);

sealed record ReviewPacketManifest(
    DateTimeOffset CreatedUtc,
    string UnitBoardSheet,
    string CutsceneContactSheet,
    IReadOnlyList<ReviewSourceManifestEntry> UnitSources,
    IReadOnlyList<ReviewSourceManifestEntry> CutsceneSources,
    IReadOnlyList<string> ReviewNotes);

sealed record CandidateReviewManifest(
    DateTimeOffset CreatedUtc,
    string CandidateBoardSheet,
    IReadOnlyList<ReviewSourceManifestEntry> Sources,
    IReadOnlyList<string> ReviewNotes);

sealed record PreparedSourceManifest(
    DateTimeOffset CreatedUtc,
    IReadOnlyList<PreparedSourceManifestEntry> Sources);

sealed record PreparedSourceManifestEntry(string SourcePath, string PreparedPath);

sealed record ReviewSourceManifestEntry(string Name, string Path, int CropCount);
