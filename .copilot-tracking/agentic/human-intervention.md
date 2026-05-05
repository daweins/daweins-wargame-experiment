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

### HI001: Sidebar-covered arena readability playtest

Status: `open`

Action type: `perform-local-action`

Context: The compact arena HUD is implemented and code-validated, but the key
UX experiment needs a 1280x800 playtest or screenshot review with the verbose
right sidebar covered or intentionally ignored.

Requested action: Check whether HQ state, Scout-7 state, objective, current
mode, ready units, legal moves, legal attacks, terrain value, attack/counter
forecast, and enemy pressure are readable from the board and compact HUD alone.
Record any moments that still require looking at the development inspector.

## Closed Items

### HI004: Request 08 returned unit atlas

Status: `closed`

Action type: `perform-local-action`

Context: Backlog item G045 originally required a returned transparent 2304x512
unit sprite atlas from the LLM art requestor pipeline. The request folder had a
ChatGPT-ready prompt and a rejected deterministic local fallback, but no returned
atlas image to review or promote.

Requested action: Use
`game/WargamePrototype/assets/art-handoff/requests/08-transparent-unit-sprite-atlas/prompt.md`
in the art requestor thread, save any returned atlas candidates in that request
folder, and note them in `response.md` without adding credentials or private
details.

Resolution: The May 4 incoming Field Tech, Kestrel vehicle, and Orison source
sheets provided enough art to assemble and promote the runtime
`assets/sprites/art_units.png` atlas. No further human action is needed for the
current runtime replacement, though final art QA remains open as normal game
review work.

### HI003: Optional pixel-art LoRA selection

Status: `closed`

Action type: `choose`

Context: The free SD 1.5 base checkpoint is installed and can generate local
candidates, but prompt-only sprite generation is inconsistent. A free local
pixel-art LoRA may improve style adherence while preserving the current local
ComfyUI workflow.

Requested action: Choose whether to add a license-approved pixel-art LoRA for
local use. Prefer clear commercial-use terms. Do not provide credentials or
private download tokens in chat.

Resolution: The user selected the SDXL plus `nerijs/pixel-art-xl` path. The
LoRA was copied into the ignored local ComfyUI LoRA folder, the C# pixel-art
pipeline now supports LoRA fields, and SDXL+nerijs candidate generation is
verified through manifest-backed prompt jobs.

### HI002: Local image model selection

Status: `closed`

Action type: `choose`

Context: The local ComfyUI source install and C# prompt-to-candidate pipeline
are in place. Real generation still needs a checkpoint model file under the
ignored local ComfyUI checkpoint folder. Model licenses and allowed uses vary,
so the model should be chosen explicitly instead of downloaded automatically.

Requested action: Choose an approved base checkpoint and optional pixel-art
LoRA for local use. Prefer models with clear commercial-use and redistribution
terms compatible with the project. Do not provide credentials or private
download tokens in chat.

Current recommendation: Start with a free local Stable Diffusion 1.5 checkpoint
such as `v1-5-pruned-emaonly.safetensors`, subject to license review. Use SD
1.5 for fast 512x512 candidate batches before trying SDXL or larger models.

Resolution: The user downloaded `v1-5-pruned-emaonly.safetensors`, and the
checkpoint was copied into the ignored local ComfyUI checkpoint folder. Local
generation through the C# prompt-to-folder pipeline is verified.

No further action is required for this item.
