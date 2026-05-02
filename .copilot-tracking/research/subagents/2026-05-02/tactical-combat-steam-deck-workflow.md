---
title: Tactical Combat Game and Steam Deck Workflow Research
description: Public research notes on engine choices, Steam Deck deployment, controller UX, and deterministic tactics development
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: research
---

## Research Questions

* Which engine and workflow best fit an Advance Wars-like tactical game?
* How should the project support laptop development and Steam Deck play without
  Steam Store publishing?
* What development practices help autonomous agents build the game safely?

## Findings

### Engine choice

Godot 4.x is the best first engine candidate. It has strong 2D tooling,
tilemaps, input actions, Linux export, controller support, text-friendly assets,
and fast iteration. The main risk is test ergonomics for deep rules logic, so
the simulation core should stay isolated from rendering.

MonoGame or FNA is the best backup if the project prefers a code-first C# stack
with conventional tests and fewer editor-managed assets. Unity is capable but
heavier and carries licensing and editor complexity. Unreal is overbuilt for a
2D tactics prototype. Bevy is appealing for Rust and ECS but has more editor and
UI workflow risk. Love2D is excellent for prototypes but less attractive for a
larger deterministic tactics codebase.

### Steam Deck constraints

Design for 1280x800 and 16:10 first, then validate 1280x720 and 1920x1080.
Input should be gamepad-first: tile cursor, confirm, cancel, unit cycling,
panels, end turn, objectives, and menus should all be reachable without a
mouse. Text needs to be readable at handheld distance.

### Deployment without Steam Store

The likely workflow is native Linux x86_64 export, local transfer, and adding
the executable as a non-Steam game in Game Mode. Transfer can use SSH, rsync,
Syncthing, manual copy, or later an itch.io private channel. Private device
details must stay in ignored local configuration or an OS credential store.

### Deterministic simulation

The core game should use a command log: move, attack, capture, produce, wait,
end turn, and similar actions. Replays should contain initial state, rules
version, optional seed, and command stream. State hashes after each command or
turn make cross-platform nondeterminism visible.

### Agentic development implications

The repo should separate concerns early:

* `combat-core/` for deterministic rules and tests
* `game/` for engine presentation, input, audio, and UI
* `tools/` for map import, replay running, validation, and deployment
* `fixtures/` for maps, saves, and golden replays

Agents should work in small slices that produce objective checks: one movement
rule, one combat forecast, one replay fixture, one input action set, one Deck
resolution check.

## Sources

* Steam Deck technical specifications: <https://www.steamdeck.com/en/tech>
* Valve Steam Deck load games docs: <https://partner.steamgames.com/doc/steamdeck/loadgames>
* Valve Proton and Steam Deck docs: <https://partner.steamgames.com/doc/steamdeck/proton>
* Godot multiple resolutions: <https://docs.godotengine.org/en/stable/tutorials/rendering/multiple_resolutions.html>
* Godot command line: <https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html>
* Godot Linux export: <https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_linux.html>
* Godot controllers: <https://docs.godotengine.org/en/stable/tutorials/inputs/controllers_gamepads_joysticks.html>
* Godot saving games: <https://docs.godotengine.org/en/stable/tutorials/io/saving_games.html>
* Unity gamepad input: <https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/Gamepad.html>
* Unity Test Framework: <https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/index.html>
* Unreal Enhanced Input: <https://dev.epicgames.com/documentation/en-us/unreal-engine/enhanced-input-in-unreal-engine>
* Unreal Linux development: <https://dev.epicgames.com/documentation/en-us/unreal-engine/linux-development-quickstart-for-unreal-engine>
* MonoGame getting started: <https://docs.monogame.net/articles/getting_started/index.html>
* MonoGame gamepad API: <https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.GamePad.html>
* FNA docs: <https://fna-xna.github.io/docs/>
* SDL game controller docs: <https://wiki.libsdl.org/SDL2/CategoryGameController>
* Bevy quick start: <https://bevy.org/learn/quick-start/getting-started/>
* Bevy gamepad input: <https://docs.rs/bevy/latest/bevy/input/gamepad/index.html>
* Love2D repository: <https://github.com/love2d/love>
* Tiled map editor: <https://doc.mapeditor.org/en/stable/manual/introduction/>
* LDtk docs: <https://ldtk.io/docs/>
* Red Blob A star introduction: <https://www.redblobgames.com/pathfinding/a-star/introduction.html>
* Game Programming Patterns command pattern: <https://gameprogrammingpatterns.com/command.html>
* rsync manual: <https://download.samba.org/pub/rsync/rsync.1>
* Syncthing getting started: <https://docs.syncthing.net/intro/getting-started.html>
* itch.io butler docs: <https://itch.io/docs/butler/>

## Recommendations

* Prototype with Godot 4.x unless a spike disproves the workflow.
* Keep the tactical rules engine-light and deterministic.
* Design for 1280x800 and gamepad navigation from the first UI slice.
* Add replay fixtures and state hashes before adding lots of content.
* Use ignored local configuration for any Steam Deck deployment details.
