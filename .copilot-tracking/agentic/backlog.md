---
title: Agentic Backlog
description: Prioritized repo-based backlog for autonomous development work
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Queue Rules

Backlog items should be small enough for one autonomous pass when possible.
Each item needs a clear outcome, verification idea, and current status.

Status values: `proposed`, `ready`, `active`, `blocked`, `done`, `dropped`.

## Ready

### G049: Extract request 08 vehicle sprite candidates

Status: `ready`

Outcome: Convert the strongest Utility Armor and Survey Scout v3 candidates into
transparent runtime-ready sprite sources, then assemble a small reviewed request
08 vehicle mini-atlas before attempting the full 9x2 unit atlas.

Verification: Use `prepare-img2img-source`, crop and alpha cleanup tooling, and
`candidate-review` to compare the cleaned Utility Armor seed `58801` and Scout
seed `58600` at 64x64 over representative terrain. Record accepted and rejected
outputs in the request 08 ledger. Do not promote C# primitive art.

### G051: Commander portrait template pass

Status: `ready`

Outcome: Use the Venn v3 one-bust prompt pattern to generate clean commander
portrait candidates without side-card or text clutter.

Verification: Generate one commander bust per job, visually reject candidates
with text, maps, side panels, or full-body drift, and record accepted portrait
source candidates in request 10.

### G048: Missions 1-10 variance matrix and quality review

Status: `ready`

Outcome: Apply the mission and campaign design rubric to Missions 1-10. Produce
a compact matrix that identifies each mission's tactical thesis, objective
verb, pressure clock, map topology, enemy doctrine, dominant unit role,
mechanic introduction, score emphasis, replay evidence, 1280x800 readability
risk, and repetition risk.

Verification: Use `docs/game/mission-campaign-design-rubric.md` as the rubric,
review the campaign documents and current deterministic playtest summaries, and
record red/yellow/green findings plus the next highest-value mission tuning
slice. Do not treat AI campaign completion alone as proof of mission quality.

## Blocked

### G045: Replace placeholder unit atlas art

Status: `active`

Outcome: Replace the request 08 deterministic placeholder unit atlas through
the local SDXL+nerijs candidate pipeline. The accepted result must keep the exact
2304x512 layout and transparent background while improving silhouette, faction
identity, polish, and 64x64 board readability beyond the current rejected C#
primitive tokens.

Blocker: The local SDXL+nerijs pipeline has produced plausible Utility Armor and
Survey Scout vehicle sources, but no complete 9x2 transparent runtime atlas has
been assembled or promoted. Field Tech and the remaining roster still need
quality-equivalent candidates.

Next action: Complete G049 by extracting and reviewing the Utility Armor v3 and
Scout v3 candidates as a vehicle mini-atlas, then run a fresh infantry-focused
Field Tech pass before expanding to the full roster.

Verification: Run the request 08 SDXL job spec with `pixelart generate`, record
accepted candidates in the request 08 ledger, generate a board-readability
review packet, visually inspect the atlas and board sheet, and keep C# primitive
art marked rejected.

## Done

### G050: Request 11 exact-topology autotile strategy

Completed: 2026-05-04

