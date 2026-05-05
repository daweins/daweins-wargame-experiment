using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

static class PixelArtPipeline
{
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static async Task<int> RunAsync(string repoRoot, string[] args)
    {
        var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "help";
        return command switch
        {
            "generate" => await GenerateAsync(repoRoot, args.Skip(1).FirstOrDefault()),
            "help" or "" => Usage(),
            _ => Usage(),
        };
    }

    static async Task<int> GenerateAsync(string repoRoot, string? specPath)
    {
        if (string.IsNullOrWhiteSpace(specPath))
        {
            Console.Error.WriteLine("Missing pixel art job spec path.");
            return Usage();
        }

        var absoluteSpecPath = ResolvePath(repoRoot, specPath);
        if (!File.Exists(absoluteSpecPath))
        {
            Console.Error.WriteLine($"Spec not found: {absoluteSpecPath}");
            return 1;
        }

        var spec = JsonSerializer.Deserialize<PixelArtJobSpec>(File.ReadAllText(absoluteSpecPath), JsonOptions)
            ?? throw new InvalidOperationException("Could not parse pixel art job spec.");
        spec.Validate();

        var outputDirectory = ResolvePath(repoRoot, spec.OutputDirectory ?? Path.Combine("private", "local-imagegen", "candidates", spec.Name));
        Directory.CreateDirectory(outputDirectory);

        using var httpClient = new HttpClient { BaseAddress = new Uri(spec.ServerUrl ?? "http://127.0.0.1:8188") };
        await VerifyComfyUiAsync(httpClient);

        var manifest = new PixelArtRunManifest(
            spec.Name,
            DateTimeOffset.UtcNow,
            spec.Model,
            spec.Lora,
            spec.LoraStrengthModel,
            spec.LoraStrengthClip,
            spec.SourceImage,
            spec.Denoise,
            spec.Prompt,
            spec.NegativePrompt,
            spec.Width,
            spec.Height,
            []);

        var sourceImageName = PrepareSourceImage(repoRoot, spec);

        for (var index = 0; index < spec.CandidateCount; index++)
        {
            var seed = spec.SeedStart + index;
            var prefix = SanitizeFileName($"{spec.Name}_{seed}");
            Console.WriteLine($"Queueing {spec.Name} seed {seed}...");

            var workflow = BuildWorkflow(spec, seed, prefix, sourceImageName);
            var promptId = await QueuePromptAsync(httpClient, workflow);
            var images = await WaitForImagesAsync(httpClient, promptId, TimeSpan.FromMinutes(spec.TimeoutMinutes));

            foreach (var image in images)
            {
                var savedPath = await DownloadImageAsync(httpClient, image, outputDirectory);
                manifest.Candidates.Add(new PixelArtCandidate(seed, image.FileName, RelativePath(repoRoot, savedPath)));
                Console.WriteLine($"Saved {RelativePath(repoRoot, savedPath)}");
            }
        }

        var manifestPath = Path.Combine(outputDirectory, "manifest.json");
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, JsonOptions));
        Console.WriteLine($"Wrote {RelativePath(repoRoot, manifestPath)}");
        return 0;
    }

    static JsonObject BuildWorkflow(PixelArtJobSpec spec, long seed, string filenamePrefix, string? sourceImageName)
    {
        var hasSourceImage = !string.IsNullOrWhiteSpace(sourceImageName);
        var workflow = new JsonObject
        {
            ["1"] = Node("CheckpointLoaderSimple", new JsonObject { ["ckpt_name"] = spec.Model }),
            ["5"] = Node("KSampler", new JsonObject
            {
                ["seed"] = seed,
                ["steps"] = spec.Steps,
                ["cfg"] = spec.Cfg,
                ["sampler_name"] = spec.Sampler,
                ["scheduler"] = spec.Scheduler,
                ["denoise"] = hasSourceImage ? spec.Denoise : 1.0,
                ["model"] = ModelLink(spec),
                ["positive"] = Link("2", 0),
                ["negative"] = Link("3", 0),
                ["latent_image"] = hasSourceImage ? Link("10", 0) : Link("4", 0),
            }),
            ["6"] = Node("VAEDecode", new JsonObject { ["samples"] = Link("5", 0), ["vae"] = Link("1", 2) }),
            ["7"] = Node("SaveImage", new JsonObject { ["images"] = Link("6", 0), ["filename_prefix"] = filenamePrefix }),
        };

        if (hasSourceImage)
        {
            workflow["9"] = Node("LoadImage", new JsonObject { ["image"] = sourceImageName });
            workflow["10"] = Node("VAEEncode", new JsonObject { ["pixels"] = Link("9", 0), ["vae"] = Link("1", 2) });
        }
        else
        {
            workflow["4"] = Node("EmptyLatentImage", new JsonObject { ["width"] = spec.Width, ["height"] = spec.Height, ["batch_size"] = spec.BatchSize });
        }

        var clipLink = ClipLink(spec);
        workflow["2"] = Node("CLIPTextEncode", new JsonObject { ["text"] = spec.Prompt, ["clip"] = clipLink });
        workflow["3"] = Node("CLIPTextEncode", new JsonObject { ["text"] = spec.NegativePrompt, ["clip"] = ClipLink(spec) });

        if (!string.IsNullOrWhiteSpace(spec.Lora))
        {
            workflow["8"] = Node("LoraLoader", new JsonObject
            {
                ["model"] = Link("1", 0),
                ["clip"] = Link("1", 1),
                ["lora_name"] = spec.Lora,
                ["strength_model"] = spec.LoraStrengthModel,
                ["strength_clip"] = spec.LoraStrengthClip,
            });
        }

        return workflow;
    }

    static string? PrepareSourceImage(string repoRoot, PixelArtJobSpec spec)
    {
        if (string.IsNullOrWhiteSpace(spec.SourceImage))
            return null;

        var sourcePath = ResolvePath(repoRoot, spec.SourceImage);
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source image not found for img2img job.", sourcePath);

        var inputDirectory = Path.Combine(repoRoot, "private", "local-imagegen", "comfy-input");
        Directory.CreateDirectory(inputDirectory);
        var targetName = SanitizeFileName($"{spec.Name}_{Path.GetFileName(sourcePath)}");
        var targetPath = Path.Combine(inputDirectory, targetName);
        File.Copy(sourcePath, targetPath, overwrite: true);
        Console.WriteLine($"Prepared img2img source {RelativePath(repoRoot, targetPath)}");
        return targetName;
    }

    static JsonArray ModelLink(PixelArtJobSpec spec) =>
        string.IsNullOrWhiteSpace(spec.Lora) ? Link("1", 0) : Link("8", 0);

    static JsonArray ClipLink(PixelArtJobSpec spec) =>
        string.IsNullOrWhiteSpace(spec.Lora) ? Link("1", 1) : Link("8", 1);

    static JsonObject Node(string classType, JsonObject inputs) => new()
    {
        ["class_type"] = classType,
        ["inputs"] = inputs,
    };

    static JsonArray Link(string nodeId, int outputIndex) => [nodeId, outputIndex];

    static async Task VerifyComfyUiAsync(HttpClient httpClient)
    {
        try
        {
            using var response = await httpClient.GetAsync("/system_stats");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "ComfyUI is not reachable. Start it with the documented local command, then run the pixelart command again.", ex);
        }
    }

    static async Task<string> QueuePromptAsync(HttpClient httpClient, JsonObject workflow)
    {
        var payload = new JsonObject
        {
            ["prompt"] = workflow,
            ["client_id"] = Guid.NewGuid().ToString("N"),
        };

        using var content = new StringContent(payload.ToJsonString(JsonOptions), Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("/prompt", content);
        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"ComfyUI rejected the prompt: {responseText}");

        var json = JsonNode.Parse(responseText)?.AsObject()
            ?? throw new InvalidOperationException("ComfyUI returned an invalid prompt response.");
        return json["prompt_id"]?.GetValue<string>()
            ?? throw new InvalidOperationException("ComfyUI prompt response did not include prompt_id.");
    }

    static async Task<List<ComfyImageRef>> WaitForImagesAsync(HttpClient httpClient, string promptId, TimeSpan timeout)
    {
        using var cancellation = new CancellationTokenSource(timeout);
        while (!cancellation.IsCancellationRequested)
        {
            using var response = await httpClient.GetAsync($"/history/{Uri.EscapeDataString(promptId)}", cancellation.Token);
            response.EnsureSuccessStatusCode();

            var history = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellation.Token))?.AsObject();
            if (history?[promptId] is JsonObject promptHistory)
            {
                var images = ExtractImages(promptHistory);
                if (images.Count > 0)
                    return images;
            }

            await Task.Delay(TimeSpan.FromSeconds(2), cancellation.Token);
        }

        throw new TimeoutException($"Timed out waiting for ComfyUI prompt {promptId}.");
    }

    static List<ComfyImageRef> ExtractImages(JsonObject promptHistory)
    {
        List<ComfyImageRef> images = [];
        if (promptHistory["outputs"] is not JsonObject outputs)
            return images;

        foreach (var output in outputs)
        {
            if (output.Value?["images"] is not JsonArray outputImages)
                continue;

            foreach (var imageNode in outputImages.OfType<JsonObject>())
            {
                var fileName = imageNode["filename"]?.GetValue<string>();
                var subfolder = imageNode["subfolder"]?.GetValue<string>() ?? "";
                var type = imageNode["type"]?.GetValue<string>() ?? "output";
                if (!string.IsNullOrWhiteSpace(fileName))
                    images.Add(new ComfyImageRef(fileName, subfolder, type));
            }
        }

        return images;
    }

    static async Task<string> DownloadImageAsync(HttpClient httpClient, ComfyImageRef image, string outputDirectory)
    {
        var query = $"filename={Uri.EscapeDataString(image.FileName)}&subfolder={Uri.EscapeDataString(image.Subfolder)}&type={Uri.EscapeDataString(image.Type)}";
        using var response = await httpClient.GetAsync($"/view?{query}");
        response.EnsureSuccessStatusCode();

        var targetPath = Path.Combine(outputDirectory, image.FileName);
        await using var fileStream = File.Create(targetPath);
        await response.Content.CopyToAsync(fileStream);
        return targetPath;
    }

    static string ResolvePath(string repoRoot, string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(repoRoot, path));

    static string RelativePath(string repoRoot, string path) =>
        Path.GetRelativePath(repoRoot, path).Replace(Path.DirectorySeparatorChar, '/');

    static string SanitizeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(value.Select(character => invalid.Contains(character) ? '-' : character).ToArray());
    }

    static int Usage()
    {
        Console.WriteLine("""
            Pixel Art Candidate Generator

            Usage:
                            dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj pixelart generate <job-spec.json>

                        Optional img2img fields:
                            sourceImage: repo-relative source image path
                            denoise: 0.25-0.65 keeps more of the source shape
            """);
        return 1;
    }
}

