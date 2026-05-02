---
title: Agentic Decision Log
description: Public-safe decisions for the autonomous development system and game direction
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Decision Protocol

Record durable decisions that affect future autonomous work. Include context,
decision, alternatives, verification, and revisit triggers.

## Decisions

### D-2026-05-02-006: 16-bit pixel art visual style

Context: The user specified that the game graphics should use a 16-bit pixel art
style.

Decision: Use a 16-bit-era pixel art visual direction for maps, units, UI icons,
combat feedback, and small portraits. Preserve crisp pixels through
nearest-neighbor filtering, integer-friendly scaling, consistent tile and sprite
dimensions, and readable silhouettes at Steam Deck distance.

Alternatives: High-resolution painted art, vector art, 3D models, or modern
smooth 2D illustration.

Verification: Product docs, first prototype spec, technical direction, active
goal, backlog, metrics, and status reflect the pixel-art requirement.

Revisit trigger: The style fails to read clearly at 1280x800, slows content
iteration, or no longer fits the tone.

### D-2026-05-02-005: First prototype mission target

Context: The user completed a stepwise game-design interview and asked to stop
there because enough information existed to draft the product and prototype
specification.

Decision: The first playable prototype is a fixed-unit chokepoint HQ defense
mission. The player must prevent HQ capture, rescue or protect a stranded scout,
then defeat the remaining enemies. It should target six to eight turns by design
without a hard turn limit, include infantry, armor, and scout roles, show an
Advance Wars-style numeric score, and express personality through short
briefing, event banter, unit barks, debrief, and environmental text.

Alternatives: Capture-race prototype, commander-power showcase,
production-first prototype, fog/scout mission, or campaign reward prototype.

Verification: `docs/game/first-prototype-spec.md` records objectives, rules,
non-goals, scoring, personality, and acceptance checks.

Revisit trigger: The prototype fails to feel tense, readable, objective-driven,
or viable as a six to eight turn mission.

### D-2026-05-02-004: Godot C# prototype preference

Context: The user prefers C# if possible with Godot, and the existing technical
direction already recommends Godot 4.x as the leading engine.

Decision: Use Godot 4.x with C# for the first prototype if practical. Keep core
tactical rules in a plain C# simulation layer that remains testable without
rendering or editor-managed scene state.

Alternatives: Godot GDScript, MonoGame/FNA, Unity, or Unreal.

Verification: The first engine spike should demonstrate project structure,
test execution, replay-friendly rules boundaries, and controller-first input
viability.

Revisit trigger: Godot C# blocks fast iteration, deterministic tests, Linux
export, or Steam Deck controller workflow.

### D-2026-05-02-003: Initial tactical product target

Context: Product discovery compared Advance Wars with adjacent tactical games
and the user chose the initial design direction.

Decision: Build a near-future sci-fi tactical combat game closest to classic
Advance Wars, with grounded humor, 20 to 40 minute AI-only battles, CO powers,
static HQ capture stakes, terrain, light logistics and supply, light
within-mission veterancy, and minor seeded randomness. Do not pursue
multiplayer, a map editor, weather systems, or Fire Emblem-style named-unit
attachment in the initial direction.

Alternatives: Into the Breach-like puzzle tactics, tactical RPG with persistent
named units, Wesnoth-style probabilistic faction warfare, Daisenryaku-style
military simulation, or multiplayer/editor-first tactics.

Verification: Product goal, technical direction, active goal, backlog, feedback,
and metrics reflect the chosen direction.

Revisit trigger: The first playable slice feels too long, too random, too close
to Advance Wars without enough identity, or not comfortable on Steam Deck.

### D-2026-05-02-002: Autonomous by default with safety hard stops

Context: The user wants high-level goals to drive continual improvement and
human involvement to be non-blocking product judgment.

Decision: The development loop should choose, implement, verify, critique,
record, and continue autonomously within invocation limits. Human input should
shape goals and priorities without blocking routine progress.

Alternatives: Human approval between every phase or a fully unguarded loop.

Verification: Prompts, orchestrator instructions, and tracking artifacts encode
non-blocking human feedback plus explicit hard-stop conditions.

Revisit trigger: The loop repeatedly makes low-quality decisions or safety risk
increases.

### D-2026-05-02-001: Copilot-only active agentic stack

Context: The user wants to use GitHub Copilot because that is where available
tokens and workflow investment already exist.

Decision: Use GitHub Copilot, VS Code custom agents, prompt files, hooks,
GitHub branches, pull requests, GitHub Actions, and local scripts as the active
agentic stack.

Alternatives: External coding-agent services, model-provider APIs, or paid
agent runtimes.

Verification: Repository instructions and blueprint enforce the Copilot-only
constraint.

Revisit trigger: The user explicitly asks to adopt another runtime.
