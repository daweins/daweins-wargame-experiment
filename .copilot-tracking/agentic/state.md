---
title: Agentic Loop State
description: Current public-safe state snapshot for autonomous development
author: GitHub Copilot
ms.date: 2026-05-03
ms.topic: planning
---

## Current State

The Copilot agentic ecosystem scaffold exists and has been validated. The
initial tactical game product goal and first prototype mission spec are defined.
The first playable Godot C# prototype now exists locally. It has gone through
several manual playtest tuning passes, including scenario expansion and sprite
readability improvements. The latest manual feedback accepts the expanded
mission as a good tactical first mission. A first graphics overhaul is now in
place for the accepted mission baseline, and the board now uses PNG sprite
sheets for terrain and unit frames instead of procedural rectangle art. The
latest sprite pass uses 64x64 sheets with richer SNES and DS-style shading and
has been positively reviewed by the user. Follow-up visual feedback found the
running board hard to read and the road tiles distracting, so the latest pass
calms the road tile, reduces terrain noise, softens the board grid, and adds a
subtle unit backing plate for readability.
The project now also has a code quality and architecture constitution that
defines periodic review triggers, suggestion classification, autofix rules,
deterministic replay expectations, and architecture boundaries.
The latest UX direction preserves the verbose sidebar as a development
inspector outside the main game screen. Future player-facing readability work
should make the graphical arena and compact in-game HUD carry more of the
decision-critical information.
The arena-first HUD implementation now adds objective and rescue markers,
terrain defense pips, player readiness badges, compact status and forecast
chips, and controller prompts below the board while leaving the verbose sidebar
available as a development inspector.
The campaign now has a narrative spine through 50 missions, with detailed
first-act mission plots and modular five-mission arcs that escalate from
expedition survival to regional coalition warfare and public control of the
Basin Stabilization Grid.
The campaign now also has an environment plan that defines reusable terrain
kits, mission-by-mission environmental flavor for the first 10 missions,
five-mission visual arcs through Mission 50, tile readability rules, and a
compact terrain rule budget.
The universe direction has been grounded around realistic political and
industrial sci-fi. Asterite is a limited industrial material, the Loom is only a
field nickname for the human-built Basin Stabilization Grid, and interstellar
travel uses the Transit Thread of scheduled beam corridors, fusion pushers,
magsail braking, depots, manifests, and courier relays. FTL messaging exists
through fixed low-bandwidth Spindle Net stations, but FTL travel does not. This
gives Orison, Sable, Treaty Oversight, Meridian, and Kestrel concrete political
leverage through freight slots, braking rights, message priority, relay
authentication, sanctions, transit delays, and local infrastructure authority.
The first-act campaign-mode spine now carries those constraints into Missions
4-10 through grounded brief fields for routes, schedules, Spindle custody,
permits, manifests, infrastructure stakeholders, Asterite supply-chain costs,
public-safety risks, and tactical objective verbs.
Missions 1-3 now have implementation-ready playable briefs with map concepts,
unit lists, objective text, grounded lore, rules, debriefs, dependency notes,
deterministic enemy-behavior budgets, and compact radio-banter hooks.
The Aster Basin starter tileset now has a concrete 64px spec for Missions 1-3 in
`docs/game/aster-basin-starter-tileset-spec.md`, including rule-terrain mapping,
tile order, mission props, overlay separation, missing assets, a 1280x800
readability mock plan, and promotion gates for `art_terrain.png`.
The Steam Deck workflow now has a sanitized local configuration schema in
`docs/game/sanitized-steam-deck-workflow.md`. It defines variable names,
transport options, redaction rules, logging shape, and validation gates without
private hostnames, IP addresses, usernames, SSH key paths, tokens, or device
details.
Replay now has an initial command stream format in
`docs/game/replay-command-log-format.md`, plus a smoke fixture that serializes,
deserializes, and replays the Mission 1 opening command stream through the core
command path to reproduce the expected final state hash.
The first economy fixture now tracks controlled properties, player income, and
player funds in the core battle state. Existing two-turn relay and fuel captures
increase income, and the deterministic end-turn path pays income at the next
player turn. Smoke coverage verifies the capture economy payout.
The first supply fixture now supports opt-in limited ammo. Empty limited-ammo
units cannot attack, lose attackable targets, and forecast zero damage. Field
Rigs can wait to restore one ammo to an adjacent friendly limited-ammo unit.
Existing campaign units keep unlimited ammo until a mission opts into the rule.
The first commander-power scaffold now supports explicit `ActivatePower`
commands for Rusk's `lock-the-line` power. Player charge builds when friendly
units take damage and survive. Activating the power spends four charge, gives
held-position friendly ground units +1 defense during the enemy phase, and
expires when the next player turn begins.
The character bible's candidate commander powers now have a dedicated rule
budget in `docs/game/co-power-rule-budget.md`. Rusk's `Lock The Line` is the
accepted first prototype candidate, constrained to a defense-only,
forecast-visible, deterministic command-log slice. Venn, Holt, and Sloane are
deferred until their supporting systems exist; Priya, Rhee, Calder, and Kravic
are rejected for the first CO-power implementation slice.
A first-six-mission unit ramp is now drafted with a compact nine-unit roster,
mission-by-mission introduction order, counter loops, stats, mission flavor,
and a generated campaign unit planning sheet.
The campaign now also has a character bible that expands the principal cast
into backgrounds, motivations, contradictions, game-long arcs, relationship
hooks, voice patterns, commander doctrine, candidate CO powers, candidate
signature units, Treaty Oversight Bureau and grid-control voice guidance, and
explicit validation gates for deterministic, forecast-visible, replayable
character mechanics.
The latest combat feedback slice makes player-initiated attacks more
interesting and informative with presentation-only effects: floating actual
damage numbers, HP bar tweening with immediately authoritative HP labels, hit
flashes, recoil, damage-state overlays, compact attack and return-fire chips,
and KO labels for destroyed targets.
The prototype now has a playable two-mission campaign flow. Returned ChatGPT
concept images are recorded in the art handoff request folders, Mission 1 opens
on its cutscene image, Mission 1 victory advances to a relay-yard cutscene
image, and Mission 2 starts from there. Mission 2 adds deterministic relay and
fuel-cache objectives, Engineer and Sapper unit roles, mission-aware HUD text,
objective markers, and smoke coverage for objective completion.
The returned art has now been reviewed and folded into runtime surfaces where it
fits: Mission 1 and Mission 2 concept frames remain direct cutscene screens, the
commander portrait appears in cutscene dialogue, terrain concepts drove a new
deterministic basin terrain sheet, and the UI icon concept drove a deterministic
`ui_icons.png` atlas used in HUD prompt and status chips. The unit concept sheet
remains reference for generated silhouettes because it is not a cuttable runtime
atlas.
The repository now has a local ComfyUI source install under ignored local
folders and a C# prompt-to-candidate pipeline in `Wargame.AssetTools`. Prompt
job specs can queue vanilla ComfyUI text-to-image workflows and copy generated
PNG candidates plus manifests into ignored art-handoff output folders. A model
selection decision has been actioned for local experimentation: SDXL base plus
the `nerijs/pixel-art-xl` LoRA is installed locally, LoRA prompt fields are
supported by the C# pipeline, and multiple manifest-backed SDXL+nerijs prompt
passes have produced sprite and cutscene references.
The latest consistency pass shows that vehicle and infantry sheets are useful
concept references, while cutscene work needs explicit eye-level cinematic
camera language to avoid top-down asset-sheet drift. Mission 1 intro, Mission 2
relay, and Mission 3 pump-station cinematic references are promising. Mission 1
rescue still needs a guided sketch or image-to-image pass.
The graphics-agent set now includes a Sprite Art Director, Tactical UX Graphics
Critic, and Graphics Integration Evaluator. The Sprite Art Director owns a
planning-first style workflow, `docs/game/pixel-art-style-guide.md`, and the
shared local prompt context in
`game/WargamePrototype/assets/art-handoff/pixelart-prompts/shared-style-context.md`.
Future sprite generation should update or consult those shared style rules
before creating prompt specs, especially for cross-view identity, crop
occupancy, faction language, camera angle, and 64x64 readability gates.
The first-act campaign mode is now planned through Mission 10. The campaign
mode document defines the linear briefing, battle, debrief, mission-select,
save metadata, score, unlock, and replay requirements for Missions 1-10. The
existing art-handoff tree now includes
`game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread`
as the monitored backlog for cutscenes, portraits, units, terrain, props,
faction style exploration, and UX overlays.
The Godot prototype now has a playable first-act campaign implementation through
Mission 10. The deterministic core exposes a campaign catalog and factory for
all 10 missions, and the Godot controller advances through intro, battle,
debrief, next mission, and final campaign-complete screens. Mission 1 victory no
longer restarts Mission 1. Smoke tests now verify the 10-mission catalog and
progression metadata alongside the existing objective, replay, AI, and scoring
checks.
The art-handoff folder now has a queue ledger at
`game/WargamePrototype/assets/art-handoff/status.md`. Requests `07`, `08`, and
`09` have deterministic local runtime-shaped fallback images in their request
folders. Request `10` has local act-one overlay, unit, terrain, Missions 1-3
cutscene reference images, and a deterministic Missions 4-10 reference-panel
sheet covering fabricator, antenna fog, bridge, settlement, blackout, fog ridge,
and refinery composition families. A new bounded SDXL+nerijs higher-art pass now
adds local candidate references for Missions 4-6 escalation, Sable/Meridian
style, Mission 8 blackout, Mission 10 refinery finale, and commander portraits.
Mission 8 and Mission 10 are cleanup candidates; Mission 4-6 and
Sable/Meridian are reference-only; the commander portrait pass is rejected for
portrait fulfillment. The latest SDXL token review packet is
`game/WargamePrototype/assets/art-handoff/local-review/20260503-144216/`.
Prompt-only SDXL+nerijs is useful for style references, but still unreliable for
transparent runtime unit atlases without guided silhouettes or cleanup.
Ordered enemy-phase playback should still wait for structured deterministic
events from the C# core.
The prototype now also has AI-vs-AI campaign playtesting support. The Godot
controller can toggle AI playtest mode with F9, and the smoke-test project can
run logged campaign attempts with `playtest-ai`. The Mission 3 blocker is now
fixed: the latest generated log, `ai-campaign-20260503-152414-01.jsonl`, records
Mission 3, `Pump Road Convoy`, as a Turn 4 player victory and starts Mission 4.
The same AI-vs-AI path now completes the full first-act campaign. The latest
generated log, `ai-campaign-20260503-152957-01.jsonl`, records victories for
Missions 1-10, Mission 9 victory on Turn 9, Mission 10 victory on Turn 12, and
campaignComplete true. A compact log-summary command now exists for future
triage.
The mission-design research pass now adds a formal quality rubric for tactical
missions and long-campaign progression in
`docs/game/mission-campaign-design-rubric.md`. Future mission work should not
treat AI campaign completion as sufficient design evidence; it should evaluate
tactical thesis, objective pressure, plan-space variety, terrain value, AI
pressure, score spread, deterministic replay evidence, and 1280x800
readability. The next ready slice is a Missions 1-10 variance matrix and
quality review.
The agentic ecosystem now also includes a periodic Cruft Cleaner. It finds
unused tracked code, art, prompt specs, and docs with explicit reference
evidence, then moves only confirmed cruft into `archive/cruft/<date>/` with a
manifest and restore guidance. It is wired into the orchestrator for hygiene
passes after several autonomous slices, before pull request prep, and after
broad art-generation batches.

