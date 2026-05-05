---
title: Agent Language Policy
description: Default language and implementation bias for agentic coding work in this repository
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Default Coding Language

All implementation work in this repository should be biased to C# by default.

This applies to:

* Gameplay and simulation code
* Tooling used directly by gameplay systems
* Automation that can reasonably be implemented in .NET
* Agent-generated examples and scaffolding
* Asset generation and tooling (e.g., sprite and cutscene generators)

**Current Status:** The repository is 100% C#-first. All asset generators,
rules core, presentation layer, tests, and tools use C# or C#-compatible
technologies (.NET, Godot C#). No Python, TypeScript, or other languages are
present.

## Allowed Exceptions

Python or other languages are acceptable only when one of these is true:

* A required third-party workflow is language-bound and cannot be wrapped in C#.
* The user explicitly requests another language for a specific task.
* Integrating with an established external tool ecosystem that requires a
  specific language (rare).

When using an exception language, note the reason in the change summary.

## Practical Rule For Agents

Before creating new code files, ask:

1. Can this be done in existing C# projects cleanly?
2. Is there an existing non-C# script this should extend instead of duplicating?

If question 1 is yes, implement in C#.
If question 2 is yes, extending the existing script is acceptable.
Otherwise, default to C#.
