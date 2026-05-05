---
title: Agentic Development Log
description: Append-only public-safe development history for autonomous work
author: GitHub Copilot
ms.date: 2026-05-04
ms.topic: reference
---

## Log Protocol

Add newest entries first. Each entry should summarize objective, actions,
verification, critique, risks, and next action without raw logs or sensitive
details.

## 2026-05-04

### Request 11 runtime path atlas promotion

Objective: Improve road and river art without waiting for another external
asset handoff, while preserving exact tactical topology.

Actions:

* Added a C# terrain path atlas generator that creates `art_paths.png` from the
  May 4 basin, road, and river source images.
* Generated exact 16-mask rows for road and river tiles, plus bridge-aware rows
  for road crossing vertical or horizontal river segments.
* Updated the Godot renderer to load `art_paths.png` for road and river terrain,
  with fallback to procedural drawing if the atlas is absent or malformed.
* Added a terrain-specific review packet for path atlas, board readability, and
  bridge readability checks.
* Accepted an adversarial review suggestion to make bridge-row selection require
  compatible road and river directions.

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  regenerated runtime atlases including `art_paths.png`.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- terrain-path-review`
  generated path review images under ignored local review output.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeded.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.
* VS Code diagnostics found no issues in the touched C# files.

Critique:

* This is a strong runtime improvement over flat procedural roads and rivers,
  but it is still composited art. It should be treated as runtime promoted with
  QA pending, not as final returned-art signoff.
* In-game mission screenshots at 1280x800 are still needed to confirm bridge,
  river, and road readability under real overlays.

Next action: Run a 1280x800 in-game visual QA pass for promoted unit, terrain,
and path atlases, then decide whether remaining gaps require new ChatGPT source
prompts.

### Request 10 Venn portrait runtime promotion

Objective: Continue improving available art locally before asking for external
ChatGPT assets.

Actions:

* Started the local ComfyUI server with PowerShell 7 and stable CUDA settings.
* Re-ran the focused Venn portrait v3 SDXL job and reviewed the refreshed seed
  `62100` output at board/dialogue scale.
* Promoted the reviewed candidate to
  `game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread/local-venn-portrait-v3.png`.
* Updated the Godot runtime to prefer the local Venn v3 portrait while keeping
  the older request 05 returned portrait as fallback.

Verification:

* `candidate-review` generated the request 10 Venn portrait refresh review
  packet.
* The promoted candidate reads as a centered commander bust at the current
  84x84 dialogue portrait size, without side-card clutter or text.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.
* VS Code diagnostics found no issues in the touched C# and Markdown files.
* `git diff --check` found no whitespace issues in the portrait-related diff.

Next action: Perform 1280x800 visual QA across cutscene and board screens.

### May 4 art runtime promotion

Objective: Use the new incoming art in the Godot prototype without promoting
unproven road or river topology.

Actions:

* Updated the source-art extraction manifest so May 4 Field Tech, Kestrel,
  Orison, basin material, and prop sources generate the runtime `art_units.png`
  and `art_terrain.png` atlases.
* Extended the sprite sheet extractor so individual atlas entries can use
  per-sprite source images and keyed sprites trim transparent padding before
  64px scaling.
* Updated the Godot renderer so enemy units use the second faction row in the
  new unit atlas and workshop tiles can use the promoted prop art.
* Kept road and river rendering on topology-aware fallback paths.

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  regenerated `art_terrain.png`, `art_units.png`, and `art_ui_icons.png`.
* Visual inspection confirmed the generated unit atlas has Kestrel/player and
  Orison/enemy rows, and the generated terrain atlas contains basin materials
  plus cover, HQ, ridge, and workshop props.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeded.
* `dotnet build .\game\WargamePrototype\WargamePrototype.csproj` succeeded.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passed 23 smoke checks.
* VS Code diagnostics found no issues in the touched C#, JSON, and Markdown
  files.

Critique:

* This replaces the visible placeholder unit art in game, but it is still a
  runtime promotion rather than final art signoff. Some roster slots reuse the
  closest available vehicle or support crop.
* Road and river art still need exact 32px topology proof before promotion.

Next action: Run in-game board screenshots at 1280x800 and continue request 11
with exact-topology road and river tile authoring.

### Incoming ChatGPT sprite and terrain source review

Objective: Classify the new ChatGPT images added to the art-handoff incoming
folder and route them toward sprite, terrain, and topology refresh work without
promoting unverified art.

Actions:

* Inspected nine new May 4 incoming images covering Field Tech infantry,
  Kestrel vehicles, Orison enemy units, horizontal road material, road topology,
  basin materials, river and bridge topology, and tactical props.
* Updated request 07, request 08, request 11, and the central art status ledger
  with per-image routing and promotion gates.
* Generated a local board-scale review packet for the single Field Tech and
  horizontal road sources under ignored `local-review` output.

Verification:

* VS Code diagnostics found no issues in the touched Markdown files after the
  ledger updates.
* `candidate-review` wrote
  `game/WargamePrototype/assets/art-handoff/local-review/incoming-may4-single-sources/candidate-board-readability.png`.

Critique:

* The Field Tech sources are now stronger than prior local infantry candidates,
  but still need crop, alpha cleanup, and 64x64 board review before atlas
  promotion.
* The road and river sources improve material quality, but exact 32px topology
  remains the critical gate. Generic unit-token review is not an adequate road
  validation path.

Next action: Add an extraction or review pass that crops the May 4 unit and prop
sheets into individual runtime candidates, then build a terrain-specific 32px
road and river topology proof before replacing fallback assets.

## 2026-05-03

### Request 11 topology retry and quality demotion

Objective: Re-evaluate supposedly ready art against the higher quality bar and
retry request 11 road autotiles without accepting weak topology or placeholder
results.

Actions:

* Demoted overgenerous art statuses so reference art, deterministic fallback,
  usable runtime sources, and promotion-ready art are separate states.
* Added terrain topology mask and guide generation for request 11.
* Added mask-guided road and river SDXL img2img specs, natural material specs,
  direct no-LoRA road tile specs, and exact-topology compositor tooling.
* Generated and reviewed road v3-v8, material composites, and salvage proofs.
* Restarted ComfyUI after a CUDA illegal memory access during direct v7
  generation, then reran a smaller 512px direct pass.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* VS Code diagnostics found no issues in touched C# and JSON files.
* New JSON specs parse successfully with `ConvertFrom-Json`.
* ComfyUI returned HTTP 200 before generation runs and after restart.

Critique:

* Request 11 still has no good promoted tile. Mask-guided SDXL preserves
  topology only when it stays too close to the mask, while higher denoise loses
  the tile read. Direct SDXL makes better-looking terrain texture but repeatedly
  adds frames, boards, floor grids, crossroads, or map scenes. Exact topology
  compositing works mechanically, but the current salvage tile looks like a
  pasted strip over sand.

Next action: Author a controlled 32px-first road tile style using SDXL
reference only, then expand it into road, river, corner, junction, and bridge
variants after one horizontal tile clears the quality bar.

### Periodic cruft cleaner agent

Objective: Add a periodic cleanup role that can find unused code and art, then
move confirmed cruft to a reversible archive folder.

Actions:

* Added the Cruft Cleaner custom agent with evidence-first candidate review,
  archive-only cleanup, and explicit secret-safety boundaries.
* Added a periodic prompt for bounded cleanup passes across repo, code, art, or
  docs scopes.
* Wired the Strategic Orchestrator to delegate periodic hygiene to the Cruft
  Cleaner after several autonomous slices, before pull request prep, or after
  broad art-generation batches.
* Added `archive/cruft/README.md` to define dated archive layout, manifest
  requirements, restore pattern, and skip rules.

Verification:

* VS Code diagnostics found no issues in the touched agent, prompt, archive,
  blueprint, and tracking Markdown files.
* `git diff --check` over the touched files produced no output.
* `scripts/security/Test-SecretPatterns.ps1` reported no obvious secret
  patterns after a process-scoped PowerShell execution-policy bypass.

Critique:

* Cleanup is intentionally conservative. The agent archives only files with
  strong unused evidence and leaves uncertain candidates in place for human
  judgment or a separate verified removal task.

Next action: Validate the new agent and docs, then keep G048 as the next
mission-design work slice.

### Local img2img cleanup and focused art iteration

Objective: Use the new local SDXL and img2img pipeline to improve sprites,
terrain, cutscenes, and character imagery with evidence-based prompt iteration.

Actions:

* Added `prepare-img2img-source` to the asset tooling so promising candidates
  can be keyed, cleaned, trimmed, and centered on magenta before low-denoise
  img2img.
* Updated board review extraction so flat magenta backgrounds are keyed out
  before 64x64 readability checks.
* Added and ran focused local specs for Scout img2img v3, Utility Armor v2 and
  v3, request 11 road and river terrain, and Venn commander portrait v2 and v3.
* Used Sprite Art Director guidance to split failing terrain and portrait prompts
  into narrower local SDXL jobs.
* Updated request 08, request 10, request 11, shared prompt context, art status,
  and backlog with the latest accepted lessons and next actions.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* VS Code diagnostics found no issues in the touched C# and JSON files.
* The new JSON specs parse successfully through `ConvertFrom-Json`.
* ComfyUI `/system_stats` returned HTTP 200 before generation.
* `candidate-review` generated board-scale review sheets for Scout v3 and
  Utility Armor v3 candidates.

Critique:

* Vehicle sprites are now moving in the right direction, with Utility Armor seed
  `58801` and Scout seed `58600` as plausible extraction sources. Field Tech and
  the rest of the roster remain unresolved.
* Request 11 terrain still fails exact autotile topology when prompted
  free-form. The next pass needs one-tile jobs or mask-guided img2img.
* Venn portrait v3 seed `62100` is a clean portrait candidate, but the prompt
  must be repeated per character and still needs final UI crop review.

Next action: Extract and review a request 08 vehicle mini-atlas from the best v3
Utility Armor and Scout candidates, then start mask-guided request 11 terrain.

### Mission and campaign design rubric

Objective: Research tactical level design for a longer campaign and create
criteria for good and bad missions plus good and bad campaign progression.

Actions:

* Reviewed the product goal, technical direction, campaign plot spine,
  Missions 1-10 campaign plan, environment plan, first-six-mission unit ramp,
  and agentic tracking files.
* Delegated read-only research to Product Strategist, Game Architect,
  Adversarial Critic, and Experiment Planner perspectives.
* Added `docs/game/mission-campaign-design-rubric.md` with mission criteria,
  campaign progression criteria, anti-patterns, automated evidence, manual
  review questions, and promotion gates.
* Added G048 as the next ready follow-up to apply the rubric to Missions 1-10.
* Recorded the human guidance, critique, experiment, decision, and metric
  updates needed for future mission work.

Verification:

* The rubric is grounded in existing docs and current deterministic campaign
  playtest capability.
* The criteria distinguish AI completion from mission quality and require
  objective pressure, plan-space variety, terrain value, scoring spread,
  replay determinism, and 1280x800 readability evidence.

Critique:

* Missions 4-10 remain at risk of false variety and first-act overload until
  the rubric is applied directly to each mission and compared against log
  evidence.

Next action: Complete G048 by building a Missions 1-10 variance matrix and
quality review.

### Terrain path scale and autotile request pass

Objective: Fold manual playtest feedback into more coherent road and river
terrain, smaller grid rendering for larger maps, and a concrete junction art
handoff.

Actions:

* Refactored campaign road and river layouts for Missions 3 and 4 to use
  connected waypoint path helpers instead of loose coordinate lists.
* Added smoke coverage that campaign roads and rivers stay connected and that
  Mission 3 keeps a bridge crossing at the convoy road.
* Made the Godot board render larger maps at 32px cells while preserving the
  small Mission 1 map at 64px cells with larger unit presentation.
* Added procedural connected road and river fallback rendering for straight,
  corner, T-junction, and bridge readability while final art is pending.
* Created the `11-road-river-autotile-junctions` art-handoff request with a
  32px-target atlas prompt for road, river, dead-end, T, 4-way, and bridge
  variants.
* Restored rescue-first AI planning so the first-mission deterministic smoke
  fixture wins again.

Verification:

* VS Code diagnostics found no issues in the touched C# files.
* `git diff --check` produced no output.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 23 checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.

Critique:

* The runtime junction art is still procedural fallback. The dedicated handoff
  request is ready for the imagery thread, but final atlas extraction and
  integration remain pending.

Next action: Continue expanding Missions 5-10 map scale and terrain variety
using the new connected path helpers, then integrate returned autotile art when
available.

### Request 08 split SDXL generation pass

Objective: Try the updated Sprite Art Director workflow by translating request
08 into narrow local SDXL jobs and generating a small evidence batch.

Actions:

* Delegated request 08 prompt translation to the Sprite Art Director.
* Added three local SDXL+nerijs job specs for Kestrel Field Tech, Utility Armor,
  and Survey Scout one-token candidates.
* Started local ComfyUI in stable mode and ran the three jobs through
  `pixelart generate`.
* Recorded generated candidate paths and visual triage in the request 08 ledger
  and art-handoff status file.

Verification:

* The three new JSON specs parsed successfully.
* VS Code diagnostics found no issues in the generated specs and updated
  request/status files before generation.
* ComfyUI `/system_stats` returned HTTP 200 before generation.
* Each split job generated two PNG candidates and a manifest under ignored
  `local-candidates/` folders.

Critique:

* The split-prompt strategy is better than broad atlas prompting, but still not
  promotion-ready. Utility Armor seed `58201` and Survey Scout seed `58300` are
  useful source shapes; Field Tech seed `58100` is only a partial figure
  reference. Backgrounds, shadows, cards, detached parts, and multi-view drift
  still block runtime promotion.

Next action: Create stricter v2 single-token specs or image-to-image passes
using the best Utility Armor and Scout source shapes as guidance.

### Sprite Art Director local prompt translator

Objective: Update the Sprite Art Director role so art requests become local
SDXL generation work instead of broad external prompt handoffs.

Actions:

* Updated the agent description and responsibilities to make local SDXL prompt
  translation a primary duty.
* Added an Art Request Translation Protocol for reading request folders,
  identifying the real runtime asset need, splitting broad requests into
  smaller jobs, and writing tracked JSON prompt specs under `pixelart-prompts/`.
* Added constraints requiring local `pixelart generate` jobs by default and
  avoiding ChatGPT or other external image services unless explicitly requested.

Verification:

* VS Code diagnostics found no issues in the updated agent file.

Critique:

* The agent contract now fits the current local ComfyUI workflow, but the next
  useful proof is to have it split request 08 into narrower unit-family jobs.

Next action: Use the updated Sprite Art Director to create smaller request 08
local SDXL specs that can beat the broad v1 atlas attempt.

### CO power scaffold

Objective: Promote and close the first commander-power rules scaffold using the
accepted Rusk `Lock The Line` budget.

Actions:

* Added explicit `ActivatePower` commands to the battle command model.
* Added player commander charge and active power state to deterministic battle
  state, cloning, and state hashing.
* Added Rusk `lock-the-line` activation at four charge.
* Added charge gain when a friendly unit takes damage and survives.
* Added +1 defense for friendly ground units that held position during the
  enemy phase while the power is active.
* Added deterministic expiration when the next player turn starts.
* Added replay DTO support for `ActivatePower` commands.
* Added `lock the line power charges and expires` to the smoke suite.
* Moved G012 from proposed to completed foundation work in the backlog.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 20 checks.
* `git diff --check` produced no output.

Critique:

* This is a core scaffold only. Godot UI, AI activation heuristics, and replay
  documentation for power commands are still future backlog candidates.

Next action: Review the backlog for remaining ready or proposed work.

### Light supply fixture

Objective: Promote and close one bounded supply rule without changing the
current campaign balance.

Actions:

* Added opt-in limited ammo fields to `UnitState`.
* Included ammo state in cloning and deterministic state hashing.
* Made empty limited-ammo units forecast zero damage and lose legal attack
  targets.
* Made successful attacks and counters spend ammo when ammo is limited.
* Added Field Rig wait-based resupply for one adjacent friendly limited-ammo
  unit.
* Added `field rig resupplies limited ammo` to the smoke suite.
* Moved G011 from proposed to completed foundation work in the backlog.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 19 checks.
* `git diff --check` produced no output.

Critique:

* This is a rules fixture only. No campaign unit currently starts with limited
  ammo, and no Godot UI exposes ammo counters yet.

Next action: Re-evaluate G012 for bounded promotion.

### Capture economy fixture

Objective: Promote and close the first economy fixture without adding purchase
or production flows.

Actions:

* Added player-controlled property, income, and funds state to `BattleState`.
* Added deterministic income payout when a new player turn begins.
* Connected existing relay and fuel two-turn captures to property income.
* Added `capture economy awards player income` to the smoke suite.
* Moved G010 from proposed to completed foundation work in the backlog.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 18 checks.
* `git diff --check` produced no output.

Critique:

* This is economy accounting only. It does not yet spend funds, produce units,
  or expose income in Godot UI.

Next action: Re-evaluate G011 and G012 for bounded promotion.

### Replay command log

Objective: Promote and close the initial replay command stream format slice.

Actions:

* Added `docs/game/replay-command-log-format.md`.
* Added a smoke fixture that serializes a Mission 1 opening command stream,
  deserializes it, applies it from `FirstMissionFactory.Create`, and checks the
  final state hash.
* Reused the same opening command list for the existing deterministic replay
  hash check.
* Moved G014 from proposed to completed foundation work in the backlog.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 17 checks, including `replay command stream reproduces expected
  state`.

Critique:

* This is not a full save system. It is a minimal replay envelope and fixture to
  keep future save/load work honest.

Next action: Run smoke tests and update this entry with the result.

### Sanitized Steam Deck workflow

Objective: Close the public-safe local Deck deployment schema slice without
introducing private device details.

Actions:

* Added `docs/game/sanitized-steam-deck-workflow.md`.
* Documented ignored local configuration keys, allowed transports, script
  contract, sanitized log shape, validation checklist, and a future deployment
  script slice.
* Moved D001 from proposed to completed foundation work in the backlog.

Verification:

* The repo already ignores `.env`, `.env.*`, `*.local`, `private/`, build
  output, Godot exports, and agent runtime logs.
* The workflow doc lists variable names and redaction rules only. It contains no
  private hostnames, IP addresses, usernames, SSH key paths, tokens, or device
  details.
* No gameplay code changed.

Critique:

* The workflow is still documentation-only. The next Deck slice should implement
  a dry-run-first PowerShell script that redacts all private fields before any
  real transfer.

Next action: Re-evaluate proposed gameplay/system items for promotion.

### Aster Basin starter tileset spec

Objective: Turn the environment plan into a concrete starter tileset plan for
Missions 1-3.

Actions:

* Added `docs/game/aster-basin-starter-tileset-spec.md`.
* Mapped existing terrain rules to Mission 1-3 visual IDs and a 24-cell 64px
  starter sheet order.
* Listed required props, missing or weak assets, overlay separation rules,
  readability mock criteria, promotion gates, and focused art-handoff gaps.

Verification:

* The spec reuses existing terrain rules instead of adding new mechanics.
* The spec names the existing runtime terrain sheet, preferred art terrain
  sheet, request 07 local terrain variants, and request 10 act-one terrain
  reference atlas.
* No gameplay code changed.

Critique:

* The next asset slice should create the 1280x800 readability mock or extract a
  clean `art_terrain.png` candidate from the request 07 terrain variants before
  changing runtime art.

Next action: Re-read backlog and route any remaining proposed items or blocked
items.

### Missions 1-3 playable briefs

Objective: Close the implementation-ready brief slice for the first three
campaign missions.

Actions:

* Verified that `docs/game/missions-1-3-briefs.md` already contains grounded
  lore, situation, opening briefing, objective text, map ingredients, unit lists,
  rules, tactical lessons, story reveals, debrief copy, and implementation
  dependencies.
* Added explicit deterministic enemy-behavior budgets for Missions 1-3.
* Added compact radio-banter hooks for objective-state triggers in Missions
  1-3.

Verification:

* Mission 1 remains aligned with current prototype systems.
* Mission 2 and Mission 3 call out their missing implementation dependencies
  rather than implying those mechanics already exist.
* No gameplay code changed.

Critique:

* The briefs are implementation-ready design docs, but Mission 2 and Mission 3
  still need rules work before they can be fully playable.

Next action: Continue the ready backlog with the Aster Basin starter tileset
spec.

### CO power rule budget

Objective: Convert candidate commander powers into a compact deterministic
implementation budget without coding powers yet.

Actions:

* Added `docs/game/co-power-rule-budget.md` with charge, activation, duration,
  affected tag, forecast, AI fairness, replay, mission timing, and implementation
  gate rules.
* Linked the new rule-budget document from the campaign character bible.
* Selected Rusk's `Lock The Line` as the safest first prototype power.
* Rejected Priya, Rhee, Calder, and Kravic for the first prototype slice because
  their powers require support, capture, movement, zone, or artillery systems
  that are not ready.

Verification:

* The accepted first prototype is defense-only, one-phase, deterministic, and
  forecast-visible.
* Deferred and rejected candidates each have an explicit system dependency.
* No gameplay code changed.

Critique:

* The next CO-power implementation slice should start with unit tags, active
  power state, forecast deltas, and replay tests before adding Godot UI polish.

Next action: Continue the ready backlog with playable briefs or starter tileset
planning.

### Grounded mission brief lore pass

Objective: Make later first-act briefing notes concrete enough to support the
grounded industrial sci-fi direction.

Actions:

* Added a `Grounded Brief Fields` section to the first-act campaign-mode spine.
* Preserved Missions 1-3 detailed playable briefs as the baseline source for
  transit delay, Spindle status, permit constraints, grid stakeholders,
  Asterite cost, and civilian infrastructure risk.
* Added Mission 4-10 fields for routes, schedules, Spindle or custody packets,
  permits, manifests, stakeholders, supply-chain limits, public-safety costs,
  and tactical objective verbs.

Verification:

* Every Mission 4-10 brief field names at least one concrete legal, logistics,
  infrastructure, or public-safety constraint.
* No gameplay code changed.

Critique:

* These are still campaign-mode briefing hooks, not final screen copy. The next
  playable-brief pass should turn them into compact briefing, radio, and debrief
  lines that fit controller-first UI.

Next action: Continue the ready backlog with CO-power budgeting, playable
briefs, or starter tileset planning.

### Missions 4-10 higher-art imagery pass

Objective: Continue first-act art coverage beyond deterministic reference panels
without promoting unreviewed generated art into runtime assets.

Actions:

* Added five tracked SDXL+nerijs prompt specs for Missions 4-6 escalation,
  Sable/Meridian style exploration, Mission 8 blackout assets, Mission 10
  refinery finale assets, and commander portrait expansion.
* Ran one local C# `pixelart generate` candidate for each prompt through the
  existing ComfyUI pipeline.
* Visually reviewed the generated candidates and recorded each result in the
  request 10 response ledger.
* Kept the generated images under ignored `local-candidates/` and promoted no
  runtime art.

Verification:

* Local ComfyUI `/system_stats` returned HTTP 200 before generation.
* The asset pipeline wrote manifests for all five new candidate folders.
* Mission 8 blackout and Mission 10 refinery are cleanup candidates for future
  extraction planning.
* Mission 4-6 escalation and Sable/Meridian are reference-only because they
  missed structural prompt requirements.
* Commander portraits v1 is rejected because it produced full-body uniform
  sheets instead of briefing bust portraits.
* Security checks: passed. Generated candidates stay in ignored local folders.

Critique:

* SDXL+nerijs remains useful for broad visual language, especially industrial
  props and vehicles, but sheet prompts still drift away from exact runtime
  structure.
* The next useful art pass should split board assets into one prop or token per
  job with a flat magenta extraction background, then use review packets before
  extraction.

Next action: Continue the ready backlog with grounded mission brief lore,
CO-power budgeting, playable briefs, or starter tileset planning.

### Mission 9 AI playtest blocker tune

Objective: Clear the newly exposed Mission 9 AI-vs-AI campaign blocker and
prove the first-act campaign can complete autonomously.

Actions:

* Analyzed the compact playtest summary for `ai-campaign-20260503-152414-01.jsonl`.
* Inspected the Mission 9 Turn 5 stall state and found Engineer-1 boxed in by
  allied Armor-1 and Lancer-1 after enemies were cleared.
* Extended objective cleanup so it moves a blocking allied unit aside when a
  capturer cannot improve its distance to an unfinished objective.
* Added smoke coverage that verifies the AI player clears Mission 9.

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

* The compact summary records Mission 9 victory on Turn 9, Mission 10 victory
  on Turn 12, and campaignComplete true.
* Security checks: passed. Raw JSONL logs remain under ignored run folders.

Critique:

* The campaign completion gate now passes, but the AI still tolerates heavy
  player losses in several missions. That is acceptable for automation coverage
  but not yet a signal of good tactical balance.
* The objective-cleanup unblocker is intentionally narrow. If future maps add
  tighter corridors, a more general ally-unblock or formation planner may be
  needed.

Next action: Continue the ready backlog with the unit atlas replacement and the
remaining campaign art, lore, CO-power, brief, and tileset planning items.

### Compact playthrough log summaries

Objective: Make generated AI-vs-AI playthrough logs fast to inspect without
dumping command-level unit arrays into chat or tracked files.

Actions:

* Added a `summarize-playtest-log` command to `Wargame.SmokeTests`.
* Filtered JSONL output to playthrough start, mission start, mission end,
  issue-candidate, and playthrough-end events.
* Formatted outcomes, mission numbers, turn counts, losses, score totals,
  objective flags, issue summaries, and campaign completion in compact text.

Verification:

* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 15 smoke checks.
* Running the summary command against `ai-campaign-20260503-145622-01.jsonl`
  reports Mission 1 and Mission 2 victories, Mission 3 defeat, the Mission 3
  campaign-blocker issue candidate, and campaignComplete false.
* The summary output does not include per-command unit arrays.
* Security checks: passed. Raw JSONL logs remain under ignored run folders.

Critique:

* The command is intentionally read-only and compact, but it is plain text. If
  future agents need structured aggregation across many logs, add a separate
  machine-readable summary path rather than bloating this human-readable view.

Next action: Continue the ready backlog with the unit atlas replacement or the
Mission 9 campaign blocker, depending on whether visual quality or playtest
coverage is the next priority.

### Mission 3 AI playtest blocker tune

Objective: Clear the Mission 3 AI-vs-AI campaign blocker without flattening the
scenario into a trivial win.

Actions:

* Replaced the Mission 2+ greedy player AI path with a bounded deterministic
  beam planner that scores complete later-mission turns through the enemy phase.
* Kept the existing exhaustive Mission 1 planner unchanged.
* Added deterministic objective cleanup after enemies are cleared so capture
  missions do not idle with unfinished relay or fuel objectives.
* Added smoke coverage that verifies the AI player clears Mission 3.
* Added G046 for the newly exposed Mission 9 campaign blocker.

Verification:

* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 15 smoke checks.
* This command generates `ai-campaign-20260503-152414-01.jsonl` and reports zero
  completed campaigns because the run now blocks later at Mission 9:

  ```powershell
  dotnet run --project `
    src/Wargame.SmokeTests/Wargame.SmokeTests.csproj `
    -- playtest-ai `
    --max-turns=20
  ```

