---
title: Game Art Handoff
description: Per-request prompt and response folders for local and returned game art
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: how-to
---

## Purpose

Use this folder as the handoff point between Copilot, local SDXL candidate
generation, optional returned source art, and the local game asset pipeline.
Each art request has its own folder with a prompt and a response file.

## Workflow

1. Open the request index under [prompts](prompts).
2. Pick a request folder under [requests](requests).
3. Prefer a tracked local SDXL job under [pixelart-prompts](pixelart-prompts)
  when one exists for the request.
4. Save reviewed source images into that same request folder when promoted from
  ignored candidate output.
5. Paste notes, local candidate paths, or image filenames into that request's
  `response.md`.
6. Tell Copilot which request folder is ready and what you want done next.

Example:

```text
I filled in requests/08-transparent-unit-sprite-atlas/response.md and generated
local candidates for the request. Please review them and turn the best ones into
game assets.
```

## Folder Pattern

Each request folder should contain:

* `prompt.md`: the source prompt or asset request brief
* `response.md`: notes, local candidate paths, image filenames, and follow-up requests
* promoted source image files, saved beside the Markdown files when accepted

## Adding A New Request

Create a new folder under [requests](requests) with a short numbered name, then
add `prompt.md` and `response.md` using the existing folders as templates.

## Legacy Shared Drop Folder

The shared [incoming](incoming) folder remains available for unsorted images,
but the preferred workflow is to save images directly inside the matching
request folder.

## Local Pixel Art Candidate Generation

Local ComfyUI prompt jobs live in [pixelart-prompts](pixelart-prompts). Use this
flow for batch generation of sprite and pixel-art candidates from tracked prompt
specs. The raw candidate outputs are ignored and should be reviewed before any
image is promoted into runtime assets.

Start the local ComfyUI server from the repo root:

```powershell
pwsh ./scripts/assets/Start-LocalComfyUI.ps1
```

Run a prompt job from another terminal:

```powershell
dotnet run `
  --project ./src/Wargame.AssetTools/Wargame.AssetTools.csproj `
  pixelart generate `
  ./game/WargamePrototype/assets/art-handoff/pixelart-prompts/scout-buggy.sample.json
```

Generated candidates are written under the ignored
`game/WargamePrototype/assets/art-handoff/local-candidates/` folder with a
`manifest.json` that records prompt, model, seed, and output filenames.

## Recommended File Names

Use short descriptive names so the next step is easy to automate:

* `terrain-survey-camp-sheet.png`
* `unit-kestrel-scout-buggy.png`
* `portrait-field-commander-01.png`
* `cutscene-mission2-relay-yard.png`

## Notes

Generated images should be treated as concept or source art until they are
reviewed, cropped, converted, or extracted into runtime assets. Do not fulfill
art requests by generating sprites from C# primitive drawing code such as
rectangles, ellipses, and polygons; that path is useful for tooling experiments
only and is not accepted as final art. Keep final runtime assets in the
existing sprite and cutscene folders after review.
