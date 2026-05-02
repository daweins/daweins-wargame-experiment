---
description: "Record a human action or decision for an intervention item and unblock autonomous follow-up work"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "item=ID action=... [notes=...]"
---

# Human Intervention Act

## Inputs

* ${input:item}: Required intervention item ID.
* ${input:action}: Required action or decision to record.
* ${input:notes}: Optional public-safe notes about the decision.

## Requirements

1. Read `.copilot-tracking/agentic/human-intervention.md`, `backlog.md`, and
   `state.md`.
2. Record the human action or decision on the selected item using only
   public-safe information.
3. Update item status to `actioned`, `deferred`, `closed`, or another fitting
   status.
4. Convert any resulting follow-up work into backlog or state updates.
5. Identify the next useful autonomous task that can proceed.
6. If the requested action involves secrets, private device details, publishing,
   deployment, or remote mutation, record only the decision class and stop before
   performing the external action unless the user explicitly asked for that
   action in this turn and it is safe under the security model.

Do not store sensitive values in the log.
