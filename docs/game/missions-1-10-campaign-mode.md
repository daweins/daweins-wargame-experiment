---
title: Missions 1-10 Campaign Mode
description: First-act campaign mode plan, mission records, progression rules, and art handoff IDs for Missions 1-10
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: concept
---

## Purpose

This document turns the first 10 campaign missions into a campaign-mode spine
that can drive implementation, save data, mission selection, briefings,
debriefs, scoring, unlocks, and art handoff. It is intentionally broader than
the current two-mission prototype, but it keeps the first act linear and
small enough to build in slices.

The imagery thread should monitor this folder for the first-act art backlog:
`game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread`.

## Campaign Mode Loop

The first act uses a linear campaign flow:

1. Mission select shows Missions 1-10, with locked, available, cleared, best
   rank, and replay states.
2. Starting a mission opens a briefing screen with commander portrait, mission
   objective, new tactical idea, enemy force, and stakes.
3. The battle runs through deterministic mission data, objective flags, seeded
   replay metadata, and AI commands.
4. Victory opens a debrief with score, objective bonuses, story reveal,
   unlocked mechanic, and next mission.
5. Defeat offers retry battle, replay briefing, return to mission select, and
   view objective summary.

First completion should never be score-gated. Medals, rank, replay validation,
gallery entries, and challenge variants can reward mastery after the player
has cleared the mission once.

## Campaign Save Record

The campaign save should include these public game-state fields before the
first 10 missions become a long playable chain:

* Campaign slot ID
* Current mission ID
* Cleared mission IDs
* Best score and rank per mission
* Seen briefing and debrief flags per mission
* Difficulty profile
* Rules version and mission data version
* RNG seed for each active or completed replay
* Command stream pointer for replay validation
* Unlocked unit families and mission verbs
* Objective flags, such as Scout-7 rescued, relay restored, pump station held,
  bridge preserved, audit convoy survived, scan data extracted, and refinery
  captured

The save file should not store private local paths, generated prompt paths, or
machine-specific asset-source details. Runtime asset IDs are enough.

## Shared First-Act Systems

The campaign shell needs these reusable systems before Mission 10 can feel like
a complete first-act finale:

| System | First mission | Minimal first-act requirement |
| --- | --- | --- |
| Briefing and debrief flow | Mission 1 | Static portrait, title, objective, stakes, and one story reveal |
| Linear mission select | Mission 1 | Locked, current, cleared, best rank, replay option |
| Objective flags | Mission 1 | Rescue, capture, hold, escort, destroy, escape, preserve |
| Score summary | Mission 1 | Speed, power, technique, and mission-specific objective bonus |
| Capture and property control | Mission 2 | Relay, cache, depots, sensor posts, HQ/refinery capture |
| Convoy or escort units | Mission 3 | Damaged crawler, audit convoy, engineer escort, scan-data exit |
| Production and income | Mission 4 | Limited field fabricator, depots, factories, income chip |
| Fog and sensors | Mission 5 | Soft fog, antenna reveal, jammer suppression, visible coverage |
| Infrastructure state | Mission 6 | Bridge demolition countdown, power restore, grid nodes |
| Restraint framing | Mission 7 | Disable or force withdrawal without civilian-grid damage |
| Full production capstone | Mission 10 | Player and enemy HQs, income pressure, commander defeat |

## Grounded Brief Fields

Each briefing should include at least one concrete logistics, authority,
infrastructure, or public-safety field. These fields keep the first act tied to
Transit Thread delay, Spindle packet limits, permit law, manifests, grid
stakeholders, and Asterite supply-chain cost instead of generic escalation.

### Missions 1-3 Baseline

The detailed playable briefs in [Missions 1-3 Playable Briefs](missions-1-3-briefs.md)
already define transit delay, Spindle status, cargo or permit constraints, grid
stakeholders, Asterite cost, and civilian infrastructure risk. Campaign-mode
briefings can quote those fields in shorter controller-friendly form.

### Mission 4 Brief Fields

* Route or schedule: The field fabricator feedstock is split between two depot
  pallets because the next replacement shipment is still months away on a
  scheduled Caldera freight window.
* Authority or permit: Venn signs Emergency Fabrication Authority `EF-14`, a
  Spindle-notice order that legalizes field production only if Kestrel keeps a
  custody record of every depot captured.
