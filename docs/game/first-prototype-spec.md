---
title: First Prototype Spec
description: Focused design specification for the first playable tactical combat prototype
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Design Interview Summary

The first prototype should validate a small, tense, readable tactical battle
before adding campaign progression, production, fog, or broad unit variety.

Core design choices from product discovery:

* Normal turns should emphasize mission objectives under pressure, with capture
  and economy as the strategic engine beneath the objective layer.
* Campaign missions should vary widely, including races, holds, breakthroughs,
  rescue missions, scarcity missions, and commander-specific scenarios.
* Most full campaign missions should use classic base production, with a few
  fixed-force battles for pacing and tactical variety.
* Combat feel can vary by faction or commander, but direct attacks should use
  classic counterattacks by default.
* Combat randomness should be limited to small damage variance and rare critical
  or glancing events. Hit and miss randomness is out of scope for the core
  combat model.
* HQs are static and mission-critical. Capturing an enemy HQ wins, and losing
  the player HQ causes immediate defeat.
* CO power charging should vary by commander identity.
* CO powers may include constrained sci-fi rule-bending effects.
* Terrain should first emphasize chokepoints, movement costs and roads, and
  defense or cover bonuses.
* Light logistics should prioritize supply radius from HQs or properties, then
  ammo and fuel, then mission-specific pressures. Convoys and support units can
  appear when a mission calls for them.
* Fog of war is mission-dependent. Missions may use no fog, soft fog, or full
  fog, with scouts and sensor units supporting fog-heavy scenarios.
* Campaign progression should use mission grades to grant resources and
  capability upgrades during a campaign run.
* Campaign rewards should prioritize new unit types, unit upgrades, campaign
  resources for optional missions or branches, CO power variants, and commander
  upgrades.
* Overall campaign balance should be fixed rather than scaling to player power.
  The final campaign score evaluates how well the player used earned
  advantages.
* Most campaign advantages reset when a campaign ends. Completion can unlock
  difficulty modes, alternate commanders or factions, cosmetics, flavor, and
  stat records.
* The story should have a strong campaign narrative with character arcs.
* The central conflict is rival factions competing over a rare material needed
  for advanced units.
* The player faction is a research expedition forced to militarize.
* Opponents should include multiple rival factions rather than one permanent
  villain.
* The player faction should be mechanically distinct through flexible upgrades
  and field modifications, mostly expressed through the campaign tech tree
  rather than mid-mission tinkering.
* Graphics should use a 16-bit-era pixel art style that keeps units, terrain,
  UI icons, and combat feedback crisp at Steam Deck distance.
* The first tech tree should emphasize new unit types and production options,
  then survivability, supply efficiency, fog tools, and specialized counters.
* AI should prioritize mission objectives, play fairly and readably, show
  faction or commander personality, handle economy and captures, make tactical
  trades, and retreat or regroup when appropriate.
* AI should follow the same rules as the player by default. Clearly communicated
  asymmetric faction rules and scenario scripting are acceptable. Hidden
  cheating is not a design goal.

## First Prototype Mission

The first playable battle should prove a terrain chokepoint defense. The player
must hold the HQ, rescue a stranded scout, then defeat the remaining enemies.

The mission should be designed to resolve in six to eight turns for competent
play, but it should not use a hard turn limit. Objective resolution should end
the mission.

### Mission Goals

Primary goals:

* Prevent the player HQ from being captured.
* Rescue or protect the stranded scout unit.
* Defeat remaining enemies after the rescue is secured.

Prototype spark priorities:

* Every move feels consequential.
* The battlefield is instantly readable.
* The briefing and tone have personality.
* Forecasts and counterattacks make combat feel strategic.
* The score screen completes the loop.
* Terrain visibly changes outcomes.
* The AI pressures the objective.

### Initial Unit Roles

The first mission should use a lean unit set:

* Infantry or capture unit
* Basic armored line unit
* Fast light vehicle or scout

The first prototype should use fixed units. Base production belongs in the next
implementation slice after the basic combat, readability, and mission loop feel
good.

### Map Ingredients

The first map should be small enough to read at Steam Deck distance. Required
ingredients:

* Player HQ behind a defensible chokepoint
* A stranded scout placed beyond or near the contested chokepoint
* Roads that create fast but exposed movement paths
* Cover terrain that changes combat forecasts
* A clear enemy approach toward the player HQ
* Enough open space for the scout and light vehicle to matter

### Visual Style

The first prototype should establish the 16-bit-era pixel art look with crisp
tile art, readable unit silhouettes, clear faction color accents, visible cover
tiles, and sharp UI icons. Placeholder art is acceptable, but it should follow
the final style direction instead of using high-resolution stand-ins.

### Rules To Prove

The first prototype should prove these tactical rules:

* Tile movement with terrain costs
* Chokepoint blocking and front-line positioning
* Terrain defense or cover impact on forecasts and damage
* Direct attack counterattacks
* Small seeded damage variance and rare critical or glancing events
* Static HQ capture defeat condition
* Rescue or protect objective state
* Enemy objective pressure toward the HQ
* Numeric mission scoring

### Scoring

Use an Advance Wars-style numeric score, but tune categories to the project
priorities:

* Objective quality first, including scout rescue and HQ safety
* Speed, based on turn count
* Technique, based on avoiding unit losses
* Power, based on combat efficiency

The first prototype should show score only. Reward previews and real campaign
rewards wait until the campaign layer exists.

### Personality And Tone

The first mission should feel authored without requiring a large narrative
system. Personality priority:

* Short mission briefing
* Event or turn banter
* Unit barks and tooltips
* End-of-mission debrief
* Environmental details and object names

The humor voice should be dry scientist banter under pressure, with gallows
humor from people who are visibly out of their depth.

## First Prototype Non-Goals

Do not include these in the first prototype unless they become necessary to make
the slice coherent:

* Base production
* Artillery or indirect fire
* Full campaign tech tree
* Campaign rewards
* Map editor
* Multiplayer
* Full fog of war
* Weather
* Persistent named-unit progression
* Broad commander roster

## Acceptance Checks

The first prototype is successful when these checks pass:

* A player can inspect the map and understand the chokepoint, HQ, scout, and
  enemy pressure within a few seconds.
* Pixel art renders crisply without blur and keeps units, terrain, cursor, and
  objective markers readable at Steam Deck distance.
* Legal movement and combat forecasts are visible before commitment.
* Direct attacks resolve with counterattacks when the defender can respond.
* Terrain changes at least one meaningful forecast or damage outcome.
* The enemy AI advances toward the objective instead of only chasing kills.
* The scout rescue objective updates mission state.
* HQ capture or loss can end the mission.
* The mission can be completed in roughly six to eight turns by design.
* The end screen reports objective, speed, technique, power, and total score.
* The mission has enough briefing, banter, and UI copy to establish the research
  expedition tone.