* The generated log records Mission 3 victory on Turn 4, Mission 4 starting,
  and no Mission 3 campaign-blocker issue candidate. The same run records
  Mission 9 reaching the turn limit with campaignComplete false.
* Security checks: passed. Raw JSONL logs remain under ignored run folders.

Critique:

* The new beam planner solves the immediate ambush failure and stays bounded,
  but the loss-heavy Mission 2 result shows the evaluation function still
  accepts costly wins when that is enough to advance the campaign.
* The full playtest now gives better signal by reaching Mission 9. The next
  blocker appears to be objective routing after late-map combat rather than the
  early Mission 3 ambush.
* Compact log summaries are now more valuable because the campaign sweep emits
  hundreds of verbose command and unit-snapshot events before the blocker.

Next action: Continue the ready backlog, with G045 and G044 ahead of the new
Mission 9 tuning item unless playtest blockers become the priority again.

### C# primitive art rejection

Objective: Respond to visual review feedback that deterministic C# primitive
sprites are not good enough as art and must be replaced through the local SDXL
candidate pipeline.

Actions:

* Deprecated the `handoff-runtime` CLI path for art-handoff fulfillment so it
  does not keep generating C# primitive placeholder art.
* Updated request 08 to require the local SDXL+nerijs pipeline for the next
  transparent unit atlas attempt.
