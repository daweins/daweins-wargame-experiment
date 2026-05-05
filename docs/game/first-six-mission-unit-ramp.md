---
title: First Six Mission Unit Ramp
description: Unit roster, counter relationships, mission flavor, and sprite plan for the first six tactical combat missions
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Design Target

The first six missions should teach combined-arms tactics without burying the
player under unit nouns. Mission 1 keeps the current three-role foundation:
infantry, armor, and scout. Missions 2 through 6 add one or two new unit types
at a time, usually one player-commanded unit and one enemy expression or
variant.

The player faction is a research expedition forced to militarize, so its units
should feel improvised, flexible, and support-oriented. Enemy factions can share
the same broad battlefield grammar, but they should lean harder into pressure,
sabotage, and purpose-built military hardware. The overlap should help the
player read threats without making the sides exact mirrors.

## Learning Budget

Use these limits while turning this brainstorm into playable missions:

* Add at most one new player-commanded unit type per mission after Mission 1.
* A second new unit in a mission should usually be enemy-only, objective-bound,
  or a visible variant of a known class.
* Do not introduce a new unit and a large new rules system in the same mission
  unless the mission is otherwise tiny.
* Every new unit needs one first-encounter puzzle that shows what it beats, what
  beats it, and what mistake it punishes.
* Every unit needs a clear 64x64 silhouette, a short board label, and forecast
  text that exposes the important counter rule before commitment.

## Stat Model

Stats use the current prototype scale, with room for later profile metadata:

* HP is the unit's maximum health.
* Move is tile movement before terrain costs.
* ATK is baseline attack before health scaling, terrain defense, and matchup
  modifiers.
* DEF reduces incoming damage before terrain defense.
* Range stays 1 for the direct-combat implementation. Range 2 or higher should
  wait for a separate forecast, counterattack, AI, and replay slice.
* Ammo is `--` until limited ammunition is implemented. Lancer and support
  units can carry ammo metadata in the design before the rule ships.

## Roster Overview

| Unit type | Player version | Enemy version | HP | Move | ATK | DEF | Range | Board label | Tactical job |
| --------- | -------------- | ------------- | -- | ---- | --- | --- | ----- | ----------- | ------------ |
| Field Tech | Field Tech | Raider Trooper | 10 | 3 | 5 | 1 | 1 | INF | Captures, rescues, screens support units, and punishes fragile specialists |
| Utility Armor | Utility Armor | Line Armor | 14 | 4 | 7 | 3 | 1 | ARM | Holds roads, breaks infantry lines, and anchors chokepoints |
| Survey Scout | Survey Scout | Pursuit Scout | 8 | 5 | 4 | 0 | 1 | SCT | Grabs space, spots later mission threats, and hunts exposed support |
| Engineer | Expedition Engineer | Sabotage Engineer | 9 | 3 | 3 | 0 | 1 | ENG | Repairs, hacks mission objects, and creates escort objectives |
| Sapper | Field Sapper | Breach Sapper | 8 | 4 | 4 | 0 | 1 | SAP | Threatens structures, disables support, and forces screening |
| Lancer | AT Lancer | Breach Lancer | 9 | 3 | 5 | 1 | 1 | AT | Hard-counters armor and heavy vehicles, but folds to infantry pressure |
| Striker | Trail Striker | Hunter Bike | 9 | 6 | 5 | 1 | 1 | STK | Punishes exposed scouts, lancers, engineers, and rigs |
| Field Rig | Field Rig | Ammo Mule | 10 | 4 | 2 | 1 | 1 | RIG | Repairs vehicles, resupplies specialist ammo, and becomes a high-value target |
| Siege Breaker | Prototype Breaker | Siege Breaker | 18 | 2 | 8 | 4 | 1 | HVY | Creates a slow mission clock that must be blocked, lanced, disabled, or baited |

## Counter Relationships

The roster uses two main rock-paper-scissors loops and one objective-pressure
loop.

### Direct Combat Triangle

Infantry beats Lancers and Sappers when it can reach them. Lancers beat Armor
and Siege Breakers. Armor beats Infantry and Scouts in open ground.

