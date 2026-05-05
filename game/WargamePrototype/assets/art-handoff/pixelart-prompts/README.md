---
title: Local Pixel Art Prompt Jobs
description: Prompt job specs for local ComfyUI sprite and pixel art candidate generation
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: how-to
---

## Purpose

This folder contains tracked prompt job specs for the local ComfyUI candidate
generation pipeline. The jobs are source prompts, not final runtime assets.
Generated PNG candidates and manifests are written to ignored output folders.

## Local Workflow

1. Place an approved checkpoint file in
   `private/local-imagegen/ComfyUI/models/checkpoints/`.
2. Start ComfyUI from the repo root. On Windows with the RTX laptop GPU, prefer
   stable mode for local batches:

   ```powershell
   pwsh ./scripts/assets/Start-LocalComfyUI.ps1 -StableMode
   ```

3. In a second terminal, run a prompt job:

   ```powershell
    dotnet run `
       --project ./src/Wargame.AssetTools/Wargame.AssetTools.csproj `
       pixelart generate `
       ./game/WargamePrototype/assets/art-handoff/pixelart-prompts/scout-buggy.sample.json
   ```

4. Review the generated candidates under
   `game/WargamePrototype/assets/art-handoff/local-candidates/`.
5. Promote only reviewed, cleaned, game-ready outputs into the deterministic
   runtime asset pipeline.

## Job Fields

* `name`: Short run name used for output folder and file prefixes
* `model`: Checkpoint file name as ComfyUI sees it
* `lora`: Optional LoRA file name as ComfyUI sees it
* `loraStrengthModel` and `loraStrengthClip`: Optional LoRA strengths
* `prompt`: Positive prompt
* `negativePrompt`: Negative prompt
* `width` and `height`: Generation size sent to ComfyUI
* `steps`, `cfg`, `sampler`, `scheduler`: Sampling settings
* `seedStart`: First deterministic seed
* `candidateCount`: Number of seeds to queue
* `batchSize`: Images per queued prompt
* `timeoutMinutes`: Per-seed wait timeout
* `serverUrl`: ComfyUI HTTP endpoint
* `outputDirectory`: Ignored local candidate output folder

## Recommended Free Local Starting Point

Start with a Stable Diffusion 1.5 base checkpoint for the first local sprite
batches. It is the best fit for the RTX 4070 Laptop GPU when the goal is many
fast candidates at 512x512, and it keeps the workflow simple because it uses the
vanilla ComfyUI checkpoint path.

Use an SD 1.5 checkpoint file such as `v1-5-pruned-emaonly.safetensors` if its
license is acceptable for the project. Place the file under
`private/local-imagegen/ComfyUI/models/checkpoints/`, then set the job's
`model` field to the exact filename.

Suggested progression:

* Fast exploration: 512x512, 20-24 steps, CFG 6.5-7.5, 8-16 candidates
* Balanced default: 512x512, 28 steps, CFG 7.0, 4-8 candidates
* Higher quality: 640x640 or 768x768, 30-36 steps, CFG 6.5-7.5, 2-4 candidates

Avoid SDXL or larger models for the first pass. They can improve composition,
but they are slower on 8 GB VRAM and are less useful until the SD 1.5 prompt
style, silhouette requirements, and curation flow are proven.

## Prompt Iteration Findings

The first local SD 1.5 passes show that the base model is useful for fast
reference candidates, but not reliable enough to produce final runtime sprites
by prompt alone.

Working settings:

* Start ComfyUI with `-StableMode` on Windows to avoid batch crashes.
* Use 512x512 jobs for unit references. The 256x256 icon pass was faster, but
   collapsed into UI cards, frames, abstract panels, and building-like shapes.
* Keep candidate counts small per run, then iterate prompts from visual review.
* Treat generated outputs as concept references that still need cleanup,
   extraction, downscaling, and palette control before promotion.

Prompt patterns that helped:

* Lead with `single centered game sprite` and `one object only`.
* Specify `plain white background`, `no frame`, `no border`, and `no text`.
* Use concrete silhouette details such as wedge front, cyan sensor slit, square
   backpack, or shoulder launcher.

Prompt patterns that hurt:

* Real-world nouns such as tank, backpack, and product-like descriptions pulled
   the model toward photorealistic objects.
* The word `icon` often produced UI frames or abstract card art.
* Sheet-like language caused text, grids, multiple views, and labels.

Next quality lever: add a free local SD 1.5-compatible pixel-art LoRA or use an
image-to-image/control workflow from deterministic silhouette sketches. Prompt
engineering alone is unlikely to make the base checkpoint consistently produce
clean 64x64 tactical sprites.

## SDXL Pixel Art Experiment

For a stronger but slower test, use SDXL base with the `nerijs/pixel-art-xl`
LoRA.

Download these files manually in the browser after reviewing their license
terms:

* `sd_xl_base_1.0.safetensors` from
   <https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0>
* `pixel-art-xl.safetensors` from
   <https://huggingface.co/nerijs/pixel-art-xl>

Place them here:

* `private/local-imagegen/ComfyUI/models/checkpoints/sd_xl_base_1.0.safetensors`
* `private/local-imagegen/ComfyUI/models/loras/pixel-art-xl.safetensors`

Start with 768x768, 24 steps, CFG 6.0, and 2 candidates. On the RTX laptop GPU,
expect each candidate to be much slower than SD 1.5 but still practical for
small review batches.

Observed local performance in stable mode:

* First SDXL+LoRA image after load: about 19 seconds
* Warm 768x768 or 1024x576 image: about 8 to 11 seconds
* Utility Armor results: strong reference quality
* Field Tech results: useful character reference sheets, but not yet isolated
   board sprites
* Cutscene results: coherent candidate backgrounds for curation

## SDXL Consistency Pass Findings

The first SDXL+nerijs consistency pass generated grouped unit sheets and early
mission cutscene candidates for Missions 1-3.

Selected candidate references:

* Vehicle roster reference:
   `local-candidates/vehicle-roster-sdxl-nerijs-v4/vehicle-roster-sdxl-nerijs-v4_53000_00001_.png`
* Infantry roster reference:
   `local-candidates/infantry-roster-sdxl-nerijs-v4/infantry-roster-sdxl-nerijs-v4_53100_00001_.png`
* Mission 1 intro cinematic reference:
   `local-candidates/mission1-intro-cinematic-sdxl-nerijs-v4/mission1-intro-cinematic-sdxl-nerijs-v4_55000_00001_.png`
* Mission 2 relay cinematic reference:
   `local-candidates/mission2-relay-cinematic-sdxl-nerijs-v4/mission2-relay-cinematic-sdxl-nerijs-v4_55200_00001_.png`
* Mission 3 pump station cinematic reference:
   `local-candidates/mission3-pump-cinematic-sdxl-nerijs-v5/mission3-pump-cinematic-sdxl-nerijs-v5_56300_00001_.png`

Prompt patterns that improved consistency:

* Split units into vehicle and infantry sheets instead of one full roster sheet.
* Use `short squat proportions` and `oversized helmets` for board-scale
   infantry references.
* Use `eye-level camera`, `visible horizon`, `not top-down`, `not isometric`,
   and `not sprite sheet` for cutscene prompts.
* Keep SDXL+nerijs one-candidate specs for larger jobs when reliable manifest
   completion matters.

Remaining limitations:

* Vehicle and infantry sheets are strong concept references, but still need
   cropping, background cleanup, shadow removal, 64x64 downscaling, and board
   readability checks before runtime promotion.
* Cutscene prompts can drift into top-down asset sheets unless cinematic camera
   language is explicit.
* Mission 1 rescue still needs another guided pass. Blind prompts either made a
   giant crawler close-up or a minimal abstract scene; an image-to-image pass
   from a simple sketch should be the next attempt.

## Model Safety

Model files are not committed. Before adding a model locally, check its license
and allowed use. Prefer models with explicit commercial-use and redistribution
terms that fit the project.
