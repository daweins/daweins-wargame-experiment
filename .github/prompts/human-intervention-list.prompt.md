---
description: "List human intervention items and identify what autonomous work can continue without waiting"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "[status=open|all] [focus=next|risk|blocked]"
---

# Human Intervention List

## Inputs

* ${input:status:open}: Optional status filter. Use `all` to include closed,
  deferred, and obsolete items.
* ${input:focus:next}: Optional focus for sorting and recommendations.

## Requirements

1. Read `.copilot-tracking/agentic/human-intervention.md`, `state.md`,
   `backlog.md`, `critiques.md`, `experiments.md`, and `metrics.md`.
2. Enumerate matching intervention items with ID, status, action type, decision
   needed, options, risk, and recommended human action.
3. Distinguish security hard stops from non-security intervention items. If a
   security hard stop is discovered, stop and report the sanitized issue.
4. Identify useful autonomous work that can continue without waiting for the
   human.
5. Update `state.md` only if the current next action changes and no sensitive
   details are introduced.

Do not ask the human for credentials, private hostnames, private device details,
or secret values.
