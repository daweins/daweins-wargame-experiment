---
title: Agentic Backlog
description: Prioritized repo-based backlog for autonomous development work
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Queue Rules

Backlog items should be small enough for one autonomous pass when possible.
Each item needs a clear outcome, verification idea, and current status.

Status values: `proposed`, `ready`, `active`, `blocked`, `done`, `dropped`.

## Ready

### G001: Prototype roadmap

Status: `ready`

Outcome: First game milestone backlog is decomposed into playable slices.

Verification: Roadmap has acceptance checks and risks.

### G002: Engine spike decision

Status: `ready`

Outcome: Godot-first spike plan is explicit and reversible.

Verification: Decision entry records criteria and fallback.

### G003: Deterministic board model

Status: `ready`

Outcome: Initial board, terrain, and unit model exists.

Verification: Unit tests or golden examples pass.

## Proposed

### G004: Movement range fixture

Status: `proposed`

Outcome: One unit can compute legal movement on one terrain set.

Verification: Deterministic movement tests pass.

### G005: Combat forecast fixture

Status: `proposed`

Outcome: One attacker and defender pair has deterministic forecast.

Verification: Forecast tests pass.

### G006: Replay command log

Status: `proposed`

Outcome: Initial command stream format is defined.

Verification: Replay fixture reproduces expected state.

### D001: Sanitized Deck workflow

Status: `proposed`

Outcome: Local Deck deploy config schema is documented without values.

Verification: Security review confirms no private details.

## Blocked

No blocked items yet.

## Done

### A001: Autonomous tracking scaffold

Completed: 2026-05-02

Evidence: Repo work tracking files, autonomous prompt, adversarial roles, and
updated orchestrator instructions exist. Diagnostics and repo secret scan pass.
