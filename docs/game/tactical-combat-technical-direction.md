---
title: Tactical Combat Game Technical Direction
description: Initial technical direction for the Advance Wars-like game and Steam Deck workflow
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: architecture
---

## Product Shape

The target game is a 2D, turn-based tactical combat game inspired most directly
by classic Advance Wars. It should be comfortable on a laptop and on Steam Deck,
with controller support as a first-class input model.

The initial product goal is a near-future sci-fi army tactics game with grounded
humor, 20 to 40 minute missions, AI-only opponents, CO powers, static HQ stakes,
terrain, light logistics, light within-mission veterancy, 16-bit-era pixel art,
and minor seeded randomness. Combat should remain mostly skill-weighted and
replayable.

The game should not pursue multiplayer, a map editor, weather systems, or Fire
Emblem-style named-unit attachment. Campaign progression may unlock or improve
leaders, units, and customization options, but individual combat units should not
persist across missions.

The first playable slice should prove the chokepoint HQ defense mission defined
in [First Prototype Spec](first-prototype-spec.md):

* Load a small map
* Render crisp pixel-art tiles, units, cursor, and status icons
* Move units on a tile grid with terrain costs
* Preview attack outcomes, including any small seeded variance range
* Resolve attacks through the C# simulation core
* Resolve direct attack counterattacks
* Apply terrain defense or cover effects
* Protect a static HQ from capture
* Rescue or protect a stranded scout unit
* End a turn
* Let a simple AI opponent pressure the HQ objective
* Report objective, speed, technique, power, and total score
* Run AI-vs-AI without UI input for development validation
* Save or replay the command stream

## Engine Recommendation

Godot 4.x with C# is the leading recommendation for the first prototype because
it has strong 2D tooling, tilemaps, input actions, Linux export, controller
support, text-friendly scene files, and fast iteration while matching the user's
language preference. The main risk is keeping core rules testable, so the
tactical simulation should stay engine-light and live in plain C# where
practical.

The backup option is MonoGame or FNA if the project needs a more code-first
architecture with stronger conventional test ergonomics and fewer editor-managed
assets. Unity and Unreal are not the first choice for this repo because they add
more editor, licensing, build, and asset complexity than this style of game
needs at the start.

## Visual Direction

Use a 16-bit-era pixel art style for the battlefield, units, UI icons, combat
feedback, and small character portraits. The implementation should preserve
crisp pixels with nearest-neighbor filtering, integer-friendly camera scaling,
consistent sprite and tile dimensions, and restrained animation frames that
communicate state clearly on Steam Deck.

The first Godot slice should choose placeholder tile and sprite sizes, import
settings, and camera scale rules that avoid blurry scaling at 1280x800. Use
placeholder assets only when they still match the pixel-art direction.

## Architecture Direction

Keep the rules model deterministic and replayable:

* Board state uses integer coordinates and explicit terrain data.
* Commands represent all player and AI actions.
* Combat calculations use integer math and stable tie-breaking.
* Minor randomness uses an explicit seed stored in replay data.
* Forecasts expose deterministic values and possible seeded variance ranges.
* The engine layer renders state and collects input but does not own rules.
* Save files include schema version, rules version, state, and command history.
* AI players use the same command interface as human players.

## Steam Deck Direction

Design and test 1280x800 first. Every screen should work with a gamepad:

* D-pad or stick moves the tactical cursor.
* Confirm selects units and actions.
* Cancel backs out of menus and movement previews.
* Shoulder buttons cycle units and panels.
* Start opens the system menu.
* Text remains readable at handheld distance.

The deployment model should avoid Steam Store publishing:

1. Export a native Linux x86_64 build.
2. Transfer it locally to the Steam Deck using user-controlled configuration.
3. Add it as a non-Steam game for Game Mode testing.
4. Keep private Deck details in ignored local configuration.

## Agentic Development Implications

The agent ecosystem should build the game in small, verifiable increments. Good
agent-sized tasks include:

* Define the board and unit data model.
* Choose pixel-art tile, sprite, camera, and import settings.
* Implement movement range on one terrain set.
* Add combat forecast tests.
* Add a golden replay fixture.
* Add an HQ capture and defeat fixture.
* Add a scout rescue objective fixture.
* Add one CO power hook after the first mission loop is playable.
* Add a deterministic AI-vs-AI smoke test.
* Create a map import format decision.
* Add controller input mapping.
* Build a sanitized local Deck deploy script.

Avoid large vague tasks such as "make the whole game". The orchestrator should
decompose that goal into thin playable slices with objective checks.
