---
title: Transparent Unit Sprite Atlas Prompt
description: Local SDXL source prompt for extractor-friendly transparent tactical unit sprites
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Prompt

Use this as the source prompt for local SDXL candidate generation:

```text
Create an extractor-friendly runtime unit sprite atlas for a 2D turn-based
tactical combat game. This must be genuine generated/painted pixel art, not
programmatic primitive art. Avoid the look of sprites assembled from simple
ellipses, rectangles, polygons, icon symbols, or generic placeholder shapes.

Canvas and layout: one image, 2304x512 pixels, exactly 18 sprites arranged in a
9 columns by 2 rows grid. Each sprite cell is exactly 256x256 pixels. Use a fully
transparent background for every sprite cell. Do not include labels, captions,
cell borders, ground shadows, presentation cards, title text, or a sheet
background.

Row 1, Kestrel player units in blue-gray with cyan accents:
1. Field Tech infantry squad
2. Utility Armor line vehicle
3. Survey Scout buggy
4. Expedition Engineer support unit
5. Field Sapper demolition infantry
6. AT Lancer anti-armor infantry
7. Trail Striker fast light vehicle
8. Field Rig repair and supply vehicle
9. Siege Breaker heavy prototype vehicle

Row 2, Orison enemy units in orange-red corporate colors with the same silhouette
roles:
1. Raider Trooper infantry squad
2. Line Armor vehicle
3. Pursuit Scout buggy
4. Sabotage Engineer support unit
5. Breach Sapper demolition infantry
6. Breach Lancer anti-armor infantry
7. Hunter Bike fast light vehicle
8. Ammo Mule support vehicle
9. Siege Breaker heavy prototype vehicle

Sprite requirements: each unit must be centered in its cell, fit within a
220x220 safe area, face three-quarter down-right, use a readable silhouette at
64x64 final size, include team-color accents in consistent locations, and avoid
fine internal details that disappear when scaled down. Keep transparent pixels
outside the silhouette. Each unit should look hand-authored, with chunky pixel
clusters, purposeful silhouette anchors, readable materials, and enough polish
to beat placeholder generated geometry.

Style: crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi
wargame, limited palette, clean dark outlines, no photorealism, no painterly
texture, no blurry antialiasing halo, no background color, no drop shadow.
Negative constraints: no C#-style primitive placeholder art, no flat vector icon
look, no simple rectangle/ellipse construction, no labels, no cards, no terrain,
no UI frame, no white or colored background, no duplicate view sheet, no
miniature characters lost inside large empty cells.
```