This triangle should remain the most visible counter system. It can be expressed
through a simple matchup modifier table:

| Attacker | Favored targets | Weak targets | Notes |
| -------- | --------------- | ------------ | ----- |
| Field Tech | Lancer, Sapper, Engineer | Armor, Siege Breaker | Cover lets infantry survive long enough to screen |
| Utility Armor | Field Tech, Scout, Striker | Lancer, Sapper swarm | Armor should feel sturdy but not self-sufficient |
| AT Lancer | Armor, Field Rig, Siege Breaker | Field Tech, Striker | Limited ammo can wait until Mission 5 |

### Support And Raid Triangle

Scout and Striker units beat exposed support. Support units keep Armor and
Lancers alive. Sustained Armor and screened Infantry punish Scouts and Strikers.

| Unit | Supports | Loses to | Support interaction |
| ---- | -------- | -------- | ------------------- |
| Survey Scout | Finds flank paths and later spots targets | Armor, covered infantry | Pairs with Lancers by revealing safe approach lanes |
| Striker | Removes Engineers, Scouts, Lancers, and Rigs | Armor, infantry traps | Forces players to protect support units instead of clumping forward |
| Field Rig | Repairs vehicles and resupplies specialists | Strikers, Sappers, Lancers | Turns damaged Armor into a reusable wall if protected |
| Engineer | Repairs light damage and hacks objectives | Strikers, Sappers, Armor | Creates a reason to escort instead of only attacking |

### Objective Pressure Loop

Sappers threaten HQs, relays, gates, and prototype anchors. Infantry screens
against Sappers. Armor and Strikers punish unsupported infantry screens.

This loop gives missions personality without requiring full economy or
production. A map can make a Sapper scary by putting a relay two turns away from
failure, then give the player a clean answer through screening, body-blocking,
or counterattack.

## Mission Introduction Plan

| Mission | New unit types | Player lesson | Enemy expression | Mission flavor |
| ------- | -------------- | ------------- | ---------------- | -------------- |
| 1. Field Peer Review | Field Tech, Utility Armor, Survey Scout | Learn movement, terrain defense, direct attacks, counterattacks, rescue, and HQ pressure | Raider Troopers, Line Armor, and a Pursuit Scout apply readable pressure | The expedition discovers the security plan was mostly a laminated evacuation map |
| 2. Warranty Voided | Engineer, Sapper | Escort an Engineer to stabilize two failing field relays while still defending HQ lanes | Breach Sappers race toward relays and punish unsupported screens | The lab equipment can repair armor, open doors, and void three procurement policies at once |
| 3. Committee On Tank Problems | AT Lancer | Use Lancers behind infantry or armor screens to stop a heavier vehicle column | Enemy Line Armor overcommits unless protected by Raider Troopers | A geology laser is reclassified as anti-armor after a tense meeting with legal |
| 4. Fast Movers, Slow Decisions | Striker | Trap fast raiders with infantry zones, armor blocks, and Scout bait | Hunter Bikes dive Engineers, Scouts, and Lancers instead of trading into Armor | The enemy has discovered the ancient military doctrine of going around things |
| 5. Please Inventory The Explosions | Field Rig | Keep a forward group supplied and repaired while rotating damaged Armor | Sappers and Strikers try to cut off the support vehicle | Logistics files list ammunition under field consumables, which is technically true |
| 6. The Prototype Has A Prototype | Siege Breaker | Combine Lancers, Sappers, Field Rigs, Scouts, and terrain to stop one slow monster | A Siege Breaker advances under escort as a mobile objective clock | The rival prototype is enormous, expensive, and almost certainly over the lab's weight limit |

## Mission Sketches

### Mission 1: Field Peer Review

The current prototype mission already proves the baseline. Field Techs hold
cover and rescue Scout-7, Utility Armor blocks the road, and the Scout teaches
mobility without counterattack safety.

Acceptance checks:

* The player can identify the three friendly roles by silhouette within a few
  seconds.
* Armor beats exposed infantry, but terrain can change the forecast.
* Scout-7 matters as an objective before becoming a normal unit.

### Mission 2: Warranty Voided

