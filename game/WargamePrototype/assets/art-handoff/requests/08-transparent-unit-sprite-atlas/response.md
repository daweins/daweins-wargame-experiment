---
title: Transparent Unit Sprite Atlas Response
description: Response notes and local candidate list for extractor-friendly transparent unit sprites
author: GitHub Copilot
ms.date: 2026-05-04
ms.topic: reference
---

## Promoted Images

Save promoted source images in this folder and list them here.

* Pending

No promoted transparent atlas has been saved in this request folder yet.

Local fallback candidates generated under ignored `local-candidates/`:

* `token-kestrel-field-tech-sdxl-nerijs-v2`
* `token-kestrel-utility-armor-sdxl-nerijs-v1`
* `token-kestrel-survey-scout-sdxl-nerijs-v1`
* `token-kestrel-field-tech-sdxl-nerijs-v3`
* `token-kestrel-utility-armor-sdxl-nerijs-v2`
* `token-kestrel-survey-scout-sdxl-nerijs-v2`
* `token-kestrel-field-tech-sdxl-nerijs-v4`
* `token-kestrel-utility-armor-sdxl-nerijs-v3`
* `token-kestrel-survey-scout-sdxl-nerijs-v3`
* `request08-unit-atlas-sdxl-nerijs-v1/request08-unit-atlas-sdxl-nerijs-v1_58000_00001_.png`
* `request08-unit-atlas-sdxl-nerijs-v1/request08-unit-atlas-sdxl-nerijs-v1_58001_00001_.png`
* `request08-unit-atlas-sdxl-nerijs-v1/request08-unit-atlas-sdxl-nerijs-v1_58002_00001_.png`
* `request08-unit-atlas-sdxl-nerijs-v1/request08-unit-atlas-sdxl-nerijs-v1_58003_00001_.png`
* `request08-kestrel-field-tech-sdxl-nerijs-v1/request08-kestrel-field-tech-sdxl-nerijs-v1_58100_00001_.png`
* `request08-kestrel-field-tech-sdxl-nerijs-v1/request08-kestrel-field-tech-sdxl-nerijs-v1_58101_00001_.png`
* `request08-kestrel-utility-armor-sdxl-nerijs-v1/request08-kestrel-utility-armor-sdxl-nerijs-v1_58200_00001_.png`
* `request08-kestrel-utility-armor-sdxl-nerijs-v1/request08-kestrel-utility-armor-sdxl-nerijs-v1_58201_00001_.png`
* `request08-kestrel-survey-scout-sdxl-nerijs-v1/request08-kestrel-survey-scout-sdxl-nerijs-v1_58300_00001_.png`
* `request08-kestrel-survey-scout-sdxl-nerijs-v1/request08-kestrel-survey-scout-sdxl-nerijs-v1_58301_00001_.png`
* `request08-kestrel-utility-armor-img2img-v2/request08-kestrel-utility-armor-img2img-v2_58400_00001_.png`
* `request08-kestrel-utility-armor-img2img-v2/request08-kestrel-utility-armor-img2img-v2_58401_00001_.png`
* `request08-kestrel-survey-scout-img2img-v2/request08-kestrel-survey-scout-img2img-v2_58500_00001_.png`
* `request08-kestrel-survey-scout-img2img-v2/request08-kestrel-survey-scout-img2img-v2_58501_00001_.png`
* `request08-kestrel-survey-scout-img2img-v3/request08-kestrel-survey-scout-img2img-v3_58600_00001_.png`
* `request08-kestrel-survey-scout-img2img-v3/request08-kestrel-survey-scout-img2img-v3_58601_00001_.png`
* `request08-kestrel-utility-armor-sdxl-nerijs-v2/request08-kestrel-utility-armor-sdxl-nerijs-v2_58701_00001_.png`
* `request08-kestrel-utility-armor-img2img-v3/request08-kestrel-utility-armor-img2img-v3_58800_00001_.png`
* `request08-kestrel-utility-armor-img2img-v3/request08-kestrel-utility-armor-img2img-v3_58801_00001_.png`

Rejected local deterministic placeholder:

* `local-transparent-unit-sprite-atlas.png`

## Local Candidate Notes

Pending promoted atlas.

May 4 incoming ChatGPT source images:

* `../../incoming/ChatGPT Image May 4, 2026, 02_09_34 PM.png` is the best new
  Field Tech infantry source. The figure has strong costume language, magenta
  key background, readable sensor backpack, tablet, and support-tool silhouette.
  It is not promoted yet because it needs alpha cleanup, crop, 64x64 board
  downsample review, and possible simplification of the backpack silhouette.
* `../../incoming/ChatGPT Image May 4, 2026, 02_09_37 PM.png` is a second Field
  Tech source with a clearer crouched board-token stance and large manipulator
  arm. It is useful as an img2img or extraction source, but the limb and arm
  shape may crowd a 64x64 tile.
* `../../incoming/ChatGPT Image May 4, 2026, 02_09_42 PM.png` is a strong
  Kestrel vehicle source sheet matching the requested six-vehicle refresh pass.
  It is the best current source for Utility Armor, Survey Scout, Trail Striker,
  Field Rig, Engineer support carrier, and Siege Breaker extraction. The sheet
  still needs per-cell crop, magenta keying, palette cleanup, and board-scale
  review before atlas assembly.
* `../../incoming/ChatGPT Image May 4, 2026, 02_11_25 PM.png` is a strong Orison
  enemy unit source sheet. It gives clear charcoal/orange faction language and
  distinct infantry, armor, scout, sapper, lancer, and support-carrier reads.
  Treat it as a cleanup and extraction source, not a promoted transparent atlas.
