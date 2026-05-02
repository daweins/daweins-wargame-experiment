---
description: "Kick off a Copilot-only autonomous development loop with repo work tracking, critique, experiments, and non-blocking human guidance"
agent: Strategic Orchestrator
tools: [read, search, edit, execute, todo, agent]
argument-hint: "goal=... [constraints=...] [guidance=...]"
---

# Agentic Loop Kickoff

## Inputs

* ${input:goal}: Required high-level goal for the loop.
* ${input:constraints:Use GitHub Copilot only; do not expose secrets}: Optional constraints or boundaries.
* ${input:guidance}: Optional non-blocking human guidance, taste notes, or
   priority nudges.

## Requirements

1. Read the repository Copilot instructions, ecosystem blueprint, security
   model, operating manual, game technical direction, and
   `.copilot-tracking/agentic/` work tracking files if they exist.
2. Record or update the active goal, constraints, assumptions, autonomy policy,
   and non-blocking human guidance in `.copilot-tracking/agentic/`.
3. Decompose the goal into thin, testable backlog items with verification ideas.
4. Consult Product Strategist, Game Architect, Test Evaluator, Security
   Sentinel, Steam Deck Integrator, Adversarial Critic, and Experiment Planner
   when their perspectives materially improve the kickoff.
5. Seed or update `state.md`, `backlog.md`, `development-log.md`,
   `human-feedback.md`, `critiques.md`, `experiments.md`, `decisions.md`, and
   `metrics.md` with public-safe information.
6. Do not request or record credentials, auth tokens, private hostnames, Steam
   Deck connection details, or local secret values.
7. Begin the first safe autonomous implementation slice when it can be done
   without secrets, credentials, private device details, or unsafe access to
   sensitive data.
8. If non-security human intervention is needed, record it in
   `human-intervention.md` and continue with other useful setup or product work
   wherever possible.

Proceed with the kickoff using the supplied goal and constraints.
