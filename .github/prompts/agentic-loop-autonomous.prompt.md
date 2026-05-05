---
description: "Run the autonomous Copilot loop until invocation budget, security hard stop, or no useful task remains"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "[goal=active-goal] [guidance=...] [intensity=normal|deep]"
---

# Agentic Loop Autonomous

## Inputs

* ${input:goal:active-goal}: Optional goal override. Defaults to
  `.copilot-tracking/agentic/active-goal.md`.
* ${input:guidance}: Optional non-blocking human guidance to incorporate into
  priority, critique, and next actions.
* ${input:intensity:normal}: Optional depth. Use `deep` for broader delegation,
  critique, and experiment planning.

## Requirements

1. Read the repository instructions, ecosystem blueprint, security model,
   operating manual, game direction, and `.copilot-tracking/agentic/` state
   files.
2. Treat human guidance as priority input, not approval gating. Halt only for
   security hard stops or when no useful safe work remains.
3. Select the highest-value safe work item from the backlog, active critiques,
   experiments, and current goal.
4. Delegate to relevant specialist agents. Always include Adversarial Critic
   and Experiment Planner before marking work complete.
5. Implement safe work without waiting for human approval when the work does not
   require secrets, credentials, private device details, or unsafe access to
   sensitive data.
6. When a non-security item needs human judgment or action, log it in
   `.copilot-tracking/agentic/human-intervention.md`, mark the blocked work
   appropriately, and continue other useful safe tasks wherever possible.
7. Run the smallest meaningful checks. If no checks exist, add or propose the
   first deterministic check.
8. Update `.copilot-tracking/agentic/state.md`, `backlog.md`,
   `development-log.md`, `critiques.md`, `experiments.md`, `decisions.md`,
   `metrics.md`, and `human-intervention.md` as appropriate.
9. Ask Development Status Reporter to refresh
   `.copilot-tracking/agentic/status/current-status.md` when reporting cadence
   is due. During active changes, this is at least every 30 minutes, especially
   after meaningful state changes and before ending the pass.
10. Continue to the next useful safe item while invocation budget remains.
11. Stop only when work is complete for the current budget, a security hard stop
   is reached, or no useful safe next action exists.

Do not use external coding-agent runtimes or expose sensitive values.
