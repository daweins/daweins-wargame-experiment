---
title: Agentic Experiment Queue
description: Experiments for improving the product and the autonomous development loop
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Experiment Protocol

Experiments should test assumptions with observable evidence. Prefer small
changes that can be evaluated by deterministic checks, replay fixtures,
screenshots, playtest notes, or measurable workflow outcomes.

Status values: `proposed`, `active`, `complete`, `inconclusive`, `dropped`.

## Proposed Experiments

### E012: Mission quality fingerprint

Status: `proposed`

Hypothesis: Good missions in the first act are not only completable; they
produce distinct tactical signatures that match their campaign roles. Bad
missions either stall, collapse into one obvious route, ignore their stated
objective, or look too similar to neighboring missions in logs and manual
review.

Method: Build a Missions 1-10 variance matrix from deterministic playtest
summaries and document review. Track each mission's objective verb, first
objective completed, turn count, score category shape, losses, command count,
objective flags, pressure clock, map topology, enemy doctrine, and readability
risk.

Evidence: Red/yellow/green findings identify whether each mission has a clear
tactical thesis, objective pressure, at least two credible plans, meaningful
terrain, distinct AI pressure, fair failure language, score spread, and
1280x800 readability.

### E011: Grounded lore brief fields

Status: `proposed`

Hypothesis: The campaign can use Asterite, grid control, slow travel, and FTL
messaging without becoming a macguffin story if every mission brief translates
them into concrete stakeholders, constraints, and map verbs.

Method: Add grounded-lore fields to the next Missions 1-3 playable brief pass:
Transit delay, Spindle packet or silence, cargo or permit constraint, legal
stakeholder, civilian infrastructure risk, and tactical objective verb.

Evidence: Each brief names who sent or blocked information, what physical help
cannot arrive, what local asset is at risk, and what the player does on the map.

### E010: Commander identity rule budget

Status: `proposed`

Hypothesis: Commander identity can add personality and tactical variety without
overloading early missions if powers are first specified as small deterministic
commands with forecast-visible effects and no-new-unit fallbacks.

Method: Draft a CO power rule budget for Venn, Rusk, Holt, and Sloane. For each
candidate, define charge source, activation command, duration, affected tags,
forecast delta, AI usage rule, replay data, counterplay, first safe mission,
and fallback if the special unit is cut.

Evidence: At least one candidate fits a compact inspect panel, can be encoded as
deterministic command data, and does not compete with Missions 1-3 unit or
objective lessons.

### E009: Starter environment kit readability

Status: `proposed`

Hypothesis: A reusable Aster Basin starter tileset can make Missions 1-3 feel
distinct through landmarks, props, and layout while preserving the current small
terrain rule vocabulary.

Method: Draft the starter tileset spec, then review a 1280x800 mock or
screenshot for 10-second readability, grayscale clarity, overlay contrast, and
friendly/enemy unit palette collisions.

Evidence: The viewer can identify HQ, roads, cover, objectives, pressure lanes,
and special props without reading the sidebar or zooming in.

### E005: Unit ramp readability and combat matrix

Status: `proposed`

Hypothesis: The compact nine-unit ramp can teach richer tactics by Mission 6
without overwhelming the current direct-combat foundation.

Method: Build a combat matrix for every proposed unit at full HP and half HP on
plain, cover, and HQ terrain. Pair it with a 1280x800 sprite readability review
that checks whether each 64x64 silhouette remains identifiable with board HP
and label overlays.

Evidence: Passing matrix checks, screenshot review, and at least one
deterministic mini-scenario per new unit family.

### E006: Act-one mission brief validation

Status: `proposed`

Hypothesis: Turning the first three campaign plot entries into mission briefs
will reveal whether the story spine produces concrete, playable tactical maps
without requiring new narrative systems.

Method: Draft Missions 1-3 with map ingredients, unit lists, objective text,
enemy pressure, radio lines, debrief copy, and implementation dependencies.

Evidence: The briefs align with existing prototype systems or identify the
smallest missing mechanics needed to support them.

### E007: Sidebar-covered arena readability

Status: `proposed`

Hypothesis: The first mission can be understandable and playable from the
graphical arena plus compact in-game HUD without relying on the verbose
development sidebar.

Method: Cover or hide the right sidebar during a 1280x800 playtest or
screenshot review. Check whether the player can identify the HQ, Scout-7,
current objective, selected mode, ready units, legal moves, legal attacks,
terrain value, attack/counter forecast, and enemy pressure.

Evidence: The first compact arena HUD implementation is now available for this
test. Pass/fail notes from sidebar-covered screenshots or playtest footage, plus
a list of every moment that required peeking at the sidebar, are still pending.

### E001: Repo continuity

Status: `proposed`

Hypothesis: Repo-based tracking improves autonomous continuity across
invocations.

Method: Require each loop pass to update state, backlog, development log,
critique, and experiments.

Evidence: The next pass can resume without relying on chat history.

### E002 Outcome: Testable Godot prototype

Status: `proposed`

Hypothesis: A Godot-first tactical prototype can still keep rules testable.

Method: Build the smallest board and unit model with a testable rules boundary.

Evidence: Movement and combat logic can be checked outside rendering.

### E003: Critique-driven slice selection

Status: `proposed`

Hypothesis: Adversarial critique improves slice selection.

Method: Require critique before marking a work item done.

Evidence: Backlog changes reflect identified risks or falsifying tests.

## Active Experiments

### E004: First mission trial playthrough

Status: `active`

Hypothesis: A thin Godot C# vertical slice can support a useful first human
trial before campaign systems, production, fog, or polished art exist.

Method: Let the user play the first mission, then collect notes on objective
clarity, control friction, AI pressure, scoring, and 16-bit pixel-art
readability.

Evidence: Awaiting manual playthrough feedback.

## Completed Experiments

### E008: Presentation-only damage feedback

Outcome: `complete`

Evidence: The first implementation keeps all damage feedback in Godot
presentation state and derives it from before-and-after snapshots around
successful attack commands. Core state still updates immediately through
`BattleRules.ApplyCommand`. The deterministic smoke suite passes, including the
replay hash check and AI-vs-AI victory proof. Godot C# build and Godot startup
also pass. Adversarial review found no core-rule boundary issue and led to one
truthfulness fix: HP labels stay authoritative while HP bars animate.
Screenshot-based clutter review remains useful as a follow-up visual QA step.

### E002: Testable Godot prototype

Outcome: `complete`

Evidence: A Godot C# project references a plain C# rules core. The smoke test
runner validates movement, terrain forecast, scout rescue, HQ defeat, replay
hash determinism, AI pressure, and score categories outside Godot.