Evidence: Added generated runtime `art_paths.png` path atlas composition from
May 4 source materials. The atlas provides exact road, river, and bridge mask
rows while preserving runtime fallback behavior when the atlas is absent or
malformed.

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  regenerated `art_paths.png`.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- terrain-path-review`
  generated path atlas, board, and bridge review images.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeded.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.

Residual risk: the atlas is runtime promoted with QA pending, not final art
signoff. Real mission screenshots at 1280x800 should confirm readability under
units, cursor boxes, objective markers, and HP bars.

### G052: Periodic cruft cleaner agent

Completed: 2026-05-03

Evidence: Added the Cruft Cleaner custom agent, a periodic prompt entry point,
orchestrator handoff wiring, and a tracked archive policy under
`archive/cruft/README.md`.

Verification:

* The cleaner archives only confirmed unused tracked code, art, prompt specs,
  and docs.
* Cleanup passes preserve original relative paths under `archive/cruft/<date>/`
  and record restore guidance in a manifest.
* The agent explicitly skips ignored private folders, runtime logs, local
  ComfyUI output, build output, credential stores, and uncertain active assets.
* The orchestrator can invoke the cleaner after three to five autonomous slices,
  before pull request prep, or after broad art-generation batches.

### G047: Mission and campaign design rubric

Completed: 2026-05-03

Evidence: Added `docs/game/mission-campaign-design-rubric.md` after a research
pass using Product Strategist, Game Architect, Adversarial Critic, and
Experiment Planner perspectives. The rubric defines good mission criteria, bad
mission anti-patterns, good and bad campaign progression, evaluation questions,
automated evidence signals, and promotion gates for tactical missions.

Verification:

* Criteria are grounded in the existing product goal, first-act mission plan,
  campaign spine, environment plan, unit ramp, deterministic replay direction,
  Steam Deck readability target, and AI-vs-AI playtest support.
* The rubric requires objective pressure, plan-space variety, terrain value,
  AI pressure, deterministic replay evidence, scoring spread, and 1280x800
  readability before missions are considered good campaign content.
* Follow-up G048 is ready to apply the rubric to Missions 1-10.

### G042: Missions 4-10 higher-art imagery pass

Completed: 2026-05-03

Evidence: Added five tracked SDXL+nerijs prompt specs for the remaining request
10 higher-art gaps and generated one local candidate per spec through the C#
`pixelart generate` pipeline. The pass covers Missions 4-6 escalation imagery,
Sable/Meridian faction exploration, Mission 8 blackout assets, Mission 10
refinery finale assets, and commander portrait expansion. Request 10's response
ledger records each result as reference-only, cleanup candidate, or rejected.
No new runtime art was promoted.

Verification:

* `Invoke-WebRequest` confirmed local ComfyUI was reachable on localhost before
  generation.
* This command generated five manifest-backed candidate folders under ignored
  `local-candidates/`:

  ```powershell
  dotnet run --project `
    .\src\Wargame.AssetTools\Wargame.AssetTools.csproj `
    pixelart generate `
    .\game\WargamePrototype\assets\art-handoff\pixelart-prompts\mission4-6-escalation-cinematic-sdxl-nerijs-v1.sample.json
  ```

* Visual review classified Mission 8 blackout and Mission 10 refinery as
  cleanup candidates, Mission 4-6 and Sable/Meridian as reference-only, and the
  commander portrait pass as rejected for portrait fulfillment.
* Security checks: passed. Generated local candidates remain under ignored art
  folders and are not promoted as runtime assets.

### G036: Grounded mission brief lore pass

Completed: 2026-05-03

Evidence: The first-act campaign-mode spine now has a `Grounded Brief Fields`
section. Missions 1-3 point to the existing detailed playable briefs, and
Missions 4-10 now each name concrete route, schedule, Spindle, permit, manifest,
stakeholder, Asterite supply-chain, public-safety, and tactical-verb hooks for
briefing and debrief writing.

Verification:

* Mission 4 names fabricator feedstock, emergency fabrication authority, depot
  custody, and grid-tap overdraw risk.
* Mission 5 names Sable braking-right authority, relay priority conflict, and
  trapped sample notes.
* Mission 6 names the heavy-route bridge, fraudulent maintenance permit, and
  coolant-bypass public-safety cost.
* Mission 7 names Meridian freight-slot pressure, heat-tap authority, and
  civilian Asterite cost.
* Mission 8 names the audit convoy landing queue, custody hash, and supply hub
  dependency.
* Mission 9 names the courier handoff route, jammer timestamps, map custody, and
  scan-data extraction verb.
* Mission 10 names Orison's refinery permit, freight-plan broadcast, power
  siphon, affected heat taps, and custody-proof broadcast.

### G034: CO power rule budget

Completed: 2026-05-03

Evidence: Added `docs/game/co-power-rule-budget.md` and linked it from the
campaign character bible. The budget defines charge sources, activation command
shape, duration limits, affected tags, forecast display requirements, AI
fairness rules, replay data needs, mission timing, and implementation gates.

Verification:

