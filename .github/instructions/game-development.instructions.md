---
description: "Use when designing or implementing the tactical combat game, Steam Deck workflow, rules, maps, AI, tests, or deployment"
applyTo:
  - "docs/game/**"
  - "game/**"
  - "combat-core/**"
  - "tools/**"
  - "tests/**"
---

# Game Development Instructions

## Direction

The current default technical direction is a 2D turn-based tactical combat game
inspired by Advance Wars, with deterministic rules, command logs, replayable
turns, controller-first UI, laptop playability, and Steam Deck deployment
outside the Steam Store.

Use Godot 4.x as the leading engine candidate unless a later decision record
selects a different stack. Keep the simulation core testable and independent
from rendering wherever practical.

Follow [Code Quality And Architecture Constitution](../../docs/game/code-quality-architecture-constitution.md)
for architecture boundaries, quality gates, periodic review triggers, and
autofix rules.

## Design Constraints

* Design first for 1280x800 and 16:10, then validate 1280x720 and 1920x1080.
* Make all core flows gamepad-first: move cursor, inspect unit, select action,
  confirm, cancel, cycle units, end turn, open menu, and review objectives.
* Keep combat deterministic. Use integer tile coordinates, seeded random number
  generation if randomness is introduced, and stable command ordering.
* Store replay data as initial state, rules version, seed, and command stream.
* Add tests for movement, combat forecast, terrain effects, capture, economy,
  turn transitions, save/load, and replay determinism before expanding content.
* Prefer small maps and golden scenarios for automated playtesting.
* Review code when gameplay rules, AI, scoring, replay, save/load, controller
  flow, generated assets, or the Godot/core boundary changes.
* Autofix mechanical and test-backed review findings when checks can verify the
  result. Do not silently autofix balance, mission design, replay format, save
  schema, architecture boundary, dependency, or security changes without
  independent acceptance.

## Steam Deck Workflow

Do not commit Steam Deck credentials, hostnames, IP addresses, SSH keys, or
usernames. Keep local deployment configuration in ignored files such as
`.env.local` or in an OS credential store.

The intended deployment flow is:

1. Build a native Linux x86_64 package on the laptop or CI.
2. Copy the build to the Steam Deck over a user-controlled local channel such as
   SSH or Syncthing.
3. Add the executable as a non-Steam game for Game Mode testing.
4. Record only sanitized deployment notes in repo docs.

## Agentic Development Expectations

Agents should keep game systems small, testable, and inspectable. When adding a
feature, define the rule behavior, add focused tests or replay fixtures, update
UX controls, and record verification evidence.
