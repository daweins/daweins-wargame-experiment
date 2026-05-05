namespace Wargame.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Extracts runtime sprite atlases from returned source-art sheets.
/// </summary>
public static class SpriteSheetExtractor
{
    private const int DefaultSpriteSize = 64;

    /// <summary>
    /// Extracts every output described by a JSON extraction manifest.
    /// </summary>
    /// <param name="manifestPath">Path to the extraction manifest.</param>
    /// <param name="assetBase">Path to the prototype assets directory.</param>
    public static void ExtractFromManifest(string manifestPath, string assetBase)
    {
        var json = File.ReadAllText(manifestPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var manifest = JsonSerializer.Deserialize<ExtractionManifest>(json, options)
            ?? throw new InvalidOperationException("Failed to parse extraction manifest.");

        foreach (var output in manifest.Outputs)
        {
            ExtractOutput(output, assetBase);
        }
    }

    private static void ExtractOutput(ExtractionOutput output, string assetBase)
    {
        var spriteSize = output.SpriteSize <= 0 ? DefaultSpriteSize : output.SpriteSize;
        var columns = output.Columns <= 0 ? output.Sprites.Count : output.Columns;
        var rows = (output.Sprites.Count + columns - 1) / columns;
        var sheet = new Canvas(columns * spriteSize, rows * spriteSize, Canvas.Transparent);
        var spriteCache = new Dictionary<string, Canvas>(StringComparer.OrdinalIgnoreCase);
        var sourceCache = new Dictionary<string, Canvas>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < output.Sprites.Count; index++)
        {
            var sprite = output.Sprites[index];
            var sourceCanvas = ResolveSourceCanvas(assetBase, output, sprite, sourceCache);
            var canvas = BuildSprite(sourceCanvas, sprite, spriteCache, spriteSize);
            spriteCache[sprite.Name] = canvas;
            sheet.CopyNonTransparent(canvas, (index % columns) * spriteSize, (index / columns) * spriteSize);
        }

        var outputPath = Path.Combine(assetBase, output.Output);
        PngWriter.WriteRgbaPng(outputPath, sheet.Pixels);
        Console.WriteLine($"Extracted {output.Sprites.Count} sprites to {outputPath}");
    }

    private static Canvas? ResolveSourceCanvas(
        string assetBase,
        ExtractionOutput output,
        ExtractionSprite sprite,
        Dictionary<string, Canvas> sourceCache)
    {
        var source = string.IsNullOrWhiteSpace(sprite.Source) ? output.Source : sprite.Source;
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        if (!sourceCache.TryGetValue(source, out var canvas))
        {
            canvas = PngReader.ReadRgbaPng(Path.Combine(assetBase, source));
            sourceCache[source] = canvas;
        }

        return canvas;
    }

    private static Canvas BuildSprite(Canvas? sourceCanvas, ExtractionSprite sprite, Dictionary<string, Canvas> cache, int spriteSize)
    {
        if (!string.IsNullOrWhiteSpace(sprite.From))
        {
            if (!cache.TryGetValue(sprite.From, out var existing))
            {
                throw new InvalidOperationException($"Sprite '{sprite.Name}' references unknown source sprite '{sprite.From}'.");
            }

            return Transform(existing, sprite.Transform);
        }

        if (sprite.Region is null)
        {
            throw new InvalidOperationException($"Sprite '{sprite.Name}' must define either region or from.");
        }

        if (sourceCanvas is null)
        {
            throw new InvalidOperationException($"Sprite '{sprite.Name}' must define a source image.");
        }

        var crop = Crop(sourceCanvas, sprite.Region);
        if (sprite.Alpha?.Mode?.Equals("corner", StringComparison.OrdinalIgnoreCase) == true)
        {
            ApplyCornerTransparency(crop, sprite.Alpha.Tolerance);
            crop = TrimTransparent(crop);
        }

        if (sprite.Tint is not null)
        {
            ApplyTint(crop, sprite.Tint);
        }

        return ScaleToSprite(crop, spriteSize, sprite.Fit ?? "cover");
    }