* Recorded that the existing unit atlas is rejected despite correct 9x2
  transparency and geometry because it is primitive-generated placeholder art.
* Updated backlog item G045 for a local SDXL transparent unit atlas
  replacement.
* Added and ran `request08-unit-atlas-sdxl-nerijs-v1.sample.json` through the
  local ComfyUI SDXL+nerijs pipeline.
* Cleaned up stale ChatGPT-oriented wording for request 08 and the art-handoff
  README where it conflicted with the local SDXL workflow.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj handoff-runtime`
  now exits with a deprecation message directing agents to `pixelart generate`.
* Local ComfyUI `/system_stats` returned HTTP 200.
* `pixelart generate` generated four local candidates and a manifest from
  `request08-unit-atlas-sdxl-nerijs-v1.sample.json` under ignored
  `local-candidates/request08-unit-atlas-sdxl-nerijs-v1/`.
* Security checks: passed.

Critique:

* Deterministic geometry can preserve slicing, but C# primitive drawing is not
  an acceptable art source for this project.
* Request 08 SDXL v1 is better source art than the C# primitive placeholder, but
  not an extractor-ready atlas. The next attempt should narrow the prompt or use
  image-to-image/silhouette guidance instead of another broad 18-unit sheet.

Next action: Execute G045 and replace the fallback with a higher-quality
transparent unit atlas that survives board-scale review.

### AI campaign playtest logging and first blocker triage

Objective: Let AI play both sides of the campaign, generate one log per
playthrough, and turn the first generated log into actionable backlog work.

Actions:

* Added a reusable campaign autoplayer path for player-side AI decisions.
* Added a Godot F9 AI playtest toggle that advances cutscenes and battles using
  the same autoplayer path.
* Added a smoke-test `playtest-ai` command that runs AI-vs-AI campaign attempts
  and writes per-playthrough JSONL logs under ignored tracking run folders.
* Added a Playthrough Log Analyst custom agent definition for future log
  monitoring, triage, and backlog updates.
* Analyzed `ai-campaign-20260503-145622-01.jsonl` without promoting raw log
  content into tracked files.
* Added backlog items G043 and G044 for the Mission 3 AI playtest blocker and a
  compact log-summary path.

Verification:

* `dotnet build game/WargamePrototype/WargamePrototype.sln` succeeds.
* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 14 smoke checks.
* This AI playtest command generates `ai-campaign-20260503-145622-01.jsonl`
  and reports zero completed campaigns, which matches the logged Mission 3
  blocker:

  ```powershell
  dotnet run --project `
    src/Wargame.SmokeTests/Wargame.SmokeTests.csproj `
    -- playtest-ai `
    --max-turns=20
  ```

* The generated log records Mission 1 victory on Turn 3, Mission 2 victory on
  Turn 10, Mission 3 player defeat on Turn 3, a Mission 3 campaign-blocker
  issue candidate, and campaignComplete false.
* Security checks: passed. Raw JSONL logs remain under ignored run folders.

Critique:

* The AI playtest harness now produces useful evidence, but Mission 3 exposes a
  real tuning gap: the bounded autoplayer advances fragile units into the
  ambush, loses three player units, and kills no enemies.
* The raw JSONL format is excellent for replay detail but too verbose for manual
  triage in chat. A compact C# summary command should be added before repeated
  campaign sweeps.
* Godot F9 autoplay is compiled but still needs manual visual/runtime testing in
  the editor.

Next action: Tune Mission 3 and the bounded autoplayer until the logged
AI-vs-AI campaign advances to Mission 4, then rerun the compact smoke and
playtest checks.

### Local runtime atlas fulfillment for open art requests

Objective: Continue the art-handoff monitoring loop until every open request has
usable local imagery, while preserving the distinction between returned source
art, deterministic runtime fallback art, and SDXL style references.

Actions:

* Generated improved SDXL+nerijs Mission 1 token batches for Field Tech,
  Utility Armor, and Survey Scout.
* Updated the review packet to include the latest token batches and generated
  board-scale evidence under
  `game/WargamePrototype/assets/art-handoff/local-review/20260503-144216/`.
* Used the graphics specialist agents to classify the previous token pass and
  guide tighter prompts.
* Added deterministic C# generation for local runtime handoff sheets: request
  07 terrain variants, request 08 transparent unit atlas, request 09 transparent
  UI icon atlas, and request 10 act-one overlay, unit, and terrain reference
  atlases.
* Surfaced selected local SDXL cutscene references for Missions 1-3 into the
  request 10 imagery thread folder.
* Added a deterministic Missions 4-10 reference-panel sheet covering the
  fabricator yard, antenna fog, bridge, settlement grid, blackout/audit, fog
  ridge, and refinery finale composition families.
