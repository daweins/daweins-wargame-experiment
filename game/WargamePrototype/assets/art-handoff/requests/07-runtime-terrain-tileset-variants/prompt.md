---
title: Runtime Terrain Tileset Variants Prompt
description: ChatGPT image prompt for extractor-friendly tactical terrain tiles with variants
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Prompt

Copy this into ChatGPT image generation:

```text
Create an extractor-friendly runtime terrain tile atlas for a 2D turn-based
tactical combat game.

Canvas and layout: one image, 1536x512 pixels, exactly 24 tiles arranged in a
6 columns by 4 rows grid. Each tile is exactly 256x128 pixels. No labels, no
captions, no title text, no decorative borders, no drop shadows outside tile
bounds, no perspective frame around the whole sheet.

Tile order, left to right, top to bottom:
1. dusty basin plain A, subtle pebbles
2. dusty basin plain B, softer cracked dirt
3. dusty basin plain C, sparse tire scuffs
4. dusty basin plain D, small rock scatter
5. dusty basin plain E, faint Asterite flecks
6. dusty basin plain F, low dust ripples
7. vertical asphalt service road center
8. horizontal asphalt service road center
9. service road T junction north
10. service road T junction east
11. service road corner north-east
12. service road corner south-east
13. survey crate cover A
14. survey crate cover B, mirrored composition
15. survey crate cover C, low barricade
16. survey crate cover D, stacked field equipment
17. basalt ridge A
18. basalt ridge B, mirrored silhouette
19. basalt ridge C, broken low ridge
20. basalt ridge D, taller shadowed ridge
21. Kestrel prefab HQ A
22. Kestrel prefab HQ B, alternate roof equipment
23. relay objective prop tile
24. fuel cache objective prop tile

Continuity requirements: plain tiles must tile cleanly on all four edges.
Road tiles must connect exactly at the center of each edge they touch. Cover,
ridge, HQ, relay, and fuel tiles must keep the tactical object centered and leave
clear open edges so units, cursor boxes, HP bars, and objective markers remain
readable.

Style: crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi
wargame, readable at Steam Deck handheld distance, limited palette, clean dark
outlines only where needed, no photorealism, no painterly texture, no excessive
bloom, no tiny unreadable detail. The art should feel compatible with an
Advance Wars-like tactical board while looking practical, industrial, dusty, and
militarized.
```
