---
description: "Run a bounded periodic cleanup pass that archives confirmed unused code and art"
agent: Cruft Cleaner
tools: [read, search, edit, execute, todo]
argument-hint: "[scope=repo|code|art|docs] [move={confirmed|report-only}]"
---

# Cruft Cleaner Periodic Pass

## Inputs

* ${input:scope:repo}: Optional cleanup scope. Use `repo`, `code`, `art`, or
  `docs`.
* ${input:move:confirmed}: Optional move mode. Use `confirmed` to archive only
  high-confidence cruft, or `report-only` to produce candidates without moving
  files.

## Requirements

1. Follow the Cruft Cleaner agent protocol.
2. Treat the pass as bounded periodic hygiene, not a broad refactor.
3. Archive only files with strong unused evidence when `move` is `confirmed`.
4. Preserve original relative paths under `archive/cruft/<date>/`.
5. Record every archived file in the pass manifest with restore guidance.
6. Leave uncertain candidates in place and report why they need human judgment
   or more evidence.
7. Avoid ignored private folders, local runtime logs, generated local candidate
   batches, build outputs, credential stores, and secret-like files.