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

### D-2026-05-03-017: Reversible archive-only cruft cleanup

Context: The user asked for a periodic agent that looks for unused code and art
and moves it to an archive folder.

Decision: Use a dedicated Cruft Cleaner agent for periodic cleanup. The cleaner
archives only confirmed unused tracked files under `archive/cruft/<date>/`,
preserves each original relative path, and records evidence plus restore
commands in a manifest. It does not delete files and does not touch ignored
private folders, local generation output, runtime logs, credential stores, or
secret-like files.

Alternatives: Delete unused files directly, rely on ad hoc cleanup by the
orchestrator, or archive broad candidate sets without strong evidence.

Verification: The agent and periodic prompt define evidence checks, archive
layout, skip rules, and reporting requirements. The orchestrator now has a
Cruft Cleanup handoff and periodic hygiene step.

Revisit trigger: Archive folders become noisy, cleanup misses important stale
assets, or future CI adds a deterministic unused-code or asset-reference tool
that can replace part of the manual evidence process.

### D-2026-05-03-016: Mission quality gates before campaign expansion

Context: The first act now has playable campaign implementation and AI-vs-AI
campaign completion, but the user asked for deeper level-design criteria to
increase strategy, fun, and variance across a longer campaign.

Decision: Use `docs/game/mission-campaign-design-rubric.md` as the mission and
campaign quality gate before expanding content volume. A mission is not good
because it is implemented, completable, or well-briefed. It needs a tactical
thesis, objective pressure, meaningful plan space, terrain value, AI objective
pressure, fairness, scoring spread, deterministic replay evidence, and
1280x800 readability.

Alternatives: Continue adding missions from the campaign spine without a formal
quality rubric, or rely on AI campaign completion as the primary sign that
missions are healthy.

Verification: G048 is ready to apply the rubric to Missions 1-10 through a
variance matrix and quality review.

Revisit trigger: The rubric becomes too heavy for agent-sized mission work, or
playtest evidence shows different criteria better predict fun and strategic
variance.

### D-2026-05-02-015: Local ComfyUI for image candidate generation

Context: The user approved a source install for local pixel-art and sprite
candidate generation on a laptop GPU with enough VRAM for useful local batches.
The repository must remain public-safe and should avoid required hosted image
APIs or credentials unless explicitly approved.

Decision: Use a source-installed local ComfyUI runtime under ignored local
folders as the image-generation sidecar. Keep repo glue C#-first through
`Wargame.AssetTools`, with tracked prompt job specs and ignored candidate
output folders. Do not commit model files, generated raw candidate batches, or
credential-bearing service configuration.

Alternatives: Manual ChatGPT image handoff only, hosted image APIs, ComfyUI
portable install, or committing generated raw candidates directly as source
assets.

Verification: ComfyUI source, the Python virtual environment, CUDA PyTorch, and
ComfyUI requirements are installed locally. The C# asset tool builds, the new
`pixelart` command prints help, the sample spec parses, smoke tests pass, and
the secret-pattern scan reports no obvious secret patterns.

Revisit trigger: Local generation is too slow or unstable for overnight batch
work, model licensing blocks useful output, generated raw candidates need a
more formal curation workflow, or the pipeline requires credentialed services.

### D-2026-05-02-014: FTL messaging without FTL travel

Context: The universe needs enough interstellar connectivity to support
politics, sanctions, markets, remote orders, and information warfare while
preserving slow logistics, local command stakes, and no instant rescue.

Decision: Use the Transit Thread for slow physical travel and the Spindle Net
for constrained FTL messaging. Ships, cargo, soldiers, fuel, auditors, and
replacement parts still move through scheduled beam corridors, braking
reservations, and manifests. Messages can outrun ships only as low-bandwidth,
audited, fixed-station packets for orders, sanctions, legal seals, market
notices, emergency votes, and compressed evidence hashes.

