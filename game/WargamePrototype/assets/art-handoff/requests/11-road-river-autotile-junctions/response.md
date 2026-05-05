---
title: Road And River Autotile Junctions Response
description: Returned asset tracking for 32px road and river autotile junction assets
author: GitHub Copilot
ms.date: 2026-05-04
ms.topic: reference
---

## Returned Images

Record returned image filenames here when the imagery thread produces them.

* Pending

No promoted returned images have been saved in this request folder yet.

Local candidates generated under ignored `local-candidates/`:

* `request11-road-autotiles-sdxl-nerijs-v1/request11-road-autotiles-sdxl-nerijs-v1_61000_00001_.png`
* `request11-road-tiles-2x2-sdxl-nerijs-v2/request11-road-tiles-2x2-sdxl-nerijs-v2_61100_00001_.png`
* `request11-road-tiles-2x2-sdxl-nerijs-v2/request11-road-tiles-2x2-sdxl-nerijs-v2_61101_00001_.png`
* `request11-river-tiles-2x2-sdxl-nerijs-v2/request11-river-tiles-2x2-sdxl-nerijs-v2_61200_00001_.png`
* `request11-river-tiles-2x2-sdxl-nerijs-v2/request11-river-tiles-2x2-sdxl-nerijs-v2_61201_00001_.png`
* `request11-road-horizontal-mask-img2img-v3/`
* `request11-road-horizontal-mask-img2img-v4/`
* `request11-road-horizontal-guide-img2img-v5/`
* `request11-road-horizontal-dirt-guide-img2img-v6/`
* `request11-natural-basin-ground-material-sdxl-v2/`
* `request11-natural-dirt-road-material-sdxl-v2/`
* `request11-road-horizontal-direct-sdxl-v7/`
* `request11-road-horizontal-direct-sdxl-v8-512/`
* `request11-salvage-road-horizontal-v2/road-horizontal-salvage-64.png`

## Review Status

The local SDXL passes are not promotion-ready.

May 4 incoming ChatGPT topology sources:

* `../../incoming/ChatGPT Image May 4, 2026, 02_13_43 PM.png` is a strong
  horizontal dusty road source. It has the correct left-right road read, usable
  basin shoulder treatment, and more natural material than the local SDXL road
  attempts. It is not promoted yet because it must be cropped, downsampled to
  32x32, checked for edge-center continuity, and tested under units and UI.
* `../../incoming/ChatGPT Image May 4, 2026, 02_20_07 PM.png` is a useful road
  topology mini-family source. It covers straight, vertical, corner, junction,
  and bridge ideas with consistent material. It includes visible cell dividers
  and some tiles do not guarantee exact edge-center continuity, so use it as a
  source for extraction and topology-guided cleanup rather than direct runtime
  promotion.
* `../../incoming/ChatGPT Image May 4, 2026, 02_22_40 PM.png` is a river and
  bridge topology source. The water and bridge read clearly, but the canal-wall
  treatment is too blocky and too orthogonal for direct use in the dry basin
  road family. It is useful for water palette, bridge proportions, and topology
  reference.
* Generic candidate review packet:
  `../../local-review/incoming-may4-single-sources/candidate-board-readability.png`.
  This confirms the horizontal road source should not be judged with the unit
  token review path. It needs terrain-specific 32px crop, continuity, and overlay
  checks instead of contain-fit sprite scaling.

* Road autotile v1 failed the extractor-sheet requirement by turning into a
  large gray building or floorplan instead of separated road and river tiles.
* Road 2x2 v2 improved the dusty road and industrial water visual language, but
  it still became a connected map scene rather than four separated tiles with
  exact edge-center topology.
* River 2x2 v2 seed `61200` is useful visual reference for dry basin banks and
  industrial runoff water, but it is still not a clean tile family. Seed `61201`
  is rejected because it became framed presentation panels.
* Mask-guided road v3 and v4 preserved the exact left-right topology, but they
  stayed too close to the flat mask and did not become acceptable terrain art.
* Richer guide v5 and dirt-guide v6 proved that higher denoise can add texture,
  but it also loses the tile read by drifting into map, canal, or pasted-strip
  results.
* Material generation without the pixel-art LoRA avoided some tile-sheet drift,
  but the generated ground remained too macro or too soft for 32px road tiles.
* The terrain compositor and salvage compositor can force exact horizontal road
  topology from local SDXL materials. The best proof, `request11-salvage-road-horizontal-v2`,
  is still rejected because the road reads as a flat strip laid over sand.
* Direct no-LoRA seed `61700` produced the best natural texture reference, but
  it added a crossroad and border. The v8 512px retry completed after a ComfyUI
  restart, but all seeds became framed boards, floor sheets, or map scenes.

## Follow-Up For Copilot

Runtime promotion update:

* `assets/sprites/art_paths.png` now provides a generated road, river, and bridge
  topology atlas. It uses the May 4 basin, road, and river sources as material
  input while enforcing exact 16-mask topology in C#.
* The atlas has 16 columns of 64px tiles and 4 rows: road, river, horizontal
  road over vertical river, and vertical road over horizontal river.
* The Godot renderer now loads `art_paths.png` when present and uses it for road
  and river tiles. It falls back to the procedural path renderer when the atlas
  is missing or malformed.
* Bridge art is selected only when road direction crosses an adjacent river
  direction, not merely when a road sits beside water.

Review evidence:

* `../../local-review/request11-path-atlas-v1/path-atlas-2x-preview.png`
* `../../local-review/request11-path-atlas-v1/path-board-readability.png`
* `../../local-review/request11-path-atlas-v1/path-bridge-readability.png`

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  regenerated `assets/sprites/art_paths.png`.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- terrain-path-review`
  generated the path atlas, board, and bridge review packet.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeded.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.

Remaining risk: this is a runtime-composited promotion, not acceptance of a
returned exact ChatGPT autotile atlas. It clears the previous flat fallback bar,
but still needs in-game 1280x800 QA across real missions and may benefit from a
future returned exact road/river material sheet.