* The budget selects Rusk's `Lock The Line` as the safest first prototype power
  with a defense-only v0 that uses existing combat math.
* Venn and Holt are deferred until inspect clarity, objective modifiers, fog,
  sensor posts, and mark forecasts exist.
* Sloane is deferred until property income and production exist.
* Priya, Rhee, Calder, and Kravic are rejected for the first prototype slice
  because their effects require support actions, capture economy, movement-cost
  changes, zone-of-control, or telegraphed strike systems not yet implemented.
* The budget requires compact inspect-panel text and deterministic command-log
  representation before implementation.

### G023: Missions 1-3 playable briefs

Completed: 2026-05-03

Evidence: `docs/game/missions-1-3-briefs.md` contains implementation-ready
briefs for Missions 1-3 with grounded lore context, situation, opening
briefing, mission objective, map concept, starting forces, new units, rules,
tactical lesson, story reveal, victory beat, debrief, tone note, implementation
dependencies, enemy behavior budgets, and radio banter hooks.

Verification:

* Mission 1 aligns with the existing prototype rescue, HQ hold, terrain cover,
  direct attack, counterattack, and enemy pressure systems.
* Mission 2 identifies capture, Engineer repair/stabilize, and Sapper matchup
  dependencies before implementation.
* Mission 3 identifies escort crawler, AT Lancer, and Hunter Bike dependencies
  before implementation.
* Added deterministic enemy-behavior budgets and compact radio-banter hooks for
  all three missions.

### G032: Aster Basin starter tileset spec

Completed: 2026-05-03

Evidence: Added `docs/game/aster-basin-starter-tileset-spec.md`, a concrete
64px starter terrain and prop plan for Missions 1-3. The spec maps existing
terrain rules to runtime visual IDs, proposes a stable 24-cell starter sheet
order, lists Mission 1-3 required props and missing assets, separates terrain
from tactical overlays, defines a 1280x800 readability mock plan, and names
promotion gates for `art_terrain.png`.

Verification:

* The spec reuses Plain, Road, Cover, HQ, Ridge, Property, and Objective object
  terrain roles rather than inventing new rules.
* The spec covers base ground, road, cover, camp HQ, relay, fuel cache, pump
  station, seam accents, objective markers, and the Steam Deck readability mock.
* The spec records existing runtime and reference assets, including
  `terrain.png`, `art_terrain.png`, request 07 terrain variants, and request 10
  act-one terrain references.

### G046: Mission 9 AI playtest blocker tune

Completed: 2026-05-03

Evidence: `CampaignAutoplayer` objective cleanup now detects when surviving
capturers are boxed in by allied units after enemies are cleared and moves the
blocking unit aside before continuing objective capture. This fixes the Mission
9 post-combat stall where Engineer-1 could not path toward the scan relay while
Armor-1 and Lancer-1 occupied adjacent route tiles. Smoke coverage now includes
Mission 9 autoplayer victory. The latest AI playtest log,
`ai-campaign-20260503-152957-01.jsonl`, records Mission 9 as PlayerVictory on
Turn 9, starts Mission 10, records Mission 10 victory on Turn 12, and ends with
campaignComplete true.

Verification:

* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 16 smoke checks.
* `dotnet build game/WargamePrototype/WargamePrototype.sln` succeeds.
* This command generates `ai-campaign-20260503-152957-01.jsonl` and reports
  completed campaigns 1/1:

  ```powershell
  dotnet run --project `
    src/Wargame.SmokeTests/Wargame.SmokeTests.csproj `
    -- playtest-ai `
    --max-turns=20
  ```

* Security checks: passed. Raw JSONL logs remain under ignored run folders.

### G044: Compact playthrough log summaries

Completed: 2026-05-03

Evidence: `Wargame.SmokeTests` now exposes `summarize-playtest-log`, a C#-first
JSONL summary command that prints only `playthrough-start`, `mission-start`,
`mission-end`, `issue-candidate`, and `playthrough-end` records. It intentionally
omits command events and unit snapshot arrays so future playtest analysis stays
compact and public-safe.

Verification:

* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 15 smoke checks.
* The summary command reports Mission 1 and Mission 2 victories, Mission 3
  defeat, the Mission 3 campaign-blocker issue candidate, and campaignComplete
  false for `ai-campaign-20260503-145622-01.jsonl` without printing
  per-command unit arrays.
* Security checks: passed. Raw JSONL logs remain under ignored run folders.

### G043: Mission 3 AI playtest blocker tune

Completed: 2026-05-03

Evidence: `CampaignAutoplayer` now uses a bounded deterministic beam planner
for later campaign missions, preserving the existing Mission 1 exhaustive
planner while scoring complete later-mission turns through the enemy phase. It
also has an objective-cleanup path for capture missions after enemies are
cleared, preventing surviving infantry or engineers from idling with unfinished
objectives. Smoke coverage now includes Mission 3 autoplayer victory. The latest
AI playtest log, `ai-campaign-20260503-152414-01.jsonl`, records Mission 3 as
PlayerVictory on Turn 4 with Mission 4 starting next and no Mission 3
campaign-blocker issue candidate. The same run advances through Mission 8 and
now blocks at Mission 9, tracked separately as G046.

Verification:

* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 15 smoke checks.
* This command generates `ai-campaign-20260503-152414-01.jsonl`, advances past
  Mission 3, starts Mission 4, and later reports the Mission 9 blocker:

  ```powershell
  dotnet run --project `
    src/Wargame.SmokeTests/Wargame.SmokeTests.csproj `
    -- playtest-ai `
    --max-turns=20
  ```

* Security checks: passed. Raw JSONL logs remain under ignored run folders.

### G041: Playable Missions 1-10 campaign implementation

Completed: 2026-05-03

Evidence: `Wargame.Core` now has a deterministic campaign catalog and factory
for Missions 1-10. The Godot controller now uses a generic campaign loop with
mission intro, battle, debrief, next-mission advancement, defeat retry, and a
campaign-complete screen after Mission 10. Mission objective text, rescue text,
failure copy, objective markers, and expanded unit role display now come from
campaign state instead of Mission 1 and Mission 2 branches. Smoke tests verify
that all 10 missions can be created with progression metadata. Targeted VS Code
diagnostics, `dotnet build game/WargamePrototype/WargamePrototype.sln`,
`dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`, and
`git diff --check` pass.

### G040: Missions 1-10 campaign mode and imagery backlog

Completed: 2026-05-03

Evidence: `docs/game/missions-1-10-campaign-mode.md` now defines the first-act
campaign shell, save metadata, shared systems, mission records for Missions
1-10, art IDs, implementation slices, and acceptance criteria. The existing
art-handoff tree under `game/WargamePrototype/assets/art-handoff/requests` now
contains `10-missions-01-10-imagery-thread` with a prompt backlog and response
ledger for cutscenes, portraits, unit sprites, terrain and props, UX overlays,
Sable/Meridian style exploration, Mission 8 blackout assets, and Mission 10
refinery finale assets. The ChatGPT image prompt index links the new request
folder.

### G039: Source-art extractor and transparent atlases

Completed: 2026-05-02

Evidence: `PngReader.cs` and `SpriteSheetExtractor.cs` add a C#-first,
manifest-driven extraction path for returned source-art sheets. `Program.cs`
now exposes `extract-art` while preserving the existing local `pixelart`
command. `source-art-extraction.json` produces `art_terrain.png`,
`art_units.png`, and `art_ui_icons.png` with transparent unit/icon backgrounds
and deterministic terrain variants. `BattleController.cs` now prefers the
extracted atlases and falls back to generated sheets. Rerunnable prompt folders
07-09 request transparent unit/icon sheets and continuous terrain variants.
AssetTools build, extractor run, smoke tests, Godot C# build, Godot headless
startup, diagnostics, and whitespace checks pass.

### G037: Two-mission campaign-flow prototype

Completed: 2026-05-02