Alternatives: No FTL at all, instant FTL travel, or free high-bandwidth ansible
communication available to any faction in the field.

Verification: `docs/game/universe-backstory.md`,
`docs/game/campaign-plot-spine.md`, and `docs/game/product-goal.md` now
distinguish instant authority from slow material help.

Revisit trigger: Mission writing uses Spindle messages as live tactical remote
control, instant reinforcements, unlimited video/data transfer, or a way around
freight, permit, custody, and local command constraints.

### D-2026-05-02-013: Basin Stabilization Grid replaces mystical Loom

Context: The user asked to sharpen the Loom, preferring realistic sci-fi and
politics over confusing or illogical macguffins.

Decision: Treat Loom only as field slang for the Basin Stabilization Grid, a
human-built industrial-control network with damaged authorization records,
obsolete emergency logic, and public-safety stakes. Asterite is a constrained
industrial material, not magic fuel.

Alternatives: Keep the Loom as a mysterious buried machine, make it alien or
sentient, or make Asterite an all-purpose plot resource.

Verification: The universe backstory, plot spine, environment plan, character
bible, product goal, active goal, and technical direction now point toward
infrastructure authority, liability, transit logistics, and public governance.

Revisit trigger: Later drafts make the grid behave like a character, erase its
human legal history, or let Asterite solve supply, energy, or travel limits by
assertion.

### D-2026-05-02-012: Character canon before commander mechanics

Context: The user asked to start developing detailed character backgrounds,
personalities, motivations, game-long arcs, interactions, catch phrases,
game-appropriate play strategies, unique units, and special powers based on the
drafted campaign, plot, and missions.

Decision: Use `docs/game/campaign-character-bible.md` as the current cast and
commander-identity direction. Treat backgrounds, motivations, contradictions,
relationships, voice patterns, and campaign arcs as narrative canon. Treat CO
powers, charge patterns, signature units, and special rules as candidate
mechanics until a separate rule budget proves they are deterministic,
forecast-visible, replayable, AI-fair, and appropriate for the mission ramp.

Alternatives: Leave characters at the one-line campaign-spine level, fully lock
all CO powers and unique units now, or make every principal character a playable
commander immediately.

Verification: The character bible defines maturity labels, global character
rules, detailed cast profiles, Bureau and Loom voice guidance, relationship
matrix, validation checklist, first implementation candidates, and rejected
early shortcuts.

Revisit trigger: Playable missions show the cast does not support distinct
mission feel, candidate powers duplicate existing unit or terrain mechanics,
or power UI/replay requirements exceed the early mission learning budget.

### D-2026-05-02-011: Reusable campaign environment kits

Context: The user asked whether the game has environmental plans for each stage
and requested interesting flavor, terrain, looks, and feels for different
environments.

Decision: Use `docs/game/campaign-environment-plan.md` as the current campaign
environment direction. Build environmental variety from 6-8 reusable kits,
landmarks, palette, faction occupation, objective props, and map topology while
keeping the terrain rule budget small and deterministic.

Alternatives: Create a bespoke tileset or terrain mechanic for every mission,
or keep every environment as a shallow palette swap.

Verification: The environment plan defines reusable kits, Missions 1-10 stage
flavor, five-mission arcs through Mission 50, tile readability rules, and a
starter tileset follow-up for Missions 1-3.

Revisit trigger: Environment art fails 1280x800 readability checks, later
mission briefs require too many new mechanics, or players cannot distinguish
terrain rule identity without the sidebar.

### D-2026-05-02-008: Direct-combat first unit ramp

Context: The user asked for a unit roster and introduction order for the first
six missions, including overlapping friendly and enemy units, counter triangles,
support interactions, mission flavor, stats, and sprites.