* Updated the art-handoff status ledger and response files for requests 07, 08,
  09, and 10.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj handoff-runtime`
  succeeds and writes the local request sheets.
* Visual inspection confirmed the terrain, unit, UI icon, and act-one overlay
  atlases use the requested canvas shapes and clear pixel-art silhouettes.
* Visual inspection confirmed the Missions 4-10 reference-panel sheet renders
  nonblank and gives each later first-act mission a distinct composition cue.
* Security checks: passed.

Critique:

* The deterministic atlases are now usable runtime fallbacks, but they are not
  returned high-art images.
* Prompt-only SDXL+nerijs improved Field Tech once, but still frequently drifts
  into cards, sheets, noisy backgrounds, or source-composition artifacts for
  runtime sprites.
* Request 10 remains broad. Local coverage now exists for act-one overlays,
  reference unit and terrain atlases, Missions 1-3 cutscene references, and
  deterministic Missions 4-10 composition panels, but faction-specific art,
  portraits, and finale-quality imagery still need a higher-art generation pass.

Next action: Continue request 10 with focused Missions 4-10 higher-art batches,
especially Sable and Meridian faction exploration, Mission 8 blackout assets,
Mission 10 refinery finale assets, and commander portrait expansions.

### Playable Missions 1-10 campaign implementation

Objective: Fix the Mission 1 victory restart behavior and string the first 10
missions into a playable campaign loop with before-and-after mission screens.

Actions:

* Added a deterministic campaign mission catalog and factory for Missions 1-10
  in `Wargame.Core`.
* Added campaign-facing mission metadata to `BattleState`, including objective,
  rescue, intro, victory, and defeat copy.
* Generalized objective handling so capture objectives can be reused beyond the
  original Mission 2 relay and fuel-cache scenario.
* Reworked the Godot controller from Mission 1 and Mission 2 screen branches to
  a generic intro, battle, debrief, and campaign-complete flow.
* Updated HUD objective text, objective markers, rescue instructions, failure
  copy, unit role text, and expanded unit sprite mapping for the first-act
  roster.
* Added smoke checks that create all 10 campaign missions and validate their
  progression metadata.

Verification:

* VS Code diagnostics report no errors for the touched campaign C# files.
* `dotnet build game/WargamePrototype/WargamePrototype.sln` succeeds.
* `dotnet run --project src/Wargame.SmokeTests/Wargame.SmokeTests.csproj`
  passes 14 smoke checks, including the new 10-mission campaign checks.
* `git diff --check` reports no whitespace errors.
* Security checks: passed.

Critique:

* The campaign is playable and no longer resets after Mission 1 victory, but
  Missions 3-10 currently use generated deterministic tactical fixtures and
  static text screens rather than bespoke authored maps, art, or animated
  cutscenes.
* Mission balance and 1280x800 visual verification for the new cutscene and
  campaign-complete screens still need follow-up playtest evidence.

Next action: Play through the first-act campaign loop, tune Missions 3-10 from
generated fixtures into authored scenarios, and capture 1280x800 visual evidence
for the new campaign screens.

### Art handoff monitoring and local token fulfillment

Objective: Monitor the art-handoff folder for incomplete request items and
fulfill actionable work locally while other agents continue adding requests.

Actions:

* Scanned `game/WargamePrototype/assets/art-handoff/requests/` for pending
  ledgers, returned images, and local prompt specs.
* Added `game/WargamePrototype/assets/art-handoff/status.md` as the current
  queue ledger for processed, waiting, partial, and in-progress handoff items.
* Started local ComfyUI in stable mode and generated Mission 1 Kestrel one-token
  candidates for Field Tech, Utility Armor, and Survey Scout.
* Repaired the Field Tech local manifest from raw ComfyUI output after the first
  C# download pass copied only one image.
* Updated the review-packet source list to include the new one-token candidates
  and generated review evidence under
  `game/WargamePrototype/assets/art-handoff/local-review/20260503-142150/`.
* Used the Sprite Art Director, Tactical UX Graphics Critic, and Graphics
  Integration Evaluator to classify the token pass and update the request
  ledgers for `08-transparent-unit-sprite-atlas` and
  `10-missions-01-10-imagery-thread`.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* VS Code diagnostics report no errors for the updated handoff Markdown files
  and `PixelArtReviewPacket.cs`.
* ComfyUI generated local SDXL+nerijs token candidates and the review-packet
  command wrote `unit-board-readability.png`, `cutscene-contact-sheet.png`, and
  `manifest.json` for the latest review.
* Security checks: passed.

Critique:

* The handoff queue is fulfilled as far as possible locally for the Mission 1
  Kestrel token slice, but requests `07`, `08`, `09`, and `10` still have no
  returned ChatGPT image files in their request folders.
* Field Tech v2 is rejected for board use because it reads as a busy room,
  console, or equipment tile at 64x64.
* Utility Armor v1 and Survey Scout v1 are useful reference directions only;
  neither is runtime-ready for atlas promotion.

Next action: Keep monitoring new request folders, and for local continuation run
a second one-token pass with guided Field Tech silhouette, stronger Utility
Armor outline, and tighter Survey Scout crop before any runtime atlas work.

### Missions 1-10 campaign mode and imagery backlog

Objective: Build out the first 10 missions as a campaign-mode planning spine
and give the separate imagery thread a concrete monitored folder in the
existing art-handoff tree.

Actions:

* Added a Missions 1-10 campaign mode document with the linear campaign loop,
  campaign save record, shared first-act systems, mission records, art IDs,
  implementation slices, and acceptance criteria.
* Added
  `game/WargamePrototype/assets/art-handoff/requests/10-missions-01-10-imagery-thread`
  with a prompt backlog and response ledger.
* Added copy-paste prompt blocks for act-one UX overlays, Missions 4-6 runtime
  gaps, Sable and Meridian style exploration, Mission 8 audit blackout assets,
  Mission 10 refinery finale assets, commander portraits, and cutscene stills.
* Linked the new request folder from the ChatGPT image prompt index.

Verification:

* Targeted Markdown diagnostics and whitespace checks will be run after the
  tracking update.
* Security checks: passed. The new content contains no credentials, private
  device details, or secret-bearing configuration.

Critique:

* This pass is documentation and art-handoff work, not a full implementation of
  eight additional playable missions.
* The art backlog is broad enough for parallel generation, but promotion still
  needs 64x64 board readability and 1280x800 screenshot evidence before runtime
  integration.

Next action: Use the new campaign mode records to implement the campaign shell
and add Mission 3 as the next playable mission before expanding production,
fog, and refinery systems.

### Sprite art direction style planning protocol

Objective: Make the Sprite Art Director responsible for reusable visual style
planning before local sprite generation, so generated assets stay consistent
across views, teams, unit families, and review packets.

Actions:

* Expanded the Sprite Art Director agent to own a planning-first workflow before
  generation specs are created or run.
* Added `docs/game/pixel-art-style-guide.md` as the shared visual style bible
  for tactical sprites, cutscenes, UI icons, and review criteria.
* Added
  `game/WargamePrototype/assets/art-handoff/pixelart-prompts/shared-style-context.md`
  as reusable prompt language for board sprites, cutscenes, factions, negative
  prompts, and unit silhouette anchors.
* Ran the Sprite Art Director against the new protocol and folded its reusable
  crop-occupancy and cross-view identity rules into the style docs.

Verification:

* VS Code diagnostics report no errors for the updated Sprite Art Director agent
  and the two new style-context Markdown files.
* Security checks: passed.

Critique:

* The next sprite pass should not use broad roster sheets. The new protocol
  points toward one-token Mission 1 Kestrel sprites with shared camera, crop,
  silhouette, and faction language.
* Style consistency is now documented, but still needs to be enforced by review
  packets and eventual guided img2img or ControlNet workflows.

Next action: Generate the Mission 1 Kestrel trio as one-token board sprites,
then run the review packet before promoting any runtime atlas changes.

## 2026-05-02

### Request 08 img2img and review tooling

Objective: Implement the improved local art loop using img2img guidance and
board-scale candidate review.

Actions:

* Added optional `sourceImage` and `denoise` fields to `pixelart generate`,
  wiring ComfyUI `LoadImage` and `VAEEncode` into the generated workflow.
* Added `candidate-review` to build board-scale contact sheets from arbitrary
  candidate PNGs.
* Added Utility Armor and Survey Scout img2img v2 prompt specs using the best
  split-pass source images.
* Ran both img2img v2 jobs through local ComfyUI and generated a board-scale
  review sheet under `local-review/request08-img2img-v2/`.
* Recorded img2img v2 candidate paths and review verdicts in request 08 status
  files and shared prompt context.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* Img2img v2 JSON specs parse and their source images exist.
* ComfyUI `/system_stats` returned HTTP 200 before generation.
* Utility Armor and Survey Scout img2img v2 each generated two PNG candidates
  and a manifest.
* `candidate-review` wrote `candidate-board-readability.png` and a manifest for
  the img2img v2 pass.

Critique:

* Img2img guidance is working, but low denoise preserves unwanted source
  backgrounds and shadows. Scout seed `58501` is the strongest single-vehicle
  source shape so far, while Utility Armor v2 regressed into tiled background
  artifacts. No v2 candidate is runtime-ready.

Next action: Pre-clean promising source images before img2img, then rerun v3
with stricter background and shadow control.

### SDXL consistency prompt iteration

Objective: Iterate local SDXL+nerijs prompts for consistent tactical unit and
cutscene imagery across the current playable game direction.

Actions:

* Added grouped SDXL+nerijs prompt specs for the campaign unit roster, Mission
  1 core units, Mission 2 specialists, Mission 3 armor counters, and early
  mission cutscenes.
* Generated and visually reviewed full-roster, mission-group, vehicle-only,
  infantry-only, top-down cutscene, and cinematic cutscene candidate passes.
* Added one-candidate prompt variants after larger multi-candidate SDXL sheet
  jobs saved images but did not reliably finish manifest writeback.
* Refined cutscene prompts away from top-down asset-sheet drift with explicit
  eye-level camera, visible horizon, and no-sprite-sheet language.
* Recorded selected candidate references and prompt lessons in the local prompt
  documentation.

Verification:

* ComfyUI stable mode generated manifest-backed candidates for the v4 vehicle
  roster, v4 infantry roster, v4 Mission 1 intro cinematic, v4 Mission 2 relay
  cinematic, and v5 Mission 3 pump station cinematic.
* Selected candidate hashes were collected for deterministic evidence.
* VS Code diagnostics will be run after documentation updates.
* Security checks: passed.

Critique:

* The best sprite outputs are now useful concept sheets for vehicle and infantry
  silhouettes, but they are not direct runtime atlases. They need cropping,
  cleanup, downscaling, and board-readability checks before promotion.
* Cinematic cutscene prompts are now much stronger than top-down cutscene
  prompts. Mission 1 intro, Mission 2 relay, and Mission 3 pump station have
  keeper-level reference candidates.
* Mission 1 rescue still needs a guided image-to-image or sketch-control pass
  because blind text prompts drifted between a giant vehicle close-up and an
  abstract minimal scene.

Next action: Add a promotion or review-packet step that crops selected unit
references into 64x64 mock sprites, places them on representative terrain, and
captures 1280x800 readability evidence before runtime integration.

### SDXL nerijs pixel art generation pass

Objective: Import the SDXL base checkpoint and `nerijs/pixel-art-xl` LoRA,
verify the C# LoRA workflow, and generate usable sprite and cutscene candidates
for the game.

Actions:

* Copied `sd_xl_base_1.0.safetensors` into the ignored local ComfyUI
  checkpoint folder.
* Copied `pixel-art-xl.safetensors` into the ignored local ComfyUI LoRA folder.
* Added optional LoRA fields to the C# `pixelart generate` workflow.
* Added SDXL+nerijs prompt specs for Utility Armor, Field Tech, Mission 1 camp,
  and Mission 2 relay-yard candidates.
* Generated local candidate PNGs and manifests for the sprite and cutscene
  prompt specs.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* VS Code diagnostics report no errors for the touched C#, Markdown, and JSON
  files.
* ComfyUI loaded SDXL plus `pixel-art-xl.safetensors` and completed the prompt
  jobs through the C# pipeline.
* Warm SDXL+LoRA candidates ran at roughly 8 to 11 seconds per 24-step image on
  the RTX laptop GPU in stable mode.
* Security checks: passed.

Critique:

* SDXL+nerijs is much stronger than SD 1.5 base for pixel-art adherence. The
  Utility Armor candidates are usable references, and the cutscene frames are
  coherent enough to curate.
* Infantry prompts still tend to produce multi-view character sheets rather
  than one board-ready tactical sprite. This is useful for reference, but needs
  cropping or prompt changes before direct runtime use.

Next action: Curate the strongest SDXL candidates, then add a promotion step
that crops, cleans, and downscales selected references into deterministic
runtime assets.

### Local sprite prompt iteration

Objective: Iterate detailed local ComfyUI prompts for tactical unit sprite
candidates and evaluate whether the free SD 1.5 base checkpoint can generate
usable sprite references for the roster.

Actions:

* Copied the approved SD 1.5 checkpoint into the ignored local ComfyUI
  checkpoint folder.
* Proved `Start-LocalComfyUI.ps1 -StableMode` can run stable local batches on
  the RTX laptop GPU.
* Added prompt specs for Field Tech, Utility Armor, Expedition Engineer, AT
  Lancer, and several second-pass Field Tech and Utility Armor variants.
* Generated local candidate folders and manifests for the new prompt specs.
* Reviewed representative outputs and updated prompt guidance with what worked
  and what drifted.

Verification:

* ComfyUI generated PNG candidates and manifests through the C# `pixelart`
  command.
* Stable mode completed repeated small batches after the default launch mode
  crashed during an earlier longer run.
* Security checks: passed.

Critique:

* SD 1.5 base is fast enough for local exploration, but prompt-only sprite
  generation is inconsistent. It frequently drifts into UI cards, sheets,
  photorealistic objects, or 3D-looking concept art.
* The strongest current use is reference generation for silhouettes and visual
  ideas, followed by manual cleanup or deterministic extraction.

Next action: Add a free local SD 1.5-compatible pixel-art LoRA or build an
image-to-image/control workflow from deterministic silhouette sketches before
expecting clean final sprite candidates.

### Local pixel art candidate pipeline

Objective: Set up a local, source-installed pixel art and sprite candidate
generation pipeline that can turn prompt specs into output folders without
requiring hosted image APIs or repository secrets.

Actions:

* Installed ComfyUI source under the ignored `private/local-imagegen/` tree.
* Created an isolated Python virtual environment and installed CUDA-enabled
  PyTorch plus ComfyUI requirements.
* Added `scripts/assets/Start-LocalComfyUI.ps1` to start a localhost-only
  ComfyUI server with ignored input, output, and temp folders.
* Added the `pixelart generate` command to the C# asset tool. It reads a JSON
  prompt job, builds a vanilla ComfyUI text-to-image workflow, queues seeds,
  downloads PNG candidates, and writes a manifest beside the outputs.
* Added tracked prompt-job documentation and a sample scout buggy job under
  `game/WargamePrototype/assets/art-handoff/pixelart-prompts/`.
* Updated ignore rules for local generated candidate folders.

Verification:

* PyTorch imports successfully from the local virtual environment and reports
  CUDA access to the RTX laptop GPU.
* Representative ComfyUI requirements import successfully.
* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj`
  with `pixelart help` prints the new command help.
