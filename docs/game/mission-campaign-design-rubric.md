---
title: Mission And Campaign Design Rubric
description: Criteria for evaluating tactical mission quality and long-campaign progression
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: concept
---

## Design Target

Missions should make the player feel like a clever field commander solving a
readable tactical problem under pressure. The best missions combine a clear
board state, a concrete objective, a grounded reason for the fight, and at
least two plausible plans that carry different costs.

The campaign should widen the player's strategic vocabulary over time. It
should teach, vary, combine, and then stress-test ideas without turning every
mission into a pile of new units, new rules, new overlays, and new scripts.

## Good Mission Criteria

### Tactical Thesis

A good mission can be summarized in one tactical sentence.

Examples:

* Can the player split forces without losing the relay
* Can the player rescue Scout-7 without opening the HQ lane
* Can the player escort damaged crawlers while blocking road ambushes
* Can the player win the income race without losing the refinery approach

A bad mission has no thesis beyond defeating all enemies, or it uses a strong
briefing premise that never changes the player's actual decisions.

### Readable First Decision

Within a few seconds, the player should understand the primary objective, the
main danger lane, the relevant terrain, and the first meaningful choice.

Good signals:

* Objective markers point to places that matter tactically.
* Unit silhouettes, HP, terrain defense, and legal action previews are readable
  at 1280x800.
* The player can infer the risk of the fast plan and the cost of the safe plan.
* The first turn offers a real choice, not only an obvious scripted move.

Bad signals:

* The mission only becomes understandable after a failed attempt.
* The player must read a long sidebar or external document to know why a tile,
  unit, or object matters.
* Hidden information can defeat the player before its warning and counterplay
  are visible.

### Objective Pressure

Mission objectives must change movement, targeting, economy, timing, or risk.
They should not be decorative labels attached to a rout map.

Good objective pressure:

* Rescuing a unit changes the player's force or score before the final turn.
* Capturing a relay restores information, income, production, or a route.
* Destroying a jammer changes fog, artillery risk, or extraction timing.
* Preserving a bridge, convoy, hub, or heat tap changes score and board state.

Bad objective pressure:

* The optimal play is to ignore the stated objective until every enemy is dead.
* The objective is only a lock before the real rout condition.
* Failure rules are invisible, irreversible, or detached from board decisions.

### Meaningful Plan Space

Most non-tutorial missions should support at least two credible plans. The
plans do not need to be equally strong, but each should be understandable and
playable.

Useful tradeoffs include:

* Fast exposed route versus slower covered route
* Split-force tempo versus concentrated safety
* Capture economy now versus hold position now
* Reveal fog safely versus advance quickly
* Repair or resupply versus attack
* Preserve civilian infrastructure versus finish faster

A mission is weak if the best move is obvious for several turns in a row, or if
all successful players converge on the same opening, same midgame, and same
cleanup path.

### Terrain And Map Topology

Terrain should create decisions, not only decoration.

Good map criteria:

* At least one relevant terrain tile changes a combat forecast or movement
  route.
* Chokepoints have counterplay through flanks, timers, support targets, or
  objective pressure.
* Objective-critical tiles are reachable by the intended unit classes before the
  mission becomes unrecoverable.
* Roads, cover, ridges, HQs, properties, bridges, and objective objects remain
  visually distinct at handheld distance.

Bad map criteria:

* Every mission uses one road, one armor block, and one safe capture lane.
* Terrain has attractive art but no tactical consequence.
* A mission depends on exact hidden path knowledge on turn 1.
* The map is large enough to create downtime but not large enough to create
  maneuver.

### Enemy Doctrine And AI Pressure

The enemy should pressure the mission thesis. Doctrine can come from target
priorities, formations, aggression, retreat thresholds, objective weights, and
unit composition before adding special rules.

Good AI pressure:

* Orison contests income, pushes armor lanes, and tries to turn logistics into
  possession.
* Sable protects sensors, controls ridges, and punishes careless information
  play.
* Meridian uses speed, terrain tricks, and civilian stakes to force restraint.
* Enemy actions visibly advance toward HQ capture, convoy attack, jammer
  protection, depot contest, or extraction denial.

Bad AI pressure:

* The AI ignores the story objective and only attacks the nearest unit.
* Autoplayer success requires post-combat objective cleanup because the
  objective did not matter during combat.
* Enemy behavior relies on hidden cheats, gotcha reinforcements, or target rules
  the player cannot read.

### Fairness And Trust

After defeat, the player should be able to explain the loss from visible
information.

Good defeat lessons:

* I left the relay open.
* I overextended the armor without infantry screens.
* I chased score and missed the extraction window.
* I advanced through fog without repairing antennas.

Bad defeat lessons:

