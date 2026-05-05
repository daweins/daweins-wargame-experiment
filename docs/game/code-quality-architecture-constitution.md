---
title: Code Quality And Architecture Constitution
description: Durable code quality, architecture, review, and autofix rules for the tactical combat game
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: architecture
---

## Purpose

This constitution defines the quality bar for the tactical combat game and the
agentic development loop that builds it. It is meant to keep the project small,
deterministic, reviewable, and playable while still allowing fast prototype
iteration.

The standards are based on established public software practices: functional
core with imperative shell, ports and adapters, Clean Architecture dependency
direction, command-based simulation, golden master tests, deterministic replay
checks, lightweight architecture decision records, and PR-quality review
evidence.

## Constitutional Values

* Playable truth beats speculative architecture.
* Deterministic rules beat hidden engine state.
* Small reversible slices beat broad rewrites.
* Tests and replay evidence beat opinion-only review.
* Controller-first usability beats desktop-only convenience.
* Public-safe records beat private logs or chat-only memory.
* No churn is better than cosmetic churn.

## Architecture Boundaries

The plain C# simulation core is the authority for tactical truth. It owns legal
commands, unit state, movement, combat, counterattacks, objectives, scoring, AI
commands, seeded randomness, replay hashes, save semantics, and rule versions.

The Godot layer is a presentation and input adapter. It may own rendering,
cursor state, selected unit state, previews, UI messages, animation, audio,
input bindings, and local presentation flow. It must not own combat math, legal
movement, mission outcomes, AI decisions, replay authority, save schema, or
direct HP and position mutation outside core commands.

The dependency direction is one way:

* Godot presentation may reference the C# core.
* Tests and smoke runners may reference the C# core.
* The C# core must not reference Godot, UI, filesystem, networking, wall-clock
  time, local device configuration, or hosted services.
* Tooling scripts may generate assets and run checks, but generated results must
  be reproducible from committed public-safe inputs.

## Dependency Policy

`Wargame.Core` should stay .NET BCL-only unless an architecture decision records
why a dependency is necessary, what risk it carries, and how it affects replay
determinism. Godot code may depend on the Godot .NET SDK and the core project.
Tests may add mainstream .NET test tooling when the smoke runner becomes too
coarse.

Do not add external coding-agent services, model-provider SDKs, telemetry,
token-based integrations, paid runtimes, or broad cloud dependencies unless the
user explicitly asks for them. Any dependency change needs review for install
scripts, transitive risk, licensing, vulnerability posture, and unexpected
network behavior.

## Determinism And Replay

Every gameplay rule must be replayable from explicit inputs:

* Initial state
* Map identity or map content hash
* Rules version
* Save schema version when saves exist
* Random seed and random generator state
* Ordered command stream

Randomness must be explicit, seeded, and forecast-visible. Stable tie-breaking
is required for movement, AI choices, targeting, scoring, and replay hashing.
Combat math should use integer rules unless a documented reason exists.

Forecasts and resolution must come from the same rule model. If a forecast
changes, tests or replay fixtures must show whether the change is intentional.

## Quality Gates

Run the smallest meaningful deterministic checks for the change. Current gates
include:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln`
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj`
* Godot headless startup for project-loading and asset-loading checks
* VS Code diagnostics for touched C#, Markdown, JSON, and YAML files
* Repo secret scan before commit, push, publishing, or PR creation

As the project grows, add stricter gates before expanding content:

* Focused unit tests for movement, terrain, combat, objectives, scoring, AI,
  save/load, and turn transitions
* Golden replay fixtures with expected final hash and outcome
* Screenshot checks at 1280x800, then 1280x720 and 1920x1080
* Controller-flow checks for cursor, inspect, select, cancel, undo, attack,
  wait, end turn, menu, and restart
* Format and analyzer verification after shared configuration exists
* CI checks once the repository workflow is ready

## Periodic Review Triggers

Code review is risk-triggered rather than calendar-only. Run a review pass when
any of these happen:

* A meaningful code slice changes gameplay, UI flow, tools, assets, prompts, or
  instructions.