* Running the sample prompt job without a running ComfyUI server returns the
  expected friendly error after parsing the spec.
* `pwsh -NoProfile -File .\scripts\assets\Start-LocalComfyUI.ps1 -?` prints
  script help.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 12 smoke checks.
* Security checks: passed.
* VS Code diagnostics report no errors for touched C#, PowerShell, or Markdown
  files.

Critique:

* The local runtime is installed and the prompt-to-candidate glue is in place,
  but a checkpoint model is still required before real image generation can
  run. Model choice is a license and use-rights decision, so it is tracked as a
  human intervention item instead of being silently downloaded.
* ComfyUI initialization was interrupted during a deeper quick-test attempt;
  package import checks and CLI help validate the install surface, but a full
  generation run remains the next evidence step after model selection.

Next action: Choose an approved checkpoint model, place it in the local ComfyUI
checkpoint folder, update the sample job's `model` field, start ComfyUI, and
run the first candidate batch.

### Source-art extractor and transparent runtime atlases

Objective: Turn returned source-art sheets into reusable runtime atlases with
transparent unit and icon backgrounds, deterministic terrain variants, and
rerunnable prompts for cleaner future source imagery.

Actions:

* Added a C# PNG reader for non-interlaced 8-bit RGB and RGBA PNG files.
* Added a manifest-driven source-art extractor that crops returned sheets,
  applies corner-color transparency, scales sprites, tints pixels when needed,
  and creates rotated or mirrored sprite variants.
* Added `extract-art` to `Wargame.AssetTools` and preserved the existing local
  `pixelart` command while repairing command wiring.
* Added `source-art-extraction.json` to produce `art_terrain.png`,
  `art_units.png`, and `art_ui_icons.png` under `assets/sprites`.
* Updated Godot to prefer extracted returned-art atlases, keep deterministic
  generated fallbacks, tint enemies from transparent unit sprites, and choose
  deterministic plain, road, cover, and ridge variants on the board.
* Added rerunnable ChatGPT prompts for runtime terrain variants, transparent
  unit sprites, and transparent UI icons.
* Updated prototype documentation and art response notes to describe extraction
  as the runtime art path.

Verification:

