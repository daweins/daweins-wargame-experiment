---
title: Development Status Reporting
description: Periodic human-readable status reports for the autonomous development effort
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: overview
---

## Purpose

This directory stores periodic human-readable summaries of the autonomous
development effort. The reports are written for the human supervising the work:
clear enough to skim, specific enough to guide judgment, and safe enough to keep
in the repo.

## Files

* [current-status.md](current-status.md) is the latest status report.
* [reporting-cadence.md](reporting-cadence.md) defines when the Development
  Status Reporter should refresh the report.
* `reports/` keeps timestamped historical reports before the latest report is
  replaced.
* `images/` can store public-safe screenshots or other image evidence linked by
  reports.

## Reporting Agent

The assigned reporting agent is `Development Status Reporter`. The Strategic
Orchestrator should call that agent at least every 30 minutes while active
changes are underway, after meaningful tracking changes, and when the human
invokes `/development-status-report`.
