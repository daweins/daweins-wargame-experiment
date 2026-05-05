---
title: Campaign Environment Plan
description: Environmental flavor, terrain identity, and visual direction for the tactical campaign
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Current Answer

The campaign already has a plot spine, mission objectives, a first prototype
map, and a first-six-mission unit ramp. It does not yet have a dedicated
environment plan. This document fills that gap by defining reusable environment
kits, mission-by-mission flavor for the first 10 missions, and five-mission
environment arcs through Mission 50.

The goal is not to create 50 separate biomes. The goal is to make each stage
read as a distinct tactical place through terrain composition, palette,
landmarks, objective props, faction occupation, and environmental storytelling,
while keeping the rules small enough for deterministic tests and Steam Deck
readability.

## Environment Principles

* Environments should explain the mission's tactical question within a few
  seconds: chokepoint, convoy, fog field, bridge, settlement, refinery, uplink,
  or control-district route.
* Reuse terrain rules aggressively. Make places feel different through visual
  skins, landmarks, lighting state, faction dressing, and objective layout.
* Keep 16-bit pixel clarity ahead of texture richness. Roads, cover, ridges,
  HQs, properties, units, objectives, cursor, and combat overlays must remain
  readable at 1280x800.
* Use Asterite and grid-control lighting as accents on conduits, substations,
  locks, objectives, and power routes. Do not flood the whole map with glow.
* Avoid weather as a broad system. Fog, blackout, dust, and sensor anomalies are
  mission presentation or explicit scenario state, not random map modifiers.
* Every special object that affects movement, combat, visibility, capture,
  production, supply, scoring, or AI priority must appear in inspect UI,
  forecasts, tests, and replay data before it ships.

## Terrain Rule Budget

The first campaign should keep a compact tactical terrain grammar. Visual skins
can multiply later, but rule behaviors should grow slowly.

| Rules terrain | Purpose | Visual examples | Timing |
| --- | --- | --- | --- |
| Plain | Baseline movement and combat | Basin dust, concrete pad, settlement yard, utility gallery floor | Prototype |
| Road | Fast exposed movement | Dirt lane, utility road, bridge deck, civilian street, conduit track | Prototype |
| Cover | Defensive modifier | Survey crates, scrub, rocks, pipes, heat exchangers, control cabinets | Prototype |
| HQ | Static win/loss anchor | Kestrel prefab, refinery command, coalition HQ, grid control gate | Prototype |
| Ridge | Movement friction and line-shaping | Basalt ridge, antenna hill, quarry wall, sealed security barrier | Prototype or early |
| Property | Capture, income, repair, or production anchor | Depot, relay, settlement node, factory, uplink | Early campaign |
| Bridge | Road-like chokepoint variant | Concrete bridge, pipe bridge, service gantry, control span | Mission 6-ish |
| Objective object | Mission verb target | Relay, pump, jammer, convoy marker, node, power tap | Early campaign |
| Factory | Production property | Field fab, refinery bay, automated depot | Mission 4 and later |

## Reusable Environment Kits

### Survey Camp And HQ Valley

The opening kit covers Kestrel's starting HQ, nearby field roads, restricted
seams, and the valley where the expedition learns it must fight. It should feel
practical, temporary, and faintly underfunded.

* Campaign use: Missions 1, 2, 8, and 48.
* Palette and materials: Dusty ochre ground, blue-gray prefab tents, teal
  Asterite flecks, dark basalt, pale equipment labels.
* Tactical identity: Defensible HQ, road approach, rescue lane, nearby relay or
  cache objectives, compact cover pockets.
* Landmarks: Kestrel HQ prefab, jammed relay mast, restricted seam posts,
  survey crates, portable floodlights, stranded Scout-7 marker.
* Flavor: A place built for field science tries to become a fortress while the
  labeling system remains deeply optimistic.

### Industrial Utility And Pump Stations

This kit covers civilian infrastructure around the basin: pump roads, coolant
loops, substations, service depots, and utility corridors. It should feel useful
and vulnerable.

* Campaign use: Missions 3, 11, 12, and 24.
* Palette and materials: Sun-baked service roads, white and green pump housings,
  coolant blue, rusted pipes, concrete pads.
