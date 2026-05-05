---
title: Replay Command Log Format
description: Initial deterministic command stream format for battle replay fixtures
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: reference
---

## Purpose

Replay command logs are the compact, deterministic record of a battle. They are
not a full state dump. A valid replay starts from a known mission factory, rules
version, save schema version, seed, and an ordered command stream, then applies
each command through the core `BattleRules.ApplyCommand` path.

This format is intentionally small so early smoke tests can prove replayability
before save files, map imports, or a dedicated test framework exist.

## Envelope

```json
{
  "formatVersion": 1,
  "rulesVersion": "prototype-2026-05-03",
  "saveSchemaVersion": 1,
  "missionId": "mission1",
  "randomSeed": 1592598566,
  "initialStateHash": "...",
  "expectedFinalStateHash": "...",
  "commands": []
}
```

Required fields:

* `formatVersion`: command stream format version. Start at `1`.
* `rulesVersion`: human-readable rules identifier for compatibility checks.
* `saveSchemaVersion`: initial save/replay schema version. Start at `1`.
* `missionId`: mission factory identity used to recreate the initial state.
* `randomSeed`: seed stored on the initial `BattleState`.
* `initialStateHash`: hash before commands are applied.
* `expectedFinalStateHash`: hash after every command is applied.
* `commands`: ordered command list.

## Commands

Each command records only data required by `BattleCommand`.

Move:

```json
{
  "kind": "Move",
  "unitId": "Infantry-1",
  "destination": { "x": 5, "y": 3 }
}
```

Attack:

```json
{
  "kind": "Attack",
  "unitId": "Armor-1",
  "targetUnitId": "Raider-A"
}
```

Wait:

```json
{
  "kind": "Wait",
  "unitId": "Infantry-1"
}
```

End turn:

```json
{
  "kind": "EndTurn"
}
```

Commands must be applied in array order. A replay runner should stop on the
first failed command and report the command index, kind, mission id, and current
state hash.

## Compatibility Rules

Replay compatibility is valid only when all of these match:

* `formatVersion`
* `rulesVersion`
* `saveSchemaVersion`
* `missionId`
* `randomSeed`
* `initialStateHash`

If any field differs, the runner may still attempt diagnostic replay, but the
result should not be treated as authoritative.

## Current Verification

The smoke runner includes a fixture that serializes an opening Mission 1 command
stream, deserializes it, replays the commands from `FirstMissionFactory.Create`,
and verifies that the final `BattleRules.GetStateHash` matches the expected
state hash.
