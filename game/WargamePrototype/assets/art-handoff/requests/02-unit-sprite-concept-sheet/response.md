---
title: Unit Sprite Concept Sheet Response
description: Response notes and returned image list for the unit sprite concept sheet request
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Returned Images

Save returned images from ChatGPT in this folder and list them here.

* `ChatGPT Image May 2, 2026, 09_37_33 PM.png`

## ChatGPT Notes

Concept sheet received on 2026-05-02. It informs the expanded campaign unit
sheet: Field Tech, Utility Armor, Survey Scout, Engineer, Sapper, Lancer, Field
Rig, and heavy vehicle silhouettes.

## Follow-Up For Copilot

Used by the source-art extractor. The manifest crops known player unit regions,
keys out the sheet background into transparency, and writes
`assets/sprites/art_units.png`. The Godot renderer tints enemy draws from that
transparent runtime atlas, with deterministic `campaign_units.png` retained as a
fallback generated sheet.
