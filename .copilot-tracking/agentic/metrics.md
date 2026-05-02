---
title: Agentic Metrics
description: Product, process, quality, and safety signals for autonomous development
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Metrics Protocol

Track lightweight signals that help the autonomous loop choose useful work and
notice false progress. Update metrics when evidence changes.

## Current Signals

### Public-safety scan

Current value: Passing

Target: Passing before commit or publish

Evidence: Repo scan reported no obvious secret patterns.

### Markdown diagnostics

Current value: Passing

Target: Passing after docs or prompt edits

Evidence: VS Code diagnostics reported no errors after tracking and prompt
updates.

### Active game implementation

Current value: Not started

Target: First playable slice exists

Evidence: No game code yet.

### Deterministic tests

Current value: Not started

Target: First rules test exists

Evidence: No simulation code yet.

### Replay fixture coverage

Current value: Not started

Target: First replay fixture exists

Evidence: No command log yet.

### Steam Deck workflow

Current value: Not started

Target: Sanitized local config schema exists

Evidence: Direction documented only.

### Autonomous continuity

Current value: Scaffolded

Target: Next pass resumes from tracked state

Evidence: Work tracking files, autonomous prompt, critique agents, and
experiment tracking are in place.