Evidence: Returned ChatGPT concept images are now recorded in the six art
handoff response ledgers. The Godot prototype opens with the Mission 1 concept
cutscene image, plays Mission 1, advances on victory to the Mission 2 relay-yard
concept image, and then starts a new Mission 2 scenario. `BattleCore.cs` now
defines Mission 2 metadata, Engineer and Sapper profiles, relay and fuel-cache
objective progress, and `SecondMissionFactory.Create()`. The smoke runner
verifies Mission 2 objective completion and victory. `BattleController.cs`
renders mission-aware cutscene screens, HUD text, objective markers, unit labels,
and Mission 1 victory advancement. Smoke tests, Godot C# build, Godot headless
startup, and VS Code diagnostics pass.

### G038: Generated art review and runtime integration

Completed: 2026-05-02

Evidence: All returned ChatGPT concept images and generated runtime sheets were
reviewed. Mission 1 and Mission 2 concept frames remain direct cutscene screens.
The commander portrait concept is now used directly in cutscene dialogue panels.
The terrain concept sheet drove an updated deterministic `terrain.png` with
dusty basin ground, asphalt service road, Kestrel crate cover, prefab HQ, and
basalt ridge tiles. The UI icon concept drove a deterministic `ui_icons.png`
atlas used by HUD prompt and status chips. The unit concept sheet remains
reference only because it is a labeled presentation sheet, while
`campaign_units.png` remains the runtime generated unit atlas. Asset generation,
smoke tests, Godot C# build, Godot headless startup, diagnostics, and whitespace
checks pass. Adversarial critique found a Mission 2 objective-state replay hash
gap; the hash now includes mission objective state and smoke coverage confirms
objective progress changes the hash.

### G035: Grounded universe and Transit Thread

Completed: 2026-05-02

Evidence: `docs/game/universe-backstory.md` now defines a grounded political
and industrial setting with Asterite limits, the Basin Stabilization Grid, the
Transit Thread slow-travel mechanism, the Spindle Net FTL messaging layer,
historical timeline, political factions, worlds, technology rules, and
narrative rules. The campaign plot spine, environment plan, character bible,
product goal, active goal, and technical direction were updated away from
mystical Loom/macguffin framing and toward infrastructure authority, transit
logistics, message priority, liability, and public governance.

### G033: Campaign character bible

Completed: 2026-05-02

Evidence: `docs/game/campaign-character-bible.md` now defines detailed
backgrounds, personalities, motivations, contradictions, campaign arcs,
relationship hooks, voice examples, commander doctrine, candidate CO powers,
charge patterns, signature-unit candidates, counterplay risks, Bureau and Loom
voice guidance, and validation gates for deterministic replay, forecast honesty,
AI fairness, and no-new-unit fallbacks. `docs/game/campaign-plot-spine.md`
links to the character bible.

### G030: Arena-first HUD readability spike

Completed: 2026-05-02

Evidence: `BattleController.cs` now keeps the verbose sidebar as a development
inspector while adding a board-owned mission marker and compact bottom HUD pass.
The arena shows HQ and Scout-7 state, rescue-zone corners, terrain defense
pips, player readiness badges, cursor and terrain chips, selected-unit and mode
chips, compact attack/counter forecasts, and controller prompts. A focused
Adversarial Critic review found draw-order, prompt, and rescue-marker issues;
those were fixed before validation. VS Code diagnostics, deterministic smoke
checks, Godot C# build, and Godot startup pass. Sidebar-covered 1280x800 visual
readability remains tracked in E007 as follow-up evidence.

### G031: Campaign environment plan

Completed: 2026-05-02

Evidence: `docs/game/campaign-environment-plan.md` defines eight reusable
environment kits, a compact terrain rule budget, detailed environment beats for
Missions 1-10, five-mission environment arcs through Mission 50, tile
readability rules, mission-brief environment fields, and validation checks.
`docs/game/campaign-plot-spine.md` links to the environment plan.

### G025: Combat feedback presentation spike

Completed: 2026-05-02

Evidence: `BattleController.cs` now captures before-and-after snapshots for
successful attack commands and derives presentation-only damage feedback from
actual HP deltas. Player attacks and counterattacks show floating damage
numbers, HP bars tween toward authoritative core HP while HP labels stay
authoritative immediately, damaged units flash and recoil briefly, low-HP units
show damage overlays, and adjacent attack targets show compact damage and
return-fire chips. `WargamePrototype` README documents the feedback. Godot C#
build, deterministic smoke checks, Godot startup, and diagnostics pass.

