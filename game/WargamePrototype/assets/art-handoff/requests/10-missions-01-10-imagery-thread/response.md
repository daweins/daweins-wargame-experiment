---
title: Missions 1-10 Imagery Thread Response
description: Returned image ledger and follow-up notes for first-act campaign imagery
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Returned Images

Save returned images from the imagery thread in this folder and list them here.

* Pending

No returned imagery-thread files have been saved in this request folder yet.

Local fulfillment candidates generated under ignored `local-candidates/`:

* `token-kestrel-field-tech-sdxl-nerijs-v2`
* `token-kestrel-utility-armor-sdxl-nerijs-v1`
* `token-kestrel-survey-scout-sdxl-nerijs-v1`
* `token-kestrel-field-tech-sdxl-nerijs-v3`
* `token-kestrel-utility-armor-sdxl-nerijs-v2`
* `token-kestrel-survey-scout-sdxl-nerijs-v2`
* `token-kestrel-field-tech-sdxl-nerijs-v4`
* `token-kestrel-utility-armor-sdxl-nerijs-v3`
* `token-kestrel-survey-scout-sdxl-nerijs-v3`
* `mission4-6-escalation-cinematic-sdxl-nerijs-v1`
* `sable-meridian-style-sdxl-nerijs-v1`
* `mission8-blackout-kit-sdxl-nerijs-v1`
* `mission10-refinery-finale-sdxl-nerijs-v1`
* `commander-portraits-act1-sdxl-nerijs-v1`
* `request10-venn-portrait-sdxl-nerijs-v2`
* `request10-venn-portrait-sdxl-nerijs-v3`

Local request-folder assets generated or surfaced:

* `local-act1-ui-overlay-atlas.png`
* `local-act1-unit-reference-atlas.png`
* `local-act1-terrain-reference-atlas.png`
* `local-m01-hq-alert-cutscene-reference.png`
* `local-m01-scout-rescue-cutscene-reference.png`
* `local-m02-relay-yard-cutscene-reference.png`
* `local-m03-pump-station-cutscene-reference.png`
* `local-missions-04-10-reference-panels.png`
* `local-venn-portrait-v3.png`

## Review Status

In progress locally.

Latest local review packet:
`game/WargamePrototype/assets/art-handoff/local-review/20260503-144216/`.

Current review status:

* Field Tech v3 seed `57400`: best local SDXL infantry direction, but still a
  pressure-test reference rather than atlas-ready art.
* Field Tech v4: rejected for runtime use because it drifted into cards,
  presentation sheets, or portrait-like framing.
* Utility Armor v3: reference-only. It contains useful shapes but still drifts
  into source sheets and presentation panels.
* Survey Scout v3: reference-only. It needs cleaner single-buggy silhouettes and
  stronger 64x64 terrain separation.
* Runtime fallback coverage: present for act-one unit, terrain, and UI overlay
  needs through deterministic transparent atlases.
* Deterministic Mission 4-10 reference coverage: present locally as a seven-panel
  sheet covering fabricator, antenna fog, bridge, settlement grid, blackout,
  fog ridge, and refinery finale themes.
* High-art coverage: present locally for Missions 1-3 cutscene references only.
* Mission 4-6 escalation v1: reference-only. The image produced useful
  blue-white fabricator and support-machine shapes, but it did not satisfy the
  requested three-panel cinematic still structure.
* Sable/Meridian style v1: reference-only for infantry uniform language. It
  drifted into full-height character turnarounds and did not cover the requested
  props, vehicles, or faction-separated board tokens.
* Mission 8 blackout kit v1: cleanup candidate for vehicle, warehouse, and
  substation reference shapes. It uses a gray sheet background and needs crop,
  alpha cleanup, and 64x64 board-readability checks before any runtime use.
* Mission 10 refinery finale v1: cleanup candidate for Orison refinery props,
  dark vehicles, hazard-orange overlays, and industrial panels. It is not yet a
  transparent runtime sheet and needs extraction planning.
* Commander portraits v1: rejected for portrait-brief fulfillment. It produced
  full-body uniform references instead of eight readable briefing bust
  portraits.
* Venn portrait v2: partial improvement. Seed `62000` has a readable bust, but
  it includes side-card thumbnails and text-like clutter, so it is reference-only.
* Venn portrait v3: strongest local character pass so far. Seed `62100` is a
  clean centered commander bust candidate with no side-card clutter. A refreshed
  `62100` candidate was promoted to `local-venn-portrait-v3.png` and is now the
  preferred runtime commander portrait, with the older request 05 portrait kept
  as fallback. Seed `62101` is useful uniform/reference art but includes an
  office panel. Seed `62102` is rejected for text and UI clutter.

Promotion evidence:

* Runtime asset: `local-venn-portrait-v3.png`
* Review packet:
  `game/WargamePrototype/assets/art-handoff/local-review/request10-venn-portrait-v3-refresh/`
* Verification: Godot build and 23 smoke checks passed after runtime wiring.

## ChatGPT Notes

Pending returned imagery-thread files.

## Follow-Up For Copilot

Review returned images against the prompt gates, update this ledger with
accepted and rejected candidates, then create crop or extraction tasks for any
runtime-ready sprite, terrain, prop, portrait, cutscene, or UI source art.

For local continuation, use `local-missions-04-10-reference-panels.png` as the
baseline composition reference for later first-act imagery, then prioritize
higher-art gaps: escalation stills, Sable and Meridian faction exploration,
Mission 8 blackout assets, Mission 10 refinery finale assets, and commander
portrait expansions. Use the deterministic unit, terrain, overlay, and mission
reference atlases as runtime fallbacks while the higher-art pass continues.

The next local pass should split runtime candidates even further: one prop or
one board token per job, flat magenta extraction background for board assets,
and a separate eye-level prompt for each cutscene still. Commander portraits
need one bust per job. Use the Venn v3 seed `62100` prompt shape as the next
template for commander portraits, with hard negative prompts for side cards,
documents, maps, panels, captions, and text blocks.
