---
title: Agentic Metrics
description: Product, process, quality, and safety signals for autonomous development
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Metrics Protocol

Track lightweight signals that help the autonomous loop choose useful work and
notice false progress. Update metrics when evidence changes.

## Current Signals

### Product goal definition

Current value: Defined

Target: First playable slice follows the accepted product target

Evidence: Near-future sci-fi classic Advance Wars-style direction recorded with
Godot C#, AI-only play, CO powers, static HQ capture stakes, terrain, light
logistics, 16-bit-era pixel art, minor seeded randomness, and explicit
non-goals.

### Visual style definition

Current value: Defined

Target: First Godot slice renders crisp 16-bit pixel art at 1280x800

Evidence: Product docs and technical direction specify 16-bit-era pixel art.
The Godot prototype now renders a framed board with PNG terrain and unit sprite
sheets, polished unit bases and HP bars, clearer highlights, a stronger cursor,
and a more finished HUD. The current sheets are 64x64 generated assets with
richer SNES and DS-style shading, outlines, texture, and unit silhouettes.

### First prototype definition

Current value: Defined

Target: First implementation slice follows the fixed-unit chokepoint HQ defense
spec

Evidence: `docs/game/first-prototype-spec.md` records the mission objective,
unit roles, terrain focus, scoring model, tone, non-goals, and acceptance
checks.

### Godot C# stack

Current value: Godot .NET installed, project not scaffolded

Target: Minimal Godot C# project and testable C# simulation core exist

Evidence: Godot Engine .NET package installed through winget and verified as
`4.6.2.stable.mono.official.71f334935`. Technical direction and decision log
record the Godot C# preference.

### Public-safety scan

Current value: Passing

Target: Passing before commit or publish

Evidence: Repo scan reported no obvious secret patterns.

### Markdown diagnostics

Current value: Passing

Target: Passing after docs or prompt edits

Evidence: VS Code diagnostics reported no errors after tracking and prompt
updates.

### Active game implementation

Current value: First playable Godot C# prototype implemented locally

Target: First playable slice exists

Evidence: `game/WargamePrototype` contains a launchable Godot C# scene backed by
the `src/Wargame.Core` rules model, PNG sprite sheets for terrain and units,
and a polished 16-bit-style presentation pass. The sprite sheets are generated
by `scripts/assets/generate_prototype_sprites.py` so visual iterations are
repeatable.

### Deterministic tests

Current value: Smoke checks passing

Target: First C# rules test exists

Evidence: `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
passes movement, terrain, expanded roster, scout rescue, HQ defeat, replay hash,
AI pressure, AI player victory, and scoring checks.

### Replay fixture coverage

Current value: Not started

Target: First replay fixture exists

Evidence: No command log yet.

### AI-vs-AI validation

Current value: Passing

Target: Full AI-vs-AI smoke test produces deterministic replay

Evidence: The smoke runner prints a deterministic first-mission AI replay and
verifies player-side victory on the expanded scenario.

### Steam Deck workflow

Current value: Not started

Target: Sanitized local config schema exists

Evidence: Direction documented only.

### Autonomous continuity

Current value: Scaffolded

Target: Next pass resumes from tracked state

Evidence: Work tracking files, autonomous prompt, critique agents, and
experiment tracking are in place.