* Stakeholder or cost: The fabricator shares a secondary grid tap with Meridian
  cold-storage lockers, so overdraw becomes a public-safety argument, not only
  a production boost.
* Tactical verb: Capture depots, activate the fabricator, then rout the HQ
  guard before Orison can turn the custody record into a concession claim.

### Mission 5 Brief Fields

* Route or schedule: Sable arrives under braking-right authority, not local
  invitation; its recon package is what could fit through the current landing
  reservation.
* Spindle or message priority: The antenna field can certify a tiny evidence
  packet, but Sable has filed a treaty-security priority request against the
  same relay window.
* Stakeholder or cost: The trapped science team carries uncompressed sample
  notes that are too large for Spindle transmission and too valuable to abandon
  in the fog.
* Tactical verb: Repair antennas, reveal fogged contacts, extract the team, and
  deny Sable control of the sensor post.

### Mission 6 Brief Fields

* Route or schedule: The bridge is the only heavy route between the camp and
  the fabricator yard until a road-repair convoy can physically arrive.
* Manifest or permit: Orison's demolition team is using a maintenance permit for
  `load testing`, which becomes obvious fraud once charges appear on the span.
* Stakeholder or cost: The bridge also carries a coolant bypass line for two
  pump stations, so demolition would damage civilian water and Kestrel mobility
  at the same time.
* Tactical verb: Seize the bridge, stop demolition, and break Orison's road
  schedule.

### Mission 7 Brief Fields

* Route or schedule: Meridian haulers are blocking Kestrel because off-world
  orders arrive instantly, while replacement clinic batteries and heat-tap
  parts arrive only by freight slot.
* Authority or permit: Calder rejects both Orison concession papers and Sable
  security notices because neither names the settlement heat tap as a protected
  stakeholder.
* Stakeholder or cost: Asterite crates near the drill rig are not treasure; they
  are winter heat, clinic backup, and pump stability for nearby families.
* Tactical verb: Protect the drill and crates, preserve civilian grid nodes, and
  force talks without treating Meridian as a simple rout target.

### Mission 8 Brief Fields

* Route or schedule: The Treaty audit convoy arrives because its cutter was
  already in the landing queue before the first shots, not because help is
  suddenly fast.
* Spindle or custody: The auditor can certify sealed files only if the blackout
  lifts long enough for a custody hash to reach the relay desk.
* Stakeholder or cost: The supply hub supports camp food, field medicine, and
  engineering spares; losing it makes the expedition dependent on Orison fuel
  prices.
* Tactical verb: Defend the hub, escort power restoration, and keep the audit
  convoy alive.

### Mission 9 Brief Fields

* Route or schedule: The scan-data exit is a courier handoff route, not an
  escape tunnel; if Kestrel misses the window, the evidence waits another cycle.
* Spindle or map custody: The sensor packet proves official basin maps omit old
  grid corridors, but only if Kestrel extracts the scan with jammer timestamps
  intact.
* Stakeholder or cost: The hidden corridors explain why Meridian heat taps,
  Orison claims, and Sable security maps disagree about the same ridges.
* Tactical verb: Capture sensor posts, destroy jammers, and move the scan data
  out before the route closes.

### Mission 10 Brief Fields

* Route or schedule: Orison's refinery relies on a cached freight plan and a
  scheduled broadcast to make its local victory look inevitable before the next
  physical audit can arrive.
* Permit or legal claim: Sloane's refinery permit authorizes extraction support,
  not an unreported power siphon through old grid routing.
* Stakeholder or cost: The siphon steals stability from pump stations and heat
  taps that were never listed in Orison's concession filing.
* Tactical verb: Capture the refinery HQ, cut the siphon, defeat Sloane's local
  command, and broadcast the custody proof that opens Act 2.

## Mission Records

### Mission 1: Scout-7 Is Late

* Campaign role: Opening survival mission and tutorial for the tactical board.
* Primary objective: Hold the Kestrel HQ, rescue Scout-7, and defeat remaining
  Orison units.
* New tactical idea: Chokepoint defense, terrain cover, direct attacks,
  counterattacks, and HQ loss.
* Player force: Field Techs, Utility Armor, Survey Scout, and rescued Scout-7.
* Enemy force: Raider Troopers, Line Armor, and Pursuit Scout.
* Defeat triggers: HQ occupied by enemy, all player combat units destroyed, or
  Scout-7 destroyed if the scenario keeps her vulnerable.
