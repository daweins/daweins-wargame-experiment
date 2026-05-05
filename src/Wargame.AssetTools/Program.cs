// Copyright (c) Microsoft Corporation.
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wargame.Graphics;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            var command = args.FirstOrDefault()?.ToLower() ?? "all";
            var repoRoot = FindRepositoryRoot();
            var assetBase = Path.Combine(repoRoot, "game", "WargamePrototype", "assets");

            return command switch
            {
                "sprites" => GenerateSprites(assetBase),
                "handoff-runtime" => HandoffRuntimeDeprecated(),
                "extract-art" => ExtractArt(assetBase),
                "cutscenes" => GenerateCutscenes(args.Skip(1).FirstOrDefault() ?? "", assetBase),
                "pixelart" => await PixelArtPipeline.RunAsync(repoRoot, args.Skip(1).ToArray()),
                "review-packet" => PixelArtReviewPacket.Generate(repoRoot, args.Skip(1).ToArray()),
                "candidate-review" => PixelArtReviewPacket.GenerateCandidateReview(repoRoot, args.Skip(1).ToArray()),
                "prepare-img2img-source" => PixelArtReviewPacket.PrepareImg2ImgSource(repoRoot, args.Skip(1).ToArray()),
                "terrain-masks" => TerrainMaskGenerator.Generate(repoRoot, args.Skip(1).ToArray()),
                "terrain-compose" => TerrainTextureCompositor.Generate(repoRoot, args.Skip(1).ToArray()),
                "terrain-salvage-road" => TerrainTextureCompositor.GenerateFromDirectRoadCandidate(repoRoot, args.Skip(1).ToArray()),
                "terrain-paths" => TerrainPathAtlasGenerator.Generate(assetBase),
                "terrain-path-review" => TerrainPathReviewPacket.Generate(repoRoot, args.Skip(1).ToArray()),
                "all" or "" => GenerateAll(assetBase),
                _ => Usage(),
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int GenerateSprites(string assetBase)
    {
        Console.WriteLine("Generating sprite sheets...");
        var spritePath = Path.Combine(assetBase, "sprites");
        Directory.CreateDirectory(spritePath);

        PngWriter.WriteRgbaPng(Path.Combine(spritePath, "terrain.png"), SpriteGenerator.GenerateTerrain().Pixels);
        PngWriter.WriteRgbaPng(Path.Combine(spritePath, "units.png"), SpriteGenerator.GenerateUnits().Pixels);
        PngWriter.WriteRgbaPng(Path.Combine(spritePath, "campaign_units.png"), SpriteGenerator.GenerateCampaignUnits().Pixels);
        PngWriter.WriteRgbaPng(Path.Combine(spritePath, "ui_icons.png"), SpriteGenerator.GenerateUiIcons().Pixels);

        Console.WriteLine($"Generated 64x64 sprite sheets in {spritePath}");
        return 0;
    }

    static int HandoffRuntimeDeprecated()
    {
        Console.Error.WriteLine("C# primitive art generation is deprecated for art-handoff fulfillment.");
        Console.Error.WriteLine("Use dotnet run --project .\\src\\Wargame.AssetTools\\Wargame.AssetTools.csproj pixelart generate <job-spec.json> instead.");
        return 1;
    }

    static int GenerateHandoffRuntimeSheets(string assetBase)
    {
        Console.WriteLine("Generating art-handoff runtime request sheets...");
        var requestBase = Path.Combine(assetBase, "art-handoff", "requests");

        var terrainRequestPath = Path.Combine(requestBase, "07-runtime-terrain-tileset-variants");
        Directory.CreateDirectory(terrainRequestPath);
        PngWriter.WriteRgbaPng(
            Path.Combine(terrainRequestPath, "local-runtime-terrain-tileset-variants.png"),
            SpriteGenerator.GenerateRuntimeTerrainVariants().Pixels);

        var unitRequestPath = Path.Combine(requestBase, "08-transparent-unit-sprite-atlas");
        Directory.CreateDirectory(unitRequestPath);
        PngWriter.WriteRgbaPng(
            Path.Combine(unitRequestPath, "local-transparent-unit-sprite-atlas.png"),
            SpriteGenerator.GenerateTransparentUnitSpriteAtlas().Pixels);

        var iconRequestPath = Path.Combine(requestBase, "09-transparent-ui-icon-atlas");
        Directory.CreateDirectory(iconRequestPath);
        PngWriter.WriteRgbaPng(
            Path.Combine(iconRequestPath, "local-transparent-ui-icon-atlas.png"),
            SpriteGenerator.GenerateTransparentUiIconAtlas().Pixels);

        var imageryThreadPath = Path.Combine(requestBase, "10-missions-01-10-imagery-thread");
        Directory.CreateDirectory(imageryThreadPath);
        PngWriter.WriteRgbaPng(
            Path.Combine(imageryThreadPath, "local-act1-ui-overlay-atlas.png"),
            SpriteGenerator.GenerateActOneOverlayAtlas().Pixels);
        PngWriter.WriteRgbaPng(
            Path.Combine(imageryThreadPath, "local-act1-unit-reference-atlas.png"),
            SpriteGenerator.GenerateTransparentUnitSpriteAtlas().Pixels);
        PngWriter.WriteRgbaPng(
            Path.Combine(imageryThreadPath, "local-act1-terrain-reference-atlas.png"),
            SpriteGenerator.GenerateRuntimeTerrainVariants().Pixels);
        PngWriter.WriteRgbaPng(
            Path.Combine(imageryThreadPath, "local-missions-04-10-reference-panels.png"),
            SpriteGenerator.GenerateMissionFourToTenReferencePanels().Pixels);

        Console.WriteLine($"Generated request sheets in {requestBase}");
        return 0;
    }

    static int GenerateCutscenes(string specName, string assetBase)
    {
        var specsPath = Path.Combine(assetBase, "cutscenes", "specs");
        var outputPath = Path.Combine(assetBase, "cutscenes", "generated");
        Directory.CreateDirectory(outputPath);

        if (!string.IsNullOrEmpty(specName))
        {
            var specFile = Path.Combine(specsPath, specName);
            if (!File.Exists(specFile))
            {
                Console.Error.WriteLine($"Spec not found: {specFile}");
                return 1;
            }
            CutsceneGenerator.GenerateFromSpec(specFile, outputPath);
        }
        else
        {
            foreach (var specFile in Directory.GetFiles(specsPath, "*.cutscene.json"))
            {
                CutsceneGenerator.GenerateFromSpec(specFile, outputPath);
            }
        }

        return 0;
    }

    static int ExtractArt(string assetBase)
    {
        Console.WriteLine("Extracting returned source-art sheets...");
        var manifestPath = Path.Combine(assetBase, "art-handoff", "extraction", "source-art-extraction.json");
        if (!File.Exists(manifestPath))
        {
            Console.Error.WriteLine($"Extraction manifest not found: {manifestPath}");
            return 1;
        }

        SpriteSheetExtractor.ExtractFromManifest(manifestPath, assetBase);
        TerrainPathAtlasGenerator.Generate(assetBase);
        return 0;
    }

    static int GenerateAll(string assetBase)
    {
        Console.WriteLine("Generating all assets...");
        var result = GenerateSprites(assetBase);
        if (result != 0) return result;
        result = ExtractArt(assetBase);
        if (result != 0) return result;
        return GenerateCutscenes("", assetBase);
    }

    static int Usage()
    {
        Console.WriteLine("""
            Wargame Asset Generator

            Usage:
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj sprites
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj handoff-runtime  (deprecated; use pixelart generate)
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj extract-art
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj cutscenes
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj cutscenes mission1_intro.cutscene.json
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj pixelart generate <job-spec.json>
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj review-packet
              dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj candidate-review <output-directory> <candidate-image> [candidate-image...]
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj prepare-img2img-source <output-directory> <candidate-image> [candidate-image...]
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj terrain-masks <output-directory>
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj terrain-compose <output-directory> <ground-texture.png> <road-texture.png>
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj terrain-salvage-road <output-directory> <direct-road-candidate.png>
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj terrain-paths
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj terrain-path-review <output-directory>
            """);
        return 1;
    }

    static string FindRepositoryRoot()
    {
        var current = Path.GetDirectoryName(typeof(Program).Assembly.Location)
            ?? throw new InvalidOperationException("Cannot determine assembly location");

        while (current != null)
        {
            if (Directory.Exists(Path.Combine(current, ".git")))
                return current;
            current = Path.GetDirectoryName(current);
        }

        throw new InvalidOperationException("Repository root not found");
    }
}
