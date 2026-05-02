---
title: Human Intervention Log
description: Non-security items that need human judgment or action while autonomous work continues elsewhere
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Purpose

This file tracks non-security items that need human judgment, ownership, or
action. These items should not halt the autonomous loop unless every useful task
is blocked. The orchestrator should log the item, route around it, and continue
other safe work.

Security-sensitive issues are not ordinary intervention items. If work would
expose secrets, credentials, private device details, or unsafe tool access, the
loop must stop and follow the security model.

## Status Values

* `open`: Needs human attention, but autonomous work can continue elsewhere
* `discussing`: The human and agent are clarifying options
* `actioned`: The human or agent took the requested action
* `deferred`: The human chose not to act now
* `closed`: No further action is needed
* `obsolete`: The item no longer matters because context changed

## Action Types

* `choose`: The human should choose among options
* `approve-remote`: The human should approve or decline remote mutation such as
  pushing a branch, opening a PR, publishing, or deploying
* `provide-nonsecret-input`: The human can provide product, design, or workflow
  input that does not include private values
* `perform-local-action`: The human should do something outside the agent's safe
  scope, such as manually testing on a device
* `resolve-blocker`: The human should remove a non-security blocker

## Open Items

No open intervention items yet.

## Closed Items

No closed intervention items yet.
