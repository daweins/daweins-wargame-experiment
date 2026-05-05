---
title: Art Handoff Status
description: Current monitored status for art-handoff requests and local fulfillment work
author: GitHub Copilot
ms.date: 2026-05-04
ms.topic: reference
---

## Monitoring Scope

Monitor `game/WargamePrototype/assets/art-handoff/requests/` for new numbered
request folders. A request is actionable when it has a `prompt.md` and either a
non-pending `response.md`, returned image files, or a local prompt spec that can
safely fulfill the same asset need.

Generated local candidates stay under ignored `local-candidates/`. Review sheets
stay under ignored `local-review/`. Promote only reviewed, cleaned, runtime-ready
outputs into tracked sprite or cutscene folders.

## Current Queue

| Request | Status | Next action |
| --- | --- | --- |
| `01-terrain-tile-concept-sheet` | Reference only | Terrain concept and runtime extraction are partial; not a full terrain-quality pass |
| `02-unit-sprite-concept-sheet` | Reference only | Extracted unit sheet is too dark and low contrast for the current quality bar |
| `03-mission-one-cutscene-frame` | Usable runtime source | Runtime cutscene source is already used; still needs in-game crop review before final art signoff |
| `04-mission-two-relay-yard-concept` | Usable runtime source | Runtime cutscene source is already used; still needs in-game crop review before final art signoff |
| `05-character-portrait-concept` | Usable runtime source | Runtime portrait source is already used; Venn v3 remains the strongest local portrait pattern |
| `06-ui-icon-sheet` | Usable runtime source | Extracted HUD icon source is stronger than deterministic fallback; exact request 09 atlas remains separate |
| `07-runtime-terrain-tileset-variants` | Runtime promoted with QA pending | May 4 basin material and prop sheets now feed `art_terrain.png`; road and river remain topology fallback work |
| `08-transparent-unit-sprite-atlas` | Runtime promoted with QA pending | May 4 Field Tech, Kestrel vehicle, and Orison sheets now feed a two-row `art_units.png` atlas |
| `09-transparent-ui-icon-atlas` | Fallback plus reference | No returned exact transparent atlas is present; deterministic fallback exists and request 06 icons are stronger reference material |
| `10-missions-01-10-imagery-thread` | Partial runtime promotion | Venn portrait v3 is now the preferred runtime commander portrait; broader mission, faction, and character coverage remains open |
| `11-road-river-autotile-junctions` | Runtime promoted with QA pending | Generated `art_paths.png` enforces road, river, and bridge topology from May 4 source materials |

## Active Local Fulfillment

The latest local fulfillment pass covers the formerly empty request folders with
deterministic runtime-shaped atlases and continues SDXL+nerijs token exploration
as style reference work:

* `07-runtime-terrain-tileset-variants/local-runtime-terrain-tileset-variants.png`
* `08-transparent-unit-sprite-atlas/local-transparent-unit-sprite-atlas.png`
* `09-transparent-ui-icon-atlas/local-transparent-ui-icon-atlas.png`
* `10-missions-01-10-imagery-thread/local-act1-ui-overlay-atlas.png`
* `10-missions-01-10-imagery-thread/local-act1-unit-reference-atlas.png`
* `10-missions-01-10-imagery-thread/local-act1-terrain-reference-atlas.png`
* `10-missions-01-10-imagery-thread/local-m01-hq-alert-cutscene-reference.png`
* `10-missions-01-10-imagery-thread/local-m01-scout-rescue-cutscene-reference.png`
* `10-missions-01-10-imagery-thread/local-m02-relay-yard-cutscene-reference.png`
* `10-missions-01-10-imagery-thread/local-m03-pump-station-cutscene-reference.png`
* `10-missions-01-10-imagery-thread/local-missions-04-10-reference-panels.png`
* `10-missions-01-10-imagery-thread/local-venn-portrait-v3.png`
* `token-kestrel-field-tech-sdxl-nerijs-v4`
* `token-kestrel-utility-armor-sdxl-nerijs-v3`
* `token-kestrel-survey-scout-sdxl-nerijs-v3`
* `request08-unit-atlas-sdxl-nerijs-v1`
* `request08-kestrel-field-tech-sdxl-nerijs-v1`
* `request08-kestrel-utility-armor-sdxl-nerijs-v1`
* `request08-kestrel-survey-scout-sdxl-nerijs-v1`
* `request08-kestrel-utility-armor-img2img-v2`
* `request08-kestrel-survey-scout-img2img-v2`
* `request08-prepared-img2img-sources-v3`
* `request08-kestrel-survey-scout-img2img-v3`
* `request08-kestrel-utility-armor-sdxl-nerijs-v2`
* `request08-kestrel-utility-armor-img2img-v3`
* `request11-road-autotiles-sdxl-nerijs-v1`
* `request11-road-tiles-2x2-sdxl-nerijs-v2`
* `request11-river-tiles-2x2-sdxl-nerijs-v2`
* `request11-road-horizontal-mask-img2img-v3`
* `request11-road-horizontal-mask-img2img-v4`
* `request11-road-horizontal-guide-img2img-v5`
* `request11-road-horizontal-dirt-guide-img2img-v6`
* `request11-dry-basin-ground-texture-sdxl-nerijs-v1`
* `request11-dirt-road-material-texture-sdxl-nerijs-v1`
* `request11-natural-basin-ground-material-sdxl-v2`
* `request11-natural-dirt-road-material-sdxl-v2`
* `request11-road-horizontal-direct-sdxl-v7`
* `request11-road-horizontal-direct-sdxl-v8-512`
* `request11-salvage-road-horizontal-v2`
* `request10-venn-portrait-sdxl-nerijs-v2`
* `request10-venn-portrait-sdxl-nerijs-v3`
* `mission4-6-escalation-cinematic-sdxl-nerijs-v1`
* `sable-meridian-style-sdxl-nerijs-v1`
* `mission8-blackout-kit-sdxl-nerijs-v1`
* `mission10-refinery-finale-sdxl-nerijs-v1`
* `commander-portraits-act1-sdxl-nerijs-v1`

