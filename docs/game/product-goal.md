---
title: Tactical Combat Product Goal
description: Initial product goal for the Steam Deck-first tactical combat game
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Product Thesis

Build a realistic near-future sci-fi tactical combat game for laptop and Steam
Deck that captures the army-command clarity of Advance Wars while adding
grounded political conflict, dry humor, CO identity, light logistics,
16-bit-era pixel art, and deterministic replayability.

The player should feel like a clever field commander making readable tactical
choices under pressure. Battles should mostly reward planning and positioning,
with small seeded variance to keep outcomes lively without making success feel
arbitrary.

## Core Experience

The game is closest to classic Advance Wars. It should prioritize disposable
units, property captures, terrain-aware positioning, unit counters, CO
advantages, static HQ stakes, and 20 to 40 minute missions against AI opponents.

The campaign can offer broader progression through leaders, experience, unit
unlocks, and unit customization or improvement. Individual combat units should
not become named persistent party members. Within a mission, light veterancy is
welcome when it encourages careful unit use without turning every loss into a
campaign setback.

## Design Pillars

### Readable Army Tactics

Maps, units, attacks, captures, terrain, supply, and CO effects should be
legible at handheld distance. The player should understand why a forecast looks
the way it does before committing to an action.

### 16-Bit Pixel Clarity

Graphics should use a 16-bit-era pixel art style. Units, terrain, UI icons, and
combat feedback should read crisply on Steam Deck without high-resolution
illustration, blurry scaling, or overly busy detail.

### Skill-Weighted Outcomes

Combat may include minor seeded randomness, but strategy should dominate.
Forecasts must expose the possible outcome range and replay data must store the
seed, rules version, initial state, and command stream.

### Commander Personality

Commanding officers are a main source of faction identity. They should shape
tactics through charge rules, powers, and strategic preferences without
overwhelming the unit-counter foundation. HQs are static and mission-critical,
with HQ capture creating immediate win or loss conditions.

### Grounded Political Sci-Fi

The campaign should prioritize believable incentives: resource concessions,
industrial liability, public-safety authority, civilian infrastructure,
strategic parity, regulatory capture, and frontier governance. Rare materials
and automated systems must have supply-chain limits, legal stakeholders,
failure modes, and visible map objectives. Avoid mystical macguffins, ancient
destiny, sentient infrastructure, or technology that solves logistics by
assertion.

Interstellar travel should reinforce those politics. The setting uses scheduled
beam corridors, fusion pushers, magsail braking, depots, manifests, and courier
relays rather than instant FTL travel. FTL messaging exists through fixed,
low-bandwidth, audited Spindle Net stations, so orders and sanctions can outrun
ships while people, parts, fuel, and armies remain constrained by transit
logistics. Freight slots, braking rights, message priority, relay
authentication, insurance, and transit sanctions are political weapons.

### Grounded Humor

The tone should be grounded and readable, with a sense of humor. Jokes should
come from character voice, absurd military bureaucracy, field improvisation, and
near-future sci-fi weirdness rather than undercutting the stakes of battle.

### AI-First Play

The game is single-player only. There is no map editor and no multiplayer goal.
For development and balancing, the simulation should support full AI-vs-AI runs
with deterministic logs and replayable outcomes.

## Initial Feature Scope

The first playable prototype should prove the chokepoint HQ defense mission
defined in [First Prototype Spec](first-prototype-spec.md). The smallest
complete battle loop should include:

* Load a small test map.
* Render the map, units, cursor, and status icons in a crisp 16-bit pixel art
  style.
* Move units on a tile grid with terrain costs.
* Preview combat with visible deterministic and seeded-variance components.
* Resolve direct attacks and counterattacks.
* Apply terrain defense or cover effects.
* Protect a static HQ and resolve immediate defeat if it is captured.
* Rescue or protect a stranded scout unit.
* End turns between player and AI.
* Report objective, speed, technique, power, and total score.
* Run AI-vs-AI without UI input.
* Save and replay the command stream.

## Non-Goals

The initial product direction excludes these features:

* Multiplayer
* Map editor
* Weather systems
* Fire Emblem-style named-unit attachment
* Persistent individual combat units across the campaign
* Grand-strategy diplomacy, research trees, or large logistical simulation

## Technical Preference

Use Godot 4.x with C# if practical. Keep tactical rules in a testable C# core
that is independent from Godot rendering and input. Godot should present state,
collect controller-first input, and run the battle scene, while the simulation
core owns legal actions, forecasts, combat resolution, AI commands, and replay
validation.