* A hidden unit deleted the convoy without warning.
* The timer failed but I did not know what it measured.
* The forecast did not show the real damage range.
* Reinforcements punished normal play from an unmarked edge.

### Scoring And Mastery

First completion should never be score-gated. Scoring should diagnose command
style and reward mastery.

Good scoring:

* Objective score is the most important category.
* Speed, technique, and power separate clean, fast, cautious, and aggressive
  play.
* Mission-specific bonuses reward the intended lesson: Scout-7 rescued, bridge
  preserved, convoy intact, power restored, scan data extracted, refinery
  captured.
* A slow clean win and a fast reckless win have visibly different score shapes.

Bad scoring:

* A rout can earn a great score after ignoring the mission premise.
* Optional objectives punish normal first-clear play instead of rewarding
  mastery.
* One score category dominates every mission.
* The rank system teaches less interesting play than the mission itself.

## Bad Mission Anti-Patterns

### Rout After Doing The Thing

The player completes the advertised objective, then spends the final third of
the map cleaning up enemies. Fix this by making the objective resolve the
tactical state: open a route, force withdrawal, end the mission, remove a timer,
change income, reveal threats, or trigger a closing push.

### Single Correct Route Escort

The escort path is a memorized corridor. Fix this by offering at least two
routes, each with a readable cost, and by making the escorted object predictable
instead of obstructive.

### Fog Tax

Fog only hides enemies that punish normal movement. Fix this by giving the
player sensor tools, safe scouting options, visible danger language, and score
rewards for low hidden-contact damage.

### Production Snowball

The map is decided by the first capture and then becomes an income chore. Fix
this with unit-now versus depot-later tradeoffs, resource caps, commander
pressure, HQ threats, contested repair points, or a timed tactical objective.

### Capstone Pileup

A finale introduces major new rules while also asking the player to prove old
ones. Fix this by making capstones recombine already-proven systems and moving
new rules into earlier tutorials or optional bonuses.

### Objective Costume

Briefing text promises politics, logistics, or civilian stakes, but the map
could belong to any generic skirmish. Fix this by tying the objective to a
concrete board verb, visible stakeholder, and mechanical consequence.

## Good Campaign Progression Criteria

### Lesson Arc Structure

Every three to five missions should form a lesson arc:

1. Introduce a concept in a clean context.
2. Vary the concept with a new map shape or enemy doctrine.
3. Combine it with an older concept.
4. Stress-test it under pressure.
5. Pay it off in a capstone or clean victory.

The current first act should roughly follow this rhythm:

* Missions 1-3 build core literacy: movement, terrain, direct attacks,
  counterattacks, rescue, capture, convoy, and infrastructure stakes.
* Missions 4-6 introduce first systems: limited production, income pressure,
  bridge or demolition pressure, and the first clean tactical win over Orison.
* Missions 7-9 vary incentives through restraint, blackout defense, fog,
  jammers, and evidence extraction.
* Mission 10 recombines proven systems in a first-act production and HQ-pressure
  capstone.

### Pacing Rhythm

Good campaigns alternate mental muscles. Across any five missions, vary at
least three of these dimensions:

* Objective verb
* Map topology
* Enemy doctrine
* Dominant unit role
* Economy or production state
* Information state
* Pressure clock
* Civilian or infrastructure constraint
* Emotional beat

Bad campaigns repeat the same optimization with new nouns: capture a point,
hold a lane, rout the map, repeat.

### Mechanic Introduction Budget

Use tight introduction limits so new rules create strategy instead of fatigue.

* Early campaign: add at most one new player-commanded unit type per mission.
* Tutorial missions: introduce one major rule or one new unit with a small
  objective variation, not both at full weight.
* Mid campaign: combine known systems more often than adding new systems.
* Late campaign: increase pressure, map shape, doctrine, and consequences before
  adding more nouns.
* CO powers, fog, supply, production, indirect fire, and campaign branching each
  count as major systems.

### Unit And Counter Longevity

Progression is good when new units create new questions without replacing old
answers.

Good progression:

* Infantry, armor, scouts, terrain, screens, captures, and HQ pressure still
  matter late in the campaign.
* Each new unit has a favored target, weak target, support relationship, and
  objective role.
* A new unit appears at least three times: tutorial puzzle, mixed-arms variation,
  and counter-pressure mission.
* Factions feel different through doctrine and map goals before requiring
  bespoke rules.

Bad progression:

* New units are direct upgrades.
* Enemy stats inflate until old counters stop working.
* Campaign unlocks turn basic missions into chores for strong players and walls
  for weaker players.
* Score rewards become required power instead of mastery recognition.

### Story Escalation And Victory Rhythm