* Tactical identity: Convoy lanes, exposed road speed, side gullies, utility
  structures that must be repaired or held.
* Landmarks: Pump Station Three, pipe bridges, culverts, coolant tanks, crawler
  wrecks, relay sheds.
* Flavor: Engineers treat pumps and pipes like coworkers with difficult but
  redeemable personalities.

### Ridge, Antenna, And Fog Line

This kit supports sensor missions and Sable pressure. It should feel higher,
colder, more militarized, and less trustworthy than the starting basin.

* Campaign use: Missions 5, 9, 13, and 16-20.
* Palette and materials: Chalky ridge stone, cool slate, silver antenna arrays,
  muted lavender fog overlays, cold blue signal lights.
* Tactical identity: Sightlines, ridge chokepoints, jammers, sensor post
  captures, artillery lanes, and hidden flank pockets.
* Landmarks: Antenna masts, Sable survey stakes, jammer towers, sensor dishes,
  corrupted map evidence, sealed utility corridors, exposed Asterite residue.
* Flavor: The map is lying in a professional tone of voice.

### Bridge And Route Control

This kit can appear inside other regions whenever a route itself is the
objective. Bridges should read as road rules with chokepoint drama.

* Campaign use: Missions 6, 14, 22, and 47.
* Palette and materials: Concrete gray, warning red, dry flood-channel tan,
  hazard paint, cracked supports.
* Tactical identity: Narrow crossings, demolition pressure, traffic control,
  blockade positioning, convoy protection.
* Landmarks: Demolition charges, bridge placards, cracked pylons, toll booths,
  service gantries, improvised barricades.
* Flavor: Infrastructure paperwork meets armor columns at unacceptable speed.

### Meridian Settlement Grid

This kit covers lived-in civilian spaces at the edge of the resource war. It
should be warm, patched, and dense with meaning without becoming visually busy.

* Campaign use: Missions 7, 14, and 21-25.
* Palette and materials: Warm amber lights, patched steel, dusty red trails,
  teal domestic Asterite taps, faded signage.
* Tactical identity: Convoys, restraint framing, civilian power nodes, terrain
  traps, scarce repairs, settlement defense.
* Landmarks: Heat taps, medical convoy markers, settlement fences, hauler bays,
  power-grid conduits, community shelters.
* Flavor: Outsiders keep describing normal life as insurgent infrastructure.

### Refinery And Extraction Zone

This kit belongs to Orison and late prototype escalation. It should be sharp,
legalistic, dangerous, and overbuilt.

* Campaign use: Missions 10, 26-30, and 37.
* Palette and materials: Industrial black, Orison orange, molten copper lights,
  smoky gray, teal siphon glow, hazard yellow.
* Tactical identity: Production pressure, income fights, HQ capture, exposed
  conveyor roads, cache races, mobile refinery anchors.
* Landmarks: Refinery HQ, smokestacks, conveyor belts, power siphons, legal
  claim billboards, compliance terminals.
* Flavor: Corporate villainy arrives with a safety webinar and a disputed
  invoice for the damage it caused.

### Grid Node And Automated Depot

This kit begins when the Basin Stabilization Grid becomes a battlefield system
instead of a buried liability. It should feel like hardened industrial
infrastructure: clean, precise, neglected, and readable rather than mystical.

* Campaign use: Missions 31-45.
* Palette and materials: White-gray utility plates, teal status lights, black
  sealed gates, old hazard striping, red lockout markers.
* Tactical identity: Substation shutdowns, automated depots, route locks,
  barrier chokepoints, relay chains, pressure from maintenance hatches.
* Landmarks: Grid substations, maintenance-drone hatches, sealed route
  barriers, shield relays, old safety signage, depot doors.
* Flavor: The battlefield starts enforcing old paperwork with hydraulic force.

### Grid Control District And Coalition Approach

The final kit combines familiar basin shapes with the old civil-defense and
industrial operations center under the basin. It should make the campaign feel
like it has returned home changed, not like it has entered a mystical core.

* Campaign use: Missions 46-50.
* Palette and materials: Coalition faction accents over Aster Basin terrain,
  deep utility teal and black, white emergency lighting, worn Kestrel markings.
