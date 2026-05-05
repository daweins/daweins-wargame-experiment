---
title: Human Feedback Queue
description: Non-blocking human guidance and judgment for the autonomous loop
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## How To Use This File

Add guidance, product judgment, concerns, taste notes, changed goals, or nudges
here. The autonomous loop should read the newest feedback, incorporate it into
priority and critique, mark it consumed when reflected in work, and continue
without waiting for approval unless a hard-stop condition applies.

Status values: `new`, `considered`, `adopted`, `rejected`, `deferred`.

## Feedback

### H031: Tactical mission and campaign quality criteria

Date: 2026-05-03

Status: `adopted`

Guidance: Research level design for a tactical game with multiple missions over
a longer campaign. Increase strategy, fun, and variance. Create criteria for
identifying good missions, bad missions, good campaign progression, and bad
campaign progression.

Disposition: Added `docs/game/mission-campaign-design-rubric.md` with criteria
for mission tactical thesis, readable first decisions, objective pressure,
meaningful plan space, map topology, AI doctrine, fairness, scoring, campaign
lesson arcs, pacing rhythm, mechanic introduction budgets, unit-counter
longevity, story escalation, anti-patterns, automated evidence, manual review
questions, and promotion gates. Added follow-up tracking to apply the rubric to
Missions 1-10 through a variance matrix and deterministic playtest evidence.

### H024: Grounded interstellar travel mechanism

Date: 2026-05-02

Status: `adopted`

Guidance: Develop a sci-fi grounded interstellar travel mechanism that supports
the campaign politics, then update timelines and world flavor accordingly.

Disposition: Added the Transit Thread and Spindle Net to
`docs/game/universe-backstory.md`: scheduled beam corridors, fusion pushers,
magsail braking fields, depots, manifests, fixed low-bandwidth FTL messaging,
relay authentication, and transit permits. Updated the timeline, Caldera
flavor, Inner Systems, Transit Nodes, campaign plot spine, and product goal so
interstellar travel creates political leverage through freight slots, braking
rights, courier delays, message priority, sanctions, and logistics without FTL
travel.

### H023: Grounded Loom and universe backstory

Date: 2026-05-02

Status: `adopted`

Guidance: Sharpen the Loom because it is confusing, prefer realistic sci-fi,
use more politics than illogical macguffins, and start developing universe
backstory including history, political factions, worlds, and technologies.

Disposition: Added `docs/game/universe-backstory.md`, reframed Asterite as a
limited industrial material, reframed the Loom as field slang for the
human-built Basin Stabilization Grid, updated the campaign plot spine,
environment plan, character bible, product goal, active goal, and technical
direction to prioritize grounded political and industrial sci-fi.

### H022: Character backgrounds and commander identity

Date: 2026-05-02

Status: `adopted`

Guidance: Based on the drafted campaign, plot, and missions, start developing
detailed character backgrounds, personalities, motivations, arcs throughout the
game, interactions, catch phrases, game-appropriate play strategies, unique
units, and special powers.

Disposition: Added `docs/game/campaign-character-bible.md` with character
backgrounds, contradictions, campaign arcs, interaction hooks, voice examples,
commander doctrine, candidate CO powers, charge patterns, signature-unit
candidates, counterplay risks, Treaty Oversight Bureau and Loom voice guidance,
and validation gates that keep character mechanics deterministic,
forecast-visible, replayable, and optional until implemented.

### H021: Campaign environment planning

Date: 2026-05-02

Status: `adopted`

Guidance: Determine whether the campaign has environmental plans for each
stage, then come up with interesting flavor, terrain, looks, and feels for the
different environments.

Disposition: Added `docs/game/campaign-environment-plan.md` with reusable
environment kits, a compact terrain rule budget, detailed Missions 1-10
environment beats, five-mission environment arcs through Mission 50, tile
readability rules, mission-brief environment fields, and validation checks.

### H020: Combat feedback readability

Date: 2026-05-02

Status: `adopted`

Guidance: Think of ways to make combat more interesting and informative,
including small animations, visible sprite damage indicators, and HP damage
numerical animations.

Disposition: Adopted as the next combat presentation direction. Prioritize
short player-attack feedback first: floating HP loss numbers, HP bar and label
tweening, hit flash or recoil, HP-threshold damage overlays, and clearer
counterattack or terrain-defense callouts. Ordered enemy-phase playback,
critical or glancing result tags, destruction attribution, and replay stepping
should wait for structured combat events from the core instead of parsing text
messages or inventing order from snapshots.

### H018: First six mission unit ramp

