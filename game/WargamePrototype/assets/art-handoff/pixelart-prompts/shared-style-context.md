---
title: Shared Pixel Art Prompt Context
description: Reusable prompt language and negative constraints for local tactical pixel-art generation jobs
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Purpose

This file stores reusable prompt context for local ComfyUI jobs. Prompt specs
should copy from these blocks instead of inventing a new style language for each
run. Update this file when review packets reveal a reusable lesson.

## Board Sprite Shared Positive Context

```text
single tactical board unit sprite for a 16-bit turn-based strategy game,
grounded industrial sci-fi pixel art, orthographic three-quarter top-down
camera, facing lower-right, centered full body or full vehicle visible, chunky
readable silhouette, thick dark outline, limited 16-bit palette, crisp pixels,
simple internal detail, small stable team-color accent, flat pure magenta
background for extraction, no floor, no cast shadow, one complete unit only,
fills most of the final square crop, no inset composition, designed to remain
readable when cropped to 64x64 over busy terrain
```

## Board Sprite Shared Negative Context

```text
multiple characters, sprite sheet, turnaround sheet, duplicate poses, labels,
text, letters, numbers, watermark, UI card, border, frame, white background,
gray background, scenic background, floor plane, terrain, road, building,
interior room, facility, tilemap, floor plan, equipment layout, modular panels,
console, source-card composition, portrait, bust, closeup, front portrait view,
side view, tall realistic human, long legs, thin limbs, tiny weapon lines,
detailed face, anime closeup, photorealistic, 3d render, painterly, soft blur,
bloom, glow, smoke, cast shadow, cropped feet, cropped wheels, extra arms,
extra legs, inconsistent scale
```

## Kestrel Faction Context

```text
Kestrel expedition equipment, blue-gray panels, teal visor or sensor slit, tan
field gear, practical survey hardware, compact repair tools, field-science
militarized under pressure, improvised but disciplined
```

## Orison Faction Context

```text
Orison pressure-force equipment, charcoal-gray armor, orange hazard accents,
angular military silhouette, breach tools, clean corporate-industrial hardware,
purpose-built and aggressive
```

## Unit Silhouette Anchors

* Field Tech: oversized helmet, teal visor, compact armor vest, tan survey pack
* Engineer: square repair backpack, tool arm, small dish, cable spool
* Sapper: satchel, hazard stripe, compact breaching tool
* Lancer: long anti-armor tool angled away from body, readable thick barrel
* Scout: low survey buggy, teal sensor slit, fast compact wheel or crawler base
* Striker: lower sharper fast profile, aggressive nose, orange hazard accent
* Armor: broad wedge rover, thick tracks or wheels, clear turret or sensor nose
* Field Rig: cargo crates, hose reel, small crane arm, support-vehicle mass
* Siege Breaker: extra-wide heavy chassis, slow mass, large armored front

## Cutscene Shared Positive Context

```text
cinematic 16-bit pixel-art scene for a grounded industrial sci-fi tactics game,
eye-level camera, visible horizon, clear mission focal object, readable
silhouettes, Kestrel blue-gray and teal accents, Orison charcoal and orange
hazard accents where relevant, dusty Aster Basin materials, practical machinery,
crisp pixel art, restrained atmospheric depth
```

## Cutscene Shared Negative Context

```text
top-down map, isometric board, sprite sheet, asset sheet, labels, captions,
watermark, UI frame, logo, fantasy magic, glowing everywhere, photorealism, 3d
render, blurry painterly lighting, empty abstract landscape, cropped focal
object
```

## Current Lessons

* Generate one board token at a time before attempting full roster sheets.
* Use flat magenta or another chroma background for extraction instead of asking
  for transparency.
* Review board sprites at 64x64 over terrain before judging source images.
* Use guided image-to-image or ControlNet sketches for Mission 1 rescue and any
  unit whose exact silhouette matters.
* Board tokens should fill roughly 70 to 82 percent of the final 64x64 crop,
  with no surrounding source-card margin.
* Preserve the same three silhouette anchors across board sprite, cutscene
  cameo, UI portrait, damaged state, animation frame, and enemy recolor.
* Field Tech prompts must lead with a standing helmeted infantry figure, boots
  visible, arms visible, and no environment; otherwise SDXL may interpret field
  equipment as rooms, panels, or console props.
* Img2img with low denoise preserves useful silhouette, but it also preserves
  bad source backgrounds, cast shadows, and source-sheet framing. Pre-clean or
  crop source images before img2img when the source candidate has beige ground,
  grid tiles, cards, or shadows.
* Utility Armor v3 showed that low-denoise img2img from a cleaned source works
  better than asking SDXL for another full text-only vehicle. Start around
  `denoise: 0.32`, lower if the silhouette changes, and raise only if lighting
  artifacts remain.
* For board-scale review, key out flat magenta backgrounds before judging the
  sprite. Judge the 64x64 silhouette, faction read, and terrain separation, not
  the temporary extraction color.
* Road and river autotile prompts should not request the whole atlas. Split to
  one topology or a tiny 2x2 family, and prefer img2img topology masks when edge
  continuity matters.
* Request 11 retries showed that topology masks alone are not enough. Low
  denoise preserves exact road shape but leaves placeholder-looking masks;
  higher denoise adds texture but drifts into map scenes, canals, boards,
  frames, floors, or pasted strips. Direct SDXL can create better terrain
  texture, but it repeatedly violates exact road topology. Treat SDXL output as
  reference or material input for exact autotiles until a 32px-first authored
  tile clears visual review.
* Portrait prompts should say one centered bust only and ban side cards,
  documents, maps, screens, panels, captions, labels, and text blocks. The Venn
  v3 prompt shape is the current best local portrait template.
