---
description: "Code quality, architecture, periodic review, and autofix rules for the tactical combat project"
applyTo:
  - "**/*.cs"
  - ".github/agents/**"
  - ".github/instructions/**"
  - ".github/prompts/**"
  - ".github/copilot-instructions.md"
  - "docs/game/**"
  - "game/**"
  - "src/**"
  - "scripts/**"
---

# Code Quality And Architecture Instructions

Follow [Code Quality And Architecture Constitution](../../docs/game/code-quality-architecture-constitution.md)
when changing game code, tools, assets, or game-facing documentation.

## Core Rules

* Keep tactical rules in the plain C# core, not in Godot presentation code.
* Keep Godot responsible for rendering, input, UI, animation, and presentation
  state.
* Keep gameplay deterministic, seeded, command-driven, and replayable.
* Add or update focused checks when gameplay rules, AI, scoring, replay,
  save/load, controller flow, or generated assets change.
* Avoid new dependencies unless the need, risk, and architecture impact are
  recorded.
* Preserve public safety: never read, print, store, or commit secrets, private
  device details, credential stores, ignored logs, local MCP config, or private
  deployment values.

## Periodic Review And Autofix

Run a code review pass after meaningful code slices, before commits or pull
requests, after three to five autonomous slices, and whenever a change touches
core rules, replay/save data, AI, randomness, dependencies, deployment,
security, instructions, prompts, or generated artifacts.

Classify review suggestions before fixing them:

* Mechanical
* Test-backed bug
* Architecture boundary
* Gameplay or product judgment
* Security or public-safety issue
* Speculative cleanup

Autofix accepted suggestions immediately when they are bounded, local,
public-safe, and verifiable. Mechanical fixes may be accepted by the
implementing agent when checks confirm them. Behavior-changing fixes need
independent acceptance from the user, a relevant read-only specialist,
pre-existing documented behavior, a pre-existing failing test or replay fixture,
or a tracked decision that names its independent evidence. A newly authored test
or decision by the same implementer is not independent acceptance.

Do not perform cosmetic churn from a no-issue review. Stop immediately for a
security hard stop when secrets, private device details, credential stores,
ignored logs, local MCP config, or credentialed tool output are involved. Route
non-secret broad permissions, ambiguous dependency changes, destructive
operations, unreproducible generated artifacts, or unreviewed semantic gameplay
or architecture changes to human intervention or independent review.

After autofix, rerun impacted checks and record public-safe evidence of the
finding, fix category, files touched, verification, residual risk, and next
safe action.