Date: 2026-05-02

Status: `adopted`

Guidance: Brainstorm unit types sufficient to support the first six missions,
with one or two new unit types per mission during the ramp-up. Enemy and
friendly units should overlap but not be exact. Include rock-paper-scissors
triangles, support interactions, stats, descriptions, mission flavor, sprites,
and introduction order.

Disposition: Added a first-six-mission unit ramp document with a compact
nine-unit roster, direct-combat and support counter loops, mission-by-mission
introduction plan, stats, mission flavor, sprite plan, and implementation
order. Extended the sprite generator with a separate campaign unit planning
sheet.

### H019: Campaign plot spine

Date: 2026-05-02

Status: `adopted`

Guidance: Develop a detailed plot for the first 10 missions, starting with
small stakes and personal survival, then revealing more of the wider plot as
the campaign expands outward. Stakes should rise periodically, with short
bursts where the player solves problems or beats the current adversary. Sketch
the campaign in five-mission increments beyond that up to 50 missions.

Disposition: Added `docs/game/campaign-plot-spine.md` with the Kestrel Survey
Expedition premise, major factions, commander cast, Asterite mystery, detailed
Missions 1-10, and modular five-mission arcs through Mission 50.

### H030: Arena-first UX direction

Date: 2026-05-02

Status: `adopted`

Guidance: Preserve the verbose sidebar during initial development, but treat it
as outside the main game screen. Surface more of the decision-critical
information inside the graphical arena itself.

Disposition: Adopted as the next UX direction. The sidebar remains a verbose
development inspector, while future player-readability work should make the
arena and compact in-game HUD answer core tactical questions: objective state,
selected mode, legal movement, legal attacks, Scout-7 rescue state, terrain
value, combat forecast, readiness, and enemy pressure. Added a backlog item,
critique, experiment, and decision for an arena-first HUD/readability spike.

### H017: Periodic review and architecture constitution

Date: 2026-05-02

Status: `adopted`

Guidance: Add instructions that require periodic code review and autofix of
accepted suggestions. Research and write a code quality and architecture
constitution for this project.

Disposition: Added a project constitution for architecture boundaries, quality
gates, review triggers, suggestion classification, autofix rules, stop
conditions, and evidence standards. Wired the rules into repo-wide Copilot
instructions, game instructions, agentic workflow instructions, and a new
cross-cutting instruction file.

### H016: Board readability and road cleanup

Date: 2026-05-02

Status: `adopted`

Guidance: The running board is hard to read, and the road tiles look especially
bad in the first mission view.

Disposition: Simplified the generated terrain sheets by replacing the diagonal
road strip with a calmer full-tile dirt road, reducing plain and ridge texture
noise, and adding a subtle unit backing plate in the Godot renderer so units
separate more clearly from the terrain. Also wrapped the unit-shape legend text
that clipped in the side panel. Follow-up feedback says the result is looking
much better.

### H015: SNES and DS visual generation

Date: 2026-05-02

Status: `adopted`

Guidance: The first sprite sheets still read as more 8-bit than desired. Move
the style up a generation toward SNES or Game Boy DS, with richer pixel art.

Disposition: Added a repeatable sprite generator that emits 64x64 terrain and
unit sheets with stronger outlines, more shading, richer terrain texture, and
larger native unit frames. Updated the Godot renderer to use 64x64 sprite
regions. Follow-up feedback says this is much better.

### H014: Move to sprite assets

Date: 2026-05-02

Status: `adopted`

Guidance: The rectangle-rendered procedural art explains the visual quality
problem. Move the prototype to actual sprite assets.

Disposition: Added PNG sprite sheets for terrain and units under the Godot
project, then updated the renderer to draw sheet regions for map tiles and unit
frames while preserving HP bars, badges, highlights, and UI chrome.

### H013: Graphics overhaul request

Date: 2026-05-02

Status: `adopted`

Guidance: Do a graphics overhaul and make the prototype look nice.

Disposition: Reworked the immediate-mode Godot presentation with a framed
battlefield, richer terrain pixel art, cleaner movement and attack highlights,
stronger cursor treatment, polished unit bases and HP bars, improved HUD
section styling, and a more finished score panel.

### H012: Expanded first mission accepted

Date: 2026-05-02

Status: `adopted`

Guidance: The expanded mission is much better and works as a good tactical
first mission.

Disposition: Treat the expanded first mission as the current playable baseline.
Future tuning should preserve its tactical shape while improving presentation,
controller feel, screenshot validation, and later systems.