The player receives one Engineer. The enemy introduces Sappers. The map has two
failing relays that can be stabilized by ending an Engineer turn next to them.
Sappers can sabotage those relays or threaten the HQ if ignored.

New interactions:

* Engineer repairs 2 HP on an adjacent friendly unit instead of attacking.
* Engineer can stabilize a relay by waiting adjacent to it.
* Sapper deals bonus damage to support units and mission objects.

### Mission 3: Committee On Tank Problems

The player receives AT Lancers as the first hard counter. The enemy fields a
small armor column with infantry escorts. The lesson is that Lancers solve armor
only when screened.

New interactions:

* Lancer gains a large matchup bonus against Armor and Siege Breaker units.
* Lancer loses trades into Infantry and Strikers.
* Utility Armor can protect Lancers from infantry pressure.

### Mission 4: Fast Movers, Slow Decisions

Enemy Hunter Bikes arrive as fast support hunters. The player may command one
Trail Striker in a side lane, but the main lesson can work with enemy-only
Strikers if the learning budget is tight.

New interactions:

* Striker has high movement and bonus damage against support and recon units.
* Striker loses to Armor and infantry traps.
* Scouts can bait Strikers into bad road positions.

### Mission 5: Please Inventory The Explosions

The player receives a Field Rig and a limited forward force. If ammo is not
implemented yet, the rig can repair adjacent vehicles only. If ammo is ready,
it can also restore one Lancer shot.

New interactions:

* Field Rig repairs 2 HP on an adjacent friendly vehicle or specialist.
* Field Rig resupplies 1 ammo when limited ammo exists.
* Sappers and Strikers prioritize Field Rigs, creating a logistics escort
  puzzle.

### Mission 6: The Prototype Has A Prototype

The enemy deploys a Siege Breaker under escort. The Siege Breaker is a slow
heavy unit in the direct-combat implementation. If range systems are ready by
then, it can become a true siege platform with range 3 to 5 and a no-move-fire
constraint.

New interactions:

* Siege Breaker is too durable for Armor duels.
* Lancers and Sappers provide the cleanest answers, but both need screens.
* Field Rig sustain lets the player rotate blockers instead of racing damage
  blindly.

## Sprite Plan

The generated campaign sheet lives at
`game/WargamePrototype/assets/sprites/campaign_units.png`. It uses the same
64x64 frame size and two-row team structure as the current prototype unit sheet.

Column order:

1. Field Tech
2. Utility Armor
3. Survey Scout
4. Engineer
5. Sapper
6. AT Lancer
7. Striker
8. Field Rig
9. Siege Breaker

Sprite readability requirements:

* Infantry and Engineer must both be upright, but Engineer needs a backpack,
  tool arm, or dish silhouette.
* Sapper needs a satchel or hazard stripe so it does not read as a normal
  infantry unit.
* Lancer needs a long weapon silhouette that points away from the body.
* Striker needs a low fast profile distinct from the existing Scout buggy.
* Field Rig needs a boxy logistics silhouette with crates or a small crane.
* Siege Breaker needs to be wide, slow, and visibly heavier than Armor.

## Implementation Order

1. Add profile metadata for board label, tags, and sprite index while keeping
   current Infantry, Armor, and Scout behavior unchanged.
2. Move matchup modifiers from a hardcoded switch to a data table with tests.
3. Add Engineer and Sapper as direct-combat units with mission-object actions.
4. Add Lancer and its armor matchup tests.
5. Add Striker AI priorities for support hunting.
6. Add Field Rig repair first, then ammo only after replay and UI support are
   ready.
7. Add Siege Breaker as a direct heavy objective unit before considering ranged
   siege behavior.

## Open Balance Risks

The current damage model is lethal enough that high ATK and high DEF values can
erase counterplay. Keep most new units near the existing stat band and let
matchup modifiers carry the counter identity.

Support units will add UI and AI complexity. Their first mission should be
small, fixed-force, and deterministic, with a replay test proving the AI attacks
or protects support units intentionally.

The full roster should not ship into campaign play until each new unit has a
combat matrix check for full HP, half HP, plain terrain, cover, HQ defense, and
counterattack outcomes.
