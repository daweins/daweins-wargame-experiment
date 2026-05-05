---
name: art-handoff
description: "Use when: /art-handoff, copy ChatGPT art prompts, collect returned images, and file them in request folders - Brought to you by daweins-wargame-experiment"
---

# Art Handoff

## Overview

Use this skill to run the ChatGPT image-generation handoff for the tactical game
art pipeline. The workflow selects an unfinished art request, prints the full
copy-paste prompt in a code block, waits for the returned image, then files the
image into the matching request folder with a useful name.

Invoke as `/art-handoff`.

## Prerequisites

* The repository contains `game/WargamePrototype/assets/art-handoff`.
* Art requests live under `game/WargamePrototype/assets/art-handoff/requests`.
* Each request folder contains `prompt.md` and `response.md`.
* The user can paste or attach the returned image in chat, or provide a local
  image file path if the chat surface cannot expose pasted images as files.

## Quick Start

1. Find incomplete request folders in numbered order.
2. Pick the first incomplete request unless the user named a request.
3. Read that request's `prompt.md`.
4. Print only the ChatGPT-ready prompt text in a fenced `text` code block, with
   a short line before it saying which request was selected.
5. Ask the user to paste or attach the returned image from ChatGPT.
6. When the image is available, save it in the same request folder.
7. Update that request's `response.md` with the saved filename and any useful
   notes from the user.
8. Report the saved path and the request folder status.

## Parameters Reference

| Option | Default | Meaning |
| --- | --- | --- |
| Request | First incomplete request | A request folder name, number, or title to process |
| Image source | User paste or attachment | Returned ChatGPT image to file into the request folder |
| Filename | Auto-generated | Descriptive filename based on request slug and sequence number |

## Request Selection

Treat a request as incomplete when any of these are true:

* `response.md` still contains `* Pending` under `## Returned Images`.
* The request folder has no image files with extensions `.png`, `.jpg`, `.jpeg`,
  `.webp`, or `.gif`.
* `response.md` has no listed returned image filename.

When multiple requests are incomplete, choose the earliest numbered folder. If
the user names a specific request, use that request even if earlier folders are
also incomplete.

## Prompt Presentation

Extract the ChatGPT-ready text from `prompt.md`. Prefer the first fenced
`text` code block under `## Prompt`. If no fenced prompt exists, print the
meaningful prompt body after the line `Copy this into ChatGPT image generation:`.

Present the prompt like this:

````text
Selected request: requests/01-terrain-tile-concept-sheet

Copy this into ChatGPT image generation:

```text
<full prompt text>
```

Paste or attach the returned image here when ChatGPT finishes. If the pasted
image does not appear as an accessible file, save it anywhere locally and send
me the file path.
````

Do not shorten, paraphrase, or split the prompt. The user should be able to copy
the whole prompt from one code block.

## Image Filing

When the user provides the returned image:

1. Determine the source image path from the chat attachment, pasted image, or
   user-provided local file path.
2. Preserve the image extension when possible.
3. Name the image using the request folder slug and a two-digit sequence:
   `<request-slug>-chatgpt-01.png`.
4. If that filename exists, increment the sequence number.
5. Copy or move the source image into the selected request folder.
6. Do not overwrite existing images.
7. Do not store images outside the selected request folder unless the user asks.

Examples:

* `terrain-tile-concept-sheet-chatgpt-01.png`
* `mission-one-cutscene-frame-chatgpt-01.png`
* `ui-icon-sheet-chatgpt-02.webp`

## Response Update

After filing the image, update that request's `response.md`:

* Replace `* Pending` under `## Returned Images` with one bullet per saved image.
* Add any text response, generation notes, or user comments under
  `## ChatGPT Notes`.
* Add a concise next-step request under `## Follow-Up For Copilot` when the user
  provides one.
* Keep YAML frontmatter intact.
* Preserve Markdown lint rules: frontmatter first, no duplicate headings, no
  multiple consecutive blank lines, and a single trailing newline.

## Script Reference

This skill has no bundled scripts. Use workspace file tools for reads and
edits. On Windows, use PowerShell only for file-copy operations when a chat
attachment or user-provided image path needs to be copied into the request
folder.

## Troubleshooting

If the user pastes an image but no accessible file path is available, explain
that the current chat surface did not expose the pasted image as a local file.
Ask the user to save the image locally or drop it into the matching request
folder, then send the file path or filename.

If the request folder cannot be determined from the current turn, inspect
`game/WargamePrototype/assets/art-handoff/requests` and continue with the
earliest incomplete request.

If multiple images are supplied for one request, file each image with an
incremented sequence number and list them all in `response.md`.

> Brought to you by daweins-wargame-experiment