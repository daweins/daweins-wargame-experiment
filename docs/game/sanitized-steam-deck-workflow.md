---
title: Sanitized Steam Deck Workflow
description: Public-safe local deployment configuration schema for Steam Deck testing
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: concept
---

## Purpose

This document defines the public-safe shape of the local Steam Deck deployment
workflow. It documents configuration keys, file boundaries, logging rules, and
validation gates without storing private hostnames, usernames, IP addresses,
SSH key paths, credentials, or device-specific details in the repository.

The target workflow remains local and user-controlled:

1. Export a native Linux x86_64 Godot build.
2. Transfer the build to the Steam Deck through an explicitly configured local
   channel.
3. Add or refresh the build as a non-Steam game for Game Mode testing.
4. Record only sanitized results in tracked docs or work logs.

## Secret Boundary

Never commit or paste these values into source, docs, prompts, issues, pull
requests, logs, screenshots, or agent tracking files:

* Steam Deck hostnames or IP addresses.
* Steam Deck usernames.
* SSH private key paths or key contents.
* Syncthing device IDs, tokens, or folder secrets.
* Private LAN paths, mount names, or device nicknames.
* Local export folders that reveal personal profile paths.
* Any credential, token, passphrase, or connection string.

Private configuration belongs in ignored local files, environment variables, or
an operating-system credential store. The repo already ignores `.env`, `.env.*`,
`*.local`, `private/`, build output, Godot exports, and agent runtime logs.

## Local Configuration Schema

Use an ignored local file such as `.env.local` or a machine-specific file under
`private/`. The file may define these keys. Values are intentionally omitted.

```dotenv
WARGAME_DECK_TRANSPORT=
WARGAME_DECK_BUILD_CONFIGURATION=
WARGAME_DECK_EXPORT_PRESET=
WARGAME_DECK_LOCAL_EXPORT_DIR=
WARGAME_DECK_REMOTE_APP_DIR=
WARGAME_DECK_REMOTE_COMPAT_DIR=
WARGAME_DECK_SSH_HOST=
WARGAME_DECK_SSH_PORT=
WARGAME_DECK_SSH_USER=
WARGAME_DECK_SSH_KEY_PATH=
WARGAME_DECK_SYNCTHING_FOLDER=
WARGAME_DECK_POST_COPY_COMMAND=
```

Allowed `WARGAME_DECK_TRANSPORT` values:

* `ssh`: copy over a user-controlled SSH connection.
* `syncthing`: write to a local synced folder and let Syncthing transfer it.
* `manual`: export locally and stop with sanitized instructions for the user.

Do not add defaults for host, user, key path, remote directory, or synced folder
in tracked scripts. A script may require those values at runtime, but it must not
print them.

## Script Contract

Future deployment scripts should follow this contract:

* Accept an explicit configuration path, but default only to ignored local paths.
* Read environment variables without echoing raw values.
* Validate that required values are present for the selected transport.
* Build or export into ignored `build/`, `dist/`, or `out/` paths.
* Print sanitized status messages such as `export succeeded`, `copy succeeded`,
  `remote launch skipped`, or `configuration missing`.
* Redact host, user, path, and key values in errors.
* Avoid shell history leakage by not passing secrets on command lines.
* Avoid broad filesystem scans of user profile folders.
* Exit nonzero on missing configuration, failed export, failed transfer, or a
  failed post-copy command.

Scripts may print relative repository paths and artifact names. They should not
print private absolute paths except inside ignored local logs controlled by the
user.

## Sanitized Log Shape

Tracked logs or agentic status entries may include only this kind of summary:

```text
Deck deploy dry run: succeeded
Transport: ssh
Export preset: linux-x86_64
Artifact: WargamePrototype.x86_64
Private values printed: no
```

Do not include the resolved host, username, remote directory, SSH key path, LAN
address, Syncthing folder, or local profile path.

## Validation Checklist

Before a Deck deployment script is committed, verify:

* The script runs without a local config and reports missing configuration
  without printing private values.
* The script supports a dry-run mode that validates config shape and export
  commands without transferring files.
* The script writes any detailed runtime log only under ignored paths.
* The tracked docs contain variable names only, not example secrets or private
  device details.
* `scripts/security/Test-SecretPatterns.ps1` or an equivalent secret scan passes
  on tracked files.
* A reviewer can understand the deploy flow without needing the user's machine
  details.

## Future Implementation Slice

The next implementation slice should add a PowerShell deployment script with
`-ConfigPath`, `-DryRun`, `-Transport`, and `-NoPostCopy` parameters. It should
read the schema above, redact private fields in output, and stop before any
real transfer unless required fields exist in an ignored local config.
