---
title: Road And River Autotile Junctions Prompt
description: ChatGPT image prompt for 32px road and river autotile junction assets
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Monitoring Folder

Have the imagery thread monitor this folder:
`game/WargamePrototype/assets/art-handoff/requests/11-road-river-autotile-junctions`.

Save returned images beside this file and record filenames in `response.md`.
Use short filenames such as `road-river-autotiles-32.png` or
`road-river-bridge-variants-32.png`.

## Prompt

Copy this into ChatGPT image generation:

```text
Create an extractor-friendly runtime autotile atlas for road and river junctions
in a 2D turn-based tactical combat game.

Canvas and layout: one image, 1024x512 pixels, exactly 32 tiles arranged in an
8 columns by 4 rows grid. Each tile cell is exactly 128x128 pixels, designed to
be downsampled or extracted into crisp 32x32 runtime tiles. No labels, no
captions, no title text, no decorative borders, no drop shadows outside tile
bounds, no presentation cards, no perspective frame around the sheet.

Tile order, left to right, top to bottom:
1. road straight horizontal
2. road straight vertical
3. road corner north-east
4. road corner south-east
5. road corner south-west
6. road corner north-west
7. road T junction north-east-west
8. road T junction north-south-east
9. road T junction south-east-west
10. road T junction north-south-west
11. road 4-way junction
12. road dead end north
13. road dead end east
14. road dead end south
15. road dead end west
16. road bridge crossing vertical river, horizontal road deck
17. river straight horizontal
18. river straight vertical
19. river corner north-east
20. river corner south-east
21. river corner south-west
22. river corner north-west
23. river T junction north-east-west
24. river T junction north-south-east
25. river T junction south-east-west
26. river T junction north-south-west
27. river 4-way junction
28. river dead end north
29. river dead end east
30. river dead end south
31. river dead end west
32. road bridge crossing horizontal river, vertical road deck

Continuity requirements: every road or river connection must meet exactly at the
center of the tile edge. Road tiles should read as dusty asphalt or compacted
service road over dry basin ground. River tiles should read as impassable blue
industrial runoff water with clear banks. Bridge tiles must show road decking
crossing water and must remain readable at 32x32 pixels.

Runtime requirements: tiles must remain readable inside a 1280x800 tactical
board with units, cursor boxes, HP bars, objective markers, and team frames on
top. Avoid tiny noise, thin lines, high-frequency texture, and details that only
work at source resolution. Edges must tile cleanly with matching variants.

Style: crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi
wargame, compatible with an Advance Wars-like board but more industrial,
practical, dusty, and militarized. Limited palette, clean dark outlines only
where needed, no photorealism, no painterly texture, no excessive bloom, no
text, no symbols, no UI decoration.
```
