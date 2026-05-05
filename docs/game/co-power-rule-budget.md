---
title: CO Power Rule Budget
description: Compact implementation guardrails for deterministic commander powers
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: design
---

## Purpose

This document turns the character bible's candidate commander powers into a
small implementation budget. It does not implement powers. It defines the rules
contract that a first playable CO-power slice must satisfy before code work
starts.

Commander powers should add authored tactical personality without creating
hidden passives, replay drift, unreadable forecasts, or controller-hostile UI.

## Non-Negotiable Constraints

* A power is an explicit command, not an invisible passive.
* A power lasts for at most one player turn plus the following enemy phase.
* A power uses deterministic integer modifiers only.
* A power can affect at most two unit tags or one map-object family.
* Forecast panels must show every numeric combat delta before commitment.
* Inspect panels must fit name, charge, duration, affected tags, effect, and
  counterplay in compact controller-friendly text.
* Replay data must reproduce activation, expiration, affected state, and combat
  results from command data, state, rules version, and seed.
* AI players use the same public rule or receive a clearly telegraphed scenario
  rule.
* The first implementation must not introduce persistent named combat units,
  cross-mission charge carryover, hidden terrain modifiers, or random power
  outcomes.

## Data Budget

The smallest useful implementation needs these fields in rules data. Names are
illustrative, not a code contract yet.

* `CommanderId`: stable campaign commander key.
* `PowerId`: stable power key.
* `Charge`: integer from 0 to 6.
* `ChargeCost`: integer activation threshold.
* `Duration`: `PlayerTurn`, `EnemyPhase`, or `UntilNextPlayerTurn`.
* `AffectedTags`: one or two unit tags, such as `Infantry`, `Armor`, `Scout`,
  `Support`, or `GroundCombat`.
* `Effect`: deterministic modifier record, such as `DefenseDelta = 1`.
* `ActivationCommand`: command-stream entry with commander, power, active side,
  turn number, and rules version.
* `ExpirationTurn`: deterministic cleanup point.

Do not add custom effect code per commander until this generic shape proves too
small. First effects should be data-shaped modifiers consumed by forecast and
combat resolution.

## Charge Sources

Use a small in-mission charge meter. Charge does not persist between missions.
One source should usually add one point, and a power should cost four to six
points so it appears once in a normal early mission.

Allowed first-pass charge sources:

* Friendly unit takes damage and survives.
* Friendly unit deals counterattack damage.
* Friendly unit captures, rescues, stabilizes, or holds a mission objective.
* Enemy unit fails to capture or occupy a protected objective zone.

Deferred charge sources:

* Repairs, resupply, fog reveal, convoy progress, production, and zone strikes.
  These should wait until those systems exist in rules, forecasts, and AI.
* Score rank, story flags, campaign medals, or cross-mission performance.

## Activation Command

The command should be available only at the start of the player's command menu
or before ending the turn. It should fail deterministically if charge is too
low, the side has no commander power, a power is already active for that side,
or the current mission disables powers.

Activation spends charge immediately, records the command, applies visible
status icons to affected units, and updates forecasts before any further combat
command can be previewed.

## Forecast Display

Forecasts must show the before-and-after delta for every affected value:

* Incoming damage reduction, such as `-1 damage from Lock The Line`.
* Counterattack increase, if enabled, such as `+1 counter damage`.
* Capture, repair, reveal, or movement values only after those systems have
  dedicated forecast fields.

The forecast should never require the player to remember a briefing line. If a
power changes a number, the changed number appears in the combat or objective
preview.

## AI Fairness

AI-vs-AI playtests must activate powers through the same command path as a
human player. The first AI heuristic can be simple:

* Activate defensive powers if at least two affected units are exposed to enemy
  attack or one protected objective would otherwise be threatened.
* Treat visible enemy defensive powers as forecast changes, not as hidden
  knowledge.
* Do not add AI-only discounts, charge boosts, or damage exceptions.

Enemy-only powers may exist later as scenario rules, but they still need visible
warning, deterministic timing, and replay records.

## Replay Data