    private static Canvas Crop(Canvas source, ExtractionRegion region)
    {
        var canvas = new Canvas(region.Width, region.Height, Canvas.Transparent);
        for (var row = 0; row < region.Height; row++)
        {
            for (var col = 0; col < region.Width; col++)
            {
                var sourceX = region.X + col;
                var sourceY = region.Y + row;
                if (sourceX >= 0 && sourceX < source.Width && sourceY >= 0 && sourceY < source.Height)
                {
                    canvas.SetPixel(col, row, source.Pixels[sourceY][sourceX]);
                }
            }
        }

        return canvas;
    }

    private static void ApplyCornerTransparency(Canvas canvas, int tolerance)
    {
        var key = canvas.Pixels[0][0];
        for (var row = 0; row < canvas.Height; row++)
        {
            for (var col = 0; col < canvas.Width; col++)
            {
                var pixel = canvas.Pixels[row][col];
                if (ColorDistance(pixel, key) <= tolerance)
                {
                    canvas.SetPixel(col, row, (pixel.R, pixel.G, pixel.B, 0));
                }
            }
        }
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
                {
                    continue;
                }

                minX = Math.Min(minX, col);
                minY = Math.Min(minY, row);
                maxX = Math.Max(maxX, col);
                maxY = Math.Max(maxY, row);
            }
        }

        if (maxX < minX || maxY < minY)
        {
            return source;
        }

        return Crop(source, new ExtractionRegion
        {
            X = minX,
            Y = minY,
            Width = maxX - minX + 1,
            Height = maxY - minY + 1
        });
    }

    private static void ApplyTint(Canvas canvas, ExtractionTint tint)
    {
        for (var row = 0; row < canvas.Height; row++)
        {
            for (var col = 0; col < canvas.Width; col++)
            {
                var pixel = canvas.Pixels[row][col];
                if (pixel.A == 0)
                {
                    continue;
                }

                canvas.SetPixel(col, row, (
                    Clamp(pixel.R * tint.R),
                    Clamp(pixel.G * tint.G),
                    Clamp(pixel.B * tint.B),
                    pixel.A));
            }
        }
    }

    private static Canvas ScaleToSprite(Canvas source, int spriteSize, string fit)
    {
        var scale = fit.Equals("contain", StringComparison.OrdinalIgnoreCase)
            ? Math.Min(spriteSize / (double)source.Width, spriteSize / (double)source.Height)
            : Math.Max(spriteSize / (double)source.Width, spriteSize / (double)source.Height);

        var scaledWidth = Math.Max(1, (int)Math.Round(source.Width * scale));
        var scaledHeight = Math.Max(1, (int)Math.Round(source.Height * scale));
        var offsetX = (spriteSize - scaledWidth) / 2;
        var offsetY = (spriteSize - scaledHeight) / 2;
        var canvas = new Canvas(spriteSize, spriteSize, Canvas.Transparent);

        for (var row = 0; row < spriteSize; row++)
        {
            for (var col = 0; col < spriteSize; col++)
            {
                var scaledX = col - offsetX;
                var scaledY = row - offsetY;
                if (scaledX < 0 || scaledX >= scaledWidth || scaledY < 0 || scaledY >= scaledHeight)
                {
                    continue;
                }

                var sourceX = Math.Clamp((int)(scaledX / scale), 0, source.Width - 1);
                var sourceY = Math.Clamp((int)(scaledY / scale), 0, source.Height - 1);
                canvas.SetPixel(col, row, source.Pixels[sourceY][sourceX]);
            }
        }

        return canvas;
    }

    private static Canvas Transform(Canvas source, string? transform) => transform?.ToLowerInvariant() switch
    {
        "rotate90" => Rotate90(source),
        "rotate180" => Rotate180(source),
        "rotate270" => Rotate270(source),
        "flipx" => FlipX(source),
        "flipy" => FlipY(source),
        null or "" or "none" => Clone(source),
        _ => throw new InvalidOperationException($"Unsupported transform: {transform}")
    };

    private static Canvas Clone(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        canvas.CopyNonTransparent(source, 0, 0);
        return canvas;
    }

    private static Canvas Rotate90(Canvas source)
    {
        var canvas = new Canvas(source.Height, source.Width, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                canvas.SetPixel(source.Height - 1 - row, col, source.Pixels[row][col]);
            }
        }

        return canvas;
    }

    private static Canvas Rotate180(Canvas source) => Rotate90(Rotate90(source));

    private static Canvas Rotate270(Canvas source) => Rotate90(Rotate180(source));

    private static Canvas FlipX(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                canvas.SetPixel(source.Width - 1 - col, row, source.Pixels[row][col]);
            }
        }

        return canvas;
    }

    private static Canvas FlipY(Canvas source)
    {
        var canvas = new Canvas(source.Width, source.Height, Canvas.Transparent);
        for (var row = 0; row < source.Height; row++)
        {
            for (var col = 0; col < source.Width; col++)
            {
                canvas.SetPixel(col, source.Height - 1 - row, source.Pixels[row][col]);
            }
        }

        return canvas;
    }

    private static int ColorDistance((byte R, byte G, byte B, byte A) first, (byte R, byte G, byte B, byte A) second) =>
        Math.Abs(first.R - second.R) + Math.Abs(first.G - second.G) + Math.Abs(first.B - second.B);

    private static byte Clamp(double value) => (byte)Math.Clamp((int)Math.Round(value), 0, 255);
}

