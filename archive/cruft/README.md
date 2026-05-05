---
title: Cruft Archive
description: Reversible archive for confirmed unused code, art, prompts, and documentation
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Purpose

This folder holds files that were confirmed unused by a bounded cleanup pass.
The archive preserves original repository paths so cleanup remains reversible
and reviewable.

## Layout

Each cleanup pass writes to a dated folder:

```text
archive/cruft/YYYY-MM-DD/<original repo-relative path>
archive/cruft/YYYY-MM-DD/manifest.md
```

The manifest records the original path, archive path, evidence checked,
restore command, and residual risk for each moved item.

## Archive Rules

* Move files here only when search evidence shows they are unused.
* Preserve the original path beneath the dated archive folder.
* Do not archive ignored private data, runtime logs, credential files, local
  ComfyUI output, model files, build output, or Godot import caches.
* Do not archive active backlog items, current art-handoff candidates, runtime
  atlas contracts, Godot scene dependencies, or reflection-sensitive code unless
  a separate verified change removes the dependency first.
* Prefer `git mv` for tracked files so reviewers can see that the file moved
  instead of disappearing.

## Restore Pattern

Use the manifest restore command for the relevant entry. For tracked files, the
usual shape is:

```powershell
git mv .\archive\cruft\YYYY-MM-DD\path\to\file.ext .\path\to\file.ext
```