sealed record PixelArtJobSpec(
    string Name,
    string Model,
    string Prompt,
    string NegativePrompt,
    string? Lora = null,
    double LoraStrengthModel = 0.8,
    double LoraStrengthClip = 0.8,
    string? SourceImage = null,
    double Denoise = 1.0,
    int Width = 512,
    int Height = 512,
    int Steps = 28,
    double Cfg = 7.0,
    string Sampler = "euler",
    string Scheduler = "normal",
    long SeedStart = 1000,
    int CandidateCount = 4,
    int BatchSize = 1,
    int TimeoutMinutes = 20,
    string? ServerUrl = null,
    string? OutputDirectory = null)
{
    public void Validate()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(Model);
        ArgumentException.ThrowIfNullOrWhiteSpace(Prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(NegativePrompt);
        if (LoraStrengthModel < -20 || LoraStrengthModel > 20)
            throw new ArgumentOutOfRangeException(nameof(LoraStrengthModel), "LoRA model strength must be between -20 and 20.");
        if (LoraStrengthClip < -20 || LoraStrengthClip > 20)
            throw new ArgumentOutOfRangeException(nameof(LoraStrengthClip), "LoRA CLIP strength must be between -20 and 20.");
        if (Denoise <= 0 || Denoise > 1)
            throw new ArgumentOutOfRangeException(nameof(Denoise), "Denoise must be greater than 0 and less than or equal to 1.");
        if (Width <= 0 || Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(Width), "Width and height must be positive.");
        if (Steps <= 0)
            throw new ArgumentOutOfRangeException(nameof(Steps), "Steps must be positive.");
        if (CandidateCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(CandidateCount), "Candidate count must be positive.");
        if (BatchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(BatchSize), "Batch size must be positive.");
        if (TimeoutMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(TimeoutMinutes), "Timeout minutes must be positive.");
    }
}

sealed record PixelArtRunManifest(
    string Name,
    DateTimeOffset CreatedUtc,
    string Model,
    string? Lora,
    double LoraStrengthModel,
    double LoraStrengthClip,
    string? SourceImage,
    double Denoise,
    string Prompt,
    string NegativePrompt,
    int Width,
    int Height,
    List<PixelArtCandidate> Candidates);

sealed record PixelArtCandidate(long Seed, string SourceFileName, string LocalPath);

sealed record ComfyImageRef(string FileName, string Subfolder, string Type);