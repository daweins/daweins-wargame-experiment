---
title: Wargame Agentic Development Experiment
description: Repository for a Copilot-driven agentic programming ecosystem and tactical combat game prototype
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: overview
---

## Wargame Agentic Development Experiment

This repository has two linked goals:

* Build a highly hands-off agentic programming ecosystem using GitHub Copilot,
  VS Code custom agents, prompt files, hooks, GitHub branches, repo-based work
  tracking, adversarial critique, experiment planning, and development logs.
* Use that ecosystem to develop a tactical combat game in the family of
  Advance Wars, playable on this laptop and deployable to a Steam Deck without
  requiring Steam Store publication.

The repo assumes it may become public. Never commit credentials, auth tokens,
private hostnames, device secrets, `.env` files, personal access tokens, SSH
keys, private MCP configuration, or generated logs that could contain sensitive
data.

## Starting Points

* [docs/agentic-programming/copilot-ecosystem-blueprint.md](docs/agentic-programming/copilot-ecosystem-blueprint.md)
* [docs/agentic-programming/security-model.md](docs/agentic-programming/security-model.md)
* [docs/agentic-programming/operating-manual.md](docs/agentic-programming/operating-manual.md)
* [docs/game/tactical-combat-technical-direction.md](docs/game/tactical-combat-technical-direction.md)

## Copilot Workflow

Use the workspace custom agents in `.github/agents/` and prompts in
`.github/prompts/` from VS Code Chat. The intended rhythm is:

1. Start with `/agentic-loop-kickoff` for a new high-level objective.
2. Run `/agentic-loop-autonomous` when you want continual safe improvement until
  the current invocation budget, a security hard stop, or no useful task
  remains.
3. Run `/agentic-loop-iteration` for a resumable autonomous development pass.
4. Run `/agentic-loop-assess` when you want the coordinating agent to evaluate
   progress, risks, and the next best work items.
5. Keep all sensitive configuration outside the repo and outside chat prompts.

The durable repo work system lives in `.copilot-tracking/agentic/`. Add human
guidance to chat or `.copilot-tracking/agentic/human-feedback.md`; the loop
should treat it as non-blocking input. Non-security items that need human
judgment go in `.copilot-tracking/agentic/human-intervention.md`, while the loop
continues other useful work. Security-sensitive issues still halt the loop.

Use `/human-intervention-list`, `/human-intervention-discuss`, and
`/human-intervention-act` to review, discuss, and resolve intervention items.

The Development Status Reporter refreshes
`.copilot-tracking/agentic/status/current-status.md` at least every 30 minutes
while active changes are underway. It keeps timestamped old reports under
`.copilot-tracking/agentic/status/reports/` and may link public-safe screenshots
or other image evidence. Use `/development-status-report` when you want an
immediate human-readable summary.

GitHub Copilot cloud agent can be used for branch and pull request work after
the repo is hosted on GitHub, but it must not receive access to secrets unless
those secrets are stored in GitHub's protected secret mechanisms and explicitly
approved for that task.
