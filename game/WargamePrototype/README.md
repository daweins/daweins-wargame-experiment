---
title: Wargame Prototype
description: Local trial instructions for the first tactical combat mission
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: how-to
---

## Trial Playthrough

The prototype opens directly into the first mission: hold the HQ, rescue
Scout-7, defeat the raiders, and review the objective, speed, technique, power,
and total score.

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

Battle previews show expected HP loss before you commit. ATK raises outgoing
damage. DEF, cover, and HQ terrain reduce incoming damage. Higher current HP
also makes a unit hit harder.

After you end your turn, the log shows an enemy phase recap with red unit moves,
HP changes, and destroyed units.

Scout-7 starts stranded. Move any infantry or armor unit to a tile directly next
to Scout-7 to rescue them. The yellow ring marks this stranded state.

## Sprite Assets

The playable scene uses PNG sprite sheets under `assets/sprites`:

* `terrain.png` contains 64x64 tiles for plain, road, cover, HQ, and ridge.
* `units.png` contains 64x64 infantry, armor, and scout frames for player and
  enemy teams.

The renderer loads these sheets directly as Godot image textures and draws
texture regions with nearest filtering, so newly generated PNGs work in local
headless checks without waiting on editor import metadata.

Regenerate the current prototype sheets with:

```powershell
python .\scripts\assets\generate_prototype_sprites.py
```

## Current Prototype Scope

The trial slice includes a plain C# rules core, deterministic smoke checks, a
polished sprite-sheet tactical board, an expanded fixed-unit mission, movement
with terrain costs, direct attacks, counterattacks, seeded damage variance,
scout rescue, HQ defeat, enemy objective pressure, and mission scoring.

Production, campaign progression, fog of war, indirect fire, polished animation,
full replay UI, and Steam Deck deployment are intentionally out of scope for
this first trial.
