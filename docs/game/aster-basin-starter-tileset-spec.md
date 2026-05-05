---
title: Aster Basin Starter Tileset Spec
description: Concrete 64px terrain and prop spec for Missions 1-3
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: design
---

## Purpose

This spec turns the campaign environment plan into a runtime-oriented starter
tileset for Missions 1-3. It uses the existing terrain rules and 64px board
grid, then separates what must ship as deterministic runtime tiles from what can
remain art-reference material.

Runtime target: 64x64 pixel-art tiles, nearest-neighbor scaling, readable at
1280x800 on Steam Deck, and compatible with the current Godot board renderer.

## Existing Asset Baseline

Current runtime and reference assets:

| Asset | Role | Status |
| --- | --- | --- |
| `game/WargamePrototype/assets/sprites/terrain.png` | Five-tile primitive runtime sheet | Usable fallback |
| `game/WargamePrototype/assets/sprites/art_terrain.png` | Preferred runtime terrain sheet when present | Current runtime path loads it first |
| `game/WargamePrototype/assets/art-handoff/requests/07-runtime-terrain-tileset-variants/local-runtime-terrain-tileset-variants.png` | Deterministic 6x4 64px terrain variant sheet | Best local runtime-shaped source |
| `game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread/local-act1-terrain-reference-atlas.png` | Act-one terrain reference atlas | Same visual language as request 07; use as reference and extraction guide |

The current Godot controller uses `TileSize = 64`, draws a 12x8 board, and tries
`art_terrain.png` before falling back to `terrain.png`. The starter tileset
should therefore preserve 64px tile cells and a predictable sheet order.

## Rule Terrain Mapping

The first three missions should reuse existing terrain rules. Visual variety
comes from tile variants, not new movement or defense rules.

| Rule terrain | Movement / defense role | Starter visual IDs |
| --- | --- | --- |
| Plain | Baseline movement and combat | `plain-dust-a`, `plain-dust-b`, `plain-seam-fleck` |
| Road | Fast exposed movement | `road-horizontal`, `road-vertical`, `road-corner`, `road-intersection` |
| Cover | Defense +1 | `cover-crates`, `cover-brush`, `cover-rock-low` |
| HQ | Static win/loss anchor | `hq-kestrel-prefab`, `hq-camp-pad` |
| Ridge | Movement friction / stronger cover if enabled | `ridge-basalt-a`, `ridge-basalt-b` |
| Property | Capture or objective anchor | `property-relay`, `property-fuel-cache`, `property-pump-station` |
| Objective object | Mission verb marker | `marker-scout7`, `marker-seam`, `marker-crawler`, `marker-relay-warning` |

If the core rules do not yet distinguish Ridge or Property from existing terrain
types, keep the art IDs anyway and bind them to existing Plain, Cover, HQ, or
objective-marker behavior until the rules catch up.

## Tile Sheet Order

Use a 6-column sheet with 64px cells. The request 07 local sheet already follows
this broad shape and can guide extraction.

| Index | Tile ID | Rule terrain | Mission use |
| --- | --- | --- | --- |
| 0 | `plain-dust-a` | Plain | All Missions 1-3 |
| 1 | `plain-dust-b` | Plain | All Missions 1-3, visual breakup |
| 2 | `plain-seam-fleck` | Plain | Mission 1 restricted seam and Mission 3 residue hints |
| 3 | `plain-cable-trench` | Plain | Mission 2 relay yard |
| 4 | `plain-coolant-stain` | Plain | Mission 3 pump approach |
| 5 | `plain-scrub-edge` | Plain | Mission 1 rescue lane and Mission 3 side gullies |
| 6 | `road-horizontal` | Road | Mission 1 east road, Mission 3 service road |
| 7 | `road-vertical` | Road | Mission 2 relay approach |
| 8 | `road-corner-ne` | Road | Missions 2-3 road bends |
| 9 | `road-corner-se` | Road | Missions 2-3 road bends |
| 10 | `road-t-junction` | Road | Mission 3 fork |
| 11 | `road-intersection` | Road | Mission 2 relay/cache split |
| 12 | `cover-crates-a` | Cover | Mission 1 camp cover |
| 13 | `cover-crates-b` | Cover | Mission 2 service sheds |
| 14 | `cover-brush` | Cover | Mission 1 southern flank |
| 15 | `cover-rock-low` | Cover | Mission 3 ambush breaks |
| 16 | `ridge-basalt-a` | Ridge or Cover | Mission 1 chokepoint ridge |
| 17 | `ridge-basalt-b` | Ridge or Cover | Mission 3 northeast ridge |
| 18 | `hq-kestrel-prefab` | HQ | Mission 1 and optional rear HQ maps |
| 19 | `hq-camp-pad` | HQ or Plain | HQ footprint extension |
| 20 | `property-relay` | Property | Mission 2 relay station |
| 21 | `property-fuel-cache` | Property | Mission 2 fuel depot |
| 22 | `property-pump-station` | Property | Mission 3 pump station |
| 23 | `property-service-shed` | Property or Cover | Mission 2 relay yard and Mission 3 pump flank |

