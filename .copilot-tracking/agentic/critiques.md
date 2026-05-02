---
title: Adversarial Critiques
description: Public-safe critique log for risks, failures, and improvement pressure
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Critique Protocol

Adversarial agents should look for weak assumptions, missing tests, product
dead ends, UX problems, security risks, hidden coupling, false progress,
overlarge slices, and experiments that would disprove the current plan.

Status values: `open`, `mitigated`, `accepted`, `rejected`, `obsolete`.

## Open Critiques

### C004: Trial prototype still needs human play evidence

Status: `open`

Area: Product

Finding: The first mission now builds and launches, but trial readiness is not
the same as proven fun, readable pressure, or balanced six-to-eight-turn play.

Recommended pressure test: Have the user play the first mission, record where
the objective, controls, forecasts, AI pressure, and pixel-art readability fail,
then tune one focused pass.

### C001: Resumable autonomy

Status: `open`

Area: Autonomy

Finding: A prompt invocation cannot literally run forever, so unbounded
improvement needs resumable state and repeated self-directed passes.

Recommended pressure test: Verify every pass updates state, backlog, log,
critique, and next action.

### C003: Autonomy safety risk

Status: `open`

Area: Safety

Finding: More autonomy increases risk of accidentally handling private data.

Recommended pressure test: Keep hard-stop conditions narrow, explicit, and
enforced by scanner plus instructions.

## Mitigated Critiques

### C002: Unproven game direction

Mitigation: Implemented a first playable Godot C# mission slice backed by a
plain C# rules core and smoke checks.