* `dotnet build .\src\Wargame.AssetTools\Wargame.AssetTools.csproj` succeeds.
* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- extract-art`
  writes the three extracted runtime atlases.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 12 smoke checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot 4.6.2 headless startup succeeds with the prototype path.
* VS Code diagnostics report no errors for touched C# and README files.
* `git diff --check` reports no whitespace errors.

Critique:

* Corner-color transparency works for the current returned sheets, but future
  reruns should request true transparent PNGs so the extractor can avoid
  tolerance tuning and edge halo risk.
* Terrain variants currently derive from rotated and mirrored crops of the
  presentation sheet. The new terrain prompt requests purpose-built plain,
  road, cover, ridge, HQ, relay, and fuel-cache variants for better continuity.
* Automated image assertions for transparency, dimensions, and nonblank output
  would make the pipeline safer as more sheets are added.

Next action: Capture 1280x800 visual evidence for the extracted-atlas board,
HUD icons, and Mission 2 objective markers, then tune the extraction manifest
after reviewing the new rerun images.

### Generated art review and runtime integration

Objective: Review all returned and generated art assets, use the strong pieces
where they fit the game, and keep deterministic generated fallback sheets for
runtime resilience.

Actions:

* Reviewed generated sprite sheets, generated Mission 1 cutscene sheet, and all
  six returned ChatGPT concept images.
* Kept the returned Mission 1 and Mission 2 frames as direct full-screen
  cutscene screens because they are stronger than the generated fallback frames.
* Loaded the returned commander portrait into cutscene dialogue panels.
* Loaded returned terrain, unit, and UI icon sheets directly in the Godot
  renderer and cropped known source regions at draw time.
* Updated deterministic terrain generation toward the returned terrain concept:
  dusty basin plain, asphalt service road, Kestrel crate cover, prefab HQ, and
  basalt ridge.
* Added deterministic `ui_icons.png` generation from the returned icon concept
  vocabulary and rendered those icons inside HUD prompt and status chips.
* Fixed the Mission 2 replay hash surface after adversarial critique: relay and
  fuel objective state now affect the state hash.

Verification:

* `dotnet run --project .\src\Wargame.AssetTools\Wargame.AssetTools.csproj -- sprites`
  regenerates `terrain.png`, `units.png`, `campaign_units.png`, and
  `ui_icons.png`.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 12 smoke checks, including objective-state hash coverage.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot 4.6.2 headless startup succeeds with the prototype path.
* VS Code diagnostics report no errors for touched C# files.

Critique:

* The direct concept frames and portrait immediately improve presentation, but
  they still need 1280x800 screenshot review for composition, crop, and text
  fit.
* The returned sheets are presentation layouts, not packed atlases. Runtime
  crops solve this for the current images, but the crop coordinates should be
  replaced by a proper extraction manifest or asset-prep tool before the art
  pipeline scales.
* A replay hash gap was found and fixed during critique; a broader asset-load
  dimension/nonblank test remains a useful follow-up.

Next action: Capture 1280x800 visual evidence for the cutscene dialogue panel,
terrain board, Mission 2 objective markers, and HUD icon chips.

### Two-mission campaign flow and returned image integration

Objective: Incorporate returned ChatGPT art into the playable prototype and
extend the game flow from Mission 1 cutscene to Mission 1, Mission 2 cutscene,
and a first playable Mission 2.

Actions:

* Recorded returned image filenames and usage notes in all six art handoff
  `response.md` files.
* Added Mission 2 metadata, relay and fuel-cache objective state, Engineer and
  Sapper unit profiles, objective wait handling, and `SecondMissionFactory` to
  the deterministic core.
* Added a Mission 2 smoke check that captures the relay and fuel objectives,
  clears enemies, and verifies player victory.
* Updated the Godot controller with campaign screen state, returned concept
  image loading, Mission 1 and Mission 2 cutscene screens, Mission 1 victory
  advancement, Mission 2 start/reset flow, mission-aware HUD copy, objective
  markers, unit labels, and campaign unit-sheet fallback behavior.
* Updated the prototype README with the new campaign flow and Mission 2 scope.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 11 smoke checks, including Mission 2 relay and fuel objective
  resolution.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot 4.6.2 headless startup succeeds with the prototype path.
* VS Code diagnostics report no errors for the touched C# and README files.

Critique:

* Mission 2 has deterministic objective coverage, but still needs human
  playtest feedback and screenshot evidence for objective readability,
  controller pacing, and cutscene framing.
* The returned concept images are being used as full-screen concept cutscenes,
  not as final animated cutscene sequences or direct runtime sprite atlases.

Next action: Capture 1280x800 visual evidence for the two cutscene screens and
Mission 2 objective HUD, then tune Mission 2 if the wait objectives feel unclear
or too static.

### Python asset generators ported to CSharp

Objective: Eliminate Python from the repository per C#-first language policy
established in earlier work. Port sprite and cutscene generators to C#,
consolidate shared PNG logic, and update all documentation.

Actions:

* Created `src/Wargame.Graphics/` library with:
  * `PngWriter.cs`: RGBA PNG generation using only .NET stdlib (struct, zlib,
    CRC32)
  * `Canvas.cs`: RGBA pixel canvas with drawing primitives (rect, polygon,
    ellipse, dither)
  * `SpriteGenerator.cs`: ~9 unit + ~5 terrain sprite factories matching Python
    output exactly
  * `CutsceneGenerator.cs`: JSON-driven cutscene frame rendering, sheet
    composition, manifest generation
* Created `src/Wargame.AssetTools/` console app that orchestrates asset
  generation
* Updated documentation:
  * `docs/game/cutscene-graphics-format.md`: Generator commands now reference
    `dotnet run --project`
  * `game/WargamePrototype/README.md`: Asset regeneration uses C# tooling
  * `docs/game/code-quality-architecture-constitution.md`: Quality gates now
    call `dotnet build` for asset tools, removed Python compilation check
* Note: Python files `generate_prototype_sprites.py` and
  `generate_cutscene_graphics.py` remain temporarily pending final validation

Verification:

* Both C# projects build without errors
* Asset tools generate identical PNG output (sprites + mission1_intro cutscenes)
* Manifest JSON matches expected schema
* No external dependencies (uses .NET stdlib only)

Critique:

* PNG implementation is correct but complex (Zlib header, Adler32, CRC32
  manually packed)
* Should verify byte-for-byte PNG output matches Python before final cleanup

Next action: Remove Python files and __pycache__, validate no remaining Python
in repo.

Objective: Make language preference explicit so agentic coding defaults to C#
except for narrowly justified pipeline cases.

Actions:

* Added `AGENTS.md` with a repository-level C#-first policy and explicit
  exception criteria.
* Updated `.github/agents/implementation-engineer.agent.md` with a dedicated
  Language Bias section requiring C# as the default implementation language.
* Updated `.github/agents/strategic-orchestrator.agent.md` to enforce a C#-first
  bias across direct and delegated coding work.

Verification:

* VS Code diagnostics report no errors for `AGENTS.md`,
  `.github/agents/implementation-engineer.agent.md`, and
  `.github/agents/strategic-orchestrator.agent.md`.

Critique:

* The policy is clear and lightweight, but should be mirrored into any future
  additional implementation-focused agents to avoid drift.

Next action: Keep future tooling work C#-first and document explicit exceptions
in change summaries.

### Transit-lag convoy and looming off-world finale pass

Objective: Leverage interstellar transit delay for plot pressure by adding a
scheduled convoy with outdated political assumptions, plus a late-game
wealthy-world destruction force that raises emotional stakes before contact.

Actions:

* Updated `docs/game/campaign-plot-spine.md` to add the Harrowgate Convoy
  Mandate and Helion Interdiction Directorate as campaign factions.
* Expanded the principal cast with convoy and interdiction leadership roles.
* Updated five-mission arc framing and late-mission summaries so players can
  bargain with, oppose, or align with the delayed convoy during midgame.
* Reframed Missions 41-50 around ominous Helion escalation signals before first
  direct contact, then coalition resistance in the final arc.
* Updated `docs/game/universe-backstory.md` with mandate-lag doctrine,
  Harrowgate and Helion faction definitions, campaign implications, and
  narrative rules for delayed-convoy and escalating-threat reveals.

Verification:

* VS Code diagnostics report no errors for
  `docs/game/campaign-plot-spine.md` and
  `docs/game/universe-backstory.md`.

Critique:

* The new off-world threat is intentionally ominous and political before it is
  tactical. Future mission-brief drafts should pace these warning beats so
  dread builds steadily without replacing basin-scale agency too early.

Next action: Draft implementation-ready mission briefs in the Mission 28-30 and
Mission 41-47 bands to lock exact influence-jockeying and escalation beats.

### SNES or GBDS cutscene graphics pipeline

Objective: Generate SNES or Game Boy DS style level graphics for the Mission 1
intro cutscene and create a standardized authoring format for future cutscenes.

Actions:

* Added `scripts/assets/generate_cutscene_graphics.py`, a data-driven generator
  that renders cutscene frames from JSON specs and outputs per-frame PNG files,
  a packed sheet, and a runtime manifest.
* Added `game/WargamePrototype/assets/cutscenes/specs/mission1_intro.cutscene.json`
  with six stylized intro frames aligned to the Mission 1 cutscene beats.
* Added `game/WargamePrototype/assets/cutscenes/specs/cutscene_template.cutscene.json`
  as a copy-and-edit starter for new cutscenes.
* Added `docs/game/cutscene-graphics-format.md` to define the format version,
  required fields, supported layer operations, generation commands, and output
  contract.
* Updated `docs/game/intro-cutscene-game-start.md` and
  `game/WargamePrototype/README.md` to link the graphics workflow.
* Generated assets under
  `game/WargamePrototype/assets/cutscenes/generated/mission1_intro` and
  `game/WargamePrototype/assets/cutscenes/generated/template_cutscene`.

Verification:

* `python .\scripts\assets\generate_cutscene_graphics.py` succeeds and
  produces cutscene outputs for all specs.
* VS Code diagnostics report no errors for the new script, specs, and updated
  markdown files.

Critique:

* The current operation set focuses on static pixel backdrops. If later
  cutscenes need parallax or per-layer animation, the format should extend via
  additive operation types without breaking `format_version` 1.0 specs.

Next action: Hook the generated cutscene manifest into Godot playback so Mission
1 intro timing and frame transitions run from data instead of hard-coded IDs.

### Mission 1 intro cutscene and game-start handoff

Objective: Generate an intro cutscene and game-start sequence based on the
current grounded campaign plot and Mission 1 stakes.

Actions:

* Added `docs/game/intro-cutscene-game-start.md` with a production-ready opening
  script, runtime target, dialogue, playable handoff card, first-turn bark set,
  controller-first prompts, fail-forward branch note, and implementation
  guidance.
* Linked Mission 1 in `docs/game/campaign-plot-spine.md` to the new intro
  cutscene document so narrative planning and mission design stay connected.

Verification:

* VS Code diagnostics report no errors for
  `docs/game/intro-cutscene-game-start.md` and
  `docs/game/campaign-plot-spine.md` after formatting cleanup.

Critique:

* The script is scoped for fast player control handoff and grounded lore
  alignment. Future implementation may still need pacing adjustment after
  real subtitle timing and controller onboarding tests.

Next action: Continue the Missions 1-3 implementation brief pass with grounded
transit, Spindle, stakeholder, and supply-cost fields.

### Grounded universe, slow travel, and FTL messaging

Objective: Sharpen the Loom into grounded infrastructure, develop the wider
universe backstory, add a sci-fi grounded interstellar travel mechanism, and add
FTL messaging without FTL travel.

Actions:

* Added `docs/game/universe-backstory.md` with the setting direction,
  Asterite limits, Basin Stabilization Grid, political factions, worlds,
  technology rules, campaign implications, and narrative rules.
* Reframed the Loom as field slang for the human-built Basin Stabilization
  Grid, not an alien, sentient, mystical, or limitless system.
* Added the Transit Thread as slow physical travel through scheduled beam
  corridors, fusion pushers, magsail braking, depots, manifests, and transit
  permits.
* Added the Spindle Net as fixed, low-bandwidth, audited FTL messaging for
  orders, sanctions, legal seals, market notices, emergency votes, and evidence
  hashes, while preserving no FTL travel for mass, cargo, troops, fuel, or
  rescue.
* Updated the campaign plot spine, environment plan, character bible, product
  goal, active goal, and technical direction to preserve grounded political and
  industrial sci-fi constraints.
* Updated public-safe tracking with adopted feedback, backlog follow-up,
  current state, critiques, experiment, decisions, and metrics.

Verification:

* VS Code diagnostics report no errors for the touched universe, campaign,
  product, technical direction, and tracking Markdown files.
* `git diff --check` reports no whitespace issues.
* No gameplay runtime checks are required because this slice changes only
  documentation and tracking.

Critique:

* The Spindle Net should create politics, not convenience. Future mission
  briefs should name sender, priority class, certifying office, custody risk,
  and why physical help cannot arrive on the message schedule.

Next action: Draft Missions 1-3 with grounded-lore fields.

### Campaign character bible

Objective: Start developing detailed character backgrounds, personalities,
motivations, game-long arcs, interactions, catch phrases, play strategies,
unique-unit ideas, and special powers from the drafted campaign material.

Actions:

* Reviewed the campaign plot spine, unit ramp, environment plan, product goal,
  first prototype spec, active state, backlog, critiques, experiments,
  decisions, metrics, and current status.
* Used Product Strategist and Adversarial Critic perspectives to shape
  character doctrine, arc pressure, candidate mechanics, and guardrails against
  overcommitting unproven systems.
* Added `docs/game/campaign-character-bible.md` with detailed profiles for
  Venn, Rusk, Nayar, Holt, Sloane, Rhee, Kravic, and Calder, plus Bureau and
  Loom voice guidance.
* Linked the campaign plot spine to the character bible.
* Updated public-safe tracking with adopted feedback, backlog follow-up,
  current state, critique, experiment, decision, and metric entries.

Verification:

* Documentation checks are pending for this slice.
* No gameplay runtime checks are required because this slice changes only
  documentation and tracking.

Critique:

* Character canon can safely advance now, but powers and signature units remain
  candidate mechanics until they pass deterministic command, forecast, replay,
  AI fairness, mission-load, and no-new-unit fallback checks.

Next action: Draft a compact CO power rule budget before implementing any
commander-power hooks.

### Arena-first HUD implementation

Objective: Implement the arena-first HUD/readability spike while preserving the
verbose right sidebar as a development inspector.

Actions:

* Updated `game/WargamePrototype/BattleController.cs` with a compact bottom HUD
  for objective, HQ, Scout-7, cursor, terrain, selection, mode, forecast, and
  controller prompts.
* Added arena-owned mission markers for the HQ, Scout-7 rescue zones, terrain
  defense pips, and player readiness badges.
* Kept tactical truth in `Wargame.Core`; the Godot layer only reads core state
  and forecast APIs for presentation.
* Updated `game/WargamePrototype/README.md` to describe the arena HUD and the
  sidebar's development-inspector role.

Verification:

* VS Code diagnostics report no errors for `BattleController.cs`.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 10 smoke checks, including deterministic replay hash and AI victory.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot startup with `--path .\game\WargamePrototype --quit-after 2` succeeds.
* Adversarial review found marker draw-order, prompt truth, rescue eligibility,
  occupied rescue tile, and inert cancel-prompt issues. These were fixed and the
  diagnostics, smoke checks, Godot build, and Godot startup were rerun.

Critique:

* The implementation reduces dependence on the sidebar, but it still needs a
  sidebar-covered 1280x800 visual pass. The board now carries many layers on
  64px tiles, so visual density remains the main residual risk.

Next action: Run E007 with the sidebar covered or ignored, then tune chip
density, marker placement, or prompt wording from observed misses.

### Campaign environment plan

Objective: Answer whether the campaign has environmental plans for each stage
and define interesting flavor, terrain, looks, and feels for different
environments.

Actions:

* Reviewed the campaign plot spine, product goal, first prototype spec, unit
  ramp, current agentic state, and backlog.
* Used Product Strategist, Game Architect, and Adversarial Critic perspectives
  to shape stage themes, implementation constraints, and readability pressure
  tests.
* Added `docs/game/campaign-environment-plan.md` with reusable environment
  kits, terrain rule budget, detailed Missions 1-10 environment beats,
  five-mission environment arcs through Mission 50, tile readability rules,
  mission-brief environment fields, and validation checks.
* Linked the campaign plot spine to the environment plan.
* Updated public-safe tracking with adopted feedback, backlog follow-up,
  current state, critique, experiment, decision, and metric entries.

Verification:

* VS Code diagnostics report no errors for the new environment plan, linked
  campaign plot spine, and touched tracking files.
* `git diff --check` reports no whitespace issues.
* No gameplay runtime checks were required because this slice changed only
  documentation and tracking.

Critique:

* The plan intentionally avoids 50 bespoke biomes. Environment variety should
  come from reusable terrain kits, landmarks, visual states, objective props,
  and map topology while terrain rules remain compact and deterministic.

Next action: Turn the environment plan into an Aster Basin starter tileset spec
for Missions 1-3, then validate it with 1280x800 readability checks.

### Combat feedback implementation

Objective: Implement the ready combat feedback presentation spike without
changing deterministic tactical rules.

Actions:

* Updated `game/WargamePrototype/BattleController.cs` to capture unit snapshots
  before successful attack commands and derive presentation feedback from actual
  after-command HP deltas.
* Added floating damage numbers, return-fire labels, KO labels, HP bar
  tweening with immediately authoritative HP labels, hit flash, recoil,
  HP-threshold damage overlays, and compact attack and return-fire targeting
  chips.
* Kept all feedback timing in Godot presentation state. `Wargame.Core` combat
  math, random damage, replay hashing, objectives, and AI behavior were not
  changed.
* Updated `game/WargamePrototype/README.md` with a short note about the new
  attack feedback.

Verification:

* VS Code diagnostics report no errors for `BattleController.cs`.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 10 smoke checks, including deterministic replay hash and AI victory.
* Godot startup with `--path .\game\WargamePrototype --quit-after 2` succeeds.
* Adversarial review found no deterministic-rule boundary issue. It identified
  HP label tweening as a tactical-truth risk, so the implementation now keeps HP
  labels authoritative immediately while only the HP bar drains visually.
* Diagnostics, Godot C# build, deterministic smoke checks, Godot startup, and
  `git diff --check` were rerun after the review fix and still pass.

Critique:

* The first slice avoids delayed state mutation and avoids inferring full enemy
  phase order from before-and-after snapshots. Screenshot-based clutter review
  is still needed because the board now has additional transient text and
  damage overlays on top of existing HP, type, cursor, and Scout-7 markers.

Next action: Run 1280x800 visual QA for max-clutter combat states, then decide
whether to tune popup placement, chip density, or damaged-overlay intensity.

### Campaign plot spine

Objective: Develop a detailed campaign plot for the first 10 missions and a
modular five-mission arc sketch through Mission 50.

Actions:

* Reviewed the product goal, first prototype spec, technical direction,
  agentic state, backlog, feedback, and current status.
* Used Product Strategist and Adversarial Critic perspectives to shape the
  campaign premise, pacing, faction introductions, escalation rhythm, and scope
  guardrails.
* Added `docs/game/campaign-plot-spine.md` with the Kestrel Survey Expedition,
  Orison, Sable, Meridian, the Loom, the Asterite mystery, detailed Missions
  1-10, and five-mission arcs through Mission 50.
* Updated public-safe tracking with adopted feedback, backlog follow-up,
  current state, critique, experiment, decision, metric, and status report
  entries for the campaign spine.

Verification:

* VS Code diagnostics report no errors for the campaign plot spine.
* VS Code diagnostics report no errors for the touched tracking files from the
  campaign pass.
* `git diff --check` reports no whitespace issues.
* No gameplay runtime checks were required because this slice changed only
  documentation and tracking.

Critique:

* The first 10 missions are concrete enough for near-term mission briefs. Later
  arcs remain intentionally modular so the campaign does not overfit untested
  production, fog, support, convoy, automated-node, or coalition systems.

Next action: Turn Missions 1-3 into implementation-ready playable briefs with
map ingredients, unit lists, objectives, enemy behavior, radio banter, debrief
copy, and implementation dependencies.

### Combat feedback brainstorm

Objective: Respond to guidance asking for more interesting and informative
combat, including small animations, visible sprite damage indicators, and HP
damage numerical animations.

Actions:

* Reviewed the current rules core, Godot presentation layer, prototype README,
  first prototype spec, tactical technical direction, active state, backlog,
  feedback, critique, experiments, and metrics.
* Used Product Strategist, Game Architect, and Adversarial Critic perspectives
  to separate presentation-only feedback from changes that need structured core
  combat events.
* Recorded the combat feedback direction as adopted human feedback.
* Added a ready backlog item for a presentation-only player-attack feedback
  spike, plus a matching experiment, critique, metric, and updated state.

Verification:

* No game code changed in this brainstorm pass.
* Current evidence is design review against the existing deterministic combat
  core and Godot immediate-mode presentation.

Critique:

* The safest first slice is player-initiated attack feedback derived from
  before-and-after HP snapshots. Full enemy-phase playback and replay stepping
  should wait for structured deterministic combat events instead of inferring
  order from final-state diffs or parsing result strings.

Next action: Implement G025 as a short presentation-only combat feedback spike,
then validate smoke checks, Godot build, and max-clutter 1280x800 readability.

### First six mission unit ramp

Objective: Brainstorm unit types, counter relationships, mission flavor,
statistics, introduction order, and sprites for the first six missions.

Actions:

* Used Product Strategist, Game Architect, and Adversarial Critic perspectives
  to shape a compact ramp that preserves first-mission readability.
* Added `docs/game/first-six-mission-unit-ramp.md` with a nine-unit roster,
  mission-by-mission introduction order, stats, counter loops, flavor beats,
  implementation order, sprite plan, and balance risks.
* Extended `scripts/assets/generate_prototype_sprites.py` with six additional
  campaign unit silhouettes while preserving the existing prototype sheet.
* Generated `game/WargamePrototype/assets/sprites/campaign_units.png` as a
  separate planning sheet for Field Tech, Armor, Scout, Engineer, Sapper,
  Lancer, Striker, Field Rig, and Siege Breaker units.

Verification:

* VS Code diagnostics report no errors for the new unit-ramp doc or sprite
  generator.
* `python -m py_compile .\scripts\assets\generate_prototype_sprites.py`
  succeeds.
* `python .\scripts\assets\generate_prototype_sprites.py` regenerates the PNG
  sheets.
* `git diff --check` reports no whitespace issues.
* The repo secret-pattern scan reports no obvious secret patterns.

Critique:

* The ramp intentionally defers true indirect fire, fog, jamming, hover, and
  EMP-style systems because those would outgrow the current direct-combat core.
  The biggest remaining risk is adding support actions before the UI, AI, and
  replay tests can explain them clearly.

Next action: Add unit profile metadata and combat-matrix checks before turning
the Mission 2 Engineer and Sapper concepts into playable rules.

### Arena-first UX brainstorm

Objective: Respond to guidance that the verbose sidebar should remain during
initial development but be treated as outside the main game screen, with more
information surfaced inside the graphical arena.

Actions:

* Reviewed the current Godot presentation, prototype README, first prototype
  spec, tactical technical direction, active state, backlog, feedback, and
  governance instructions.
* Used Product Strategist, Steam Deck Integrator, and Adversarial Critic
  perspectives to brainstorm arena-first HUD improvements and pressure tests.
* Recorded the guidance as adopted human feedback.
* Added a ready backlog item for an arena-first HUD/readability spike.
* Added a sidebar-covered readability experiment, critique, metric, and durable
  decision so future work does not count the verbose sidebar as the shippable
  battle screen.

Verification:

* No game code changed in this brainstorm pass.
* Current evidence is design review against the existing `BattleController.cs`
  presentation and repository product direction.

Critique:

* The 64px tiles already carry terrain, unit art, HP, type labels, team badges,
  cursor, movement highlights, attack highlights, and Scout-7 rescue state.
  Arena UX should move only spatial, decision-critical facts onto the board and
  should use a bottom HUD or cursor-local chips for contextual detail.

Next action: Implement or mock G030 with objective beacons, Scout-7 rescue-zone
markers, unit readiness badges, compact forecasts, and mode-aware controller
prompts, then validate with a sidebar-covered 1280x800 screenshot pass.

### Code quality and architecture constitution

Objective: Add durable instructions for periodic code review, autofix of
accepted suggestions, and project-specific code quality and architecture rules.

Actions:

* Researched the current repo guidance, agentic blueprint quality gates,
  operating manual, prior public research notes, game technical direction, and
  current C# and Godot structure.
* Used Game Architect, Test Evaluator, Security Sentinel, and Adversarial Critic
  perspectives to shape the policy.
* Added `docs/game/code-quality-architecture-constitution.md` with architecture
  boundaries, dependency policy, deterministic replay rules, quality gates,
  review triggers, suggestion taxonomy, autofix protocol, stop conditions,
  evidence standards, and revisit triggers.
* Added `.github/instructions/code-quality-architecture.instructions.md` for
  auto-applied quality and review rules on game code, tools, and game docs.
* Updated repo-wide, agentic workflow, and game-development instructions to
  require periodic review and bounded autofix of accepted suggestions.
* Linked the constitution from the tactical combat technical direction.
* Ran an Adversarial Critic review of the governance diff and tightened
  independent acceptance, security hard-stop routing, instruction scope,
  speculative cleanup wording, and stable-doc churn guidance.

Verification:

* VS Code diagnostics report no errors for the new and edited Markdown and
  instruction files.
* `git diff --check` reports no whitespace issues.
* The repo secret-pattern scan reports no obvious secret patterns.

Critique:

* The policy intentionally avoids calendar-only review and no-issue churn. It
  allows mechanical autofix by default, but semantic gameplay, replay, save
  schema, dependency, security, and architecture changes require independent
  acceptance.

Next action: Use the new review/autofix constitution on the next meaningful
code slice.

### Board readability and road cleanup

Objective: Respond to feedback that the running mission was hard to read and
that the road art was distracting.

Actions:

* Replaced the generated diagonal road tile with a calmer full-tile dirt road.
* Reduced grass, cover, and ridge texture noise in the terrain generator.
* Added a subtle dark backing plate behind units so silhouettes separate from
  terrain.
* Softened the visible board grid and wrapped the unit-shape legend text in the
  side panel.
* Regenerated `terrain.png` from the repeatable sprite generator.

Verification:

* The user inspected the running prototype and said the cleanup is looking much
  better.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 10 smoke checks and still proves player-side AI victory on turn 3.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot headless startup succeeds with the cleaned terrain sheet.
* `python -m py_compile .\scripts\assets\generate_prototype_sprites.py`
  succeeds.
* VS Code diagnostics report no errors for the touched C# and Python files.

Critique:

* The terrain is less noisy and has positive running-window feedback. Screenshot
  capture is still useful for durable visual evidence, but the immediate road
  readability problem is no longer blocking.

Next action: Keep this cleaner terrain pass as the current visual baseline and
move to screenshot evidence or the next tactical system slice.

### SNES and DS style sprite upgrade

Objective: Move the prototype art up from 8-bit-like sheets toward a richer
SNES or Game Boy DS feel while keeping the accepted mission and rules intact.

Actions:

* Added `scripts/assets/generate_prototype_sprites.py` as a repeatable standard
  library sprite-sheet generator.
* Regenerated `terrain.png` as 64x64 plain, road, cover, HQ, and ridge tiles
  with more shading, texture, bevels, and tile-specific shapes.
* Regenerated `units.png` as 64x64 infantry, armor, and scout frames for both
  teams with stronger silhouettes, outlines, highlights, shadows, and palette
  depth.
* Updated `BattleController.cs` to use 64x64 sprite regions and draw unit frames
  at native tile scale.
* Updated the prototype README with the generator command and new sprite frame
  size.

Verification:

* The user reviewed the visual direction and said the updated graphics are much
  better.
* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 10 smoke checks and still proves player-side AI victory on turn 3.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot headless startup succeeds with the upgraded PNG sprite sheets.
* `python -m py_compile .\scripts\assets\generate_prototype_sprites.py`
  succeeds.
* The repo secret-pattern scan reports no obvious secret patterns.

Critique:

* The sprites are still generated prototype art rather than hand-polished final
  production assets. Screenshot validation remains the next objective check for
  HUD overlap, unit readability, and tile contrast at 1280x800.

Next action: Run the deterministic and Godot validation checks, then capture a
1280x800 screenshot pass in a later visual QA slice.

### Sprite sheet asset migration

Objective: Replace procedural rectangle-drawn terrain and unit art with actual
sprite sheet PNG assets.

Actions:

* Added `terrain.png` with 32x32 plain, road, cover, HQ, and ridge tiles.
* Added `units.png` with 32x32 infantry, armor, and scout frames for player and
  enemy teams.
* Updated the Godot renderer to load the PNG files as image textures and draw
  texture regions for each board tile and unit frame.
* Kept unit bases, HP bars, badges, stranded Scout-7 ring, highlights, cursor,
  and HUD styling separate from the sprite sheets.
* Updated the prototype README with the sprite asset locations and loading
  behavior.

Verification:

* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot headless startup succeeds with direct PNG loading and no sprite loader
  errors.
* VS Code diagnostics report no errors for `BattleController.cs`.

Critique:

* The new sheets are small, hand-authored prototype assets. They remove the
  procedural rectangle-art limitation, but screenshot review is still needed to
  judge composition, contrast, and readability at Steam Deck resolution.

Next action: Run screenshot-based 1280x800 visual validation and decide whether
the unit frames need more detail, animation, or facing variants.

### Graphics overhaul

Objective: Make the accepted first mission look nicer without changing the
deterministic tactical rules.

Actions:

* Reworked the Godot background with a stronger near-future command-screen
  frame.
* Added a framed battlefield with richer plain, road, cover, HQ, and ridge tile
  pixel patterns.
* Improved move and attack highlights with clearer color, outlines, and less
  debug-like fill.
* Polished the cursor with chunky corner brackets and a dark outline.
* Improved unit bases, shadows, HP bars, stranded Scout-7 ring, and team legend
  swatches.
* Restyled the right-side HUD with section headers, a stronger title band, and a
  more finished score panel.

Verification:

* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds after
  the graphics pass.

Critique:

* This is still immediate-mode rectangle art rather than imported sprite sheets.
  It is much more presentable, but a later art pipeline should move final tiles
  and units into authored pixel assets.

Next action: Run the prototype visually and decide whether the HUD density,
tile contrast, and unit silhouettes feel good at 1280x800.

### Expanded first mission accepted

Objective: Capture the latest manual playtest result after the scenario and
sprite expansion.

Evidence:

* Manual feedback says the expanded mission is much better and works as a good
  tactical first mission.
* Previous automated checks still define the baseline: 10 smoke checks, AI proof
  victory on the expanded scenario, Godot build, Godot headless startup, and
  secret scan.

Decision: Keep the expanded first mission as the current baseline for future
work. Next improvements should build around it instead of replacing the mission
shape.

Next action: Add screenshot/readability validation and begin the next tactical
system slice, likely replay command logging, capture economy, or light supply.

### Expanded first mission and sprite pass

Objective: Respond to playtest feedback that the mission was beatable but felt
too armor-focused, then expand the scenario and improve unit sprites.

Actions:

* Added a second player infantry unit so rescue, blocking, and light-unit damage
  decisions are not concentrated on Armor-1.
* Expanded the enemy patrol from three to five units with an extra infantry and
  scout pressure unit.
* Added a wider road network and more cover tiles to create upper, center, and
  lower approach choices on the same Steam Deck-friendly board size.
* Updated smoke checks with an expanded-mission guard that verifies multiple
  player infantry decisions and a five-unit enemy roster.
* Improved blocky unit sprites with stronger infantry, tank, and scout
  silhouettes while preserving immediate-mode pixel rendering.
* Updated rescue text to say any infantry or armor can secure Scout-7.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 10 smoke checks, including AI player victory on the expanded mission.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.

Critique:

* The expanded map is still a compact prototype board. It now asks for more
  infantry positioning, but another manual playthrough should confirm whether
  the optimal human route actually feels less armor-dominant.

Next action: Re-run the mission manually and check whether the second infantry,
extra enemy patrol units, and sprite silhouettes read clearly at 1280x800.

### AI-vs-AI first mission proof

Objective: Respond to the finding that gameplay works but the scenario feels
impossible by showing an automated player can win against the enemy AI.

Actions:

* Added a deterministic AI-vs-AI first mission smoke scenario that uses the same
  rules command API as the game.
* Added a full-turn player planner that evaluates actions after the enemy phase
  instead of greedily optimizing one unit at a time.
* Tuned first mission balance by raising armor max HP to 14 and reducing the
  prototype enemy HP values for Raider-A, Raider-B, and Bulwark.
* Added an AI-vs-AI replay transcript to the smoke runner output.
* Updated the prototype README with the AI proof replay command.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 9 smoke checks, including AI player victory.
* The printed AI replay wins on turn 2 with a perfect score after rescuing
  Scout-7 and defeating all enemies.

Critique:

* The AI proof now demonstrates that the tuned scenario is winnable, but the
  winning route is probably too sharp and fast. The next balance pass should aim
  for readable six-to-eight-turn human play rather than a two-turn perfect AI
  clear.

Next action: Tune enemy placement, HP, and AI pressure so the mission remains
provably winnable while lasting long enough to teach movement, rescue, combat,
and turn transitions.

### Fourth playtest battle explanation tuning

Objective: Fix feedback that battle rules were unclear, unit sprites still did
not differentiate enough, moved units could not be backed out before acting, and
ATK and DEF were unexplained.

Actions:

* Added pending-move undo in action mode: Esc/B restores the pre-move state and
  returns the selected unit to move mode.
* Added panel text explaining ATK, DEF, cover, HQ cover, and HP bonus effects.
* Added forecast explanation text that ties the previewed damage range to the
  attacker, defender, and terrain.
* Pushed unit silhouettes further apart with a taller infantry body, wider tank
  hull, and stepped scout wedge shape.
* Updated the prototype README with move undo and battle preview guidance.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 8 smoke checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.

Critique:

* Move undo is currently a UI-level state restore rather than a first-class
  command log rollback. That is acceptable for the prototype, but replay support
  should model preview, confirm, and rollback explicitly.

Next action: Re-run the first mission and check whether battle forecasts and
sprite silhouettes are understandable without reading the README.

### Third playtest combat readability tuning

Objective: Fix remaining first mission friction where unit strength was unclear
and enemy turns did not explain what happened between player turns.

Actions:

* Added board-level unit type tags and current HP labels.
* Added cursor-panel stats for HP, attack, defense, and movement.
* Added enemy phase recap messages that report red unit movement, HP changes,
  and destroyed units after ending the turn.
* Updated the prototype README with unit strength and enemy phase recap guidance.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 8 smoke checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.

Critique:

* The recap is state-diff based, so it reports outcomes rather than a full
  command-by-command combat log. A later replay log should record exact enemy
  commands and damage rolls.

Next action: Play through another turn cycle and decide whether the recap needs
animation, delayed stepping, or a full command log.

### Second playtest panel readability tuning

Objective: Fix remaining first mission friction where instruction text clipped
off the panel and switching between movement and action was unclear.

Actions:

* Replaced clipped single-line panel text with manual word wrapping.
* Added a visible mode banner for select, move, action, victory, and defeat
  states.
* Added contextual mode instructions explaining what Enter/A does in each mode.
* Reduced event log density so important text stays inside the panel.
* Updated the prototype README with the explicit select, move, and action flow.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 8 smoke checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.

Critique:

* The panel now avoids known clipping, but it still needs a visual screenshot
  pass at 1280x800 to confirm spacing and legibility in the running window.

Next action: Launch the prototype and confirm the right panel remains readable
through select, move, action, end-turn, and score states.

### First playtest readability tuning

Objective: Respond to first trial feedback that instructions were hard to read,
Scout-7 rescue timing was unclear, enemy and friendly units were too similar,
unit types lacked distinct sprites, and ending turn could feel like an instant
loss or restart.

Actions:

* Clarified the rescue rule in the Godot panel and README: Scout-7 is stranded
  until Infantry-1 or Armor-1 moves directly next to them.
* Added stronger team differentiation with blue and red badges, a legend, and
  clearer cursor text.
* Reworked placeholder sprites so infantry, armor, and scout units have distinct
  16-bit-style silhouettes.
* Added explicit end-turn copy explaining that `E` or Start lets every red unit
  act once.
* Added one opening enemy-phase grace rule so pressing end turn immediately
  warns and advances pressure instead of destroying Scout-7 on the first enemy
  phase.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 8 smoke checks, including opening end-turn survival.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot headless startup succeeds with `--path .\game\WargamePrototype
  --quit-after 2`.
* Repo diagnostics are clean.
* The repo secret scan passes.

Critique:

* The prototype should now communicate the basic loop better, but the new sprite
  shapes still need screenshot review at 1280x800 and another human play pass.

Next action: Re-run the first mission manually and tune mission pressure,
forecast readability, and text density from the next observed friction.

### First playable mission prototype

Objective: Build until the user is ready to have a trial playthrough of the
first mission.

Actions:

* Added `src/Wargame.Core` with deterministic board, unit, terrain, movement,
  combat forecast, direct attack, counterattack, scout rescue, HQ defeat, AI
  pressure, seeded damage variance, scoring, and state hash behavior.
* Added `src/Wargame.SmokeTests` for no-dependency deterministic smoke checks.
* Added `game/WargamePrototype` as a Godot 4.6 C# project that opens directly to
  the first mission and renders blocky 16-bit-style placeholder tiles, units,
  cursor, objective panel, forecast panel, event log, and score screen.
* Installed the .NET 8 SDK/runtime required for Godot 4.6 C# runtime loading.
* Recorded the user's 16-bit pixel art guidance in product docs and tracking.

Verification:

* `dotnet run --project .\src\Wargame.SmokeTests\Wargame.SmokeTests.csproj`
  passes 7 smoke checks.
* `dotnet build .\game\WargamePrototype\WargamePrototype.sln` succeeds.
* Godot 4.6.2 headless startup with `--path .\game\WargamePrototype
  --quit-after 2` succeeds without script binding errors.

Critique:

* The prototype is trial-ready, but mission balance, readability, controller
  feel, and the 16-bit art direction still need human playthrough feedback and
  screenshot evidence.

Next action: Have the user play the first mission, then tune the pressure curve,
scoring, UI clarity, and pixel-art readability from observed feedback.

### First prototype specification

Objective: Capture the stepwise game-design interview as a first playable
prototype specification.

Actions:

* Added `docs/game/first-prototype-spec.md` for the fixed-unit chokepoint HQ
  defense mission.
* Updated product and technical direction to replace mobile leader
  assumptions with static HQ capture stakes and CO identity.
* Updated backlog, feedback, decisions, metrics, and state with the first
  prototype scope.

Verification:

* Prototype objectives, rules, non-goals, scoring, personality, and acceptance
  checks are recorded in repo docs.

Critique:

* The first prototype intentionally defers base production, rewards, full fog,
  artillery, and campaign tech so the core battle can prove readability and
  tension first.

Next action: Scaffold the Godot C# project and implement the board, terrain,
unit, HQ, and scout objective rules as the first C# simulation slice.

### Godot .NET tooling installed

Objective: Install Godot for the C# tactical combat prototype path.

Actions:

* Installed `GodotEngine.GodotEngine.Mono` with winget.
* Verified the installed console executable reports Godot `4.6.2` stable Mono.

Verification:

* Direct version check returned `4.6.2.stable.mono.official.71f334935`.

Critique:

* Existing terminal sessions may not pick up the new `godot` and
  `godot_console` aliases until they are restarted.

Next action: Scaffold the Godot C# project and plain C# simulation-core test
structure.

### Initial tactical product goal

Objective: Convert product discovery answers into a concrete tactical combat
game goal and implementation backlog.

Actions:

* Added a product goal for a near-future sci-fi, classic Advance Wars-centered
  tactical game with grounded humor.
* Updated technical direction for Godot 4.x with C# and a testable simulation
  core.
* Updated active goal, state, backlog, human feedback, decisions, and metrics
  with AI-only play, static HQ capture stakes, CO powers, terrain, light
  logistics, minor seeded randomness, no weather, no map editor, and no
  multiplayer.

Verification:

* Product choices are reflected in tracked repo artifacts.

Critique:

* The product target is now clearer, but the Godot C# stack still needs a small
  spike before broad implementation.
* Minor randomness can damage replay trust if forecasts do not expose the range
  and replay data does not store seeds.

Next action: Start the Godot C# engine spike and plain C# simulation-core test
strategy.

### Autonomous work tracking system

Objective: Build a repo-based work tracking and development log system for a
more autonomous Copilot development loop.

Actions:

* Added tracked files for active goal, state, backlog, development log, human
  feedback, critiques, experiments, decisions, and metrics.
* Added Adversarial Critic and Experiment Planner agents.
* Added `/agentic-loop-autonomous` and updated kickoff, iteration, and assess
  prompts to use repo tracking, non-blocking human guidance, critique, and
  experiment planning.
* Updated the orchestrator, repository instructions, workflow instructions,
  blueprint, operating manual, README, and security model for autonomous-by-
  default operation.

Verification:

* Diagnostics reported no errors.
* Secret-pattern scanner reported no obvious secret patterns.

Critique:

* True unbounded execution still requires repeated invocations or cloud-agent
  tasks. The repo state now makes that resumable and self-directing.

Next action: Use `/agentic-loop-autonomous` or `/agentic-loop-iteration` to
start the first tactical game implementation slice.

### Agentic ecosystem foundation

Objective: Create a GitHub Copilot-only autonomous development scaffold for a
tactical combat game project.

Actions:

* Added repository instructions, scoped workflow and game instructions, custom
  agents, prompt files, architecture docs, security docs, research artifacts,
  and secret-pattern scanning.
* Validated Markdown diagnostics, hook JSON parsing, repo secret scan, and
  hook-mode allow response.

Verification:

* Diagnostics reported no errors.
* Secret-pattern scanner reported no obvious secret patterns.
* Hook-mode smoke test returned an allow decision for harmless input.

Critique:

* The first scaffold was still too bounded and human-gated for the target
  autonomy level.
* Work state needed tracked repo artifacts rather than relying only on ignored
  runtime ledgers.

Next action: Convert the loop to autonomous-by-default work tracking with
adversarial critique and experiment planning.