Good campaign escalation increases stakes while still giving periodic closure.
The player should win visible problems often enough to feel capable, even as the
wider crisis expands.

Good progression:

* Early stakes stay personal: HQ, Scout-7, fuel, relay, water, repair capacity.
* Later stakes expand through tactical consequences: maps, routes, depots,
  substations, permits, evidence, authority keys, civilian utilities.
* Clean victories happen at regular intervals, especially around Missions 6, 10,
  15, 20, 25, 30, 35, 40, 45, and 50.
* Later missions revisit earlier places or verbs with new meaning.

Bad progression:

* The story raises stakes only through exposition.
* The player goes too long without a clean win.
* Every act ends with the same kind of assault.
* The 50-mission spine becomes rigid before playable evidence supports it.

## Campaign Progression Anti-Patterns

### Vertical Accumulation

The campaign adds more units, powers, overlays, and objectives, but player
decisions become flatter because the same solved tool handles everything.

### Score-Gated Power Spiral

Strong players earn more power and face easier future missions, while weaker
players earn fewer tools and face harder ones. Keep first-clear progression
mostly unlock-based and bounded. Put score rewards into medals, sidegrades,
challenge variants, cosmetics, optional missions, or limited-use advantages that
do not determine basic campaign viability.

### Forced Novelty

Every mission demands a new mechanic to feel different. Prefer changing terrain,
objective pressure, enemy priorities, unit mix, resource scarcity, and emotional
stakes before adding a new rule.

### Capstone Bloat

Every act finale becomes a large map with all systems active. A good capstone is
decisive because it recombines the right systems, not because it includes all of
them.

## Evaluation Matrix

Use this table when reviewing missions or five-mission arcs.

| Criterion | Good signal | Bad signal | Evidence |
| --- | --- | --- | --- |
| Tactical thesis | One clear decision problem | Generic rout or vague premise | Mission brief and first turns |
| Objective pressure | Objective changes board behavior | Objective is cleanup or label | Objective flags and replay |
| Plan space | Two credible plans with tradeoffs | One obvious route | Playtest notes and command logs |
| Terrain value | Terrain changes forecasts or paths | Terrain is visual only | Forecast and movement checks |
| Enemy doctrine | AI pressures mission goal | AI only attacks nearest unit | AI log summaries |
| Fairness | Defeat is explainable | Hidden or unclear failure | Manual playtest notes |
| Scoring | Score diagnoses play style | Score rewards dull play | Score category spread |
| Readability | Works at 1280x800 | Requires sidebar or memory | Screenshot or playtest review |
| Determinism | Replay hashes match | Randomness or hidden state drifts | Golden replay fixtures |
| Campaign freshness | Neighbor missions differ | Repeated verb and topology | Arc variance matrix |

## Automated Evidence

Use deterministic automation to catch false progress before manual tuning.

Per mission, collect:

* Outcome
* Turn count
* Player losses and enemy losses
* Objective flags
* Score total and objective, speed, technique, and power breakdowns
* First objective completed
* Command count
* Final state hash
* Rejected command count
* Turns with no objective progress
* AI issue candidates

Good automated signals:

* Missions complete within intended turn envelopes or fail for explainable
  design reasons.
* Objective order and score shape match the mission role.
* Neighboring missions produce distinct tactical signatures.
* Replays are stable under the same mission version, rules version, seed, and
  command stream.

Bad automated signals:

* Repeated perfect scores.
* Repeated stalls after enemies are cleared.
* Victory without touching the signature objective.
* Command logs that look nearly identical across adjacent missions.
* Autoplayer success that depends on opaque routes humans would not read.

## Manual Review Questions

Ask these after a 1280x800 playtest or replay review:

* What was the interesting choice on turn 1, turn 3, and the decisive turn
* Which unit was unusually valuable here
* Which normally strong habit did the mission punish
* What did the map make tempting that was actually risky
* Could a slower, safer player win with a lower score
* Did the mission's story objective produce a mechanical consequence
* Did the AI threaten the objective in a way the player could read first
* Would the mission still be fun after the surprise is gone

## Promotion Gates

Before a mission is treated as good campaign content, it should pass these
gates:

* Mission metadata, objective flags, unit IDs, and map bounds validate.
* Objective-critical tiles are reachable by intended units.
* The mission has one tactical thesis and one dominant new lesson or remix.
* At least one deterministic replay fixture covers the mission's signature
  objective.
* AI or autoplayer logs show objective pressure, not only combat cleanup.
* Score categories separate at least two play styles.
* A 1280x800 review confirms objective, threat, unit, terrain, and forecast
  readability without relying on the development sidebar.
* New mechanics have forecast display, replay state, AI behavior, and smoke
  coverage before the mission depends on them.
