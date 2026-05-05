---
title: ChatGPT Image Prompts
description: Index of per-request folders for ChatGPT-generated tactical game art concepts
author: GitHub Copilot
ms.date: 2026-05-04
ms.topic: reference
---

## Request Folders

Use these folders instead of one shared prompt bank. Each folder has its own
`prompt.md`, `response.md`, and space for returned image files.

| Request | Prompt | Response |
| --- | --- | --- |
| Terrain tile concept sheet | [prompt](../requests/01-terrain-tile-concept-sheet/prompt.md) | [response](../requests/01-terrain-tile-concept-sheet/response.md) |
| Unit sprite concept sheet | [prompt](../requests/02-unit-sprite-concept-sheet/prompt.md) | [response](../requests/02-unit-sprite-concept-sheet/response.md) |
| Mission one cutscene frame | [prompt](../requests/03-mission-one-cutscene-frame/prompt.md) | [response](../requests/03-mission-one-cutscene-frame/response.md) |
| Mission two relay yard concept | [prompt](../requests/04-mission-two-relay-yard-concept/prompt.md) | [response](../requests/04-mission-two-relay-yard-concept/response.md) |
| Character portrait concept | [prompt](../requests/05-character-portrait-concept/prompt.md) | [response](../requests/05-character-portrait-concept/response.md) |
| UI icon sheet | [prompt](../requests/06-ui-icon-sheet/prompt.md) | [response](../requests/06-ui-icon-sheet/response.md) |
| Runtime terrain tileset with variants | [prompt](../requests/07-runtime-terrain-tileset-variants/prompt.md) | [response](../requests/07-runtime-terrain-tileset-variants/response.md) |
| Transparent unit sprite atlas | [prompt](../requests/08-transparent-unit-sprite-atlas/prompt.md) | [response](../requests/08-transparent-unit-sprite-atlas/response.md) |
| Transparent UI icon atlas | [prompt](../requests/09-transparent-ui-icon-atlas/prompt.md) | [response](../requests/09-transparent-ui-icon-atlas/response.md) |
| Missions 1-10 imagery thread | [prompt](../requests/10-missions-01-10-imagery-thread/prompt.md) | [response](../requests/10-missions-01-10-imagery-thread/response.md) |
| Road and river autotile junctions | [prompt](../requests/11-road-river-autotile-junctions/prompt.md) | [response](../requests/11-road-river-autotile-junctions/response.md) |

## Shared Style Block

Most request prompts include this style direction already:

```text
Style: crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi
wargame, readable silhouettes, limited palette, clean dark outlines, strong
terrain and unit readability at Steam Deck handheld distance, no photorealism,
no painterly texture, no excessive bloom, no tiny unreadable detail. The art
should feel compatible with an Advance Wars-like tactical board while looking
more grounded, practical, and militarized.
```

Use the linked request folders for prompt copying and response filing.

## Sprite And Terrain Refresh Queue

Use these prompts before trying another full atlas. Each prompt asks for a small
source image that Copilot can crop, key, run through local img2img, and review
at board scale. Save returned images in the matching request folder or in
`../incoming` if the destination is uncertain.

Priority order:

1. Field Tech board token
2. Kestrel vehicle refresh sheet
3. Orison enemy unit refresh sheet
4. Horizontal road tile source
5. Road topology mini-family
6. River and bridge mini-family
7. Basin terrain material tiles
8. Tactical terrain prop refresh sheet

### Prompt 1: Kestrel Field Tech Board Token

Attach these reference files when pasting the prompt, if possible:

* `incoming/ChatGPT Image May 3, 2026, 09_02_24 PM.png`
* `incoming/ChatGPT Image May 3, 2026, 09_08_40 PM.png`

Save returned images to
`../requests/08-transparent-unit-sprite-atlas/`.

