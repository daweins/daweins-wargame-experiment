---
title: Agentic Backlog
description: Prioritized repo-based backlog for autonomous development work
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Queue Rules

Backlog items should be small enough for one autonomous pass when possible.
Each item needs a clear outcome, verification idea, and current status.

Status values: `proposed`, `ready`, `active`, `blocked`, `done`, `dropped`.

## Ready

No ready items are currently queued ahead of another manual playtest pass.

## Done

### G018: SNES and DS style sprite upgrade

Completed: 2026-05-02

Evidence: A repeatable sprite generator now emits 64x64 terrain and unit sheets
with richer shading, outlines, silhouettes, and texture. `BattleController.cs`
uses 64x64 source regions and draws unit frames at native tile scale. Manual
feedback says the updated graphics are much better.

### G017: Sprite sheet asset migration

Completed: 2026-05-02

Evidence: `game/WargamePrototype/assets/sprites` now contains PNG sheets for
terrain and units. `BattleController.cs` loads those sheets as image textures
and draws texture regions for board tiles and unit frames. Godot build and
headless startup checks pass.

### G008: Pixel-art readability spike

Completed: 2026-05-02

Evidence: The Godot scene now uses a framed battlefield, richer pixel-patterned
terrain, polished unit bases and HP bars, clearer highlights, a stronger cursor,
and a more finished right-side HUD. Build and startup checks pass.

### G016: Expanded first mission roles and sprites

Completed: 2026-05-02

Evidence: The first mission now includes two player infantry units, one armor,
Scout-7, and five enemy units. Smoke checks verify the expanded roster and AI
victory. Godot sprites have clearer infantry, armor, and scout silhouettes.

### G013: AI-vs-AI smoke test

Completed: 2026-05-02

Evidence: `src/Wargame.SmokeTests` prints a deterministic AI replay through the
same command interface used by the playable prototype and verifies player-side
victory.

### G015: First mission playtest tuning

Completed: 2026-05-02

Evidence: First playtest feedback drove clearer rescue instructions, stronger
enemy/friendly differentiation, distinct infantry/armor/scout placeholder
sprites, explicit end-turn copy, and a smoke-tested opening grace phase so
ending turn immediately does not defeat the player.

### G009: Numeric score fixture

Completed: 2026-05-02

Evidence: `src/Wargame.Core` calculates objective, speed, technique, power, and
total score. `src/Wargame.SmokeTests` verifies score categories.

### G007: Objective AI pressure fixture

Completed: 2026-05-02

Evidence: Enemy AI advances toward mission objectives and the smoke test verifies
that enemy distance to the player HQ decreases after an enemy phase.

### G006: HQ and scout objective fixture

Completed: 2026-05-02

Evidence: `src/Wargame.Core` resolves static HQ defeat and scout rescue state.
`src/Wargame.SmokeTests` verifies both outcomes.

### G005: Combat forecast fixture

Completed: 2026-05-02

Evidence: Combat forecasts include seeded variance ranges and terrain defense.
Smoke checks verify that cover changes expected damage.

### G004: Movement and terrain fixture

Completed: 2026-05-02

Evidence: Movement range uses terrain costs, ridges, roads, and occupied tiles.
Smoke checks verify chokepoint movement and blockers.

### G003: Deterministic board model

Completed: 2026-05-02

Evidence: `src/Wargame.Core` contains deterministic board, terrain, unit, team,
state, command, scoring, and state-hash models.

### G002: Godot C# engine spike

Completed: 2026-05-02

Outcome: Minimal Godot 4.x C# project shape is selected and kept compatible with
a plain C# simulation core.

Evidence: `game/WargamePrototype` builds as a Godot 4.6 C# project referencing a
plain C# rules core. Godot headless startup succeeds.

## Proposed

### G010: Capture economy fixture

Status: `proposed`

Outcome: Infantry can capture a property and update per-turn income.

Verification: Capture and income tests pass.

### G011: Light supply fixture

Status: `proposed`

Outcome: One ammo, fuel, or resupply rule affects legal actions and forecasts.

Verification: Supply tests and replay fixture pass.

### G012: CO power scaffold

Status: `proposed`

Outcome: One CO power hook can influence a battle without requiring mobile
leaders on the board.

Verification: CO charge and power effect tests pass.

### G014: Replay command log

Status: `proposed`

Outcome: Initial command stream format is defined.

Verification: Replay fixture reproduces expected state.

### D001: Sanitized Deck workflow

Status: `proposed`

Outcome: Local Deck deploy config schema is documented without values.

Verification: Security review confirms no private details.

## Blocked

No blocked items yet.

## Completed Foundation

### G001: Product goal and prototype scope

Completed: 2026-05-02

Evidence: Product goal captured as near-future sci-fi classic Advance Wars-style
army tactics with Godot C#, AI-only play, CO powers, static HQ capture stakes,
terrain, light logistics, minor seeded randomness, and no map editor or
multiplayer.

### A001: Autonomous tracking scaffold

Completed: 2026-05-02

Evidence: Repo work tracking files, autonomous prompt, adversarial roles, and
updated orchestrator instructions exist. Diagnostics and repo secret scan pass.