## Active Mode

Autonomous by default, with human input treated as non-blocking guidance.
Security issues halt the loop. Non-security human decisions are logged in
`human-intervention.md`, and the loop continues other useful work when possible.

## Current Focus

Use the repo-based work tracking and development log system to continue from
the completed lore, combat-feedback, arena-HUD, first-act campaign-flow, and
mission-design rubric spikes into Missions 1-10 quality review, visual QA,
playtest tuning for Missions 3-10, authored mission briefs and maps, starter
tileset planning, CO power budgeting, unit-ramp checks, and periodic reversible
cruft cleanup.

## Last Completed Work

* Created Copilot custom agents, prompt files, instructions, security docs, game
  direction docs, and secret-pattern scanner.
* Validated Markdown diagnostics, hook configuration parsing, repo secret scan,
  and hook-mode allow response.
* Added repo-based work tracking, non-blocking human feedback, adversarial
  critique, experiment planning, and autonomous continuation prompts.
* Added a human intervention log and prompt interface for non-security items
  that need human judgment while autonomous work continues elsewhere.
* Captured the initial product goal: classic Advance Wars center of gravity,
  realistic near-future sci-fi setting, grounded political conflict, grounded
  humor, minor seeded randomness, no persistent individual units, AI-only play,
  static HQ capture stakes, CO powers, terrain, light logistics, 16-bit-era
  pixel art, and Godot C# preference.