C# primitive art generation is no longer accepted as art-handoff fulfillment.
The deterministic request atlases are legacy placeholders only. The unit atlas
is explicitly rejected despite correct transparency and geometry. Use the local
SDXL+nerijs `pixelart generate` pipeline for replacements.

Latest review packets:

* `game/WargamePrototype/assets/art-handoff/local-review/request08-v3-sprite-review/`
* `game/WargamePrototype/assets/art-handoff/local-review/request08-utility-v3-review/`
* `game/WargamePrototype/assets/art-handoff/local-review/request10-venn-portrait-v3-refresh/`
* `game/WargamePrototype/assets/art-handoff/local-review/request11-path-atlas-v1/`

Review result:

* Field Tech v3 seed `57400` remains the strongest local SDXL infantry
  direction. Field Tech v4 drifted into cards, sheets, and portrait-like
  framing.
* Utility Armor v3 contains interesting reference shapes, but still drifts into
  presentation cards or source sheets and is not promotion-ready.
* Survey Scout v3 contains reference shapes, but still needs cleaner single
  buggy crops and stronger 64x64 terrain separation.
* Mission 8 blackout v1 and Mission 10 refinery v1 are the strongest new
  higher-art reference sheets, but both need crop, alpha cleanup, and board
  readability review before runtime extraction.
* Mission 4-6 escalation v1 is useful as fabricator/support-machine reference
  only; it missed the requested cinematic-still panel structure.
* Sable/Meridian v1 is useful as uniform reference only; it missed the requested
  prop and vehicle coverage.
* Commander portraits v1 is rejected for portrait fulfillment because it
  produced full-body uniform sheets rather than briefing bust portraits.
* Request 08 unit atlas v1 is better source art than the C# primitive atlas, but
  it is not promotion-ready because it produced concept sheets, extra views,
  inconsistent counts, side-view vehicles, and white backgrounds instead of a
  precise transparent 9x2 atlas.
* Request 08 should now continue as three narrow Kestrel one-token jobs before
  any further atlas attempt: Field Tech infantry, Utility Armor rover, and
  Survey Scout buggy, each centered on flat magenta for cleanup and review.
* Request 08 split v1 generated six local candidates. Utility Armor seed
  `58201` is the strongest source image, Survey Scout seed `58300` has a useful
  low buggy silhouette, and Field Tech seed `58100` is a partial figure
  reference. None are runtime-ready because they still have beige backgrounds,
  shadows, cards, detached parts, or multi-view sheet drift.
* Request 08 img2img v2 generated four local candidates and a board-scale review
  sheet. Scout seed `58501` is the strongest single-vehicle source shape so far,
  but it still has beige background and cast shadow. Utility Armor img2img v2
  regressed into tiled board/background artifacts. No v2 candidate is ready for
  runtime promotion.