### G026: First six mission unit ramp

Completed: 2026-05-02

Evidence: `docs/game/first-six-mission-unit-ramp.md` now defines a compact
nine-unit roster, stats, rock-paper-scissors loops, support interactions,
mission-by-mission introduction order, mission flavor, sprite requirements, and
implementation order. The sprite generator now emits the separate planning
sheet `game/WargamePrototype/assets/sprites/campaign_units.png`. Diagnostics,
generator syntax check, regeneration, whitespace check, and secret-pattern scan
pass.

### G022: Campaign plot spine

Completed: 2026-05-02

Evidence: `docs/game/campaign-plot-spine.md` defines the campaign premise,
factions, commander cast, rare-material mystery, detailed Missions 1-10, and
modular five-mission arcs through Mission 50. The outline starts with personal
survival, escalates outward by region and faction, and includes clean victory
beats at regular intervals.

### G020: Code quality and architecture constitution

Completed: 2026-05-02

Evidence: `docs/game/code-quality-architecture-constitution.md` now defines
project architecture boundaries, dependency policy, deterministic replay rules,
quality gates, review triggers, suggestion taxonomy, autofix protocol, stop
conditions, evidence standards, and revisit triggers. Repo, agentic workflow,
and game instructions now require periodic review and bounded autofix of
accepted suggestions.

### G019: Board readability and road cleanup

Completed: 2026-05-02

Evidence: The terrain generator now emits a calmer road tile without diagonal
stripe repetition, quieter grass and ridge texture, and regenerated PNG terrain
sheets. `BattleController.cs` softens the board grid, adds a subtle backing
plate behind units, and wraps the side-panel unit-shape legend. Smoke tests,
Godot build, Godot startup, generator syntax check, and diagnostics pass.

### G018: SNES and DS style sprite upgrade

Completed: 2026-05-02

Evidence: A repeatable sprite generator now emits 64x64 terrain and unit sheets
with richer shading, outlines, silhouettes, and texture. `BattleController.cs`
uses 64x64 source regions and draws unit frames at native tile scale. Manual
feedback says the updated graphics are much better.

### G017: Sprite sheet asset migration

Completed: 2026-05-02

Evidence: `game/WargamePrototype/assets/sprites` now contains PNG sheets for
terrain and units. `BattleController.cs` loads those sheets as image textures
and draws texture regions for board tiles and unit frames. Godot build and
headless startup checks pass.

### G008: Pixel-art readability spike

Completed: 2026-05-02

Evidence: The Godot scene now uses a framed battlefield, richer pixel-patterned
terrain, polished unit bases and HP bars, clearer highlights, a stronger cursor,
and a more finished right-side HUD. Build and startup checks pass.

### G016: Expanded first mission roles and sprites

Completed: 2026-05-02

Evidence: The first mission now includes two player infantry units, one armor,
Scout-7, and five enemy units. Smoke checks verify the expanded roster and AI
victory. Godot sprites have clearer infantry, armor, and scout silhouettes.

### G013: AI-vs-AI smoke test

Completed: 2026-05-02

Evidence: `src/Wargame.SmokeTests` prints a deterministic AI replay through the
same command interface used by the playable prototype and verifies player-side
victory.

### G015: First mission playtest tuning

Completed: 2026-05-02

Evidence: First playtest feedback drove clearer rescue instructions, stronger
enemy/friendly differentiation, distinct infantry/armor/scout placeholder
sprites, explicit end-turn copy, and a smoke-tested opening grace phase so
ending turn immediately does not defeat the player.

### G009: Numeric score fixture

Completed: 2026-05-02

Evidence: `src/Wargame.Core` calculates objective, speed, technique, power, and
total score. `src/Wargame.SmokeTests` verifies score categories.

### G007: Objective AI pressure fixture

Completed: 2026-05-02

Evidence: Enemy AI advances toward mission objectives and the smoke test verifies
that enemy distance to the player HQ decreases after an enemy phase.

### G006: HQ and scout objective fixture

Completed: 2026-05-02