Decision: Use a compact nine-unit ramp built on the current direct-combat core:
Field Tech, Utility Armor, Survey Scout, Engineer, Sapper, AT Lancer, Striker,
Field Rig, and Siege Breaker. Introduce at most one new player-commanded unit
per mission after Mission 1, with a second unit usually appearing as an enemy
variant or objective-bound threat. Defer true indirect fire, fog, jamming,
hover, and EMP systems until separate rule, UI, AI, and replay slices exist.

Alternatives: Add a broader Advance Wars-style roster immediately, introduce
artillery and fog by Mission 2, or create exact mirrored faction unit sets.

Verification: `docs/game/first-six-mission-unit-ramp.md` records the roster,
stats, counter loops, mission plan, flavor, sprite plan, and implementation
order. `campaign_units.png` provides a first generated 64x64 silhouette sheet.

Revisit trigger: The roster fails combat-matrix checks, support units prove too
opaque in play, or mission ramp feedback shows too many new nouns too quickly.

### D-2026-05-02-009: Modular campaign spine

Context: The user asked for a detailed first 10 missions and five-mission
increments beyond that up to 50 missions, with small personal stakes first and
periodic escalation as the campaign expands outward.

Decision: Use `docs/game/campaign-plot-spine.md` as the current campaign story
spine. Missions 1-10 are concrete enough to guide near-term prototyping.
Missions 11-50 are intentionally modular five-mission arcs, not locked content
commitments, so future playable evidence can reshape them.

Alternatives: Leave campaign structure implicit until more mechanics exist, or
fully lock all 50 missions in detail immediately.

Verification: The campaign spine defines factions, commanders, the Asterite
mystery, detailed first-act mission beats, and clean victory points at regular
intervals.

Revisit trigger: Playable missions show the faction order, tactical mechanic
cadence, or rare-material mystery does not support fun 20 to 40 minute battles.

### D-2026-05-02-010: Sidebar as development inspector

Context: The current Godot prototype uses a verbose right sidebar for objective
copy, mode instructions, controls, legend, inspect data, forecasts, combat math,
and log messages. The user wants to keep that sidebar during initial
development, but treat it as outside the main game screen while more
decision-critical information moves into the graphical arena.

Decision: Treat the verbose sidebar as a development inspector, not the primary
player HUD. Future UX work should judge the shippable battle screen by the
arena plus compact in-game HUD. The arena should communicate spatial and
immediate tactical facts such as objective beacons, Scout-7 rescue state, legal
movement, legal attacks, readiness, terrain value, enemy pressure, and compact
attack/counter forecasts. Full prose, formulas, and logs may remain in the
sidebar during development.

Alternatives: Keep the sidebar as the primary HUD, hide the sidebar immediately
before equivalent arena cues exist, or move all sidebar content onto the board.

Verification: A sidebar-covered readability test should show whether the player
can understand the first mission at 1280x800 without relying on the inspector.

Revisit trigger: Arena overlays make the 64px tiles cluttered, compact HUD text
fails at Steam Deck distance, or playtests still require frequent sidebar peeks.

### D-2026-05-02-007: Review and autofix constitution

Context: The user asked for instructions that require periodic code review,
autofix of accepted suggestions, and a researched code quality and architecture
constitution for the tactical combat project.

Decision: Use a risk-triggered review model instead of calendar-only review.
Run reviews after meaningful slices, three to five autonomous slices, boundary
changes, dependency or security changes, generated artifact changes, failed
checks, pre-commit work, and pull request prep. Autofix accepted suggestions
when they are bounded, local, public-safe, and verifiable. Require independent
acceptance for semantic gameplay, replay, save schema, dependency, security, and
architecture changes.

Alternatives: Review on every turn regardless of risk, defer review to pull
requests only, or allow the implementing agent to automatically apply all review
suggestions.

Verification: Repo instructions, game instructions, agentic workflow
instructions, and `docs/game/code-quality-architecture-constitution.md` now
record the policy.

Revisit trigger: Review passes create churn without finding meaningful defects,
or semantic autofixes repeatedly change behavior without enough evidence.

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