* Captured the first prototype spec: fixed-unit chokepoint HQ defense with scout
  rescue, direct counterattacks, terrain effects, objective AI pressure, and
  numeric scoring.
* Implemented a Godot C# first mission scene backed by a plain C# rules core.
* Added blocky 16-bit-style placeholder rendering for the map, units, cursor,
  status panel, and score screen.
* Added smoke checks for movement, terrain forecasts, scout rescue, HQ defeat,
  replay hash determinism, AI pressure, and scoring.
* Added an AI-vs-AI proof replay and expanded the first mission with a second
  infantry unit, a five-unit enemy patrol, wider lanes, extra cover, and clearer
  unit sprites.
* Added a first graphics overhaul with richer terrain, framed board styling,
  polished HUD sections, improved highlights, unit bases, HP bars, cursor, and
  score presentation.
* Added actual PNG sprite sheets for terrain and units, then updated the Godot
  renderer to draw texture regions from those sheets.
* Added a repeatable sprite generator and upgraded the sheets to native 64x64
  frames with richer terrain detail, unit silhouettes, outlines, and palette
  depth.
* Cleaned up the generated terrain after running-window feedback: calmer road
  art, quieter plains and ridges, softer grid lines, unit backing plates, and a
  wrapped side-panel legend.
* Added a code quality and architecture constitution plus instruction hooks for
  periodic code review and bounded autofix of accepted suggestions.
