---
title: Copilot Agentic Loop Operating Manual
description: How to start, guide, resume, and supervise the autonomous Copilot-based development loop
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: how-to
---

## Operating Model

The loop is autonomous by default and human stop controlled. Copilot does not
run as an invisible background daemon in this repo. Instead, each invocation
performs as much safe planning, implementation, assessment, critique, and
experiment work as the invocation budget allows, then records enough state for
the next invocation to continue.

Human guidance is non-blocking. Add goals, judgments, priorities, and nudges;
the loop should incorporate them and keep moving. Security-sensitive issues halt
the loop. Non-security items needing human judgment are logged and routed around
where possible.

## Repo Work Tracking

The durable system of record is `.copilot-tracking/agentic/`:

* `active-goal.md` records the current goal and autonomy policy.
* `state.md` records current status and next best actions.
* `backlog.md` tracks prioritized work items.
* `development-log.md` records append-only public-safe work history.
* `human-feedback.md` captures your non-blocking guidance.
* `human-intervention.md` tracks non-security items that need your judgment or
   action while autonomous work continues elsewhere.
* `critiques.md` records adversarial findings.
* `experiments.md` tracks hypotheses and evidence.
* `decisions.md` stores durable decisions.
* `metrics.md` tracks quality, product, process, and safety signals.
* `status/current-status.md` stores the latest human-readable development
   status report.

## Start A New Goal

Use `/agentic-loop-kickoff` in VS Code Chat and provide the goal. Example:

```text
/agentic-loop-kickoff goal=Create the first playable tactical combat prototype with one unit, one map, movement, attack forecast, and replay logging.
```

The orchestrator should update `.copilot-tracking/agentic/`, decompose the
goal, consult useful specialist and adversarial agents, and begin the first safe
implementation slice when no security hard stop applies.

## Run Autonomous Work

Use `/agentic-loop-autonomous` when you want the loop to keep improving until
the current invocation budget runs out, a security hard stop appears, or no
useful work remains.

```text
/agentic-loop-autonomous goal=active-goal intensity=deep
```

Use `guidance=...` to add a nudge without turning it into an approval gate.

## Read Status Reports

The Development Status Reporter refreshes
`.copilot-tracking/agentic/status/current-status.md` at least every 30 minutes
while active changes are underway, at the end of autonomous passes, after
meaningful tracking changes, before long pauses, and when you run:

```text
/development-status-report period=current
```

Reports include a full timestamp, keep old reports in
`.copilot-tracking/agentic/status/reports/`, and may link public-safe
screenshots or other image evidence. They summarize the current state, what the
project is working on, how long the effort has been underway, how the loop is
working, what has worked well, where challenges have appeared, open
human-intervention items, and next useful autonomous work.

## Work Human Intervention Items

Use these prompts when non-security items need your input:

```text
/human-intervention-list status=open
/human-intervention-discuss item=HINT-001 guidance=...
/human-intervention-act item=HINT-001 action=defer notes=...
```

The agent should keep these entries public-safe. Do not provide credentials,
private hostnames, private device details, or secret values through these
prompts.

## Run An Iteration

Use `/agentic-loop-iteration` when you want a resumable autonomous pass that can
cover one or more safe slices.

```text
/agentic-loop-iteration objective=active-goal maxSlices=auto
```

The orchestrator should pick useful safe work, delegate to specialists when
useful, edit only what is needed, run checks, request adversarial critique,
update tracking files, and continue while budget remains.

## Assess Progress

Use `/agentic-loop-assess` when you want a coordinator pass without broad
implementation.

```text
/agentic-loop-assess focus=risk
```

Good assessment moments include before security-sensitive work, before creating
a PR, before adding a deployment script, after a long chain of changes, or when
you want adversarial critique without edits.

## Provide Non-Blocking Guidance

Add guidance directly in chat with a prompt invocation or append it to
`.copilot-tracking/agentic/human-feedback.md`. Guidance can include product
judgment, taste, priorities, concerns, or a changed goal. The loop should mark
feedback as considered, adopted, rejected, or deferred once it is reflected in
work tracking.

## Use Copilot Cloud Agent

After the repo is hosted on GitHub, Copilot cloud agent can take issues and
produce branches or pull requests. Use it for bounded tasks with clear acceptance
criteria. Keep these rules:

* Do not commit sensitive files expecting content exclusions to hide them.
* Do not configure credentialed MCP tools until you approve the exact scope.
* Prefer read-only MCP tools if any are needed.
* Use GitHub environment secrets rather than committed configuration.
* Review the pull request diff, logs, and checks before merging.

## Pause And Resume

To pause, tell the orchestrator to stop and record the current state. To resume,
run `/agentic-loop-autonomous`, `/agentic-loop-assess`, or
`/agentic-loop-iteration`.

The next invocation should read `.copilot-tracking/agentic/`, summarize the
current state, and continue from the recorded next action.

## Safety Checklist

Before committing, publishing, or opening the repo:

1. Run `scripts/security/Test-SecretPatterns.ps1 -Mode Repo`.
2. Confirm `.env`, local deployment files, MCP configuration, and runtime logs
   are ignored.
3. Review changed docs for private hostnames, usernames, IP addresses, and local
   device details.
4. Enable GitHub secret scanning and push protection after the repo is hosted.
5. Keep GitHub Actions token permissions least-privilege when workflows are
   added.

## Good Work Items

Agent-sized game tasks should be thin and verifiable:

* Define one map representation.
* Implement movement range for one unit type.
* Add combat forecast for one attacker and defender pair.
* Add replay command serialization.
* Add one controller navigation screen.
* Add one 1280x800 layout validation note or check.
* Add one sanitized deployment script parameter.

Avoid asking for the whole game in one implementation pass. Give the
orchestrator the whole game as the goal, then let it choose the next thin slice.