* Board-scale review packet:
  `../../local-review/incoming-may4-single-sources/candidate-board-readability.png`.
  The two Field Tech single-source images survive the first 64x64 review better
  than prior local infantry attempts. `02_09_34 PM.png` has the strongest full
  standing read, while `02_09_37 PM.png` has a useful crouched board-token pose
  but a busier arm silhouette.

Local split-job specs added for the next request 08 pass:

* `request08-kestrel-field-tech-sdxl-nerijs-v1.sample.json`
* `request08-kestrel-utility-armor-sdxl-nerijs-v1.sample.json`
* `request08-kestrel-survey-scout-sdxl-nerijs-v1.sample.json`

The broad v1 atlas was split because prompt-only atlas jobs keep drifting into
concept sheets, extra views, inconsistent counts, source cards, and non-keyable
backgrounds. These specs isolate one Kestrel board token at a time on flat
magenta so each candidate can be reviewed for silhouette, crop occupancy,
faction accent, and 64x64 terrain readability before any atlas assembly.

Local SDXL+nerijs review notes:

* Field Tech v2 is rejected for runtime use because it reads as a room,
  console, or equipment tile at 64x64 instead of a standing infantry unit.
* Utility Armor v1 is reference-only. It has useful vehicle mass, but needs
  stronger outline contrast and cleaner extraction.
* Survey Scout v1 is reference-only. Candidate `57301` has the best low buggy
  direction, but needs larger subject occupancy and less shadow/background
  dependence.
* Field Tech v3 seed `57400` is the strongest local SDXL infantry direction,
  but is still a pressure-test reference rather than runtime-ready atlas art.
* Utility Armor v3 and Survey Scout v3 did not beat the deterministic runtime
  bar. They still drift into sheets, cards, or noisy background fragments.
* `local-transparent-unit-sprite-atlas.png` has the requested transparent 9x2
  geometry, but it is rejected as art because it was generated from C# primitive
  shapes. Do not use it as fulfillment or as the quality bar.
* Request 08 SDXL v1 generated four local candidates from seeds `58000` through
  `58003`. These are materially better source art than the C# placeholder, but
  they are not extractor-ready atlases. They drift into broad concept sheets,
  extra views, inconsistent sprite counts, side-view vehicles, shadows, and
  non-transparent white backgrounds.
* Split request 08 v1 generated six one-token local candidates from seeds
  `58100`, `58101`, `58200`, `58201`, `58300`, and `58301`. The split approach
  is more useful than the broad atlas prompt, but not promotion-ready yet.
* Field Tech seed `58100` has a readable helmeted figure, but includes detached
  equipment parts, a beige background, and a ground shadow. Seed `58101` is
  rejected because it became a card or room composition.
* Utility Armor seed `58201` is the strongest source candidate in this pass:
  clear vehicle mass, readable wheels, and good pixel rendering. It still needs
  single-view cleanup, faction color correction, and background removal. Seed
  `58200` is rejected because it reads like a tiled board/card composition.
* Survey Scout seed `58300` has a promising low buggy silhouette, but it uses a
  beige background and heavy cast shadow. Seed `58301` is rejected as a multi-
  view vehicle sheet with the wrong palette.
* Img2img v2 generated four local candidates from Utility Armor seed `58201` and
  Survey Scout seed `58300`, plus a board-scale review sheet at
  `local-review/request08-img2img-v2/candidate-board-readability.png`.
* Utility Armor img2img v2 did not fix the source-sheet problem. It preserved or
  amplified the tiled board background, so both seeds remain source-reference
  only and should not be promoted.
* Survey Scout img2img v2 produced the strongest single-vehicle source shapes so
  far, especially seed `58501`, but both seeds still preserve beige background,
  cast shadow, and a slightly presentation-rendered angle. They need source
  cleanup or stronger background control before atlas promotion.
* Source cleanup tooling now prepares promising candidates for img2img by
  keying backgrounds, removing basin-ground or shadow artifacts, trimming the
  subject, and centering it on a 768x768 magenta extraction canvas.
* Survey Scout img2img v3 from cleaned seed `58501` produced readable board
  candidates. Seed `58600` is the best Scout direction in this pass, but it is
  still too dark at 64x64 and should be brightened or simplified before atlas
  assembly.
* Utility Armor v2 seed `58701` produced a strong single heavy vehicle source.
  Utility Armor img2img v3 seeds `58800` and `58801` are the first request 08
  heavy-vehicle candidates that read well on the board-scale review sheet.
  Seed `58801` is the current preferred Utility Armor source for extraction.
* Latest local review sheets:
  `local-review/request08-v3-sprite-review/candidate-board-readability.png` and
  `local-review/request08-utility-v3-review/candidate-board-readability.png`.

## Follow-Up For Copilot

May 4 promotion update:

* The runtime `assets/sprites/art_units.png` atlas now uses the May 4 Field Tech,
  Kestrel vehicle, and Orison enemy source sheets.
* The atlas is assembled as two 64px rows: Kestrel/player units on row 0 and
  Orison/enemy units on row 1.
* The Godot renderer now selects row 1 for enemy units when a two-row unit atlas
  is available, and it skips broad team color tinting for faction-authored rows.
* The extractor now trims transparent padding after magenta keying, so unit
  silhouettes occupy their 64x64 runtime cells more effectively.

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  regenerated `assets/sprites/art_units.png`.
* Visual inspection of `assets/sprites/art_units.png` confirmed the two-row
  Kestrel and Orison atlas is present and materially stronger than the rejected
  C# primitive placeholder.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeded.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.

Remaining unit risk: this is a runtime promotion, not final art signoff. Several
slots reuse vehicle or support-source crops, and the board still needs in-game
readability QA at 1280x800.