* Added a 50-mission campaign plot spine that starts with the Kestrel Survey
  Expedition's personal survival, escalates through Orison, Sable, Meridian,
  and grid-control arcs, and preserves regular player victory beats.
* Added a campaign environment plan that gives each stage distinct terrain,
  flavor, palette, landmarks, and tactical identity while keeping terrain rules
  reusable and Steam Deck-readable.
* Drafted the first-six-mission unit ramp and generated a separate nine-unit
  campaign sprite planning sheet.
* Added a campaign character bible with detailed commander backgrounds, arcs,
  interactions, voice examples, doctrine, candidate CO powers, signature-unit
  candidates, and validation guardrails.
* Added a grounded universe backstory that defines Asterite limits, the Basin
  Stabilization Grid, Transit Thread interstellar travel, Spindle Net FTL
  messaging, the historical timeline, political factions, worlds, technologies,
  and narrative rules.
* Captured a combat-feedback direction that prioritizes visible player-attack
  damage numbers, HP tweening, damage-state overlays, hit flash or recoil, and
  clearer tactical callouts without changing deterministic combat rules.
* Implemented the first combat feedback presentation spike in the Godot layer
  without changing the C# rules core.
* Implemented the arena-first HUD/readability spike in the Godot layer with HQ,
  Scout-7, rescue-zone, terrain, readiness, forecast, mode, cursor, and prompt
  cues while preserving the sidebar as a development inspector.
* Incorporated returned ChatGPT concept images into the Godot campaign flow and
  built a first playable Mission 2 with deterministic relay and fuel objectives.
* Reviewed all returned and generated art, then used the viable pieces in the
  runtime: direct cutscene backgrounds, direct commander portrait, generated
  terrain updates, and generated HUD icon atlas.
* Installed a local ComfyUI source runtime and added a C# pixel-art candidate
  generator command backed by tracked prompt job specs.
* Iterated SDXL+nerijs prompts for grouped unit sheets and early mission
  cutscenes, producing selected vehicle, infantry, Mission 1 intro, Mission 2
  relay, and Mission 3 pump-station references.
* Added graphics specialist agents and a reusable pixel-art style guide plus
  shared prompt context for consistent local sprite generation.
* Added an art-handoff status ledger, generated Mission 1 Kestrel one-token
  local candidates, and recorded their review-packet result.
* Added a playable Missions 1-10 campaign implementation with deterministic
  mission catalog data, intro and debrief screens, generic progression, final
  campaign completion, expanded unit role display, and smoke coverage.
* Added AI-vs-AI playtest support, per-playthrough JSONL logs, a Playthrough Log
  Analyst custom agent, and log-analysis backlog items for campaign blockers
  and compact log summaries.
* Tuned the campaign autoplayer with bounded later-mission beam planning and
  objective cleanup so Mission 3 now clears in AI-vs-AI playtests and Mission 4
  starts.
* Added a compact `summarize-playtest-log` command that extracts only
  playthrough lifecycle, mission lifecycle, issue-candidate, and playthrough-end
  records from verbose JSONL logs.