* Score bonuses: Scout-7 rescued, HQ never entered, low unit loss, fast clear.
* Story reveal: Scout-7 saw armed Orison contractors at a seam that was
  supposed to be unimportant.
* Unlocks: Mission select, replay entry, basic rescue and HQ objective glossary.
* Required art IDs: `m01-cutscene-alert`, `m01-cutscene-scout7`,
  `m01-terrain-survey-camp`, `m01-prop-restricted-seam`, `ui-rescue-hq-danger`.

### Mission 2: Inventory Adjustment

* Campaign role: First property and split-objective mission.
* Primary objective: Capture or hold the comms relay and fuel cache before
  Orison locks down both.
* New tactical idea: Capture progress, property control, and dividing forces
  between nearby objectives.
* Player force: Field Techs, Utility Armor, Survey Scout, Expedition Engineer.
* Enemy force: Raider Troopers, Line Armor, Breach Sapper.
* Defeat triggers: HQ lost, relay and cache both lost, or all player combat
  units destroyed.
* Score bonuses: Relay restored early, fuel cache held, engineer survives,
  authenticated packet sent.
* Story reveal: Orison had access to Kestrel's relay timetable and joint fuel
  manifest.
* Unlocks: Capture glossary, Engineer profile, objective marker variants.
* Required art IDs: `m02-cutscene-relay`, `m02-terrain-relay-yard`,
  `m02-prop-fuel-cache`, `unit-engineer`, `unit-sapper`, `ui-capture-fuel`.

### Mission 3: Road To Pump Station Three

* Campaign role: First convoy and infrastructure-preservation mission.
* Primary objective: Escort damaged crawlers to Pump Station Three, block
  ambush routes, and hold the station.
* New tactical idea: Road speed, exposed movement, blocking, and convoy
  pressure.
* Player force: Field Techs, Utility Armor, Survey Scout, Expedition Engineer,
  damaged crawler convoy markers.
* Enemy force: Orison light vehicles, Raider Troopers, Pursuit Scouts.
* Defeat triggers: Pump station captured, all crawlers destroyed, HQ lost if a
  rear HQ exists, or all player combat units destroyed.
* Score bonuses: Every crawler survives, pump station never disabled,
  ambushes cleared, low turn count.
* Story reveal: Pump filters contain Asterite residue from an old utility
  corridor missing from official maps.
* Unlocks: Convoy route marker, road movement glossary, infrastructure score
  bonus.
* Required art IDs: `m03-cutscene-convoy`, `m03-cutscene-pump`,
  `m03-terrain-pump-station`, `m03-prop-crawler`, `ui-convoy-route`.

### Mission 4: Emergency Fabrication Authority

* Campaign role: First controlled production and income mission.
* Primary objective: Capture two depots, activate the field fabricator, then
  defeat the enemy HQ guard.
* New tactical idea: Depots, limited production, income pressure, and fortified
  enemy positions.
* Player force: Existing Kestrel roster plus first AT Lancer introduction if
  the armor-counter slice is ready.
* Enemy force: Orison armored detachment, depot guards, and optional Breach
  Lancer preview.
* Defeat triggers: Kestrel HQ lost, fabricator captured or destroyed, all player
  combat units destroyed.
* Score bonuses: Both depots captured, fabricator activated quickly, no depot
  recaptured by Orison.
* Story reveal: Orison has filed a legal claim over the entire Aster Basin.
* Unlocks: Production panel, depot income chip, AT Lancer profile if included.
* Required art IDs: `m04-cutscene-fabricator`, `m04-terrain-fabricator-yard`,
  `m04-prop-depot`, `unit-at-lancer`, `ui-production-income`.

### Mission 5: Fog Over Antenna Field

* Campaign role: Introduces Sable and soft fog without making the map feel
  arbitrary.
* Primary objective: Repair antenna stations, locate the trapped science team,
  extract them, and prevent Sable from taking the field.
* New tactical idea: Soft fog, sensor coverage, recon units, and reveal zones.
* Player force: Kestrel core units with Holt and Scout-7 as briefing voices.
* Enemy force: Sable recon force, disciplined infantry, and sensor-screen units.
* Defeat triggers: Science team lost, all antenna stations captured by Sable,
  HQ lost if present, or all player combat units destroyed.
* Score bonuses: Science team extracted, antennas repaired, low hidden-contact
  damage, Sable sensor post denied.
