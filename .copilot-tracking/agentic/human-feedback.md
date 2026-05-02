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