* Fixed the Mission 9 AI-vs-AI blocker by moving allied blockers aside during
  post-combat objective cleanup, allowing the surviving Engineer to reach the
  scan relay and complete the campaign.

## Next Best Actions

1. Replace the placeholder request 08 unit atlas with a higher-quality
  transparent 9x2 sprite atlas that passes board-readability review.
2. Play through the new Missions 1-10 campaign loop and tune Missions 3-10 from
  generated fixtures into authored scenarios.
3. Capture 1280x800 visual evidence for the new intro, debrief, campaign
  complete, HUD, and objective-marker surfaces.
4. Run the sidebar-covered 1280x800 arena readability pass for the new compact
  HUD and marker layer.
5. Add a promotion or review-packet step that crops selected SDXL+nerijs unit
  references into 64x64 mock sprites and checks them on representative terrain.
6. Continue request 10 with focused Missions 4-10 higher-art batches for Sable,
  Meridian, Mission 8 blackout assets, Mission 10 refinery assets, and commander
  portrait expansions.
7. Playtest Mission 2 for pacing, objective clarity, and unit-role interest.
8. Draft Missions 1-3 as implementation-ready playable briefs with grounded
  lore fields for transit delay, Spindle message priority, cargo or permit
  constraint, grid stakeholder, Asterite supply-chain cost, and tactical
  objective verb.
9. Draft a CO power rule budget from the character bible before implementing
   any commander powers.
10. Add unit profile metadata and combat-matrix checks while preserving current
  Infantry, Armor, and Scout behavior.
11. Turn the environment plan into an Aster Basin starter tileset spec for
  Missions 1-3.
12. Add broader 1280x800 readability evidence for the 16-bit pixel-art direction
  and campaign unit planning sheet.

## Open Risks

* True unbounded execution still depends on repeated Copilot invocations or
  cloud-agent tasks. The repo system makes each invocation resumable and
  self-directing.
* The Godot C# stack builds and smoke-runs locally, but the mission still needs
  human playtest feedback.
* Human intervention routing needs to be exercised during the first remote,
  deployment, or destructive-operation decision.
* Minor randomness must stay seeded, forecast-visible, and replayable.
* Sprite-sheet visuals are functional, but not yet validated by screenshots
  across 1280x800, 1280x720, and 1920x1080.
* The new arena HUD reduces sidebar dependence, but the sidebar-covered
  1280x800 readability pass is still pending. Tile overlays also risk clutter
  because 64px tiles now carry terrain, unit art, HP, type labels, team badges,
  cursor, highlights, readiness badges, rescue corners, and terrain pips.
* The 50-mission campaign spine is intentionally modular. Later arcs should not
  be treated as locked mission commitments until the tactical vocabulary has
  been validated in playable form.
* Environment variety can explode scope if treated as 50 bespoke tilesets or a
  new terrain mechanic for every story noun. The plan should stay focused on a
  small rule budget, reusable kits, and screenshot readability checks.
* The first-six-mission roster could become too complex if support, ammo,
  range, fog, jamming, hover, and disable systems ship before focused rule and
  replay tests exist.
* Character powers and signature units could quietly become a second rules
  engine if implemented before the CO power command, forecast, AI, replay, and
  mission-introduction budgets are defined.
* The grounded universe can drift back into macguffin logic if Asterite, grid,
  Transit Thread, or Spindle Net reveals stop naming supply-chain costs, legal
  stakeholders, route constraints, message custody, and tactical map verbs.
* Combat effects can become false clarity if they obscure board facts, display
  forecast values as though they were resolved damage, or delay state mutation
  in a way that conflicts with deterministic replay.
* Mission 2 is mechanically verified but still needs human playtest and
  screenshot evidence. The relay and fuel wait objectives may need stronger
  affordances or different pacing after controller-first play.
* AI-vs-AI campaign playtesting now completes Missions 1-10 within the 20-turn
  cap, but several victories are loss-heavy. Mission 2, Mission 3, Mission 4,
  Mission 5, Mission 7, and Mission 10 should be tuned for better technique and
  less throwaway-unit behavior after the basic campaign completion gate is
  preserved.
* Raw playthrough logs are intentionally verbose and ignored. Repeated analysis
  should use the compact C# summary path to avoid flooding chat or tracked files
  with command-level unit snapshots.
* The local image-generation pipeline is installed but has not produced a real
  candidate batch yet because a license-approved checkpoint model still needs
  to be chosen and placed in the ignored local ComfyUI model folder.
