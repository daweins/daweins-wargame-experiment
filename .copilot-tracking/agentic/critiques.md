---
title: Adversarial Critiques
description: Public-safe critique log for risks, failures, and improvement pressure
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Critique Protocol

Adversarial agents should look for weak assumptions, missing tests, product
dead ends, UX problems, security risks, hidden coupling, false progress,
overlarge slices, and experiments that would disprove the current plan.

Status values: `open`, `mitigated`, `accepted`, `rejected`, `obsolete`.

## Open Critiques

### C013: Playable campaign can masquerade as good campaign

Status: `open`

Area: Mission Design

Finding: The first-act campaign can now complete through AI playtesting, but
completion is not the same as fun, strategy, variance, fairness, or Steam Deck
readability. A mission can be deterministic and solvable while still being a
rout map with objective labels, one obvious route, weak score spread, or
objectives that matter only after combat is solved.

Recommended pressure test: Apply the mission design rubric to Missions 1-10.
Record each mission's tactical thesis, objective pressure, plan space, terrain
value, AI objective pressure, score spread, readability risk, and repetition
risk. Treat the flattest or most overloaded mission as the next tuning slice.

### C012: FTL messaging can erase local stakes

Status: `open`

Area: Universe Design

Finding: FTL messages help the politics breathe, but they can undermine the
campaign if they become high-bandwidth remote command, instant evidence dumps,
or a way to summon outside enforcement at dramatic convenience.

Recommended pressure test: Every Spindle packet in mission writing should name
sender, recipient, priority class, certifying office, custody risk, and why the
message cannot move people, parts, live targeting data, or immediate rescue.

### C011: Grounded lore can drift back into macguffins

Status: `open`

Area: Universe Design

Finding: Asterite, the Basin Stabilization Grid, the Transit Thread, and the
Spindle Net are useful setting engines, but each can become a vague solution if
future briefs do not keep costs, ownership, permits, failure modes, and tactical
verbs visible.

Recommended pressure test: Every reveal should identify a supply-chain cost,
legal stakeholder, route or message constraint, tactical objective, and civilian
consequence.

### C010: Character mechanics can become a second rules engine

Status: `open`

Area: Commander Design

Finding: Detailed commander backgrounds, powers, signature units, and faction
doctrine are valuable for campaign identity, but CO powers and character-linked
units can outgrow the current core if they ship before command logging,
forecast display, AI fairness, replay representation, and mission-introduction
budgets exist.

Recommended pressure test: Treat powers and signature units as candidate
mechanics. Before implementation, require a compact inspect-panel description,
numeric forecast deltas, deterministic activation command, visible AI rule,
counterplay, first safe mission, and no-new-unit fallback.

### C009: Environment variety can become scope and readability debt

Status: `open`

Area: Art Direction and Rules

Finding: Environmental variety is needed for a 50-mission campaign, but treating
each stage as a bespoke biome or adding a new terrain rule for every story noun
would explode tile count, AI expectations, forecast explanations, replay tests,
and Steam Deck readability risk.

Recommended pressure test: Build 6-8 reusable environment kits with a small
terrain rule budget. Run 10-second read, grayscale, unit-palette collision, and
overlay contrast checks before accepting new tilesets.

### C008: Combat effects can obscure tactical truth

Status: `open`

Area: UX and Replay

Finding: Floating numbers, flashes, recoil, and damaged overlays can make
combat feel better, but they can also hide unit HP, type, team, cursor state, or
Scout-7 markers. They can also mislead players if they display forecast damage
instead of actual seeded damage, or if animation timing starts to delay the
deterministic rules timeline.

Recommended pressure test: Start with presentation-only player-attack feedback.
Verify state, seed, command count, and final HP are unchanged by effects.
Capture max-clutter 1280x800 screenshots. Do not animate full enemy phases or
combat replay order until the core exposes structured deterministic events.

### C005: Unit ramp can outgrow the current core

Status: `open`

Area: Game Design

Finding: The first-six-mission roster is useful planning, but support actions,
ammo, range, fog, jamming, hover movement, or EMP-style disables would add new
rules, UI states, AI priorities, and replay expectations beyond the current
direct adjacent combat core.

Recommended pressure test: Before implementing each new unit, run a combat
matrix and a one-card role test covering its job, best target, worst matchup,
support interaction, required map feature, and why existing units cannot fill
the same role.

### C006: Campaign spine can overfit untested systems

Status: `open`

Area: Product

Finding: A 50-mission outline is valuable as a direction-setting spine, but it
can become too rigid if later missions specify mechanics, faction content, or
story turns before the tactical vocabulary has been validated.

Recommended pressure test: Keep Missions 1-10 concrete, keep Missions 11-50
modular, and make every five-mission arc survive cutting or replacing one
mission without collapsing the campaign.

### C007: Sidebar can mask arena readability

Status: `open`

Area: UX

Finding: The current right sidebar explains objective state, mode, controls,
legend, inspect data, forecast ranges, combat math, and log history. As long as
the sidebar is visible and counted as part of the main screen, it can hide that
the arena itself may not yet communicate enough for controller-first play.

Recommended pressure test: Run a sidebar-covered 1280x800 readability pass.
Treat every necessary sidebar peek as a candidate for an objective beacon,
rescue marker, readiness badge, cursor-local chip, bottom HUD prompt, compact
forecast, or enemy-phase arena cue.

### C004: Trial prototype still needs human play evidence

Status: `open`

Area: Product

Finding: The first mission now builds and launches, but trial readiness is not
the same as proven fun, readable pressure, or balanced six-to-eight-turn play.

Recommended pressure test: Have the user play the first mission, record where
the objective, controls, forecasts, AI pressure, and pixel-art readability fail,
then tune one focused pass.

### C001: Resumable autonomy

Status: `open`

Area: Autonomy

Finding: A prompt invocation cannot literally run forever, so unbounded
improvement needs resumable state and repeated self-directed passes.

Recommended pressure test: Verify every pass updates state, backlog, log,
critique, and next action.

### C003: Autonomy safety risk

Status: `open`

Area: Safety

Finding: More autonomy increases risk of accidentally handling private data.

Recommended pressure test: Keep hard-stop conditions narrow, explicit, and
enforced by scanner plus instructions.

## Mitigated Critiques

### C002: Unproven game direction

Mitigation: Implemented a first playable Godot C# mission slice backed by a
plain C# rules core and smoke checks.
