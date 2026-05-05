// Copyright (c) Microsoft Corporation.
// SPDX-License-Identifier: MIT
namespace Wargame.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

/// <summary>
/// Generates SNES/GBDS-style cutscene graphics from JSON specifications.
/// </summary>
public static class CutsceneGenerator
{
    public class CutsceneSpec
    {
        [System.Text.Json.Serialization.JsonPropertyName("format_version")]
        public string? FormatVersion { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("style_profile")]
        public string? StyleProfile { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("cutscene_id")]
        public string? CutsceneId { get; set; }
        public Dictionary<string, JsonElement>? Resolution { get; set; }
        public Dictionary<string, JsonElement>? Palette { get; set; }
        public List<FrameSpec>? Frames { get; set; }
        public Dictionary<string, JsonElement>? Sheet { get; set; }
    }

    public class FrameSpec
    {
        public string? Id { get; set; }
        public string? Background { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; } = 1200;
        public List<LayerCommand>? Layers { get; set; }
    }

    public class LayerCommand
    {
        public string? Op { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("radius_x")]
        public int RadiusX { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("radius_y")]
        public int RadiusY { get; set; }
        public JsonElement? Color { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("color_a")]
        public JsonElement? ColorA { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("color_b")]
        public JsonElement? ColorB { get; set; }
        public int Step { get; set; } = 6;
        public List<List<int>>? Points { get; set; }
    }

    public static void GenerateFromSpec(string specPath, string outputDir)
    {
        var json = File.ReadAllText(specPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var spec = JsonSerializer.Deserialize<CutsceneSpec>(json, options) ?? throw new InvalidOperationException("Failed to parse spec");

        if (spec.FormatVersion != "1.0")
            throw new InvalidOperationException($"Unsupported format_version: {spec.FormatVersion}");

        var palette = ParsePalette(spec);
        var width = GetIntFromElement(spec.Resolution?["width"]) ?? 320;
        var height = GetIntFromElement(spec.Resolution?["height"]) ?? 180;
        var cutsceneId = spec.CutsceneId ?? "unknown";

        var outRoot = Path.Combine(outputDir, cutsceneId);
        Directory.CreateDirectory(outRoot);

        var frames = new List<Canvas>();
        var frameIds = new List<string>();

        foreach (var frameSpec in spec.Frames ?? new List<FrameSpec>())
        {
            frameIds.Add(frameSpec.Id ?? "frame");
            frames.Add(BuildFrame(width, height, frameSpec, palette));
        }

        // Save individual frames
        for (int i = 0; i < frames.Count; i++)
        {
            var framePath = Path.Combine(outRoot, $"{frameIds[i]}.png");
            PngWriter.WriteRgbaPng(framePath, frames[i].Pixels);
        }

        // Save sheet
        var columns = GetIntFromElement(spec.Sheet?["columns"]) ?? 3;
        var sheet = ComposeSheet(frames, columns, width, height);
        var sheetPath = Path.Combine(outRoot, $"{cutsceneId}_sheet.png");
        PngWriter.WriteRgbaPng(sheetPath, sheet.Pixels);

        // Save manifest
        SaveManifest(outRoot, cutsceneId, spec, frameIds);

        Console.WriteLine($"Generated cutscene graphics: {outRoot}");
    }

    private static Canvas BuildFrame(int width, int height, FrameSpec frame, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        var bgColorName = frame.Background ?? "transparent";
        var bgColor = palette.ContainsKey(bgColorName) ? palette[bgColorName] : ((byte)0, (byte)0, (byte)0, (byte)0);
        var canvas = new Canvas(width, height, bgColor);

        foreach (var layer in frame.Layers ?? new List<LayerCommand>())
        {
            DrawLayer(canvas, layer, palette);
        }

        return canvas;
    }

    private static void DrawLayer(Canvas canvas, LayerCommand layer, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        switch (layer.Op)
        {
            case "rect":
                canvas.DrawRect(layer.X, layer.Y, layer.Width, layer.Height, ParseColor(layer.Color, palette));
                break;

            case "ellipse":
                canvas.DrawEllipse(layer.X, layer.Y, layer.RadiusX, layer.RadiusY, ParseColor(layer.Color, palette));
                break;

            case "polygon":
                if (layer.Points != null)
                {
                    var points = layer.Points.Select(p => (p[0], p[1])).ToList();
                    canvas.DrawPolygon(points, ParseColor(layer.Color, palette));
                }
                break;

            case "dither":
                canvas.DrawDither(
                    layer.X, layer.Y, layer.Width, layer.Height,
                    ParseColor(layer.ColorA, palette),
                    ParseColor(layer.ColorB, palette),
                    layer.Step
                );
                break;
        }
    }

    private static (byte, byte, byte, byte) ParseColor(JsonElement? element, Dictionary<string, (byte, byte, byte, byte)> palette)
    {
        if (element?.ValueKind == JsonValueKind.String)
            return palette[element.Value.GetString() ?? "transparent"];

        if (element?.ValueKind == JsonValueKind.Array)
        {
            var arr = element.Value.EnumerateArray().Select(e => (byte)e.GetInt32()).ToArray();
            return (arr[0], arr[1], arr[2], arr[3]);
        }

        return (0, 0, 0, 0);
    }

    private static Dictionary<string, (byte, byte, byte, byte)> ParsePalette(CutsceneSpec spec)
    {
        var palette = new Dictionary<string, (byte, byte, byte, byte)> { { "transparent", (0, 0, 0, 0) } };

        if (spec.Palette != null)
        {
            foreach (var (name, elem) in spec.Palette)
            {
                var arr = elem.EnumerateArray().Select(e => (byte)e.GetInt32()).ToArray();
                palette[name] = (arr[0], arr[1], arr[2], arr[3]);
            }
        }

        return palette;
    }

    private static Canvas ComposeSheet(List<Canvas> frames, int columns, int frameWidth, int frameHeight)
    {
        var rows = (frames.Count + columns - 1) / columns;
        var sheet = new Canvas(columns * frameWidth, rows * frameHeight, Canvas.Transparent);

        for (int i = 0; i < frames.Count; i++)
        {
            var col = i % columns;
            var row = i / columns;
            sheet.CopyNonTransparent(frames[i], col * frameWidth, row * frameHeight);
        }

        return sheet;
    }

    private static void SaveManifest(string outRoot, string cutsceneId, CutsceneSpec spec, List<string> frameIds)
    {
        var manifest = new
        {
            cutscene_id = cutsceneId,
            style_profile = spec.StyleProfile,
            format_version = spec.FormatVersion,
            frame_width = GetIntFromElement(spec.Resolution?["width"]) ?? 320,
            frame_height = GetIntFromElement(spec.Resolution?["height"]) ?? 180,
            frames = frameIds.Select((id, idx) => new
            {
                id,
                file = $"{id}.png",
                duration_ms = spec.Frames?[idx]?.DurationMs ?? 1200,
            }).ToList(),
            sheet = new
            {
                file = $"{cutsceneId}_sheet.png",
                columns = GetIntFromElement(spec.Sheet?["columns"]) ?? 3,
                rows = (frameIds.Count + (GetIntFromElement(spec.Sheet?["columns"]) ?? 3) - 1) / (GetIntFromElement(spec.Sheet?["columns"]) ?? 3),
            },
        };

        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(outRoot, $"{cutsceneId}_manifest.json"), json);
    }

    private static int? GetIntFromElement(JsonElement? element)
    {
        return element?.ValueKind == JsonValueKind.Number ? element.Value.GetInt32() : null;
    }
}
