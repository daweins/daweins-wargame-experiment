---
description: "Discuss one human intervention item, options, tradeoffs, and non-blocking alternatives"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "item=ID [guidance=...]"
---

# Human Intervention Discuss

## Inputs

* ${input:item}: Required intervention item ID.
* ${input:guidance}: Optional human guidance, constraints, or decision context.

## Requirements

1. Read `.copilot-tracking/agentic/human-intervention.md` and related tracking
   files.
2. Summarize the selected item, why it needs human intervention, and why it is
   not a security hard stop.
3. Present available options, tradeoffs, recommended choice, and the safe work
   the autonomous loop can continue in parallel.
4. If guidance is provided, update the item status, discussion notes, or
   disposition without storing sensitive values.
5. If the discussion reveals a security issue, stop and follow the security
   model instead of continuing the non-security flow.

Do not request or record credentials, private device details, or secret values.
