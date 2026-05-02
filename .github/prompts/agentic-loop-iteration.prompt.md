---
description: "Run an autonomous Copilot development pass from repo work tracking until budget or hard-stop"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "[objective=active-goal] [maxSlices=auto] [guidance=...]"
---

# Agentic Loop Iteration

## Inputs

* ${input:objective:active-goal}: Optional objective. Defaults to the active
   goal and current state in `.copilot-tracking/agentic/`.
* ${input:maxSlices:auto}: Optional maximum number of work slices to execute.
   Use `auto` to continue until invocation budget, security hard stop, or no
   useful task remains.
* ${input:guidance}: Optional non-blocking human guidance to incorporate.

## Requirements

1. Read the relevant instructions, blueprint, security model, operating manual,
   and `.copilot-tracking/agentic/` work tracking files.
2. Treat human guidance as non-blocking priority input.
3. Select the highest-value safe work slice from active goal, backlog, critiques,
   experiments, metrics, and current state.
4. Delegate to specialist agents when product, architecture, testing, security,
   Steam Deck, critique, or experiment perspective would improve the outcome.
5. Implement safe selected work without waiting for human approval unless a
   security hard stop applies.
6. When non-security human intervention is needed, log or update an item in
   `.copilot-tracking/agentic/human-intervention.md`, then continue other useful
   safe work wherever possible.
7. Run the smallest meaningful checks available. If no checks exist, add or
   propose the first deterministic check.
8. Run or recommend the secret-pattern scan before treating the pass as
   complete.
9. Ask Adversarial Critic to challenge the result and Experiment Planner to
   recommend the next experiment before marking work done.
10. Update state, backlog, development log, critiques, experiments, decisions,
   metrics, and human intervention items with public-safe changes and evidence.
11. Ask Development Status Reporter to refresh the current status report when
   reporting cadence is due or meaningful tracking state changed.
12. Continue to the next useful safe item while budget remains and `maxSlices`
   allows it.

Do not use external coding-agent runtimes or expose sensitive values.
