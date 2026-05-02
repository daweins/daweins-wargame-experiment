---
title: Copilot Agentic Programming Ecosystem Blueprint
description: Copilot-only architecture for long-running multi-agent development of the tactical combat game
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: architecture
---

## Goal

Create a GitHub Copilot-centered development ecosystem that can accept a high
level objective, decompose it into work, execute continual development with
minimal human steering, critique its own progress, run small experiments, and
keep enough repo-based state to resume without relying on chat history.

The system must never expose credentials, auth tokens, private host details, or
sensitive local data to public repository state, logs, prompts, issues, pull
requests, or generated artifacts.

## Research Synthesis

Public autonomous coding systems have converged on the same operating loop:
intake a bounded task, inspect context, make a plan, edit in isolation, run
verification, preserve evidence, and return control through a diff, branch,
pull request, or review gate.

The strongest lesson for this repo is that the durable system of record should
not be one long chat. Use explicit artifacts:

* Repository instructions for standing policy
* Custom agents for role behavior and tool boundaries
* Prompt files for repeatable workflows
* Hooks and scripts for deterministic safety checks
* Repo-based work tracking for goals, state, backlog, feedback, human
  intervention, critique, experiments, decisions, metrics, stop, resume, audit,
  and handoff
* Git branches and pull requests for reviewable changes
* Tests and replay fixtures as objective gates

## Copilot-Only Stack

The active stack is:

* VS Code GitHub Copilot Chat for local agent mode and custom agents
* Workspace custom agents in `.github/agents/`
* Prompt files in `.github/prompts/`
* Always-on instructions in `.github/copilot-instructions.md`
* File-scoped instructions in `.github/instructions/`
* Optional hooks in `.github/hooks/`
* Local scripts under `scripts/`
* GitHub Copilot cloud agent for background branch and PR work after the repo is
  hosted on GitHub
* GitHub Actions for repeatable setup, validation, and later build pipelines

Do not add OpenAI Codex, Claude Code, Devin, Cursor agents, external AutoGen or
LangGraph runtimes, CrewAI, or other model-provider execution systems as
required dependencies. Their public designs can inform patterns, but this repo's
execution path stays inside Copilot unless the user explicitly changes that
decision.

## Agent Roles

The first role set is intentionally small:

* Strategic Orchestrator coordinates the loop, maintains repo work tracking,
  decomposes work, delegates to specialists, and decides continuation or hard
  stop points.
* Product Strategist turns high-level game goals into scoped player value,
  milestones, acceptance criteria, and backlog slices.
* Game Architect owns technical direction, architecture decisions, deterministic
  simulation, engine boundaries, and Steam Deck constraints.
* Implementation Engineer edits code and docs, runs targeted commands, and keeps
  changes small enough to review.
* Test Evaluator designs and runs verification, replay scenarios, regression
  checks, and quality gates.
* Security Sentinel reviews secret handling, public-safety boundaries, tool
  access, dependency risk, and generated artifacts.
* Steam Deck Integrator owns deployment workflow design, controller-first UX,
  Linux builds, and laptop-to-Deck iteration.
* Adversarial Critic challenges plans and changes by finding weak assumptions,
  false progress, missing evidence, and pressure tests.
* Experiment Planner turns uncertainty into small falsifiable experiments and
  recommends the next evidence-producing action.

## Long-Running Loop

The loop is autonomous by default and stop controlled by the user. Within
current Copilot capabilities, true unbounded execution is approximated through
long autonomous prompt invocations, repeated resumable invocations, and optional
Copilot cloud-agent tasks after the repo is hosted. The orchestrator keeps state
in repo work tracking files so repeated invocations behave like a durable loop.

```text
Ingest -> Prioritize -> Delegate -> Implement -> Validate -> Critique -> Experiment -> Route Intervention -> Record -> Continue
```

Each iteration should:

1. Read the latest public docs and `.copilot-tracking/agentic/` state.
2. Select the highest-value safe goal or a small batch of compatible goals.
3. Ask specialist agents for bounded outputs when useful.
4. Make focused safe changes without waiting for routine approval.
5. Run the smallest meaningful checks.
6. Ask adversarial and experiment agents to challenge the result.
7. Record verification evidence, critique, experiment updates, and residual
  risks.
8. Log non-security human decisions in `human-intervention.md` and route around
  them when possible.
9. Continue to the next useful safe action while invocation budget remains.

## Repo Work Tracking

Tracked work files live under `.copilot-tracking/agentic/`. They are the
public-safe system of record for autonomous work:

* `active-goal.md` for the current high-level goal and autonomy policy
* `state.md` for current status and next actions
* `backlog.md` for prioritized work
* `development-log.md` for append-only work history
* `human-feedback.md` for non-blocking human guidance
* `human-intervention.md` for non-security human decisions and actions
* `critiques.md` for adversarial findings
* `experiments.md` for proposed and completed experiments
* `decisions.md` for durable decisions
* `metrics.md` for evidence signals

Raw runtime logs, local paths, and private run details belong under ignored
runtime paths such as `.copilot-tracking/agentic/runs/`.

## Human Role, Intervention, And Security Stops

The human role is non-blocking product judgment: review current state, adjust
goals, provide taste notes, add feedback, and nudge priorities. The loop should
incorporate that input and continue.

Non-security human decisions should be logged in `human-intervention.md` and
should not halt unrelated autonomous work. Examples include choosing between
product options, approving remote mutation, deciding whether to publish or
deploy, or deciding whether to perform destructive cleanup.

The loop halts only for security-sensitive conditions such as:

* Configuring any MCP server with write tools or credentials
* Adding secrets to GitHub environments
* Reading or exposing credentials, private device details, ignored secret files,
  local MCP configuration, private key files, credential stores, or suspected
  secret-like values

## Quality Gates

Early in the project, quality gates are mostly documentation and safety checks.
As the game code appears, gates should become stricter:

* Secret scan before commit and before publishing
* Unit tests for deterministic combat rules
* Replay determinism checks
* Save/load compatibility checks
* Headless or automated smoke tests where the engine supports them
* Steam Deck resolution and controller smoke checks
* PR summary with tests run and residual risks

## Near-Term Roadmap

1. Finish the Copilot customization layer.
2. Use repo work tracking as the current source of truth for autonomous work.
3. Decide the initial game stack, with Godot 4.x plus a deterministic core as
   the current leading recommendation.
4. Create a small technical spike for one unit, one terrain type, movement, and
   attack forecast.
5. Add tests and replay fixtures before expanding content.
6. Add a local Deck deployment script that reads ignored configuration and
   prints sanitized output.
