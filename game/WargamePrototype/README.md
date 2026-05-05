---
title: Wargame Prototype
description: Local trial instructions for the first tactical combat mission
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: how-to
---

## Trial Playthrough

The prototype now opens on the Mission 1 cutscene concept image, then starts
the first mission: hold the HQ, rescue Scout-7, defeat the raiders, and review
the objective, speed, technique, power, and total score.

After a Mission 1 victory, press Enter/A to advance to the Mission 2 relay-yard
cutscene concept image. Press Enter/A again to start Mission 2, where Tech or
Engineer units must wait on the relay and fuel cache objectives for two turns
each before clearing the remaining Orison units.

Build the Godot C# project:

```powershell
dotnet build .\game\WargamePrototype\WargamePrototype.sln
```

Run it with the Godot executable installed by winget:

```powershell
$godot = Join-Path $env:LOCALAPPDATA "Microsoft\WinGet\Packages\GodotEngine.GodotEngine.Mono_Microsoft.Winget.Source_8wekyb3d8bbwe\Godot_v4.6.2-stable_mono_win64\Godot_v4.6.2-stable_mono_win64_console.exe"
& $godot --path .\game\WargamePrototype
```

Run the deterministic AI-vs-AI proof replay:

```powershell
dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj
```

The smoke runner prints a first-mission AI replay that reaches player victory.

## Controls

* Arrow keys or D-pad move the cursor.
* Enter or gamepad confirm changes meaning by mode.
* Escape or gamepad cancel clears the current selection.
* `E` or Start ends the player turn and lets every red unit act once.
* Tab or right shoulder cycles ready units.
* `R` restarts the mission.

On cutscene screens, Enter/A advances into the next playable mission.

The prototype uses three explicit input modes:

* Select mode: move the cursor to a blue ready unit and press Enter/A.
* Move mode: blue highlighted tiles are legal moves. Press Enter/A on one to
  move the selected unit.
* Action mode: choose an adjacent red unit and press Enter/A to attack, or press
  Enter/A on the selected unit's current tile to wait.
* In action mode after moving, Escape/B undoes that move and returns to move
  mode. Escape/B again cancels the unit selection.

Each unit shows a type tag and current HP on the board. The cursor panel shows
HP, attack, defense, and movement for the unit under the cursor.

The arena now carries the main play HUD below the board, with objective, HQ,
Scout-7, cursor, terrain, selection, mode, forecast, and controller prompt
chips. The right sidebar remains available as a verbose development inspector.
Player units also show compact readiness badges: ready, action-only, done, or
stranded.

Battle previews show expected HP loss before you commit. ATK raises outgoing
damage. DEF, cover, and HQ terrain reduce incoming damage. Higher current HP
also makes a unit hit harder.

When a player attack resolves, the board shows the actual HP damage as floating
numbers, briefly flashes damaged units, drains their HP bars toward the new
authoritative HP, and marks heavily damaged units with small sprite overlays.

After you end your turn, the log shows an enemy phase recap with red unit moves,
HP changes, and destroyed units.

Scout-7 starts stranded. Move any infantry or armor unit to a tile directly next
to Scout-7 to rescue them. The yellow ring marks this stranded state.

## Sprite Assets

The playable scene uses PNG sprite sheets under `assets/sprites`:

* `art_terrain.png` contains extracted 64x64 returned-art tiles for plain,
  road, cover, HQ, ridge, and deterministic plain/road/cover/ridge variants.
* `art_units.png` contains extracted transparent returned-art unit sprites.
* `art_ui_icons.png` contains extracted transparent returned-art HUD icons.
* `terrain.png` contains 64x64 tiles for plain, road, cover, HQ, and ridge.
* `units.png` contains 64x64 infantry, armor, and scout frames for player and
  enemy teams.
* `campaign_units.png` expands the two-team sheet with Engineer, Sapper,
  Lancer, Striker, Field Rig, and Siege Breaker concept silhouettes.
* `ui_icons.png` contains deterministic 64x64 command and status icons inspired
  by the returned UI icon concept sheet.

Returned ChatGPT concept images are used selectively:

* Mission 1 and Mission 2 concept frames are loaded directly as full-screen
  cutscene screens.
* The commander portrait concept is loaded directly into cutscene dialogue
  panels.
* The returned terrain, unit, and UI icon sheets are extractor inputs. The
  extractor crops known source regions, keys out unit and icon backgrounds, and
  writes runtime atlases under `assets/sprites`.
* The deterministic generated terrain, unit, and UI icon sheets remain fallback
  assets and reproducible local-generation references.

The renderer loads these sheets directly as Godot image textures and draws
texture regions with nearest filtering, so both returned ChatGPT PNGs and newly
generated PNGs work in local headless checks without waiting on editor import
metadata.

Regenerate all assets (sprites and cutscenes) from the repository root with:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj
```

Or generate only sprites:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- sprites
```

Or re-extract returned source art into runtime atlases:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art
```

The extraction manifest is
`assets/art-handoff/extraction/source-art-extraction.json`. It defines source
PNGs, crop rectangles, corner-color transparency for sprites and icons, and
rotated or mirrored tile variants.

Or generate cutscenes:

```powershell
dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- cutscenes
```

The authoring format for reusable cutscene specs is documented in
`docs/game/cutscene-graphics-format.md`.

## Current Prototype Scope

The trial slice includes a plain C# rules core, deterministic smoke checks, a
polished sprite-sheet tactical board, a two-mission campaign spine, cutscene
handoff screens using returned concept art, a portrait-backed dialogue panel,
extracted returned-art terrain, transparent unit, and transparent HUD icon
atlases with deterministic generated fallback sheets, movement with terrain
costs, direct attacks, counterattacks, seeded damage variance, scout rescue,
two-turn Mission 2 relay and fuel objectives, HQ defeat, enemy objective
pressure, and mission scoring.

Production, fog of war, indirect fire, polished animation, full replay UI,
full Engineer repair/Stabilize actions, full Sapper property sabotage, and Steam
Deck deployment remain intentionally out of scope for this first campaign-flow
trial.
