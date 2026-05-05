---
title: Cutscene Graphics Format
description: Standard JSON format and generation workflow for SNES or Game Boy DS style cutscene art
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Purpose

This format standardizes how we define and generate cutscene background art for
Mission intros, interstitials, and debrief sequences.

The format is data driven and renderer agnostic:

* One JSON spec file describes palette, frame timing, and layered pixel shapes.
* A C# generator renders consistent SNES or Game Boy DS style PNG output.
* Output includes individual frames, a packed sheet, and a manifest for runtime
  playback.

## File Locations

* Specs: `game/WargamePrototype/assets/cutscenes/specs/*.cutscene.json`
* Output: `game/WargamePrototype/assets/cutscenes/generated/<cutscene_id>/`
* Generator: `src/Wargame.AssetTools/Program.cs`

## Schema Version

All specs currently use `"format_version": "1.0"`.

## Required Top-Level Fields

| Field | Type | Notes |
| --- | --- | --- |
| `format_version` | string | Must be `1.0` |
| `cutscene_id` | string | Folder and manifest identity |
| `style_profile` | string | Visual profile identifier |
| `resolution` | object | `width` and `height` in pixels |
| `palette` | object | Named RGBA colors (`[r,g,b,a]`) |
| `frames` | array | Ordered frame definitions |

Optional:

* `sheet.columns`: frame columns when packing sheet output

## Frame Fields

Each frame object supports:

| Field | Type | Notes |
| --- | --- | --- |
| `id` | string | Output frame filename stem |
| `duration_ms` | number | Playback duration for this frame |
| `background` | string or RGBA array | Palette key or inline color |
| `layers` | array | Ordered draw operations |

## Supported Layer Operations

| Operation | Required keys |
| --- | --- |
| `rect` | `x`, `y`, `width`, `height`, `color` |
| `ellipse` | `x`, `y`, `radius_x`, `radius_y`, `color` |
| `polygon` | `points`, `color` |
| `dither` | `x`, `y`, `width`, `height`, `color_a`, `color_b`, optional `step` |

Colors accept either:

* A palette key string such as `"sky_night_mid"`
* An inline RGBA array such as `[58, 84, 132, 255]`

## Example Skeleton

```json
{
  "format_version": "1.0",
  "cutscene_id": "example_cutscene",
  "style_profile": "snes_gbds_hybrid_v1",
  "resolution": { "width": 320, "height": 180 },
  "sheet": { "columns": 3 },
  "palette": {
    "sky": [24, 36, 68, 255],
    "ground": [52, 61, 74, 255],
    "accent": [219, 90, 78, 255]
  },
  "frames": [
    {
      "id": "01_opening",
      "duration_ms": 1200,
      "background": "sky",
      "layers": [
        { "op": "rect", "x": 0, "y": 120, "width": 320, "height": 60, "color": "ground" },
        {
          "op": "polygon",
          "points": [[0, 130], [60, 96], [140, 122], [220, 90], [320, 128], [320, 180], [0, 180]],
          "color": "accent"
        }
      ]
    }
  ]
}
```

## Generation Commands

Generate all assets (sprites and cutscenes):

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj
```

Generate only sprites:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- sprites
```

Generate all cutscenes:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- cutscenes
```

Generate one cutscene:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- cutscenes mission1_intro.cutscene.json
```

## Output Contract

For a cutscene id like `mission1_intro`, generation produces:

* `game/WargamePrototype/assets/cutscenes/generated/mission1_intro/01_*.png`
* `game/WargamePrototype/assets/cutscenes/generated/mission1_intro/mission1_intro_sheet.png`
* `game/WargamePrototype/assets/cutscenes/generated/mission1_intro/mission1_intro_manifest.json`

The manifest carries frame order, durations, and sheet layout metadata so the
runtime can sequence cutscenes without parsing the authoring spec.
