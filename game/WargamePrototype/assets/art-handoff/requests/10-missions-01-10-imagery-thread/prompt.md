---
title: Missions 1-10 Imagery Thread Prompt Backlog
description: Monitored first-act imagery backlog and copy-paste prompts for tactical campaign assets
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Monitoring Folder

Have the imagery thread monitor this folder:
`game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread`.

Save returned images beside this file and record filenames in `response.md`.
Use short filenames such as `m05-sable-arrival-cutscene.png`,
`m07-meridian-raider-sprite.png`, or `ui-first-act-overlays.png`.

## Shared Style Contract

Apply this style block to every prompt unless the specific prompt overrides it:

```text
Crisp 16-bit-era tactical pixel art for a grounded near-future sci-fi tactics
game, compatible with an Advance Wars-like tactical board but more industrial,
practical, and militarized. Strong readable silhouettes, clean dark outlines,
limited palette, nearest-neighbor pixel clarity, no photorealism, no painterly
softness, no excessive bloom, no tiny unreadable detail, no labels, no captions,
no UI card framing, no decorative sheet borders. Teal Asterite light is an
accent only, not a full-scene glow.
```

Board sprites must use this camera language:

```text
Orthographic three-quarter top-down board sprite, facing lower-right, full unit
or vehicle visible, centered, thick outline, flat transparent or flat magenta
extraction background, fills 70 to 82 percent of a final 64x64 crop, no terrain,
no shadow, no labels, no multiple views, no portrait crop.
```

Cutscene stills must use this camera language:

```text
Eye-level or slightly elevated cinematic pixel-art still, visible horizon when
showing a place, clear mission focal object, no top-down board map, no sprite
sheet layout, no presentation card, no text labels.
```

## Priority Order

Generate in this order unless a gameplay implementation slice needs a later
asset sooner:

1. Board-critical missing runtime families for Missions 4-6
2. Sable and Meridian style exploration for Missions 5, 7, and 9
3. Mission 8 blackout, audit convoy, and power-restore assets
4. Mission 10 refinery finale kit and Sloane assets
5. Commander portrait atlas expansions
6. Optional extra cutscene stills after terrain, units, and UX overlays read
   clearly at 64x64 and 1280x800

## Asset Backlog

| ID | Asset family | Missions | Required outputs |
| --- | --- | --- | --- |
| `cutscene-m01-m03` | Early Kestrel stills | 1-3 | HQ alert, Scout-7 stranded, relay yard, convoy road, pump station hold |
| `cutscene-m04-m06` | Escalation stills | 4-6 | Fabricator activation, Sable arrival in fog, bridge demolition standoff |
| `cutscene-m07-m10` | First-act reveal stills | 7-10 | Calder confrontation, audit blackout, blank-map reveal, refinery victory |
| `portraits-act1` | Commander portraits | 1-10 | Venn, Rusk, Priya, Holt, Sloane, Rhee, Calder, Treaty auditor |
| `units-kestrel-specialists` | Kestrel support sprites | 2-8 | Engineer, AT Lancer, Trail Striker, Field Rig, crawler convoy token |
| `units-orison-act1` | Orison enemy sprites | 1-10 | Raider, Line Armor, Pursuit Scout, Breach Sapper, refinery security, Sloane marker |
| `units-sable` | Sable enemy sprites | 5, 9 | Recon infantry, sensor unit, artillery silhouette, command marker |
| `units-meridian` | Meridian enemy sprites | 7 | Raider, scout bike, trail ambusher, civilian silhouette, Calder marker |
| `terrain-m01-m03` | Starter terrain and props | 1-3 | Survey camp, relay yard, fuel cache, pump station, pipe bridge, crawler wreck |
| `terrain-m04-m06` | Fabricator and bridge props | 4-6 | Field fabricator, depots, printer arms, bridge deck, demolition charges |
| `terrain-m07-m10` | Settlement, fog ridge, refinery | 7-10 | Heat tap, drill rig, audit convoy, substation, jammer, sensor post, refinery HQ |
| `ui-act1-objectives` | UX overlay atlas | 1-10 | Rescue, capture, fuel, convoy, production, fog, sensor, jammer, demolition, blackout, restraint, scan, refinery |

## Copy-Paste Prompt: Act-One UX Overlay Atlas