## Mission-Specific Kits

### Mission 1: Scout-7 Is Late

Required starter visuals:

- Dust plain variants for the camp valley.
- East-west road tile and road-edge variants for the attack lane.
- Basalt ridge tiles that clearly read as defensive terrain.
- Camp HQ prefab and camp-pad support tile.
- Survey crates or scrub cover near the rescue route.
- Scout-7 marker, restricted seam marker, and jammed relay mast prop.

Missing or weak assets:

- `marker-scout7` needs a small stranded buggy or beacon silhouette readable
  under units and cursor overlays.
- `marker-seam` needs restrained teal flecks; it should not look like a magic
  objective pool.

### Mission 2: Inventory Adjustment

Required starter visuals:

- Road split and intersection tiles between relay and fuel cache.
- Relay station property tile with dish mast or antenna silhouette.
- Fuel cache property tile with orange or amber storage shapes.
- Cable trench plain variant for the relay yard.
- Service shed cover tile and capture warning marker.

Missing or weak assets:

- `property-relay` and `property-fuel-cache` need distinct silhouettes at 64px
  so the player can read the split objective without text.
- Capture-progress overlay should stay in UI icons, not be baked into terrain.

### Mission 3: Road To Pump Station Three

Required starter visuals:

- Long service-road horizontal and fork variants.
- Pump station property tile with coolant-blue and white-green industrial
  identity.
- Culvert or pipe-bridge prop for road pressure.
- Low rock and brush cover for ambush breaks.
- Crawler route marker, crawler wreck prop, and water pressure monitor prop.

Missing or weak assets:

- `property-pump-station` needs to read as a protected infrastructure endpoint,
  not a generic factory.
- `marker-crawler` should be visually separate from normal unit sprites so the
  escort target remains readable under selection and objective overlays.

## UX And Overlay Separation

Do not bake tactical state into terrain art. These should remain separate UI or
overlay assets:

- Cursor, movement range, attack range, capture progress, rescue marker, convoy
  route, relay warning, HQ danger, objective complete, and selected-unit state.
- Team ownership tint for HQs and properties.
- Fog, blackout, jammer, or power-state overlays for later missions.

Terrain art may include inactive landmarks such as dish masts, pump housings,
crates, or seam flecks, but the active state must remain deterministic and
inspectable.

## 1280x800 Readability Mock Plan

Build one static mock before promoting any replacement runtime sheet:

- Canvas: 1280x800.
- Board: 12x8 tiles at 64px, placed at the current prototype board origin.
- Mission 1 mock: HQ left, road lane east, ridge chokepoint, rescue marker
  southeast, at least five units and cursor overlay.
- Mission 2 mock: relay northeast, fuel cache east-center, split road, cover,
  capture progress overlay, and objective marker icons.
- Mission 3 mock: long road convoy lane, pump station east, two crawlers, fork
  ambush cover, Lancer and Hunter Bike silhouettes.

Review criteria:

- Terrain class is identifiable in less than two seconds.
- Units remain more visually important than terrain.
- Objective tiles remain readable with cursor, range, and ownership overlays.
- Teal Asterite accents are sparse and do not compete with selection colors.
- Roads form unambiguous movement paths at handheld distance.
- HQ, relay, fuel cache, and pump station are distinguishable without reading
  the HUD.

## Promotion Gate

Promote a terrain update to `game/WargamePrototype/assets/sprites/art_terrain.png`
only after:

- The sheet uses 64px cells and the agreed tile order.
- The first row of the sheet keeps existing fallback terrain indices stable or
  the renderer mapping is updated in the same change.
- The 1280x800 mock passes visual inspection.
- Godot loads the sheet without falling back to `terrain.png`.
- AI-vs-AI smoke tests still complete, proving the art update did not require
  rule changes.

## Art Handoff Gaps

Next art requests or local generation jobs should target one asset family per
job:

- Mission 1 objective props: Scout-7 buggy, restricted seam, jammed relay mast.
- Mission 2 properties: relay station, fuel cache, service shed, cable trench.
- Mission 3 properties: pump station, pipe bridge, crawler route marker,
  crawler wreck.
- Readability mock: one 1280x800 board composition using runtime tiles and
  current unit sprites.

Broad environment sheets are useful for reference, but runtime work should
prefer flat-background prop or tile jobs that can be extracted and reviewed at
64px.