```text
Create one extractor-friendly Kestrel Field Tech infantry board token for a 2D
turn-based tactical combat game. Use the attached soldier images only as costume,
helmet, backpack, sensor, and color reference. Convert the idea into a chunky
runtime board sprite, not a portrait or concept illustration.

Canvas: one square image, 1024x1024 pixels. Use either a fully transparent
background or a flat pure magenta background for keying. No terrain, no ground
shadow, no title text, no labels, no UI card, no border, no multiple views.

Subject: a small Kestrel Field Tech infantry squad token, three-quarter top-down
orthographic camera, facing lower-right. The unit should be readable after crop
and downsample to 64x64 pixels. Fill about 75 percent of the canvas. Make the
silhouette distinct from a rifleman by including a compact sensor backpack,
tool arm, comms tablet, cable spool, or repair kit. Keep weapons understated;
this is a specialist support unit, not a front-line assault trooper.

Faction language: blue-gray armor plates, tan field gear, restrained cyan visor
or sensor accents. Practical near-future expedition equipment, grounded and
militarized.

Style: crisp 16-bit-era tactical pixel art, chunky pixel clusters, limited
palette, strong dark outline where needed, readable materials, no photorealism,
no painterly softness, no blurry halo, no excessive tiny detail, no flat vector
icon look, no simple primitive shapes.
```

### Prompt 2: Kestrel Vehicle Refresh Sheet

Save returned images to
`../requests/08-transparent-unit-sprite-atlas/`.

```text
Create an extractor-friendly Kestrel vehicle source sheet for refreshing runtime
unit sprites in a 2D turn-based tactical combat game.

Canvas and layout: one image, 1536x768 pixels, transparent or flat pure magenta
background, exactly six separated vehicles arranged in a 3 columns by 2 rows
grid. No labels, no captions, no title text, no cell borders, no terrain, no
ground shadows, no presentation cards.

Vehicle order:
1. Utility Armor line vehicle, compact armored rover with broad readable hull
2. Survey Scout buggy, low fast scout with sensor mast and visible wheels
3. Trail Striker fast light attack vehicle, narrow and aggressive
4. Field Rig repair and supply vehicle, boxy utility truck with crates and arm
5. Expedition Engineer support carrier, equipment-heavy non-tank silhouette
6. Siege Breaker heavy prototype, wide slow assault vehicle

Sprite requirements: every vehicle uses orthographic three-quarter top-down
camera, facing lower-right, full vehicle visible, centered in its cell, fills 70
to 82 percent of its cell, readable after crop and downsample to 64x64 pixels.
Use consistent Kestrel blue-gray armor, tan field gear, restrained cyan sensor
accents, and strong silhouette differences between roles.

Style: crisp 16-bit-era tactical pixel art, grounded near-future military
industrial design, chunky pixel clusters, limited palette, clean outline, no
photorealism, no painterly texture, no thin noisy details, no duplicate view
sheet, no white background, no map or board scene.
```

### Prompt 3: Orison Enemy Unit Refresh Sheet

Save returned images to
`../requests/08-transparent-unit-sprite-atlas/`.

```text
Create an extractor-friendly Orison enemy unit source sheet for refreshing
runtime sprites in a 2D turn-based tactical combat game.

Canvas and layout: one image, 1536x768 pixels, transparent or flat pure magenta
background, exactly six separated units arranged in a 3 columns by 2 rows grid.
No labels, no captions, no title text, no cell borders, no terrain, no ground
shadows, no presentation cards.

Unit order:
1. Raider Trooper infantry squad, compact corporate-security infantry
2. Line Armor vehicle, angular medium armor
3. Pursuit Scout buggy, fast low scout vehicle
4. Breach Sapper demolition infantry with charge pack
5. Breach Lancer anti-armor infantry with readable launcher silhouette
6. Ammo Mule support vehicle, compact logistics carrier

Sprite requirements: every unit uses orthographic three-quarter top-down camera,
facing lower-right, full unit visible, centered in its cell, fills 70 to 82
percent of its cell, readable after crop and downsample to 64x64 pixels.

Faction language: charcoal gray and industrial black armor, orange-red corporate
hazard accents, angular overbuilt silhouettes, practical equipment, no fantasy
or alien styling.

Style: crisp 16-bit-era tactical pixel art, grounded near-future wargame,
chunky pixel clusters, limited palette, clean outline, no photorealism, no
painterly texture, no tiny unreadable detail, no duplicate views, no terrain,
no card framing.
```