* Story reveal: Sable has partial Asterite maps that predate Kestrel's survey.
* Unlocks: Fog glossary, sensor overlay, Sable faction dossier.
* Required art IDs: `m05-cutscene-sable-arrival`, `m05-terrain-antenna-field`,
  `m05-prop-antenna`, `unit-sable-recon`, `ui-fog-sensor`.

### Mission 6: Bridge Warranty Void

* Campaign role: First clear victory over Orison's opening push.
* Primary objective: Seize the bridge, prevent demolition, then break the
  Orison force on the far bank.
* New tactical idea: Bridge chokepoints, demolition pressure, and
  counteroffensive positioning.
* Player force: Kestrel core, AT Lancer, optional Trail Striker if the fast-unit
  slice is ready.
* Enemy force: Orison armor, demolition infantry, Breach Sappers, and a local
  commander marker.
* Defeat triggers: Bridge destroyed, HQ lost, or all player combat units
  destroyed.
* Score bonuses: Bridge preserved, demolition units stopped early,
  counterattack objective completed, low losses.
* Story reveal: Orison's local command depends on one road and one extraction
  schedule.
* Unlocks: Bridge tile variant, demolition countdown UI, first-act clean-win
  medal.
* Required art IDs: `m06-cutscene-bridge`, `m06-terrain-bridge`,
  `m06-prop-demolition-charge`, `unit-striker`, `ui-demolition-countdown`.

### Mission 7: Peer Review With Rockets

* Campaign role: Introduces Meridian as a civilian-protector faction rather
  than a simple enemy.
* Primary objective: Protect the survey drill and Asterite crates while
  disabling enough Meridian units to force talks.
* New tactical idea: Fast enemies, terrain tricks, and restraint as mission
  framing.
* Player force: Kestrel combined arms with limited production or fixed
  reinforcements depending on implementation maturity.
* Enemy force: Meridian raiders, scouts, trail ambushers, and Calder's command
  marker.
* Defeat triggers: Drill destroyed, civilian heat tap destroyed, HQ lost if
  present, or all player combat units destroyed.
* Score bonuses: Civilian grid preserved, crates protected, low overkill or
  civilian damage, enough Meridian units disabled without rout-focused play.
* Story reveal: Meridian settlements use small Asterite taps for heat, power,
  and medical systems.
* Unlocks: Meridian faction dossier, restraint objective badge, civilian-grid
  marker.
* Required art IDs: `m07-cutscene-calder`, `m07-terrain-settlement-grid`,
  `m07-prop-drill-rig`, `m07-prop-heat-tap`, `unit-meridian-raider`,
  `ui-restraint-civilian-grid`.

### Mission 8: The Audit Arrives

* Campaign role: Multi-objective defense with bureaucracy pressure and blackout
  presentation.
* Primary objective: Defend HQ and supply hub, escort engineers to restore
  power, and keep the audit convoy alive.
* New tactical idea: Repair escorts, supply hub protection, blackout state, and
  noncombatant convoy survival.
* Player force: Kestrel core, Engineer, Field Rig if repair logistics are ready.
* Enemy force: Orison remnants plus Sable edge-pressure markers.
* Defeat triggers: Audit convoy destroyed, supply hub captured, HQ lost, or all
  player combat units destroyed.
* Score bonuses: Auditor survives, power restored quickly, sealed files
  unlocked, supply hub undamaged.
* Story reveal: Kestrel's sponsor classified Asterite as strategically
  sensitive before deployment.
* Unlocks: Blackout overlay, audit dossier, sealed-file story gallery entry.
* Required art IDs: `m08-cutscene-audit-blackout`, `m08-terrain-supply-hub`,
  `m08-prop-audit-convoy`, `m08-prop-substation`, `unit-field-rig`,
  `ui-blackout-power-file`.

### Mission 9: Blank Map Territory

* Campaign role: Turns the resource fight into a map-coverup mystery.
* Primary objective: Break through a fogged ridge, capture sensor posts,
  destroy jammers, and escape with scan data.
* New tactical idea: Fog plus jammers, sensor captures, artillery danger tiles,
  and extraction after objective capture.
* Player force: Kestrel recon-forward force with Engineers and Scouts.
* Enemy force: Sable sensor troops, jammer crews, artillery silhouettes, and
  disciplined screens.