* The `prepare-img2img-source` command now creates magenta-backed img2img
  sources from local candidates. The second cleanup heuristic removes the worst
  basin-ground and shadow artifacts before low-denoise img2img.
* Request 08 Scout img2img v3 produced board-readable seeds `58600` and `58601`.
  Seed `58600` has the cleaner vehicle read, but it is too dark at 64x64 and
  should be brightened or post-processed before promotion.
* Request 08 Utility Armor v2 seed `58701` produced a strong single vehicle on a
  white background. After cleanup, Utility Armor img2img v3 seeds `58800` and
  `58801` produced clean magenta-backed single-vehicle candidates. Seed `58801`
  is the current strongest heavy-vehicle sprite candidate at board scale.
* Request 11 full road atlas v1 failed by turning into a large gray facility or
  floorplan. Road v2 improved color and road-water readability but still became
  a connected map scene instead of separated extractor tiles. River v2 seed
  `61200` is useful as river visual language; seed `61201` became framed panels
  and is rejected. The next request 11 pass should use one tile per job or a
  deterministic topology mask as img2img guidance.
* Request 11 mask-guided v3 and v4 preserved straight-road topology but barely
  redrew the flat mask, so the results read as placeholders rather than art.
  Richer v5 and v6 guides added texture but drifted into map, canal, or pasted
  strip reads. Material-composite and direct-candidate salvage tooling can force
  exact topology, but the best proof `request11-salvage-road-horizontal-v2` is
  still a flat band over sand and is not promotion-ready.
* Request 11 direct no-LoRA v7 produced the best natural terrain texture in seed
  `61700`, but it added a crossroad and border, then ComfyUI hit a CUDA illegal
  memory access during the batch. Restarting ComfyUI and retrying at 512px in
  v8 completed, but all visible seeds became framed boards, floor sheets, or
  map scenes. Direct SDXL remains useful for visual reference only, not exact
  autotile topology.
* Request 10 Venn portrait v2 reduced full-body drift but included side cards,
  text, and clutter. Venn portrait v3 seed `62100` is promoted as the preferred
  runtime commander portrait. Seed `62101` is usable as uniform/reference art but
  includes an office panel. Seed `62102` is rejected for text and UI clutter.

The May 4 incoming unit and terrain sources are now promoted into runtime atlas
assets. Use `assets/sprites/art_units.png` and `assets/sprites/art_terrain.png`
as the current game-facing art refresh, with board-scale QA still pending.

May 4 incoming review:

* Field Tech sources: `incoming/ChatGPT Image May 4, 2026, 02_09_34 PM.png` and
  `incoming/ChatGPT Image May 4, 2026, 02_09_37 PM.png`.
* Kestrel vehicle source sheet:
  `incoming/ChatGPT Image May 4, 2026, 02_09_42 PM.png`.
* Orison enemy source sheet:
  `incoming/ChatGPT Image May 4, 2026, 02_11_25 PM.png`.
* Road and river sources: `incoming/ChatGPT Image May 4, 2026, 02_13_43 PM.png`,
  `incoming/ChatGPT Image May 4, 2026, 02_20_07 PM.png`, and
  `incoming/ChatGPT Image May 4, 2026, 02_22_40 PM.png`.
* Basin material and prop sources:
  `incoming/ChatGPT Image May 4, 2026, 02_22_26 PM.png` and
  `incoming/ChatGPT Image May 4, 2026, 02_26_02 PM.png`.
* First board-scale review packet for single sources:
  `local-review/incoming-may4-single-sources/candidate-board-readability.png`.

Promotion outcome:

* Field Tech, Kestrel vehicle, and Orison sources were cropped, keyed, assembled,
  and promoted into `assets/sprites/art_units.png`.
* Basin material and tactical prop sources were cropped, keyed where needed, and
  promoted into `assets/sprites/art_terrain.png`.
* Road and river sources remain source references only because exact 32px
  topology from the returned sheets is still unproven.
* A generated runtime path atlas, `assets/sprites/art_paths.png`, now enforces
  exact road, river, and bridge topology using May 4 source materials.
* Verification passed through runtime atlas extraction, asset tooling build,
  Godot C# build, diagnostics, and 23 deterministic smoke checks.

## Monitoring Rule

When another agent adds a new request folder, classify it in this file before
running generation or extraction. If the folder has no returned image and cannot
be fulfilled locally, leave it as waiting instead of inventing completion.
