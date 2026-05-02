---
title: Agentic Work Tracking System
description: Repo-based work tracking and development log system for autonomous Copilot loops
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: overview
---

## Purpose

This directory is the durable, repo-based operating memory for autonomous
development. It keeps high-level goals, current state, backlog, development
history, human guidance, critiques, experiments, decisions, and metrics in
tracked Markdown files.

The active loop should update these files as it works. Local run transcripts,
raw logs, private prompts, and machine-specific details belong under ignored
runtime paths, not here.

## Files

* [active-goal.md](active-goal.md) records the current product goal, constraints,
  autonomy policy, and stop conditions.
* [state.md](state.md) records the latest autonomous-loop state and next action.
* [backlog.md](backlog.md) is the prioritized queue of agent-sized work items.
* [development-log.md](development-log.md) is the append-only public-safe work
  history.
* [human-feedback.md](human-feedback.md) captures non-blocking human guidance,
  judgments, and nudges.
* [human-intervention.md](human-intervention.md) tracks non-security items that
  need human judgment or action while autonomous work continues elsewhere.
* [critiques.md](critiques.md) records adversarial reviews and unresolved risks.
* [experiments.md](experiments.md) tracks proposed, active, and completed
  experiments.
* [decisions.md](decisions.md) records durable architecture, process, and product
  decisions.
* [metrics.md](metrics.md) tracks product, process, quality, and safety signals.

## Safety Rule

Do not store secrets, credentials, private hostnames, Steam Deck connection
details, local file system secrets, ignored runtime logs, or raw tool output in
these files. Store only public-safe summaries.
