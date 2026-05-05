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

### Periodic cruft cleanup

Current value: Defined

Target: Unused tracked code, art, prompt specs, and docs can be archived
periodically without deleting files or touching ignored private/local output

Evidence: `.github/agents/cruft-cleaner.agent.md` defines evidence-first,
archive-only cleanup with explicit skip rules. `.github/prompts/cruft-cleaner-periodic.prompt.md`
provides a periodic entry point. `archive/cruft/README.md` defines dated archive
layout, manifests, restore commands, and safety boundaries. The Strategic
Orchestrator has a Cruft Cleanup handoff and periodic hygiene step.

### Mission design quality rubric

Current value: Defined

Target: Tactical missions and long-campaign progression are evaluated for
strategy, fun, variance, fairness, replayability, scoring quality, and Steam
Deck readability before content volume expands

Evidence: `docs/game/mission-campaign-design-rubric.md` defines criteria for
good missions, bad mission anti-patterns, good and bad campaign progression,
automated evidence, manual review questions, and promotion gates. G048 is ready
to apply the rubric to Missions 1-10.

### Grounded universe definition

Current value: Defined

Target: The setting supports realistic political sci-fi with constrained rare
materials, public infrastructure, slow physical travel, limited FTL messaging,
and concrete faction incentives

Evidence: `docs/game/universe-backstory.md` defines Asterite as a limited
industrial material, the Loom as field slang for the Basin Stabilization Grid,
Transit Thread travel as scheduled infrastructure rather than FTL travel, and
Spindle Net messaging as fixed, low-bandwidth, audited FTL packets that cannot
move mass or live tactical data. `docs/game/campaign-plot-spine.md` and
`docs/game/product-goal.md` now capture the split between instant authority and
slow material help.

### Commander identity definition

Current value: Character bible defined, mechanics not implemented

Target: Commanders create distinct tactical personality through doctrine,
briefings, interactions, voice, and eventually deterministic CO powers that do
not overwhelm unit counters or replay clarity

Evidence: `docs/game/campaign-character-bible.md` defines detailed profiles for
the principal cast, relationship hooks, voice examples, commander doctrine,
candidate powers, candidate charge sources, signature-unit candidates,
counterplay risks, Bureau and Loom voice guidance, validation gates, and first
implementation candidates. Powers and signature units are explicitly candidates
pending a CO power rule budget.

### Combat feedback readability

Current value: First presentation slice implemented

Target: Attacks, counterattacks, damage, terrain defense, and unit destruction
are understandable from board feedback without relying only on the text log

Evidence: Player-initiated attacks now produce presentation-only feedback from
actual HP deltas: floating damage numbers, HP bar tweening with immediate
authoritative HP labels, hit flash and recoil, low-HP damage overlays, compact
attack and return-fire chips, and KO labels for destroyed targets. `dotnet
build` for the Godot project, the deterministic smoke suite, Godot startup, and
diagnostics pass. Ordered enemy-phase playback still waits for structured core
events.

### Arena-first UX readiness

Current value: First compact arena HUD slice implemented

Target: The first mission can be understood and played with the verbose sidebar
treated as a development inspector outside the main game screen

Evidence: Human feedback now directs the project to preserve the verbose
sidebar during initial development while surfacing more decision-critical
information in the graphical arena. `BattleController.cs` now adds HQ and
Scout-7 state, rescue-zone markers, terrain defense pips, player readiness
badges, cursor and terrain chips, selected-unit and mode chips, compact
attack/counter forecast chips, and controller prompts below the board. VS Code
diagnostics, deterministic smoke checks, Godot C# build, and Godot startup
pass. Sidebar-covered 1280x800 visual validation is still pending.

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
Follow-up readability feedback drove a calmer road tile, quieter terrain
texture, softer grid lines, unit backing plates, and a wrapped side-panel
legend. The user inspected the result in the running prototype and said it is
looking much better.

### First prototype definition

Current value: Defined

Target: First implementation slice follows the fixed-unit chokepoint HQ defense
spec

Evidence: `docs/game/first-prototype-spec.md` records the mission objective,
unit roles, terrain focus, scoring model, tone, non-goals, and acceptance
checks.

### Campaign spine definition

Current value: Defined

Target: Campaign story direction supports detailed Act 1 mission planning and a
modular 50-mission long arc

Evidence: `docs/game/campaign-plot-spine.md` defines Kestrel, Orison, Sable,
Meridian, the Loom, the Asterite mystery, detailed Missions 1-10, and
five-mission campaign arcs through Mission 50.

### Campaign environment definition

Current value: Defined

Target: Campaign stages have distinct environmental identity without exploding
terrain rules or tileset scope

Evidence: `docs/game/campaign-environment-plan.md` defines reusable environment
kits, terrain rule budget, detailed Missions 1-10 environment beats,
five-mission environment arcs through Mission 50, tile readability rules,
mission-brief environment fields, and validation checks.

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

### Review and architecture governance

Current value: Defined

Target: Periodic review and autofix rules are explicit before broader systems
work

Evidence: The repo now has `docs/game/code-quality-architecture-constitution.md`
and `.github/instructions/code-quality-architecture.instructions.md`. The
repo-wide, agentic workflow, and game-development instructions require
risk-triggered code review, suggestion classification, bounded autofix, and
independent acceptance for semantic or safety-sensitive changes.

### First six mission unit ramp

Current value: Defined

Target: Mission 1 through Mission 6 unit introductions are scoped before broad
campaign systems expand

Evidence: `docs/game/first-six-mission-unit-ramp.md` defines a nine-unit roster,
stats, direct-combat and support counter loops, mission introduction order,
flavor, sprite requirements, implementation order, and balance risks. The
sprite generator emits `campaign_units.png` as a separate 64x64 planning sheet.

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
