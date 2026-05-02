---
description: "Assess autonomous loop progress, critiques, experiments, state, and next safe work without broad implementation"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "[focus=progress|risk|next|experiments|product] [guidance=...]"
---

# Agentic Loop Assess

## Inputs

* ${input:focus:progress}: Optional assessment focus.
* ${input:guidance}: Optional non-blocking human guidance to incorporate into
   assessment and priorities.

## Requirements

1. Read current docs, customizations, changed files, and all
   `.copilot-tracking/agentic/` work tracking files.
2. Summarize progress since the last assessment.
3. Identify blockers, risks, missing gates, stale assumptions, and false
   progress.
4. Ask Security Sentinel for review if the focus involves credentials, private
   device details, MCP, hooks, GitHub Actions, generated artifacts, or possible
   secret exposure.
5. Enumerate open human-intervention items when the focus involves publishing,
   remote mutation, deployment, destructive operations, external services, or
   product choices.
6. Ask Adversarial Critic for challenge findings and Experiment Planner for the
   next highest-value experiment.
7. Recommend the next three safe autonomous actions in priority order, including
   work that can continue without waiting on intervention items.
8. Update state, backlog, critiques, experiments, metrics, feedback status, or
   intervention items when doing so does not expose sensitive details.

Do not implement broad changes unless the user explicitly asks.