Evidence: `src/Wargame.Core` resolves static HQ defeat and scout rescue state.
`src/Wargame.SmokeTests` verifies both outcomes.

### G005: Combat forecast fixture

Completed: 2026-05-02

Evidence: Combat forecasts include seeded variance ranges and terrain defense.
Smoke checks verify that cover changes expected damage.

### G004: Movement and terrain fixture

Completed: 2026-05-02

Evidence: Movement range uses terrain costs, ridges, roads, and occupied tiles.
Smoke checks verify chokepoint movement and blockers.

### G003: Deterministic board model

Completed: 2026-05-02

Evidence: `src/Wargame.Core` contains deterministic board, terrain, unit, team,
state, command, scoring, and state-hash models.

### G002: Godot C# engine spike

Completed: 2026-05-02

Outcome: Minimal Godot 4.x C# project shape is selected and kept compatible with
a plain C# simulation core.

Evidence: `game/WargamePrototype` builds as a Godot 4.6 C# project referencing a
plain C# rules core. Godot headless startup succeeds.

## Proposed

## Completed Foundation

### G012: CO power scaffold

Completed: 2026-05-03

Outcome: One CO power hook can influence a battle without requiring mobile
leaders on the board.

Evidence: Added an explicit `ActivatePower` command and the first Rusk
`lock-the-line` rules scaffold. Player commander charge increases when a
friendly unit takes damage and survives. Activation spends four charge and adds
one defense to friendly ground units that did not move during the current player
turn, then expires when the next player turn begins.

Verification: `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
passes 20 checks, including `lock the line power charges and expires`.

### G011: Light supply fixture

Completed: 2026-05-03

Outcome: One ammo and resupply rule affects legal actions and forecasts.

Evidence: `UnitState` now supports opt-in limited ammo. Units with ammo set to
zero cannot attack, disappear from attackable targets, and forecast zero damage.
Field Rigs can wait to resupply one adjacent friendly limited-ammo unit.
Unlimited-ammo units keep their existing behavior.

Verification: `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
passes 19 checks, including `field rig resupplies limited ammo` and the replay
fixture.

### G010: Capture economy fixture

Completed: 2026-05-03

Outcome: Infantry can capture a property and update per-turn income.

Evidence: `BattleState` now tracks player-controlled properties, player income,
and player funds. Relay and fuel objective captures add one controlled property
and `BattleState.PropertyIncomeValue` to player income. Player funds are paid on
the next player turn through the deterministic end-turn path.

Verification: `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
passes 18 checks, including `capture economy awards player income`.

### G014: Replay command log

Completed: 2026-05-03

Outcome: Initial command stream format is defined.

Evidence: Added `docs/game/replay-command-log-format.md` and a smoke-runner
fixture that serializes, deserializes, and replays the Mission 1 opening command
stream from the mission factory through `BattleRules.ApplyCommand`.

Verification: `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
passes, including `replay command stream reproduces expected state`.

### G001: Product goal and prototype scope

Completed: 2026-05-02

Evidence: Product goal captured as near-future sci-fi classic Advance Wars-style
army tactics with Godot C#, AI-only play, CO powers, static HQ capture stakes,
terrain, light logistics, minor seeded randomness, and no map editor or
multiplayer.

### A001: Autonomous tracking scaffold

Completed: 2026-05-02

Evidence: Repo work tracking files, autonomous prompt, adversarial roles, and
updated orchestrator instructions exist. Diagnostics and repo secret scan pass.

### D001: Sanitized Deck workflow

Completed: 2026-05-03

Outcome: Local Deck deploy config schema is documented without values.

Evidence: Added `docs/game/sanitized-steam-deck-workflow.md`. The document
defines the local configuration schema, allowed transports, script contract,
sanitized log shape, validation checklist, and future implementation slice while
explicitly excluding private hostnames, IP addresses, usernames, SSH key paths,
Syncthing secrets, private LAN paths, and credentials.

Verification: The repository already ignores `.env`, `.env.*`, `*.local`,
`private/`, build output, Godot exports, and runtime logs. The document contains
variable names and redaction rules only, with no private values.