/// <summary>
/// Top-level source-art extraction manifest.
/// </summary>
public sealed class ExtractionManifest
{
    /// <summary>Gets or sets the sprite atlas outputs.</summary>
    public List<ExtractionOutput> Outputs { get; set; } = [];
}

/// <summary>
/// Describes one extracted sprite atlas output.
/// </summary>
public sealed class ExtractionOutput
{
    /// <summary>Gets or sets the output path relative to the asset base folder.</summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>Gets or sets the source image path relative to the asset base folder.</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>Gets or sets the output atlas column count.</summary>
    public int Columns { get; set; }

    /// <summary>Gets or sets the square sprite size in pixels.</summary>
    [JsonPropertyName("sprite_size")]
    public int SpriteSize { get; set; } = 64;

    /// <summary>Gets or sets the sprites written to the atlas in order.</summary>
    public List<ExtractionSprite> Sprites { get; set; } = [];
}

/// <summary>
/// Describes one extracted sprite or transformed variant.
/// </summary>
public sealed class ExtractionSprite
{
    /// <summary>Gets or sets the sprite name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the source sprite name for transformed variants.</summary>
    public string? From { get; set; }

    /// <summary>Gets or sets an optional source image path relative to the asset base folder.</summary>
    public string? Source { get; set; }

    /// <summary>Gets or sets the crop region for source sprites.</summary>
    public ExtractionRegion? Region { get; set; }

    /// <summary>Gets or sets the scaling fit mode. Use cover or contain.</summary>
    public string? Fit { get; set; }

    /// <summary>Gets or sets alpha-keying behavior.</summary>
    public ExtractionAlpha? Alpha { get; set; }

    /// <summary>Gets or sets the transform for variants.</summary>
    public string? Transform { get; set; }

    /// <summary>Gets or sets an optional RGB tint multiplier.</summary>
    public ExtractionTint? Tint { get; set; }
}

/// <summary>
/// Source crop rectangle.
/// </summary>
public sealed class ExtractionRegion
{
    /// <summary>Gets or sets the source x coordinate.</summary>
    public int X { get; set; }

    /// <summary>Gets or sets the source y coordinate.</summary>
    public int Y { get; set; }

    /// <summary>Gets or sets the crop width.</summary>
    public int Width { get; set; }

    /// <summary>Gets or sets the crop height.</summary>
    public int Height { get; set; }
}

/// <summary>
/// Alpha-keying settings for extracted sprites.
/// </summary>
public sealed class ExtractionAlpha
{
    /// <summary>Gets or sets the alpha-key mode.</summary>
    public string? Mode { get; set; }

    /// <summary>Gets or sets the RGB distance tolerance for keying.</summary>
    public int Tolerance { get; set; } = 20;
}

/// <summary>
/// RGB tint multiplier settings.
/// </summary>
public sealed class ExtractionTint
{
    /// <summary>Gets or sets the red multiplier.</summary>
    public double R { get; set; } = 1;

    /// <summary>Gets or sets the green multiplier.</summary>
    public double G { get; set; } = 1;

    /// <summary>Gets or sets the blue multiplier.</summary>
    public double B { get; set; } = 1;
}