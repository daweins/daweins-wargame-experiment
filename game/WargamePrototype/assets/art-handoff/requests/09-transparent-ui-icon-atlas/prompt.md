---
title: Transparent UI Icon Atlas Prompt
description: ChatGPT image prompt for extractor-friendly transparent tactical UI icons
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Prompt

Copy this into ChatGPT image generation:

```text
Create an extractor-friendly runtime UI icon atlas for a 2D turn-based tactical
combat game.

Canvas and layout: one image, 1536x256 pixels, exactly 12 icons arranged in a
12 columns by 1 row grid. Each icon cell is exactly 128x128 pixels. Use a fully
transparent background for every icon cell. Do not include labels, captions,
cell borders, title text, presentation cards, or a sheet background.

Icon order:
1. move arrow and path
2. attack crosshair or impact
3. wait hourglass
4. capture flag
5. repair wrench
6. supply crate and arrow
7. rescue person or pickup symbol
8. end turn circular arrow
9. terrain defense shield
10. objective diamond
11. HQ danger warning triangle
12. scout rescued checkmark and scout symbol

Icon requirements: each icon must be centered in its cell, fit within a 104x104
safe area, read clearly when scaled down to 20x20 pixels, use consistent stroke
weight, avoid gradients that blur at small size, and keep transparent pixels
outside the symbol.

Style: crisp 16-bit-era pixel UI icons for a grounded near-future tactical
wargame, limited palette, clean outline, cyan and warm warning accents, no
photorealism, no background color, no drop shadow.
```