* Tactical identity: Combined systems, final convoy, command HQ hold,
  control-gate push, science-team protection, objective shutdown chain.
* Landmarks: Original camp ruins, coalition command HQ, coolant convoy cargo,
  Holt recon route markers, grid control gate.
* Flavor: Mission 1 at campaign scale. Everyone is more competent, more tired,
  and less impressed by emergency labels.

## Missions 1-10 Environmental Beats

### Mission 1: Scout-7 Is Late

* Environment: Survey Camp and HQ Valley.
* Look and feel: Low morning light over dusty basin ground, camp prefabs behind
  basalt ridges, a teal seam glinting where Scout-7 went missing.
* Terrain promise: Player HQ behind a chokepoint, exposed road lane, cover near
  the rescue route, open space for scout mobility.
* Key props: Kestrel HQ prefab, stranded scout buggy, restricted seam marker,
  jammed relay mast, survey crates.
* Flavor line: The security plan was mostly a laminated evacuation map.

### Mission 2: Inventory Adjustment

* Environment: Camp relay yard and fuel cache.
* Look and feel: Pale concrete pads, orange fuel bladders, cable trenches,
  dish towers, and emergency lights taped to things that should have had lights.
* Terrain promise: Split objectives between relay and cache, roads that help
  both sides, service-shed cover around capture points.
* Key props: Relay dish, fuel tanks, service shed, access panel, cable tile,
  cache marker.
* Flavor line: Ammunition becomes a rapidly depreciating research consumable.

### Mission 3: Road To Pump Station Three

* Environment: Industrial Utility and Pump Stations.
* Look and feel: Dry flats crossed by service roads, pipe bridges, coolant blue
  tanks, and a pump station that looks too valuable to be this undefended.
* Terrain promise: Convoy lanes, exposed road speed, side-gully ambushes,
  station hold zone.
* Key props: Pump station, pipe bridge, culvert, crawler wreck, coolant line,
  water pressure monitor.
* Flavor line: The engineering team has already named the pump after someone
  they dislike, which is how they show affection.

### Mission 4: Emergency Fabrication Authority

* Environment: Field fabrication yard.
* Look and feel: Dark prefab floors, magenta and teal fabrication glow, yellow
  safety grids, depot pallets, and Orison signage applied before permission.
* Terrain promise: First production map, depot captures, armored lanes, enemy HQ
  guard, compact factory chokepoints.
* Key props: Field fabricator, depot pallets, printer arms, power cables,
  emergency authority console.
* Flavor line: Kestrel militarizes with a signature field that was absolutely
  not on the original form.

### Mission 5: Fog Over Antenna Field

* Environment: Ridge, Antenna, and Fog Line.
* Look and feel: Silver antenna masts on wet dark soil, cold blue signal lights,
  muted fog, Sable survey stakes placed with alarming confidence.
* Terrain promise: Soft fog, antenna captures, hidden ridge pockets, recon lanes
  that make Scout-7's survival matter.
* Key props: Antenna mast, damaged science camp, sensor post, fog veil overlay,
  Sable survey marker.
* Flavor line: Holt insists the fog is professionally inconvenient.

### Mission 6: Bridge Warranty Void

* Environment: Bridge and Route Control.
* Look and feel: Concrete span over a dry flood channel, demolition charges,
  red warning paint, cracked supports, and Orison armor lined up with legal
  impatience.
* Terrain promise: Bridge chokepoint, demolition pressure, counterattack across
  the far bank, blockade positioning.
* Key props: Bridge deck, demolition charge, cracked pylon, service gantry,
  warranty placard.
* Flavor line: The bridge warranty excludes hostile peer review.

### Mission 7: Peer Review With Rockets

* Environment: Meridian Settlement Grid.
* Look and feel: Warm settlement lights, patched metal fences, domestic Asterite
  heat taps, dusty side trails, and an active drill too close to people's homes.
* Terrain promise: Protect drill and crates, fast raider flanks, restraint
  framing, civilian power nodes as spatial anchors.
* Key props: Drill rig, heat tap, Asterite crate, settlement fence, hauler bay,
  side-trail marker.