The replay stream must include one activation command. It should not serialize a
full list of every affected unit unless future debugging proves that necessary.
Affected units are derived by applying `AffectedTags`, side, position, and state
at the activation turn.

Minimum replay checks before shipping powers:

* Activation with exactly enough charge succeeds and spends charge.
* Activation below cost fails without state mutation.
* Affected unit forecasts show the modifier.
* Expiration clears the modifier at the deterministic cleanup point.
* AI-vs-AI replay reproduces the same activation turn and outcome.

## Candidate Evaluation

### Accepted First Prototype: Rusk, Lock The Line

Rusk is the safest first playable commander because his candidate reinforces
existing Mission 1 lessons: chokepoints, counterattacks, infantry screens, and
deliberate holds.

Prototype effect:

* Cost: 4 charge.
* Duration: until the next player turn starts.
* Affected tags: `GroundCombat` friendly units.
* Condition: unit did not move during the current player turn.
* Effect: `DefenseDelta = 1` during the enemy phase.
* Deferred effect: counterattack bonus. Add it only after the defense-only
  version proves readable and balanced.
* Charge sources: damage taken and survived, counterattack damage dealt, and
  protected objective zones held.
* Forecast display: shield icon, duration label, and incoming-damage delta.
* Counterplay: enemy can attack moving units, break screens before activation,
  capture elsewhere, or wait out the one-phase duration.

Why it fits:

* It uses existing combat math and terrain lessons.
* It has a short duration and obvious map icons.
* It does not require repair, fog, production, movement-cost, or artillery
  systems.
* It can be validated in deterministic smoke tests and AI-vs-AI logs.

### Later Candidate: Venn, Field Hypothesis

Keep Venn as the second candidate only if the first implementation can show
objective and terrain modifier details cleanly. Start with inspect clarity and a
small defensive bonus on cover or properties. Reject the repair rider until
support actions exist.

### Later Candidate: Holt, Clean Signal

Hold Holt until soft fog, sensor posts, marked targets, and forecast-visible
concealment are implemented. Without those systems, the power is either empty
or it creates one-off reveal rules that will be thrown away.

### Later Candidate: Sloane, Emergency Appropriation

Hold Sloane until property income and production exist. A production discount
or property repair must be shown on the map before activation, or it will feel
like enemy cheating.

### Rejected For Early Prototype: Priya, Expedited Maintenance

Reject Priya for the first power slice because repair values, Field Rig support,
and action restoration are not stable first-act systems yet. Use Priya's
doctrine in mission framing until support actions have forecasts and replay
coverage.

### Rejected For Early Prototype: Rhee, Measured Advance

Reject Rhee for the first power slice because capture acceleration, support
setup, and layered formation benefits need more UI than the first inspect panel
should carry. Revisit after capture economy and Sable formation behavior exist.

### Rejected For Early Prototype: Calder, Backroad Network

Reject Calder for the first power slice because movement-cost changes can break
map puzzles, and soft zone-of-control rules do not exist yet. Revisit after
convoy and light-unit route missions are stable.

### Rejected For Early Prototype: Kravic, Fire Authorization

Reject Kravic for the first power slice because strike zones require warning
overlays, movement counterplay, deterministic delayed resolution, and new AI
planning. Revisit after artillery-style danger markers are visible and tested.

## Mission Timing

Do not introduce CO powers in Missions 1-3. Those missions are already teaching
core movement, attack, rescue, capture, convoy, and infrastructure verbs.

Best first implementation window: Mission 4 or Mission 6 as a controlled Rusk
trial. Mission 4 can frame Lock The Line as the expedition holding depots while
production comes online. Mission 6 can frame it as a bridge defense power after
the player has seen several chokepoint fights.

## Implementation Gate

Before code work starts, the backlog item should require:

* Unit profile tags in core rules.
* One command-stream type for activation.
* One active-power state record per side.
* Forecast deltas for the affected values.
* Status icon or text in Godot inspect UI.
* Smoke tests for charge, activation, expiration, forecast, replay, and AI
  parity.