* Three to five autonomous slices have completed without an architecture review.
* A change touches both `src/Wargame.Core` and `game/WargamePrototype`.
* A change touches replay, save schema, scoring, AI, randomness, dependencies,
  CI, deployment, security, or generated artifacts.
* A check fails, behavior changes unexpectedly, or a human reports confusion.
* Work is about to be committed, pushed, published, or turned into a pull
  request.

Review should involve the smallest useful independent perspective. Use Test
Evaluator for verification gaps, Game Architect for boundary drift, Security
Sentinel for public-safety risk, Adversarial Critic for weak assumptions, and
human feedback for product or taste judgment.

## Review Finding Taxonomy

Classify every review suggestion before fixing it:

* Mechanical: formatting, whitespace, naming, imports, obvious typos, generated
  files refreshed from committed inputs
* Test-backed bug: a failing or missing check exposes a concrete behavior risk
* Architecture boundary: dependency direction, core/Godot ownership, replay
  viability, save schema, dependency policy
* Gameplay or product judgment: balance, mission shape, AI personality, scoring,
  visuals, controller feel, text tone
* Security or public-safety issue: secrets, private paths, credentialed tools,
  ignored files, dependency or workflow risk
* Speculative cleanup: refactors without a concrete bug, risk, or simplification

No-issue reviews should produce no edits. Formatting churn, renames, and doc
reshuffling are not valuable unless they fix a real issue or reduce verified
risk.

## Autofix Protocol

Autofix accepted review suggestions in the same pass when the suggestion is
bounded, local, public-safe, and verifiable. Mechanical fixes can be accepted by
the implementing agent when diagnostics or deterministic checks confirm them.

Behavior-changing fixes need independent acceptance before they are applied.
Independent acceptance can come from the user, a relevant read-only specialist,
pre-existing documented behavior, a pre-existing failing test or replay fixture,
or an explicit tracked decision that names its independent evidence. A newly
authored test or decision by the same implementer does not by itself count as
independent acceptance.

Before applying an autofix, identify:

* Finding category
* Expected behavior impact
* Files likely touched
* Verification plan
* Rollback path

After applying an autofix, rerun the impacted checks and record public-safe
evidence. If the fix changes replay hashes, mission balance, save format,
dependency surface, UI readability, or future agent behavior, record the reason
in the development log or decision log.

## Autofix Stop Conditions

Stop immediately for a security hard stop when a suggestion requires or risks
any of these:

* Reading, printing, storing, or committing secrets or secret-like values
* Reading `.env*`, private keys, credential stores, local MCP config, ignored
  logs, or private Steam Deck details
* Suspected exposure of credentials, private hostnames, private device details,
  connection strings, or credentialed tool output

Route to human intervention or independent review for non-secret issues that
require judgment:

* Adding broad MCP permissions, CI permissions, credentials, external services,
  paid runtimes, model-provider SDKs, telemetry, or networked integrations
* Destructive git operations or unrelated cleanup
* Ambiguous dependency upgrades or install-script changes
* Unreviewed gameplay balance, AI heuristics, scoring, replay/save schema, or
  architecture boundary changes
* Generated artifacts whose source cannot be reproduced from committed inputs

When recording a security issue, include only the safe category and location.
Do not quote secret-like values.

## Evidence Standard

Every meaningful pass should leave reviewer-ready evidence:

* Objective and scope
* Files changed
* Review findings and categories
* Accepted suggestions and fixes applied
* Checks run and results
* Residual risk
* Next safe action

This evidence belongs in tracked public-safe files when it is durable. Raw logs,
private paths, transcripts, screenshots with sensitive content, and runtime
details should not be stored. If transient local logs are unavoidable, keep them
ignored, minimized, sanitized, and never quoted or committed.

## Revisit Triggers

Revisit this constitution when:

* The core/Godot boundary repeatedly blocks useful work.
* Replay or save support becomes a first-class system.
* CI becomes available and can own more gates.
* The project adds a real test framework or analyzer configuration.
* Steam Deck validation moves from local trial to repeatable workflow.
* Review passes create churn without finding meaningful defects.
* A security incident or near miss exposes a gap in the stop rules.
