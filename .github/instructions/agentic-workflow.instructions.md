---
description: "Use when creating or editing agentic workflow docs, repo work tracking, prompts, agents, and orchestration artifacts"
applyTo:
  - ".copilot-tracking/agentic/**"
  - "docs/agentic-programming/**"
  - ".github/agents/**"
  - ".github/prompts/**"
---

# Agentic Workflow Instructions

## Scope

These instructions apply to the Copilot-based agentic development ecosystem for
this repo. They govern repo work tracking, custom agents, prompts,
orchestration docs, and long-running autonomous development workflows.

## Principles

* Keep the active implementation stack inside GitHub Copilot and VS Code unless
  the user explicitly approves another runtime.
* Treat public research about other coding-agent systems as inspiration, not as
  a dependency plan.
* Use explicit loop phases: ingest, prioritize, delegate, implement, validate,
  critique, experiment, route intervention, record, and continue.
* Treat human feedback as non-blocking guidance for goals, priorities, and
  product judgment. Halt only for security hard stops.
* Log non-security items requiring human judgment in
  `.copilot-tracking/agentic/human-intervention.md`, then continue other useful
  safe work wherever possible.
* Use tracked repo work files for durable public-safe state. Update the current
  state instead of relying on chat history alone.
* Keep role agents bounded. A role should have a clear purpose, limited tool
  access, and an output contract.
* Prefer repeatable repo-local commands and tests over free-form shell
  exploration.
* Never store credentials, tokens, private device details, or sensitive local
  paths in ledgers, prompts, or research artifacts.

## Repo Work Tracking Contract

Every autonomous loop should maintain `.copilot-tracking/agentic/` with these
tracked public-safe files:

* `active-goal.md` for the current high-level goal and autonomy policy
* `state.md` for the latest loop state and next actions
* `backlog.md` for prioritized agent-sized work items
* `development-log.md` for append-only work history and verification evidence
* `human-feedback.md` for non-blocking human guidance and dispositions
* `human-intervention.md` for non-security human decisions and actions
* `critiques.md` for adversarial findings and pressure tests
* `experiments.md` for hypotheses, methods, evidence, and outcomes
* `decisions.md` for durable process, architecture, and product decisions
* `metrics.md` for product, process, quality, and safety signals

Runtime ledgers belong under `.copilot-tracking/agentic/runs/`, which is ignored
by git because prompts, logs, and local paths may contain sensitive details.
If a summary is safe to preserve publicly, write it to the tracked work files or
to `docs/agentic-programming/`.

## Agent Output Contract

Agents should report:

* What they changed or learned
* Files touched or inspected
* Tests or checks run
* Human intervention items created, updated, or resolved
* Critique and experiment updates
* Residual risks
* Recommended next action
* Any safety concern without revealing sensitive values

## Stop And Resume

The coordinating agent should continue autonomously while safe work remains and
invocation budget allows it. It must stop when the user explicitly pauses work
or when a security concern appears. Destructive operations, publishing,
deployment, remote mutation, external service adoption, and other non-security
human decisions should be logged as intervention items while the loop continues
other useful work. Resume by reading the tracked work files and continuing from
the recorded next action.
