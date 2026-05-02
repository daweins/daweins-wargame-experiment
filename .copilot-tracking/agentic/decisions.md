---
title: Agentic Decision Log
description: Public-safe decisions for the autonomous development system and game direction
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Decision Protocol

Record durable decisions that affect future autonomous work. Include context,
decision, alternatives, verification, and revisit triggers.

## Decisions

### D-2026-05-02-001: Copilot-only active agentic stack

Context: The user wants to use GitHub Copilot because that is where available
tokens and workflow investment already exist.

Decision: Use GitHub Copilot, VS Code custom agents, prompt files, hooks,
GitHub branches, pull requests, GitHub Actions, and local scripts as the active
agentic stack.

Alternatives: External coding-agent services, model-provider APIs, or paid
agent runtimes.

Verification: Repository instructions and blueprint enforce the Copilot-only
constraint.

Revisit trigger: The user explicitly asks to adopt another runtime.

### D-2026-05-02-002: Autonomous by default with safety hard stops

Context: The user wants high-level goals to drive continual improvement and
human involvement to be non-blocking product judgment.

Decision: The development loop should choose, implement, verify, critique,
record, and continue autonomously within invocation limits. Human input should
shape goals and priorities without blocking routine progress.

Alternatives: Human approval between every phase or a fully unguarded loop.

Verification: Prompts, orchestrator instructions, and tracking artifacts encode
non-blocking human feedback plus explicit hard-stop conditions.

Revisit trigger: The loop repeatedly makes low-quality decisions or safety risk
increases.