### Prompt 4: Horizontal Road Tile Source

Attach one of these reference files when pasting the prompt, if possible:

* `incoming/ChatGPT Image May 3, 2026, 09_08_30 PM.png`
* `incoming/ChatGPT Image May 3, 2026, 09_08_35 PM.png`

Save returned images to
`../requests/11-road-river-autotile-junctions/`.

```text
Create one extractor-friendly horizontal road tile source for a 2D turn-based
tactical combat game. Use the attached road image only as material and color
reference. The new image must be a single clean topology tile, not a map scene.

Canvas: one square image, 1024x1024 pixels. The road must connect exactly from
the center of the left edge to the center of the right edge. It must not touch
the top or bottom edge. No crossroads, no T junction, no corner, no border, no
frame, no labels, no UI markings, no vehicles, no buildings.

Tile content: dusty asphalt or compacted service road across dry Aster Basin
ground. The road should have readable shoulders, small cracks, tire scuffs,
and subtle pebbles. Keep the centerline and road width consistent at both edge
connections so Copilot can turn it into a 32x32 autotile.

Runtime requirements: the tile must still read clearly at 32x32 pixels under
units, cursor boxes, HP bars, objective markers, and grid overlays. Avoid high
frequency noise, tiny gravel speckles, photoreal texture, painterly softness,
and decorative details that disappear when downsampled.

Style: crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi
wargame, limited palette, practical dusty industrial terrain, clean dark edge
definition only where needed, no presentation card, no board-map composition.
```

### Prompt 5: Road Topology Mini-Family

Save returned images to
`../requests/11-road-river-autotile-junctions/`.

```text
Create an extractor-friendly road topology mini-atlas for a 2D turn-based
tactical combat game.

Canvas and layout: one image, 1024x512 pixels, exactly eight tiles arranged in a
4 columns by 2 rows grid. Each tile cell is exactly 256x256 pixels. No labels,
no captions, no title text, no cell borders, no presentation cards, no map
frame, no buildings, no vehicles.

Tile order, left to right, top to bottom:
1. road straight horizontal
2. road straight vertical
3. road corner north-east
4. road corner south-east
5. road T junction north-east-west
6. road T junction north-south-east
7. road 4-way junction
8. road bridge deck over vertical river, horizontal road

Continuity requirements: every road connection must meet exactly at the center
of the tile edge it touches. Road width, shoulder width, and palette must match
between tiles. Non-connected tile edges should be dry basin ground only.

Runtime requirements: design for extraction into crisp 32x32 runtime tiles. The
shape must remain readable with units and UI overlays on top. Avoid thin lines,
tiny high-frequency gravel, decorative borders, photoreal texture, and broad map
composition.

Style: crisp 16-bit-era tactical pixel art, dusty asphalt or compacted service
road over dry basin terrain, limited palette, clean tactical readability,
grounded near-future industrial setting.
```

### Prompt 6: River And Bridge Mini-Family

Save returned images to
`../requests/11-road-river-autotile-junctions/`.

