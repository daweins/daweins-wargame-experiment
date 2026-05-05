---
title: Transparent UI Icon Atlas Response
description: Response notes and returned image list for extractor-friendly transparent UI icons
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Returned Images

Save returned images from ChatGPT in this folder and list them here.

* Pending

No returned ChatGPT image file has been saved yet.

Local deterministic fulfillment:

* `local-transparent-ui-icon-atlas.png`

## Local Notes

`local-transparent-ui-icon-atlas.png` is a C# generated runtime fallback with
the requested 1536x256 canvas, 12x1 logical icon row, 128x128 cells, and true
transparent background.

The sheet is crisp and readable as runtime UI art. It remains deterministic
fallback art rather than a returned high-art source image.

## Follow-Up For Copilot

Use the local deterministic atlas as the current fallback. If a returned
high-art icon atlas arrives later, validate cell geometry and 20x20 readability
before promotion.
