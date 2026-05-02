---
description: "Generate or refresh the periodic human-readable development status report"
agent: Development Status Reporter
tools: [read, search, edit, execute, todo]
argument-hint: "[period=current|daily|since-last] [audience=human]"
---

# Development Status Report

## Inputs

* ${input:period:current}: Optional reporting period.
* ${input:audience:human}: Optional report audience.

## Requirements

1. Read `.copilot-tracking/agentic/status/reporting-cadence.md`,
   `.copilot-tracking/agentic/status/current-status.md`, and the core tracking
   files in `.copilot-tracking/agentic/`.
2. Generate a concise human-readable report covering current state, what the
   project is working on, how long the effort has been underway, how work is
   being done, what has worked well, where challenges have appeared, open human
   intervention items, and next useful autonomous work.
3. Update `.copilot-tracking/agentic/status/current-status.md`.
4. Update reporting metadata in `reporting-cadence.md` using public-safe values.
5. Do not include secrets, credentials, private device details, private local
   paths, or raw logs.

If a security issue is discovered, stop and report only the class and location.
