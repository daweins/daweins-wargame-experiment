---
title: Development Status Reporting Cadence
description: Cadence and trigger policy for periodic human-readable development summaries
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Assigned Agent

Development Status Reporter

## Cadence

Generate or refresh [current-status.md](current-status.md):

* At least every 30 minutes while active changes are underway
* At the end of each `/agentic-loop-autonomous` invocation
* After meaningful changes to active goal, state, backlog, development log,
  human feedback, human intervention, critiques, experiments, decisions, or
  metrics
* Before a long pause or handoff
* When the human invokes `/development-status-report`

Active changes are underway when the orchestrator is still making or validating
repo changes, tracking updates, screenshots, generated artifacts, or test
evidence. If 30 minutes have elapsed since the last status timestamp and active
work is continuing, refresh the report before proceeding to the next work slice
or long-running verification step.

## Report Requirements

Each report should cover:

* Current state of the development effort
* Full ISO 8601 timestamp with timezone offset for the report
* What the project is working on now
* How long the effort has been underway
* How the autonomous loop is working
* Public-safe image or screenshot evidence when useful
* What has worked well
* Where challenges or risks have appeared
* Security status, using `passed` when checks pass and details only when a
    security action is needed
* Open human-intervention items and whether work can continue around them
* Next useful autonomous work

## History Policy

Keep old reports. Before replacing [current-status.md](current-status.md), copy
the outgoing report to `reports/` with a filename-safe timestamp. Link the latest
archived report from [current-status.md](current-status.md) and record it below.

## Image Policy

Reports may embed public-safe repo-local images, including screenshots, with
standard Markdown image syntax. Durable screenshots should live under `images/`
or another tracked public-safe artifact path. Do not include images that reveal
secrets, private local paths, private hostnames, private device details,
credentialed tool output, raw logs, or sensitive machine state.

## Last Report

Last generated: 2026-05-02T20:06:05.5236012-04:00

Report path: [current-status.md](current-status.md)

Latest archived report: [reports/2026-05-02T19-51-41-2959448-04-00-current-status.md](reports/2026-05-02T19-51-41-2959448-04-00-current-status.md)