### H011: Expand scenario and sprites feedback

Date: 2026-05-02

Status: `adopted`

Guidance: The mission is beatable, but it feels very armor-focused. Expand the
scenario and improve the sprites.

Disposition: Added a second player infantry unit, expanded the enemy patrol to
five units, widened the road and cover layout, added an expanded-mission smoke
check, and improved infantry, armor, and scout sprite silhouettes.

### H010: Scenario impossible feedback

Date: 2026-05-02

Status: `adopted`

Guidance: Gameplay is working, but the scenario feels impossible. Show an
AI-vs-AI run where the player side wins.

Disposition: Added a deterministic AI-vs-AI first mission smoke scenario,
tuned prototype balance, and verified a printed AI replay that reaches player
victory.

### H009: Battle explanation and move undo feedback

Date: 2026-05-02

Status: `adopted`

Guidance: Battle rules are unclear. Sprites still do not differentiate units
enough. Movement/action differentiation is improved, but the player should be
able to back out of a move before choosing an action. ATK and DEF are unclear.

Disposition: Added action-mode move undo, stronger placeholder silhouettes,
plain-language ATK/DEF/terrain explanation, forecast explanation text, and
matching README guidance.

### H008: Unit strength and turn recap feedback

Date: 2026-05-02

Status: `adopted`

Guidance: Unit strength is hard to tell, and it is unclear what happened between
turns.

Disposition: Added board-level unit type and HP labels, cursor-panel combat
stats, an enemy phase recap for movement, HP changes, and destroyed units, and
matching README guidance.

### H007: Panel clipping and mode confusion

Date: 2026-05-02

Status: `adopted`

Guidance: Instruction text still scrolls or clips off the panel, and switching
between movement and action is unclear.

Disposition: Added manual word wrapping, a visible select/move/action mode
banner, contextual Enter/A instructions, lower log density, and matching README
mode guidance.

### H006: First mission readability feedback

Date: 2026-05-02

Status: `adopted`

Guidance: The first mission instructions were hard to read. Scout-7's stranded
state did not explain when it ended. Enemy and friendly units were hard to tell
apart. Unit graphics did not distinguish infantry, armor, and scout roles.
Ending the turn with `E` felt confusing because it could immediately produce a
loss and look like a restart.

Disposition: Updated the Godot prototype with clearer rescue instructions,
stronger team markers, distinct placeholder unit sprites, explicit end-turn
copy, defeat reason text, and a smoke-tested opening grace phase for Scout-7.

### H003: Initial tactical game product choices

Date: 2026-05-02

Status: `adopted`

Guidance: Make the game closest to classic Advance Wars in a near-future sci-fi
setting with grounded humor. Use minor randomness that remains mostly
skill-based, no persistent individual combat units, light within-mission
veterancy, possible broader campaign progression, CO powers, static HQ capture
stakes, terrain, light logistics and supply, no weather, no map editor, no
multiplayer, AI-only play with AI-vs-AI support for development, 20 to 40
minute missions, and Godot with C# if practical.

Disposition: Captured in the product goal, technical direction, active goal,
backlog, decision log, and metrics.

### H004: First prototype design interview

Date: 2026-05-02

Status: `adopted`

Guidance: Stop the interview and draft the first prototype spec. The prototype
should be a fixed-unit terrain chokepoint battle where the player holds a static
HQ, rescues or protects a stranded scout, defeats remaining enemies, and earns
an Advance Wars-style numeric score. The mission should be designed for six to
eight turns without a hard turn limit, use infantry, armor, and scout roles, and
show dry scientist and gallows humor through briefing, banter, barks, debrief,
and environmental text.

Disposition: Captured in `docs/game/first-prototype-spec.md` and reflected in
the backlog, state, decision log, and metrics.

### H005: Visual style direction

Date: 2026-05-02

Status: `adopted`

Guidance: Graphics should use a 16-bit pixel art style.

Disposition: Captured as a 16-bit-era pixel art direction in product docs,
technical direction, active goal, backlog, decision log, metrics, and status.

### H001: Autonomous development loop

Date: 2026-05-02

Status: `adopted`

Guidance: Make the development loop as autonomous as possible, with human input
as high-level non-blocking guidance rather than approval.

Disposition: Converted into active autonomy policy and prompt requirements.

### H002: Adversarial agents and experiments

Date: 2026-05-02

Status: `adopted`

Guidance: Add adversarial agents that evaluate, critique, recommend
experiments, and push continual improvement.

Disposition: Added adversarial and experiment roles, plus tracking files for
critiques and experiments.
