---
title: Agentic Experiment Queue
description: Experiments for improving the product and the autonomous development loop
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Experiment Protocol

Experiments should test assumptions with observable evidence. Prefer small
changes that can be evaluated by deterministic checks, replay fixtures,
screenshots, playtest notes, or measurable workflow outcomes.

Status values: `proposed`, `active`, `complete`, `inconclusive`, `dropped`.

## Proposed Experiments

### E001: Repo continuity

Status: `proposed`

Hypothesis: Repo-based tracking improves autonomous continuity across
invocations.

Method: Require each loop pass to update state, backlog, development log,
critique, and experiments.

Evidence: The next pass can resume without relying on chat history.

### E002 Outcome: Testable Godot prototype

Status: `proposed`

Hypothesis: A Godot-first tactical prototype can still keep rules testable.

Method: Build the smallest board and unit model with a testable rules boundary.

Evidence: Movement and combat logic can be checked outside rendering.

### E003: Critique-driven slice selection

Status: `proposed`

Hypothesis: Adversarial critique improves slice selection.

Method: Require critique before marking a work item done.

Evidence: Backlog changes reflect identified risks or falsifying tests.

## Active Experiments

### E004: First mission trial playthrough

Status: `active`

Hypothesis: A thin Godot C# vertical slice can support a useful first human
trial before campaign systems, production, fog, or polished art exist.

Method: Let the user play the first mission, then collect notes on objective
clarity, control friction, AI pressure, scoring, and 16-bit pixel-art
readability.

Evidence: Awaiting manual playthrough feedback.

## Completed Experiments

### E002: Testable Godot prototype

Outcome: `complete`

Evidence: A Godot C# project references a plain C# rules core. The smoke test
runner validates movement, terrain forecast, scout rescue, HQ defeat, replay
hash determinism, AI pressure, and score categories outside Godot.
