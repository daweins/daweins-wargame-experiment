---
title: Tactical Combat Game Technical Direction
description: Initial technical direction for the Advance Wars-like game and Steam Deck workflow
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: architecture
---

## Product Shape

The target game is a 2D, turn-based tactical combat game inspired by Advance
Wars. It should be comfortable on a laptop and on Steam Deck, with controller
support as a first-class input model.

The first playable slice should prove a tiny complete loop:

* Load a small map
* Move a unit on a tile grid
* Preview attack outcomes
* Resolve an attack deterministically
* End a turn
* Let a simple opponent act or wait
* Save or replay the command stream

## Engine Recommendation

Godot 4.x is the leading recommendation for the first prototype because it has
strong 2D tooling, tilemaps, input actions, Linux export, controller support,
text-friendly scene files, and fast iteration. The main risk is keeping core
rules testable, so the tactical simulation should stay engine-light.

The backup option is MonoGame or FNA if the project needs a more code-first
architecture with stronger conventional test ergonomics and fewer editor-managed
assets. Unity and Unreal are not the first choice for this repo because they add
more editor, licensing, build, and asset complexity than this style of game
needs at the start.

## Architecture Direction

Keep the rules model deterministic and replayable:

* Board state uses integer coordinates and explicit terrain data.
* Commands represent all player and AI actions.
* Combat calculations use integer math and stable tie-breaking.
* Randomness, if introduced, uses an explicit seed stored in replay data.
* The engine layer renders state and collects input but does not own rules.
* Save files include schema version, rules version, state, and command history.

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
* Implement movement range on one terrain set.
* Add combat forecast tests.
* Add a golden replay fixture.
* Create a map import format decision.
* Add controller input mapping.
* Build a sanitized local Deck deploy script.

Avoid large vague tasks such as "make the whole game". The orchestrator should
decompose that goal into thin playable slices with objective checks.