* Flavor line: Calder calls Kestrel tourists with artillery and better
  stationery.

### Mission 8: The Audit Arrives

* Environment: Blacked-out supply hub.
* Look and feel: Dim navy shadows, emergency red strips, gray warehouses, audit
  convoy headlights, and a sign-in kiosk in exactly the wrong place.
* Terrain promise: Multi-objective defense, repair escorts to substations,
  supply hub protection, convoy survival.
* Key props: Supply warehouse, substation, audit vehicle, sealed file terminal,
  restore-power marker.
* Flavor line: The auditor asks if the battle has a sign-in sheet.

### Mission 9: Blank Map Territory

* Environment: Altered ridge line and hidden utility corridor.
* Look and feel: Chalky ridge stone, black map-void accents, sealed service
  doors, Asterite residue, jammers, sensor towers, and terrain that looks
  edited instead of eroded.
* Terrain promise: Fogged breakthrough, sensor post captures, jammer removal,
  artillery avoidance.
* Key props: Jammer tower, sensor dish, sealed utility hatch, unverified-map
  marker, ridge cliff.
* Flavor line: Venn calls the old map optimistic fiction with contour lines.

### Mission 10: Operation Small Print

* Environment: Refinery and Extraction Zone.
* Look and feel: Orison black and orange, conveyor roads, smokestacks, molten
  copper light, teal siphon columns, and compliance screens that will not stop
  playing.
* Terrain promise: Full production, income pressure, HQ capture threat, refinery
  chokepoints, decisive enemy commander defeat.
* Key props: Refinery HQ, smokestack, conveyor, power siphon, Orison billboard,
  abandoned server rack.
* Flavor line: The victory debrief gets interrupted by an automated compliance
  webinar.

## Five-Mission Environment Arcs

### Missions 11-15: Basin Stabilization

Kestrel turns survival terrain into an operating perimeter. The same basin
colors return with more built-up roads, depots, receiver towers, barricades,
and marked evacuation lanes.

* Terrain focus: Capture chains, supply choices, road defense, artillery-ready
  ridges, and three-approach perimeter defense.
* Visual progression: The valley now has labels, field barriers, command posts,
  and a more intentional Kestrel footprint.
* Reusable assets: Forward camp, depot, receiver tower, perimeter barricade,
  evacuation convoy, marked supply road.

### Missions 16-20: The Fog Line

Sable-controlled ridges become the campaign's sensor frontier. The environment
should feel formal, cold, and controlled until the data starts contradicting the
maps.

* Terrain focus: Recon screens, jammer pockets, artillery sightlines, ridge
  captures, neutral anomaly objects used sparingly.
* Visual progression: Sable blue-gray markings and professional antenna grids
  sit on terrain that gradually reveals old utility corridors and edited map
  boundaries.
* Reusable assets: Sable bunker, jammer, artillery pad, antenna grid, corrupted
  map evidence, anomaly seam.

### Missions 21-25: The Meridian Question

The campaign moves into civilian infrastructure. These maps should feel lived
in: not fragile decoration, but places people understand better than armies do.

* Terrain focus: Convoys, restraint framing, settlement defense, trail ambushes,
  rationed production, power-node repairs.
* Visual progression: Warm lights and patched structures begin under threat and
  end with a defended coalition settlement.
* Reusable assets: Civilian building, heat tap, medical convoy, trail trap,
  settlement grid line, community shelter.

### Missions 26-30: Prototype Race

The battlefield becomes more industrial and experimental as everyone rushes
unfinished Asterite hardware into combat.

* Terrain focus: Anti-prototype counters, mobile refinery objectives, cache
  races, shifting production zones through explicit mission objects.
* Visual progression: Test pylons, overcharged seams, prototype pads, and
  scorch marks make the same terrain feel less stable.
* Reusable assets: Prototype pad, mobile refinery segment, cache vault, test
  pylon, overcharged seam, sealed control hatch.

### Missions 31-35: Emergency Protocol War

The Basin Stabilization Grid starts enforcing old emergency protocols across
human places. It should not look chaotic. It should look like neglected
industrial safety systems doing exactly the wrong thing at scale.