```text
Create an extractor-friendly river and bridge topology mini-atlas for a 2D
turn-based tactical combat game.

Canvas and layout: one image, 1024x512 pixels, exactly eight tiles arranged in a
4 columns by 2 rows grid. Each tile cell is exactly 256x256 pixels. No labels,
no captions, no title text, no cell borders, no presentation cards, no map
frame, no buildings, no vehicles.

Tile order, left to right, top to bottom:
1. river straight horizontal
2. river straight vertical
3. river corner north-east
4. river corner south-east
5. river T junction north-east-west
6. river T junction north-south-east
7. river 4-way junction
8. road bridge deck over horizontal river, vertical road

Continuity requirements: every river or bridge connection must meet exactly at
the center of the tile edge it touches. River width, bank width, water color,
and dry basin edge treatment must match between tiles. Non-connected tile edges
should be dry basin ground only.

Runtime requirements: design for extraction into crisp 32x32 runtime tiles. The
river should read as impassable blue industrial runoff water with clear banks,
not as decorative paint or a UI line. The bridge tile must clearly show road
decking crossing water at 32x32 pixels.

Style: crisp 16-bit-era tactical pixel art, grounded near-future industrial
basin, limited palette, readable banks, restrained blue-green water, no
photorealism, no painterly softness, no decorative frame.
```

### Prompt 7: Basin Terrain Material Tiles

Save returned images to
`../requests/07-runtime-terrain-tileset-variants/`.

```text
Create an extractor-friendly dry basin terrain material tile sheet for a 2D
turn-based tactical combat game.

Canvas and layout: one image, 1024x512 pixels, exactly eight tiles arranged in a
4 columns by 2 rows grid. Each tile cell is exactly 256x256 pixels. No labels,
no captions, no title text, no cell borders, no presentation cards, no roads,
no rivers, no buildings, no units.

Tile order:
1. dusty basin plain A, subtle pebbles
2. dusty basin plain B, softer cracked dirt
3. dusty basin plain C, faint tire scuffs
4. dusty basin plain D, small rock scatter
5. basalt edge dirt, low contrast
6. compacted depot ground, slightly smoother
7. Asterite-flecked dirt, teal flecks restrained
8. disturbed construction ground, light machine marks

Continuity requirements: each tile must tile cleanly on all four edges. Keep
contrast low enough that unit sprites and cursor boxes remain readable, but add
enough material detail to beat flat placeholder terrain.

Runtime requirements: design for extraction into crisp 64x64 or 32x32 terrain
tiles. Avoid high-frequency noise, noisy speckle fields, giant cracks, large
rocks at edges, photoreal texture, painterly blur, and decorative borders.

Style: crisp 16-bit-era tactical pixel art for a grounded industrial sci-fi
basin, limited palette, dusty tan and gray terrain, restrained teal mineral
flecks, practical board readability.
```

### Prompt 8: Tactical Terrain Prop Refresh Sheet

Save returned images to
`../requests/07-runtime-terrain-tileset-variants/` or
`../requests/10-missions-01-10-imagery-thread/`.

```text
Create an extractor-friendly tactical terrain prop refresh sheet for a 2D
turn-based tactical combat game.

Canvas and layout: one image, 1536x1024 pixels, transparent or flat pure magenta
background, exactly 16 separated props arranged in a 4 columns by 4 rows grid.
No labels, no captions, no title text, no cell borders, no terrain background,
no presentation cards.

Prop order:
1. survey crate low cover
2. stacked field equipment cover
3. basalt ridge low obstacle
4. broken basalt ridge obstacle
5. Kestrel prefab HQ tile
6. relay objective console
7. fuel cache objective prop
8. pump station valve prop
9. field fabricator building tile
10. printer arm fabrication prop
11. concrete bridge deck prop
12. demolition charge prop
13. sensor post prop
14. jammer tower prop
15. audit convoy token
16. refinery HQ objective prop

Requirements: every prop should use orthographic three-quarter top-down camera,
full object visible, centered in its cell, readable after crop and downsample to
64x64 pixels. Keep tactical function obvious through silhouette. Leave enough
clear alpha around each prop for objective markers, cursor boxes, and HP bars.

Style: crisp 16-bit-era tactical pixel art, grounded near-future industrial
sci-fi, limited palette, clean dark outlines, practical military and logistics
hardware, no photorealism, no painterly softness, no tiny unreadable detail, no
text labels or logos.
```