```text
Create a transparent runtime UI overlay atlas for Missions 1-10 of a 2D
turn-based tactical combat game.

Canvas and layout: one image, 2048x256 pixels, exactly 16 icons arranged in a
16 columns by 1 row grid. Each icon cell is exactly 128x128 pixels. Fully
transparent background for every cell. Do not include labels, captions, title
text, cell borders, presentation cards, or sheet background.

Icon order:
1. HQ danger warning
2. Scout rescue marker
3. capture flag
4. fuel cache badge
5. convoy route arrow
6. pump station hold zone
7. production factory
8. income depot
9. soft fog veil
10. sensor coverage sweep
11. jammer tower warning
12. demolition countdown
13. blackout power-off marker
14. restore power spark or substation
15. restraint or civilian-risk marker
16. refinery commander defeat

Style: crisp 16-bit-era pixel UI icons for a grounded near-future tactical
wargame, limited palette, clean dark outline, readable at 20x20 pixels,
consistent stroke weight, cyan status accents and warm warning accents, no
photorealism, no background color, no drop shadow.
```

## Copy-Paste Prompt: Missions 4-6 Runtime Gaps

```text
Create a transparent sprite and prop concept sheet for Missions 4-6 of a
grounded 16-bit tactical combat game.

Canvas and layout: one image, 1536x1024 pixels, transparent background, 4
columns by 4 rows of separated concepts. Do not include labels, captions, card
frames, UI panels, title text, or terrain backgrounds.

Required concepts:
1. Kestrel AT Lancer infantry, long readable anti-armor launcher
2. Kestrel Trail Striker, low fast recon-attack vehicle
3. Kestrel Field Rig, boxy logistics vehicle with crates and repair hardware
4. Orison Breach Sapper infantry with satchel and hazard stripe
5. Orison demolition infantry with compact charge pack
6. Orison local commander battlefield marker, not a portrait
7. Field fabricator building tile, active teal and magenta machinery accents
8. Depot pallet property tile with capture flag socket
9. Printer arm or fabrication prop
10. Emergency authority console prop
11. Concrete bridge deck tile
12. Demolition charge prop for bridge objective
13. Cracked bridge pylon prop
14. Service gantry bridge prop
15. Bridge warning placard prop without readable text
16. Production-active overlay symbol

Board sprite style: orthographic three-quarter top-down, facing lower-right for
units, full object visible, thick dark outline, fills 70 to 82 percent of its
cell, strong silhouettes at 64x64 crop. Kestrel uses blue-gray, teal sensors,
tan field gear. Orison uses charcoal gray, orange hazard accents, angular
purpose-built military forms. No photorealism, no labels, no multiple views per
asset.
```

## Copy-Paste Prompt: Sable And Meridian Style Exploration

```text
Create a transparent faction style exploration sheet for a grounded 16-bit
tactical combat game, covering Sable Accord and Free Meridian Compact assets.

Canvas and layout: one image, 1536x1024 pixels, transparent background, two
horizontal bands. Top band is Sable Accord. Bottom band is Free Meridian
Compact. Place eight separated concepts per faction. Do not include labels,
captions, title text, UI cards, terrain backgrounds, or presentation borders.

Sable Accord concepts:
1. disciplined recon infantry
2. sensor specialist unit with compact dish pack
3. light armored recon vehicle
4. artillery silhouette or indirect-fire vehicle
5. sensor post property prop
6. antenna mast prop
7. jammer tower prop
8. command marker for Colonel Rhee

Free Meridian concepts:
1. fast raider infantry with patched gear
2. scout bike or trail buggy
3. ambusher or saboteur silhouette
4. civilian heat-tap technician token
5. drill rig prop
6. domestic Asterite heat tap prop
7. Asterite crate prop
8. command marker for Marshal Calder

Style: Sable should feel disciplined, state-backed, cool slate and desaturated
blue-gray, clean sensor hardware, formal silhouettes. Meridian should feel
local, patched, warm amber lights, dusty red trails, practical hauler gear,
protective rather than villainous. All board concepts must be orthographic
three-quarter top-down, full unit or prop visible, thick dark outline, readable
at a 64x64 crop, no photorealism, no labels.
```

## Copy-Paste Prompt: Mission 8 Audit Blackout Kit

```text
Create a transparent sprite and prop sheet for Mission 8, The Audit Arrives, in
a grounded 16-bit tactical combat game.

Canvas and layout: one image, 1536x768 pixels, transparent background, 4 columns
by 3 rows of separated concepts. Do not include labels, captions, title text,
UI cards, terrain backgrounds, or presentation borders.

Required concepts:
1. Treaty Oversight audit vehicle, noncombatant convoy token
2. Treaty auditor tiny briefing token, noncombatant, not heroic
3. blacked-out supply warehouse property
4. active supply warehouse property variant
5. substation power-off prop
6. substation restored prop
7. sealed file terminal prop
8. restore-power objective marker
9. blackout overlay tile sample
10. emergency red-strip light prop
11. Orison remnant raider variant
12. Sable edge-pressure marker

Style: 16-bit tactical pixel art, grounded industrial sci-fi, dim navy shadows,
emergency red strips, gray warehouses, practical noncombatant convoy design,
clear silhouettes at 64x64, no photorealism, no labels, no card frames.
```