* Terrain focus: Node shutdowns, drone hatches, route sealing, emergency holds,
  evacuation lanes, joint defense.
* Visual progression: Human roads and settlements gain white-gray utility plates
  and sealed barriers where the grid has isolated them.
* Reusable assets: Grid node, maintenance hatch, sealed route barrier, shield
  relay, safety sign, depot door.

### Missions 36-40: The Hardliner Offensive

Human factions fight over grid uplinks and authority keys. The maps should
visually mix faction occupation with old utility infrastructure to show the
danger of treating life-support control as a weapon.

* Terrain focus: Uplink capture sequences, relay cascades, stockpile economy,
  fog-heavy defenses, layered control points.
* Visual progression: Sable and Orison fortifications bolt themselves onto grid
  nodes until the whole battlefield looks contested and unstable.
* Reusable assets: Uplink tower, battery stockpile, relay cascade marker,
  hardliner barricade, old project warning panel.

### Missions 41-45: Grid Collapse

The terrain becomes a map of cascading infrastructure failure. It can feel
tense and technical, but it must remain the cleanest visual grammar in the
campaign.

* Terrain focus: Automated factories, converted production properties, relay
  chains, barrier chokepoints, regional grid stabilization.
* Visual progression: Active white and teal utility plates replace some human
  props, while dark service channels and lockout markers point to routes and
  threats.
* Reusable assets: Convertible factory, active barrier, relay chain tile,
  grid branch control room, lockout marker, conduit track.

### Missions 46-50: Containment Protocol

The final arc returns to familiar places at campaign scale. It should visually
echo Mission 1 while showing that the coalition is now organized and exhausted.

* Terrain focus: Coalition-force staging, final convoy, original camp defense,
  command HQ hold, control-gate push, shutdown chain.
* Visual progression: Kestrel, Sable, Meridian, and repaired Orison hardware use
  distinct accents over basin and grid-control tiles without becoming a color
  soup.
* Reusable assets: Coalition banner marker, control-district approach road,
  command HQ, final convoy cargo, Holt recon route marker, grid control gate.

## Tile Readability Rules

Every environment kit should pass these checks before it is accepted:

* Terrain rule identity remains readable at handheld distance without opening a
  tooltip.
* Road, cover, HQ, property, ridge, objective, bridge, and blocked tiles use
  distinct silhouettes, not only color changes.
* Friendly and enemy unit sprites remain readable on every common terrain tile.
* Decorative detail stays away from HP bars, board labels, team badges, cursor
  corners, movement highlights, attack highlights, and objective markers.
* Important tiles remain distinguishable in grayscale or low-saturation review.
* Each tile has one main visual idea. A road is a road first, not a road, cable,
  crack, glow, puddle, and warning label all at once.
* Late-game grid-control tiles use cleaner shapes than early basin tiles so the
  final campaign does not become sci-fi visual noise.

The grounded universe history, political factions, technology limits, and
formal definition of the Basin Stabilization Grid are defined in
[Universe Backstory](universe-backstory.md).

## Mission Brief Environment Fields

Future mission briefs should include these fields:

* Environment kit
* Rules terrain used
* Visual variants needed
* New terrain or objective mechanics
* Tactical environment promise
* Required landmarks
* Readability risks
* Required deterministic tests
* Replay or AI validation needs
* Steam Deck screenshot checkpoint

## Validation Plan

* Run a 10-second read test on 1280x800 screenshots: the viewer should identify
  HQ, roads, cover, objectives, enemy pressure lane, and dangerous terrain.
* Run a grayscale test for each environment kit.
* Check both friendly and enemy unit palettes against each common tile.
* Add movement, defense, forecast, AI pathing, and replay checks for every new
  terrain rule before it appears in campaign play.
* Add a golden mini-map scenario for each accepted environment kit's main layout
  pattern.
* Run a tile budget audit per five-mission arc. If tile and prop count grows
  faster than the player's tactical vocabulary, merge variants.

## Next Environment Slice

The next concrete slice should be an Aster Basin starter tileset spec for
Missions 1-3. It should cover base ground, road, cover, camp HQ, relay and fuel
props, pump station props, seam accents, objective markers, and one 1280x800
readability mock before any later kit expands the art budget.
