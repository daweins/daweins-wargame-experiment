---
title: Intro Cutscene And Game Start
description: Mission 1 opening cinematic and playable handoff for the tactical combat campaign
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Purpose

This opening sequence introduces the campaign tone, stakes, and factions in a
short format that can hand control to the player quickly. It aligns with
Mission 1, where Kestrel must hold the HQ, rescue Scout-7, and survive the
first Orison raid.

## Tone Targets

* Grounded industrial sci-fi pressure, not mysticism.
* Dry humor from competent people under stress.
* Immediate personal stakes before wider political framing.

## Runtime Target

* Total non-interactive cutscene time: 75 to 95 seconds.
* First player input target: before 2:00 total session time.

Associated cutscene art assets are authored with the standard cutscene format
in [Cutscene Graphics Format](cutscene-graphics-format.md) and generated from
`game/WargamePrototype/assets/cutscenes/specs/mission1_intro.cutscene.json`.

## Intro Cutscene Script

| Time | Visual | Audio | Dialogue |
| --- | --- | --- | --- |
| 00:00-00:08 | Black screen. Sparse telemetry text appears: CALDERA, ASTER BASIN, KESTREL SURVEY CAMP. | Low generator hum and distant wind. | None. |
| 00:08-00:16 | Slow pan over camp floodlights, scaffold towers, and sample rigs. Night dust crosses the frame. | Mechanical ambience and short radio chirps. | Venn (VO): "Kestrel Expedition Log. Day 43. We came here to measure rock, not doctrine." |
| 00:16-00:24 | Close shot of a survey crawler with patched armor plates and a painted unit name. | Metal creaks and a scanner ping. | Nayar (radio): "Inventory says this crawler is a geologist. Ignore the improvised plating." |
| 00:24-00:34 | Tactical map projection flickers. A route marker labeled SCOUT-7 blinks yellow, then red. | Alert tone starts soft, then repeats. | Ops Tech (radio): "Scout-7 missed two check-ins. Relay quality is degrading." |
| 00:34-00:44 | Cut to relay mast on a ridge. Signal bars collapse to zero. Static tears through comms. | Radio static and hard signal dropout. | Holt (broken radio): "...Kestrel, I have movement near the seam. Not local crews..." |
| 00:44-00:56 | Long lens shot of unmarked vehicles advancing through dust on the service road. No faction flags. | Engine whine and bass pulse begins. | Rusk (radio): "Unknown armored column, no IFF, vectoring on camp approach." |
| 00:56-01:08 | Fast cuts: personnel running to barricades, floodlights snapping on, map markers multiplying near the chokepoint. | Alarm rises, then ducks under voice. | Venn (radio): "All sections, this is now a live defensive posture. Keep the HQ standing. Bring Scout-7 home." |
| 01:08-01:18 | UI-style inset: TRANSIT STATUS shows next heavy convoy in 11 months. SPINDLE PRIORITY marked QUEUED. | Alert fades into controlled tactical beat. | Nayar (radio): "Inner systems can send opinions instantly. Parts and reinforcements are still on a calendar." |
| 01:18-01:26 | Camera settles behind Kestrel lines at the chokepoint. Enemy silhouettes appear beyond ridge cover. | Music resolves into gameplay loop intro. | Venn (radio): "Lab safety drill with live ammunition. Positions now." |

## Playable Handoff

### On-Screen Objective Card

Display for 4 seconds, skippable:

* Primary: Defend Kestrel HQ.
* Secondary: Rescue or protect Scout-7.
* Final: Defeat remaining hostile units.

### Turn 1 Start Barks

Use one line per character, no overlap:

* Rusk: "Hold the chokepoint. Do not give them a clean road to HQ."
* Holt: "I can move if you clear the lane. I am pinned near seam marker seven."
* Nayar: "If they hit the pump line, we lose heat before dawn."
* Venn: "We stabilize first. Then we ask who signed this attack."

### First Input Prompts

Keep prompts compact and controller-first:

1. Move cursor to a friendly unit.
2. Confirm to select and preview movement.
3. Use terrain cover to reduce incoming damage.
4. End turn after setting a defensive line.

### Fail Forward Narrative Hook

If the player loses Scout-7 but holds HQ, preserve continuity with a debrief
branch that confirms Scout-7 transmitted partial seam data before loss. The
campaign can continue while emphasizing cost.

## Optional Variants

* Variant A, quiet start: begin with geology voice-over and no music until the
  signal drop.
* Variant B, urgent start: open on the missing Scout-7 marker, then cut back to
  camp setup.
* Variant C, political start: include a one-line Treaty Oversight packet that
  arrives too late to affect opening combat.

## Implementation Notes

* Keep all references grounded in established lore: Asterite is strategic
  material, not a power source, and the Basin Stabilization Grid is legacy
  infrastructure.
* Mention Transit Thread and Spindle constraints once to frame stakes without
  long exposition.
* Preserve short line lengths in subtitle-safe dialogue for 1280x800 handheld
  readability.