* Defeat triggers: Scan-data carrier destroyed, too many jammers left active by
  the extraction window if a turn pressure exists, HQ lost if present, or all
  player combat units destroyed.
* Score bonuses: All sensor posts captured, all jammers destroyed, scan data
  extracted, low artillery damage.
* Story reveal: Official maps omit old power, pump, and ore-routing corridors
  beneath modern concession boundaries.
* Unlocks: Jammer overlay, artillery danger marker, grid-route fragment gallery
  entry.
* Required art IDs: `m09-cutscene-blank-map`, `m09-terrain-fog-ridge`,
  `m09-prop-jammer`, `m09-prop-sensor-post`, `unit-sable-artillery`,
  `ui-jammer-artillery-scan`.

### Mission 10: Operation Small Print

* Campaign role: First-act capstone and Orison local-campaign defeat.
* Primary objective: Capture Orison's refinery HQ or rout Sloane's command
  force while protecting Kestrel's HQ.
* New tactical idea: Full production, income control, HQ pressure, and enemy
  commander defeat.
* Player force: Kestrel combined arms with production, repair, capture, and
  anti-armor counters available.
* Enemy force: Orison refinery security, production units, Sloane command
  marker, optional Siege Breaker as a slow capstone threat.
* Defeat triggers: Kestrel HQ captured, refinery timer completes if used,
  Sloane extracts cache if the mission has a cargo pressure object, or all
  player combat units destroyed.
* Score bonuses: Refinery HQ captured, Sloane routed, power siphons shut down,
  Kestrel HQ undamaged, high technique score.
* Story reveal: The refinery was illegally drawing power through old grid
  routing, not only mining ore.
* Unlocks: Act 2 hook, Orison defeat gallery entry, full production glossary,
  refinery environment kit.
* Required art IDs: `m10-cutscene-refinery-assault`,
  `m10-cutscene-sloane-retreat`, `m10-terrain-refinery`,
  `m10-prop-power-siphon`, `m10-prop-compliance-server`, `unit-siege-breaker`,
  `portrait-sloane`, `ui-refinery-commander-defeat`.

## Art Handoff Coverage

The art request folder should treat these as the top-level families to generate
or track:

* Cutscene stills for mission openings, major reveals, and Mission 10 victory
* Commander portraits for Venn, Rusk, Priya, Holt, Sloane, Rhee, Calder, and a
  Treaty Oversight auditor
* Unit sprites for Kestrel, Orison, Sable, Meridian, specialist support units,
  convoy tokens, and noncombatant audit vehicles
* Terrain and prop atlases for survey camp, relay yard, pump station,
  fabricator yard, bridge, Meridian settlement, blackout hub, fog ridge, and
  Orison refinery
* UX icons and overlays for rescue, capture, convoy route, fuel, production,
  income, fog, sensor coverage, jammer, demolition, blackout, power restore,
  restraint, civilian risk, artillery danger, scan data, refinery capture, and
  commander defeat

Sprites should be generated and reviewed by family before promotion. A single
beautiful source sheet is not sufficient unless the crop reads at 64x64 on
representative terrain.

## Implementation Slices

1. Add campaign metadata records for Missions 1-3, then bind the current
   two-mission prototype to mission select, briefing, debrief, and save fields.
2. Add Mission 3 as a fixed-force convoy and pump-station scenario before
   broadening production or fog.
3. Add Mission 4 production and depot rules with smoke tests for income,
   factory output, replay determinism, and AI capture priorities.
4. Add Mission 5 soft fog and sensor coverage as explicit deterministic map
   state with UI overlays and tests.
5. Add Missions 6-10 one at a time, preserving the one-major-system-per-mission
   teaching rhythm.

## Acceptance Criteria

The first-act campaign mode is ready when:

* A new player can clear Missions 1-10 in order without external documentation.
* Every mission introduces at most one major new rule or mission verb.
* Mission select accurately reflects locked, available, cleared, best score,
  best rank, and replay states.
* Briefings state objective, stakes, enemy, and new mechanic in compact
  controller-friendly text.
* Debriefs carry the story reveal and next unlock without long exposition.
* Saves survive quitting between mission select, briefing, battle, debrief, and
  next-mission prompt.
* Replays store enough seed, rules, mission version, objective flags, and
  command metadata to validate deterministic outcomes.
* Mission 10 delivers a clear first-act win: Orison's refinery falls, Sloane
  retreats, and the grid-routing broadcast opens Act 2.
