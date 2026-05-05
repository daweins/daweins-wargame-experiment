---
title: Pixel Art Style Guide
description: Shared visual style direction for local generation, sprite consistency, cutscenes, terrain, and tactical readability
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: concept
---

## Purpose

This guide defines the reusable look and feel for generated and hand-authored
pixel art in the tactical prototype. It exists so local image generation, crop
planning, review packets, and runtime atlas promotion all aim at the same game
instead of producing isolated attractive images.

The Sprite Art Director owns this document. Before generating new candidates,
it should research current game direction, update this guide when a reusable
rule is missing, and then turn the guide into prompt specs and review criteria.

## Research Inputs

Use these sources before changing style or generating new asset families:

* `docs/game/tactical-combat-technical-direction.md`
* `docs/game/campaign-environment-plan.md`
* `docs/game/first-six-mission-unit-ramp.md`
* `game/WargamePrototype/README.md`
* `game/WargamePrototype/assets/art-handoff/pixelart-prompts/README.md`
* Current runtime sprite sheets under `game/WargamePrototype/assets/sprites/`
* Latest ignored review packets under
  `game/WargamePrototype/assets/art-handoff/local-review/`
* Selected local candidates under
  `game/WargamePrototype/assets/art-handoff/local-candidates/`

## Style Thesis

The game should read as grounded industrial sci-fi rendered through crisp
late-16-bit tactical pixel art. The battlefield should feel practical,
field-repaired, and politically specific: survey prefabs, utility roads,
relay dishes, pump stations, crates, basalt ridges, teal Asterite status lights,
and Orison hazard markings.

Sprites should support repeated tactical decisions before they support detail.
Every unit must be identifiable by silhouette and team accent at 64x64 over
busy terrain, while cutscenes may carry richer atmosphere and material texture.

## Global Visual Rules

* Use nearest-neighbor pixel clarity and avoid painterly softness.
* Keep board sprites readable at 64x64, 1280x800, and handheld distance.
* Use chunky forms, clear dark outlines, and limited internal detail.
* Use terrain texture as context, not competition; units and objectives win the
  contrast hierarchy.
* Keep teal Asterite and grid-control lighting as accents, not full-scene glow.
* Avoid decorative labels, UI cards, borders, dramatic portrait lighting, and
  photorealistic material rendering in sprite prompts.

## Camera And Scale Contract

Board units use one shared camera language:

* Orthographic three-quarter top-down view
* Facing lower-right by default
* Full body or full vehicle visible
* Unit fills roughly 70 to 82 percent of the final 64x64 crop
* Centered crop with feet, wheels, or tracks included
* No horizon, ground plane, cast shadow, or cinematic depth of field
* Final proxy must survive a 64x64 crop on representative terrain

Cutscenes use a different camera language:

* Eye-level or slightly elevated cinematic camera
* Visible horizon when showing places
* Clear focal object tied to the mission objective
* No sprite sheets, orthographic board layout, labels, or top-down asset maps

## Faction Language

Kestrel assets should feel improvised, survey-oriented, and support capable.
Use blue-gray bodies, teal sensor slits or visors, tan field gear, practical
crates, and compact repair hardware.

Orison assets should feel purpose-built, legalistic, and pressure-oriented. Use
charcoal-gray bodies, orange hazard accents, angular armor shapes, breach tools,
and cleaner military silhouettes.

Sable and later factions should not be invented casually. Add their sprite
language only after the relevant mission or faction style is researched.

## Unit Family Contracts

Field Tech and infantry must be squat, helmeted, and compact. Their silhouette
comes from the helmet, visor, backpack or satchel, and stance, not face detail.

Engineers need a square repair backpack, tool arm, small dish, or cable spool.
They must not read as normal infantry with extra decoration.

Sappers need a satchel, hazard stripe, or compact breaching tool visible at
64x64. The satchel shape matters more than costume detail.

Lancers need one long, readable anti-armor tool angled away from the body. The
weapon can be simplified, but it must not become a thin unreadable line.

Scouts and strikers need low fast profiles. Scouts are survey buggies or light
recon forms; strikers are sharper and more aggressive but still compact.

Armor and siege units need width, mass, and simple directional cues. Heavy
vehicles should be broader and slower-looking than standard armor.

Field rigs need cargo, repair, or supply language: crates, crane arm, hose reel,
or compact service mast. They should read as valuable support vehicles, not as
combat tanks.

## Cross-View Identity

Every unit family should preserve the same three silhouette anchors across board
sprite, cutscene cameo, UI portrait, damaged state, animation frame, and enemy
variant. Change palette, small gear, and damage marks only after the helmet,
chassis, tool, satchel, launcher, or support hardware still reads at 64x64.

This rule matters more than local image beauty. A sprite that looks impressive
at source resolution but loses its family anchor in the review packet should
remain reference art.

## Prompt Consistency Requirements

Each generated sprite prompt should name:

* Asset family and gameplay role
* Faction language
* Camera and facing direction
* Full-body or full-vehicle crop rule
* Silhouette priority
* Team-color placement
* Flat extraction background
* Negative prompt for labels, cards, portraits, duplicate views, terrain, and
  photorealism

Avoid broad roster-sheet prompts until single-token prompts for that family have
passed review. If a sprite must have multiple views, generate one view at a
time with identical shared context and seed notes.

## Review Gates

Do not promote a candidate unless it passes all of these gates:

* It reads at 64x64 over plain, road, cover, HQ, and ridge terrain.
* Team and unit type can be identified without reading the board label.
* The crop includes the entire tactical silhouette.
* It does not rely on white background, presentation shadow, labels, or card
  framing for readability.
* It is not an inset character inside a source-card composition.
* It remains consistent with related sprites in camera angle, scale, outline,
  and palette.
* It has a public-safe manifest path and can be regenerated or replaced through
  the local pipeline.

## Current Direction

The current SDXL plus `nerijs/pixel-art-xl` setup is strong enough for cutscene
concepts and useful unit references. Prompt-only roster sheets are not yet good
enough for runtime sprites. The next style work should prioritize one-token
guided generation, image-to-image or ControlNet silhouette guidance, and review
packets that evaluate the result at 64x64 board scale.