## Copy-Paste Prompt: Mission 10 Refinery Finale Kit

```text
Create a transparent sprite and prop sheet for Mission 10, Operation Small
Print, the Orison refinery first-act finale of a grounded 16-bit tactical combat
game.

Canvas and layout: one image, 1536x1024 pixels, transparent background, 4
columns by 4 rows of separated concepts. Do not include labels, captions, title
text, UI cards, terrain backgrounds, or presentation borders.

Required concepts:
1. Orison refinery HQ property tile
2. smokestack prop
3. conveyor road tile segment
4. power siphon prop with restrained teal glow
5. legal claim billboard prop without readable text
6. compliance server rack prop
7. refinery security infantry
8. refinery security armor
9. Director Sloane commander marker, not a portrait
10. Siege Breaker heavy vehicle, wide and slow, readable counter target
11. production bay tile
12. income cache tile
13. refinery capture overlay
14. commander defeat overlay
15. emergency broadcast antenna prop
16. damaged refinery variant prop

Style: Orison assets are charcoal gray and industrial black with orange hazard
accents, molten copper lights, angular corporate-security silhouettes, legalistic
and overbuilt. Teal siphon glow is an accent only. Board concepts are
orthographic three-quarter top-down, full object visible, thick dark outline,
readable at 64x64 crop, no photorealism, no labels.
```

## Copy-Paste Prompt: Commander Portrait Atlas

```text
Create a commander portrait concept atlas for the first 10 missions of a
grounded 16-bit tactical combat game.

Canvas and layout: one image, 1024x1024 pixels, 4 columns by 2 rows, each cell a
single bust portrait on transparent background. Do not include labels, captions,
names, title text, UI frames, speech bubbles, or presentation cards.

Portrait order:
1. Dr. Elara Venn, Kestrel expedition director, calm scientist forced into command
2. Major Jonah Rusk, Kestrel security liaison, disciplined and protective
3. Chief Engineer Priya Nayar, Kestrel logistics lead, practical and sharp
4. Lt. Sera Holt, Scout-7 survivor and recon lead, alert and stubborn
5. Director Cassian Sloane, Orison resource director, polished corporate antagonist
6. Colonel Amara Rhee, Sable Accord commander, professional disciplined rival
7. Marshal Inez Calder, Free Meridian protector, patched local commander
8. Treaty Oversight auditor, exhausted bureaucratic noncombatant in field gear

Style: 16-bit-era briefing portraits, grounded sci-fi uniforms and field gear,
strong silhouettes and readable faces at 96x96, limited palette, clean dark
outlines, no photorealism, no dramatic fantasy lighting, no text.
```

## Copy-Paste Prompt: Cutscene Still Batch

```text
Create four separate eye-level cinematic pixel-art stills for a grounded
near-future tactical campaign. Use a 16-bit-era pixel-art style, visible horizon
where appropriate, crisp forms, limited palette, no text, no labels, no top-down
map view, no sprite sheet layout, no UI cards.

Canvas and layout: one image, 2048x1152 pixels, 2 columns by 2 rows. Each panel
is a separate 16:9 cutscene still. Keep clear gutters between panels and no
captions.

Panel 1, Mission 5 Sable arrival: cold fog over antenna masts, Sable recon
vehicles emerging with disciplined blue-gray lights, Kestrel science team in
the distance, sensor blackout mood.

Panel 2, Mission 7 Calder confrontation: Meridian settlement edge with warm
heat taps, patched fences, active drill rig, Calder's local defenders blocking
Kestrel without looking villainous.

Panel 3, Mission 9 blank map reveal: chalky ridge, sensor post, jammer tower,
sealed utility hatch, projected map fragment showing missing corridors without
readable text.

Panel 4, Mission 10 refinery victory: Orison refinery at dusk, power siphons
shutting down, Sloane's command vehicles retreating, a compliance server rack
still glowing absurdly in the foreground.
```

## Review Rules

Do not promote any returned image until it passes the relevant check:

* Board unit reads at 64x64 over plain, road, cover, HQ, and ridge tiles
* Objective prop has a distinct silhouette from normal terrain
* UX icon reads at 20x20 without label text
* Cutscene frame shows the mission objective clearly without becoming a board
  map or generic sci-fi landscape
* Faction identity remains visible after crop, palette reduction, and
  downscaling
* Returned file is listed in `response.md` with accepted, needs cleanup, or
  rejected status
