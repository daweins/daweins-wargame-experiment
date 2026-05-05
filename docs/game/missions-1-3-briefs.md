---
title: Missions 1-3 Playable Briefs
description: Implementation-ready briefing documents for the first three tactical combat missions
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## How To Use This Document

Each brief contains everything needed to implement a mission in the prototype:
map layout, starting forces, objectives, mechanics, story beats, and dialogue
samples. Grounded lore fields establish the political and logistical context so
briefings and debriefs can reference real constraints rather than generic danger.

Cross-references:

- Narrative spine and faction summaries: [Campaign Plot Spine](campaign-plot-spine.md)
- Universe constraints and Spindle rules: [Universe Backstory](universe-backstory.md)
- Unit stats and counter rules: [First Six Mission Unit Ramp](first-six-mission-unit-ramp.md)
- Character voice and doctrine: [Campaign Character Bible](campaign-character-bible.md)
- Environment beats: [Campaign Environment Plan](campaign-environment-plan.md)

---

## Mission 1: Scout-7 Is Late

### Lore Context

**Transit delay:** The nearest Kestrel authority with enforcement power is on
Caldera's transit hub, roughly four months out by priority courier vessel.
Expedition standing orders allow site commanders to authorize emergency action,
but enforcement review runs on the transit schedule, not the attack schedule.

**Spindle status:** The Aster Basin Spindle office is at Kestrel base camp.
It is also the office Orison needs to jam before seizing the site without
an immediate legal record. Camp relay jamming in this mission is partly a
Spindle isolation attempt. Kestrel cannot file an authenticated emergency
packet while the relay is dark.

**Cargo or permit constraint:** Orison holds a Concession Survey Permit for
the eastern seam, covering "non-destructive evaluation" only. Their contractors
arriving with armed vehicles are operating outside permit scope — which is
exactly the evidence Scout-7 may have observed and exactly why they need her
silenced before anyone can authenticate a report.

**Grid stakeholder:** The HQ sits on a Basin Stabilization Grid secondary
access point. The pump controller underneath it predates Kestrel's lease.
Orison's concession filing may or may not cover that parcel, depending on
which map version the Treaty Oversight Bureau is using.

**Asterite supply-chain cost:** Scout-7 was surveying a seam with anomalous
conductivity readings. A positive Asterite confirmation at that seam would
change the extraction license tier, the insurance category, the transit
freight class, and three Orison concession agreements. That paperwork has
not been filed. Scout-7's samples might force it.

**Civilian infrastructure risk:** The HQ camp hosts the expedition's heat
plant, coolant loop, medical station, and food stores. It is not a hardened
military installation. Losing it ends the expedition before any extraction
dispute reaches arbitration.

### Situation

Day 14 of the Kestrel Aster Basin Survey. Camp alert, level two. Scout-7's
crawler missed its 0600 check-in from Survey Sector Crimson. The camp relay
logs show a jamming spike at 0547. Unmarked vehicles, Orison contractor
markings obscured with survey-kit gray paint, are moving along the eastern
access road toward the chokepoint.

Dr. Venn's working assumption at mission start: armed extraction team
conducting an intimidation survey. She expects to call a bluff. She is wrong.

### Opening Briefing

> Speaker: Dr. Venn, interior, HQ comm station, pre-dawn light

"Scout-7 is fourteen minutes past check-in on a sector that showed anomalous
readings yesterday. The relay is jammed from somewhere east of the ridge.
Major Rusk is going to tell me there are armed vehicles on the access road,
and I am going to agree that this is alarming, and then we are going to hold
this camp until we know whether Lena is still with her crawler."

*Rusk enters.*

**Rusk:** "Three vehicles, contract gray, no visible survey equipment. They
are not here to take core samples."

**Venn:** "Then we are conducting a lab safety drill with live ammunition.
I assume that requires different forms."

**Rusk:** "We hold the chokepoint. If Scout-7 is alive, she will move toward
the camp. If she is not moving, someone from our side needs to get eyes on
her last waypoint."

**Venn:** "Then that is what we do. Hold the chokepoint, find Scout-7, and
do not let anyone past the perimeter until I know what she saw."

### Mission Objective

Hold the HQ. Rescue Scout-7. Defeat all remaining enemies.

### Map Concept

**Layout:** A horizontal valley camp with a defensive chokepoint on the
eastern approach. The HQ sits center-left, with a rocky ridge providing
the main cover band. Scout-7's last known position is southeast, across
an exposed road section. The enemy enters from the east in two loose groups.

**Size:** 10 columns × 8 rows.

**Terrain features:**

- HQ tile: center-left, Kestrel control at start
- Rocky cover band: mid-map, 3 tiles wide, north and south access gaps
- Eastern access road: open, connects east edge to chokepoint, favorable
  movement for enemy vehicles
- Scout-7 position tile: 2 tiles east of the cover band, exposed, enemy
  ZOC nearby
- Southern flank: light brush cover, accessible to scouts on either side

**Tile types used:** HQ, road (fast), rocky terrain (defense +1), brush
(defense +1), open ground.

### Starting Forces

**Player (Kestrel):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Utility Armor (1) | HQ approach, west side of chokepoint | Rusk's anchor |
| Field Tech (2) | Cover band, north and south gaps | Hold the flanks |
| Survey Scout (1) | Brush, southern edge | Available for Scout-7 rescue run |
| Scout-7 (rescue target) | Southeast, behind enemy line | Immobile; rescue completes when a friendly unit occupies the same tile |

**Enemy (Orison, mission start):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Line Armor (1) | Eastern road, 4 tiles from chokepoint | Pushes the road |
| Raider Trooper (3) | Eastern road and north flank | Screens armor, threatens cover flanks |
| Pursuit Scout (1) | Southern brush | Hunts the player's Survey Scout |

**Enemy reinforcement (Turn 3, if Scout-7 not yet rescued):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Raider Trooper (1) | Southeast, near Scout-7's position | Escalation pressure to rescue before enemy reaches her |

### Rules In Play

This mission uses only the core prototype rules: move, attack, counterattack,
terrain defense, HQ loss condition, and the rescue mechanic from the
current prototype.

**Rescue rule (existing):** A friendly unit that moves onto Scout-7's tile
rescues her. Scout-7 then becomes a normal Field Tech unit controlled by
the player.

**HQ loss condition (existing):** The player loses if an enemy unit occupies
the HQ tile at any point.

**Victory condition:** All enemy units defeated while HQ is held.

**Defeat conditions:** HQ captured by enemy, or all player combat units
destroyed.

### Tactical Lesson

The player learns to use terrain cover, manage two threats at once (push
through chokepoint and rescue to the southeast), and read the basic combat
forecast before committing. Scout-7's rescue creates a second front that
the Utility Armor alone cannot solve.

The chokepoint shows why Armor wins road fights but cannot handle both
flanks. The Scout teaches that high-move units have reachability advantages
the enemy will exploit if not watched.

### Story Reveal

When Scout-7 is rescued she reports: there were armed Orison contractors at
a sealed section of Sector Crimson, moving core-extraction equipment into a
survey access tunnel that the official maps show as geologically unremarkable.
She got samples. They are in the crawler.

### Victory Beat

The camp holds. Scout-7 is home. The crawler samples are intact. Nobody
has authenticated a legal record of the attack yet because the relay was
dark for the entire engagement.

### Debrief

**Priya:** "The relay log has a seventeen-minute gap starting at 0547.
That is not a hardware fault. Someone knew our maintenance window."

**Venn:** "They came before we could send an authenticated packet. That
is deliberate. Which means this is not a rogue contractor team. Someone
at Orison's planning level approved a Spindle blackout window."

**Rusk:** "The camp relay is back. We have one narrow channel before
they jam it again. What do we send?"

**Venn:** "Sample hash, incident time, permit scope violation, and
Scout-7's grid coordinates. Thirty words or less. Let the evidence
travel before the explanation does."

### Tone Note

Venn should sound like someone discovering, mid-sentence, that she is
now a field commander and is not going to stop being a scientist because
of it. Rusk should be steady and specific, not theatrical. Priya is not
present in Mission 1 briefing, but she appears in the debrief as the
person who noticed the relay log gap before anyone else thought to look.

---

## Mission 2: Inventory Adjustment

### Lore Context

**Transit delay:** The emergency packet from Mission 1 reached the Caldera
hub Spindle station in near-real time. A response packet acknowledging
receipt arrived within the Spindle handshake window. A human decision from
Treaty Oversight about enforcement action will take weeks to months,
because that requires a person, and people travel on transit schedules.
Kestrel has an authenticated record of the attack, but no material help
is coming soon.

**Spindle status:** Orison knows Kestrel got a packet out. Their next
priority is to prevent a second packet that includes Scout-7's sample
data. The orbital relay is the physical authentication anchor for any
Spindle-certified transmission from the basin. Whoever controls the relay
controls what gets certified. Orison's logistics team was given the
relay maintenance schedule under the original joint-survey agreement.

**Cargo or permit constraint:** The fuel cache at the relay station is
listed on Kestrel's joint-expedition manifest, co-signed by Orison's
field logistics officer. Under the agreement, neither party can
unilaterally remove shared fuel reserves without a written arbitration
notice. Orison has not filed notice. They are just taking the fuel,
which is a separate violation and a separate packet Kestrel wants to send.

**Grid stakeholder:** The relay station's power feed runs off a Basin
Stabilization Grid secondary tap. The tap predates the joint-survey
agreement. If the grid power drops, the relay goes dark regardless of
who holds the station. Neither faction currently controls the tap authority.

**Asterite supply-chain cost:** Survey crawlers need a specific fuel
blend to operate in high-altitude seam terrain. The cache at the relay
station is the only surveying-grade reserve in the basin. Losing it
means Kestrel cannot transport sample cores, which means Scout-7's
evidence stays unverified even after it is authenticated.

**Civilian infrastructure risk:** The relay station is also the
secondary emergency beacon for Meridian settlements in the eastern
valley. If Orison occupies and locks it, those settlements lose their
automated distress signal. The locals do not know this yet.

### Situation

Day 15. The camp relay is up on a narrow channel, but Orison is already
moving contractors toward the orbital relay station three kilometers
northeast. Kestrel's joint-survey agreement gives both parties access
to the station, but Orison's logistics team has the current service
codes after rotating them during their last maintenance run.

Priya identified the fuel issue before Rusk finished reading the threat
assessment. She also noticed that Orison arrived at the relay station
before the attack — suggesting the maintenance-window leak predates
yesterday's raid.

### Opening Briefing

> Speaker: Priya Nayar, exterior, camp motor pool, morning

"Good news: the relay came back up. Bad news: Orison was at the relay
station six hours before they attacked us, which means they rotated the
service codes and left their own team inside. Also, our fuel cache is
at that station. Also, without that fuel, the crawlers that are carrying
Scout-7's samples cannot reach the verification depot."

**Rusk:** "Objectives: the relay and the fuel. In that order."

**Priya:** "Ideal order, yes. Orison's order is to get the fuel and
then lock the relay. Which means we have a narrow window before this
becomes a two-front problem."

**Venn:** "Send the sample hash before we move. If we lose the relay
on the way out, the hash already traveled."

**Priya, checking manifest:** "Signed. Also, I am filing the fuel
removal as a joint-manifest violation, which is technically a separate
arbitration notice, which requires a—"

**Venn:** "Send it anyway."

**Priya:** "It is already queued."

### Mission Objective

Capture or hold the orbital relay station. Recover the fuel cache.
Defeat remaining enemies.

Capture priority: the relay first. If Orison holds the relay at the
end of any turn, the game shows a warning that the authentication
window is closing.

### Map Concept

**Layout:** A road leading northeast from the camp edge to the relay
station at the map's northeast corner. Fuel cache sits in a depot
tile at the map's east-center, closer to the camp start but reachable
by fast enemy vehicles from the relay area. Brush and low rock cover
break up the road corridor.

**Size:** 12 columns × 8 rows.

**Terrain features:**

- Relay station tile: northeast corner, neutral at start, Orison moves
  to capture it on Turn 2 if unchallenged
- Fuel cache depot: east-center, neutral at start, capturable property
- Cover band: central north, brush and rock, 2-tile depth
- Open road: direct northeast, fast movement for vehicles
- Southern bypass: slower terrain, usable for flanking without road exposure

**Tile types used:** Property tile (relay), property tile (depot),
road (fast), brush (defense +1), rocky terrain (defense +1), open ground.

**Capture rule:** A Field Tech or Engineer unit that starts its turn on a
capturable property tile and does not move makes one capture progress point
(two consecutive turns = captured). Enemy units reset capture progress.

### Starting Forces

**Player (Kestrel):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Field Tech (2) | Camp edge, western start | Primary capture units for relay and depot |
| Utility Armor (1) | Road, central-west | Road anchor and escort |
| Survey Scout (1) | South bypass entry | Flanking path and speed |
| Expedition Engineer (1) | Camp edge, near motor pool | New unit; used to stabilize relay systems |

**Enemy (Orison):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Raider Trooper (2) | Relay station and road approach | Will begin capturing relay Turn 1 |
| Line Armor (1) | Fuel depot, east-center | Holds the second objective |
| Breach Sapper (2) | Cover band, mid-map | New unit; moves toward the relay and punishes exposed Engineers |

### New Unit: Expedition Engineer (Player)

The Engineer was survey equipment yesterday. It repairs hull damage and
can stabilize failing relay electronics, which is not in any military
manual because nobody expected to need it.

**Role:** Repair 2 HP on an adjacent friendly unit instead of attacking.
Can stabilize a relay object by ending its turn adjacent to it (shows a
Stabilize action prompt). Cannot stabilize while under attack.

**Stats:** HP 9, Move 3, ATK 3, DEF 0.

**First-encounter puzzle:** The player has two objectives and one Engineer.
They must choose which objective gets the Engineer's stabilization and
which gets a Field Tech capture run. There is no single-turn answer.

### New Unit: Breach Sapper (Enemy)

Orison brought these to make sure Kestrel cannot hold relay property
by repairing it. They deal bonus damage to support units and
objective tiles.

**Role:** Deals +2 damage against Engineers, Field Rigs, and mission
property tiles. Fragile against infantry trade.

**Stats:** HP 8, Move 4, ATK 4, DEF 0.

**Counter:** Field Techs can trade favorably into Sappers. Armor can
deter approach. An Engineer should never be left adjacent to an
unsupported Sapper.

### Rules In Play

Core prototype rules plus the capture mechanic (two-turn property
capture) and the new Engineer Repair and Stabilize actions. The relay
station has a capture-warning overlay that activates if Orison holds it
at the end of any turn.

**Victory condition:** Kestrel controls both relay and fuel depot, and
all enemies are defeated.

**Alternate victory:** Kestrel controls the relay and has destroyed all
enemy units, even if the fuel depot was lost. Fuel loss triggers a
mission-debrief consequence (slower crawlers in Mission 3) but does not
end the campaign.

**Defeat condition:** Orison captures and holds the relay for two
consecutive turns (authentication window closes), or all player combat
units are destroyed.

### Tactical Lesson

Splitting attention between two objectives teaches property-control
tradeoffs. The Engineer introduces a support unit that needs screening.
The Sapper introduces a counter-to-support unit that forces the player
to protect the Engineer instead of only racing forward.

### Story Reveal

Inside the relay station's access log: Orison's logistics team rotated
the service codes at 2300 on the day before the attack. The joint-survey
agreement requires 48 hours notice for service-code changes. The rotation
was also logged as a routine maintenance update, not a security event.
Someone classified it incorrectly, which is either negligence or deliberate
cover. Venn wants to send the classification discrepancy as a separate packet.

### Victory Beat

The relay is up and authenticated. The sample hash from Mission 1,
Scout-7's location data, and the manifest violation notice are all
transmitted before the channel narrows again.

### Debrief

**Priya:** "The relay log shows the code rotation at 2300 the night
before. That is not a maintenance window. That is a timer."

**Venn:** "They planned the isolation before they deployed. This was
not a local contractor decision."

**Rusk:** "Someone at Orison's operations level cleared this. Which
means someone at Treaty Oversight's authentication desk is going to
receive a very carefully worded packet in about forty-eight seconds."

**Venn:** "Let me write that packet."

**Priya:** "It is already sent. I labelled the code-rotation
discrepancy as a Spindle certification irregularity, because technically
a mis-classified maintenance event affecting relay authentication is one."

**Venn:** "Is it?"

**Priya:** "It is now."

### Tone Note

Priya carries this mission's comedy. Her humor comes from treating
procurement violations, maintenance logs, and inventory discrepancies
with the same clinical precision that a surgeon uses for sutures.
Rusk is reassuring and operational. Venn is focused on evidence custody.

---

## Mission 3: Road To Pump Station Three

### Lore Context

**Transit delay:** Kestrel's authenticated packet reached the Caldera hub.
A response came back by Spindle three days later: the Treaty Oversight
Bureau has logged the incident as a Class 4 Permit Dispute with Potential
Hostile Action, which opens a formal review track. Review tracks run on
treaty calendar. The next scheduled review session is in six weeks.
Six weeks is not useful when the camp's coolant loop depends on Pump
Station Three surviving today.

**Spindle status:** Treaty Oversight's response is authenticated and on
record. Orison has received the same notification. The review clock
is now running, which means Orison has a window before enforcement
recommendations can travel. Their operations team will try to establish
physical facts on the ground faster than the review process can catch up.

**Cargo or permit constraint:** Pump Station Three is classified as
civilian critical infrastructure under the original basin development
charter, which predates Orison's extraction license by sixty years. The
charter requires that any extraction-related activity within 500 meters
of a civilian pump station notify the Treaty Oversight Bureau before
starting. Orison has filed no such notice. Moving vehicles toward that
station right now is a second charter violation on the same review record.

**Grid stakeholder:** Pump Station Three is Basin Stabilization Grid
infrastructure. Its pump controllers and heat taps are on the grid's
southern distribution arm. The authorization records for that arm are
split: Orison's extraction concession covers surface transit rights,
but the grid subsystems require a separate utility-corridor easement
that Orison has applied for but not received. The station has an old
grid maintenance code that local Meridian crews have been using for
years through an unofficial bypass. Kestrel does not know the bypass
exists yet.

**Asterite supply-chain cost:** The pump station's filters show trace
Asterite residue, which means the utility corridor it sits on has been
processing basin runoff through an old grid subsystem. This is not on
any official map. Scout-7's survey samples may correlate. If they do,
Kestrel's "civilian pump station emergency" is actually the first
on-record confirmation of an undocumented Asterite distribution path.

**Civilian infrastructure risk:** Pump Station Three supplies heat tap
pressure and clean water to four Meridian ridge settlements and to
Kestrel's own camp coolant loop. If the station is occupied and locked,
those settlements lose heat in roughly six hours at current temperatures.
The settlements are not in radio contact with the camp. They do not know
this threat exists.

### Situation

Day 17. Two damaged survey crawlers need to reach Pump Station Three for
repairs and to restart the camp coolant loop. Priya has patched the
crawlers enough to move them under escort, but they cannot take combat
damage. The service road is exposed. Orison light vehicles are using the
same road network for fast flanking.

Rusk wants to send the crawlers around the long southern bypass, which
is slower and avoids most of the road. Priya says the crawlers will not
make the climb on bypassed terrain with the current engine state. The
road is the only viable route.

Holt has not yet appeared in the campaign, but her presence is foreshadowed
here: Scout-7 intercepts a garbled radio burst from the northern ridge that
sounds like a distress tone on a Meridian emergency frequency.

### Opening Briefing

> Speaker: Priya Nayar, exterior, motor pool, overcast morning

"The crawlers will make the road run if we keep them moving and nobody
shoots the engine housings. Orison light vehicles are already on the
eastern road service spur, which means they did not come here to look
at rocks. They came to use the road before we do."

**Rusk:** "Escort in two columns. Armor runs the road shoulder. Infantry
holds the flank approaches. Scout forward on brush gaps. Move together."

**Priya:** "If either crawler takes more than two direct hits, I am going
to be very disappointed, because I am out of replacement heat exchangers
and I am already sad about that."

**Venn, reviewing the station schematics:** "The station filters
have Asterite residue. That corridor is not in our survey license.
We are walking into something that nobody put on a map for a reason."

**Rusk:** "We can solve the map problem after we solve the road problem."

**Venn:** "We may not be able to separate those two problems."

### Mission Objective

Escort both damaged crawlers to Pump Station Three. Block ambush routes.
Hold the station through the enemy response.

The crawlers are escort-class units: they move but cannot attack, and
they must reach the pump station depot tile to complete the objective.

### Map Concept

**Layout:** A road running left to right (west to east) with the camp
at the west edge and Pump Station Three as a property tile at the east.
The road has two ambush breaks where brush and ridgeline create natural
blocking positions. Enemy vehicles enter from east and southeast using
road forks.

**Size:** 14 columns × 8 rows.

**Terrain features:**

- Western camp edge: player start area
- Service road: center-horizontal, fast movement for vehicles
- Ambush positions (north fork, south fork): brush and rock, 2 tiles deep
  each, flanking enemy entry points
- Pump station depot: east edge, neutral at start, capturable, victory tile
- Ridge approach: northeast corner, slightly elevated, bonus defense
- Southern flank: loose rock, slower for vehicles but accessible to
  infantry and scouts

**Tile types used:** Property tile (pump station), road (fast),
brush (defense +1), rocky terrain (defense +1), ridge (defense +2),
open ground.

### Starting Forces

**Player (Kestrel):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Damaged Survey Crawler (2) | Camp edge, road start | Escort targets; Move 3, no attack, 4 HP each; mission fails if both are destroyed |
| Utility Armor (1) | Road shoulder, behind crawlers | Road anchor; leads the column |
| Field Tech (2) | North and south of road, flank screens | Block ambush approach tiles |
| Survey Scout (1) | Brush, south flank | Speed unit; spots enemy approach and contests road forks |
| AT Lancer (2) | Camp edge, behind crawlers | New unit; stays behind until enemy armor appears |

**Enemy (Orison):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Hunter Bikes (2) | East road fork, southeast brush | New enemy unit; fast, targets exposed support |
| Line Armor (1) | Northeast ridge approach | Heavy pressure, appears Turn 2 |
| Raider Trooper (2) | North fork ambush position | Infantry screens for the armor push |
| Pursuit Scout (1) | South fork | Contests the player's southern flank |

**Enemy reinforcement (Turn 4):**

| Unit | Start tile | Notes |
| ---- | ---------- | ----- |
| Raider Trooper (1) | Southeast entry | Pressure if crawlers have stalled |

### New Unit: AT Lancer (Player)

Kestrel's geology team had a high-power rock coring laser. It is not
officially reclassified as an anti-armor weapon. The legal team is
handling that in a separate review track.

**Role:** Hard counter to Armor and Siege Breakers. Fragile against
infantry and fast units.

**Stats:** HP 9, Move 3, ATK 5, DEF 1.

**Matchup:** Gains +3 effective damage against Armor and Siege Breaker.
Loses trades to Field Tech, Raider Trooper, and Striker class units.

**First-encounter puzzle:** Line Armor is on the northeast ridge.
The Lancer can destroy it in two hits if screened by infantry.
If the Lancer moves without a screen, the enemy Raider Troopers
will trade into it before it fires again. The lesson is that
hard counters require protection.

### New Unit: Hunter Bike (Enemy)

Orison's contractor fast movers. They are not built for armor fights.
They are built to reach Engineers, Scouts, and Lancers before the
escort catches up.

**Role:** High-speed support hunters. Bonus damage against support and
recon units. Poor against Armor and infantry traps.

**Stats:** HP 9, Move 6, ATK 5, DEF 1.

**Counter:** Armor blocks the road effectively. Infantry traps punish
the overextension. Scouts used as bait can pull Bikes into armor coverage.

### Rules In Play

Core prototype rules plus the capture mechanic, Engineer actions from
Mission 2, and two new interactions introduced here:

**Crawler escort rule:** The two damaged crawlers are player-controlled
but cannot attack. They move up to 3 tiles per turn. They are destroyed
at 0 HP like any unit. If both crawlers are destroyed, the player
loses the mission. If one crawler reaches the pump station depot tile,
the objective counter advances.

**Victory condition:** Both crawlers reach the pump station depot tile,
and the station is not under enemy control at end of turn.

**Alternate victory:** One crawler reaches the depot and all enemies
are defeated. The surviving crawler reaches the station during the
debrief cut. Priya notes this is "suboptimal but not catastrophic."

**Defeat conditions:** Both crawlers destroyed, station captured and
held for two turns, or all player combat units destroyed.

### Tactical Lesson

Road speed introduces the first meaningful tradeoff between fast and
safe. The Lancer is the answer to the armor problem but creates a
new protection problem. Hunter Bikes teach that exposing support
units (Engineers, Scouts, Lancers) to fast units ends badly.

The convoy structure teaches that not every unit should push forward:
Armor leads, infantry screens flanks, Scout spots and baits, Lancers
wait behind the front for the armor reveal.

### Story Reveal

Inside the pump station, Priya finds the Asterite-residue filters
that Venn flagged in the briefing, plus a set of maintenance logs
signed with a code she does not recognize. The code format matches
neither Kestrel's protocols, Orison's contractor badge numbering,
nor any official Transit Oversight registry she has access to.
The logs are recent. The last entry was two days ago.

Someone has been maintaining this station off the books.

### Victory Beat

The camp coolant loop is restored. Both crawlers are recovered.
Pump Station Three is in Kestrel hands. The Meridian ridge settlements
still have heat. They also still do not know this fight happened on
their behalf.

The intercepted radio burst from the northern ridge resolves as a
Meridian emergency frequency tone — someone testing a distress signal
rather than sending one. Rusk notes this in the post-action report
without flagging it as urgent.

### Debrief

**Priya, holding the maintenance log:** "These codes are not in any
registry I can reach. Format looks like an old grid maintenance
convention. Pre-charter, possibly pre-basin development."

**Venn:** "Which means someone with original grid access has been
keeping this station alive longer than Orison's concession has existed."

**Rusk:** "The filter residue and the maintenance logs put this
corridor on a different map than the one everyone filed with Treaty
Oversight."

**Venn:** "Send the filter analysis with the sample hash from Mission 1.
If those readings correlate, we have evidence of an undocumented
Asterite distribution path, which changes the concession license tier,
which changes what Orison can legally do here."

**Priya:** "Queued. Also, I have noted the Hunter Bike damage to the
Number Two crawler as a maintenance-induced mission extension,
which is how procurement will see it anyway."

**Rusk:** "I heard a Meridian distress frequency from the northern ridge
during the push. Probably a test. But someone is up there."

**Venn:** "Add it to the post-action log. We may need to know who
is on the other side of that ridge before Orison figures it out first."

### Tone Note

Mission 3 should feel like the team becoming more capable and more
aware that the problem is bigger than it looked on Day 14. Venn's
evidence-first instinct is starting to pay off in map intelligence
rather than just in legal filings. Rusk is steady and beginning to
trust the expedition's unconventional problem-solving. Priya's
procurement humor is a coping mechanism that also happens to produce
genuine tactical solutions.

The Meridian distress-frequency moment should be understated — a
single log entry and one Rusk line. It is a seed for Mission 5.

---

## Implementation Notes

### Enemy Behavior Budget

Mission AI should stay simple and deterministic while these briefs become
playable. Each enemy behavior should be expressible as priorities over the
existing command search, not as hidden scripts that bypass legal movement or
combat.

**Mission 1:** Orison pressure focuses on the HQ road and Scout-7 rescue lane.
Line Armor advances toward the chokepoint unless a high-value attack is legal.
Raider Troopers screen the armor and contest cover gaps. The Pursuit Scout
hunts the player's Survey Scout or blocks the rescue path, but it should not
ignore a legal HQ threat if the road is open.

**Mission 2:** Relay attackers prioritize capture progress before damage.
Sappers prefer exposed Engineers, relay-adjacent support units, or property
targets. The fuel-depot armor holds the cache until a player unit threatens the
relay, then trades only if it can protect the split-objective pressure.

**Mission 3:** Hunter Bikes prioritize exposed Lancers, Scouts, Engineers, and
crawlers, but avoid ending adjacent to Armor unless a crawler hit is available.
Line Armor pressures the road shoulder. Raider Troopers occupy ambush cover and
screen the armor so the Lancer lesson stays visible.

### Radio Banter Hooks

Use these as short in-mission lines triggered by objective state. They should
fit inside compact controller-friendly message boxes and avoid interrupting
combat forecasts.

**Mission 1:**

- Scout-7 unrecovered after Turn 2: Holt/Scout-7 sends a broken ping: "Crawler
  intact. I am less enthusiastic about the road."
- Enemy enters chokepoint cover: Rusk says, "They are testing the line. Make the
  test expensive."
- Scout-7 rescued: Venn says, "Lena, we have you. Now tell me what was worth
  jamming the relay."

**Mission 2:**

- Relay capture starts: Priya says, "Their service codes are live. I hate being
  right in sequence."
- Fuel cache threatened: Rusk says, "Do not chase the cache so hard we lose the
  relay. Evidence first."
- Engineer stabilizes relay: Priya says, "That was a field repair. No one tell
  the manufacturer what field."

**Mission 3:**

- First crawler moves onto the road: Rusk says, "Column moving. Armor forward,
  infantry wide, Lancers patient."
- Hunter Bike attacks support: Priya says, "That bike just made a maintenance
  enemy for life."
- First crawler reaches the station: Venn says, "One crawler in. Keep the pump
  alive long enough for the evidence to matter."

### Grounded Lore Field Summary

Each mission names the following concrete grounded-lore anchors that
should appear verbatim or by reference in briefings, map labels, banter,
and debriefs:

| Mission | Route / schedule | Manifest / permit | Spindle packet | Grid stakeholder | Asterite cost | Civilian risk |
| ------- | ---------------- | ----------------- | -------------- | ---------------- | ------------- | ------------- |
| 1 | Caldera hub, 4 months by priority courier | Eastern seam permit: non-destructive evaluation only | Jammed at 0547; sample hash sent after relay recovered | HQ sits on grid secondary access; parcel dispute in Orison filing | Seam confirmation would change extraction tier and insurance class | Camp heat, medical, food stores |
| 2 | Review response by Spindle in 3 days; enforcement on transit calendar | Joint-expedition fuel manifest, co-signed, no arbitration notice filed | Sample hash + manifest violation sent before relay window closes | Relay power from grid secondary tap; tap authority unresolved | Fuel loss grounds sample-transport crawlers; evidence stays unverified | Meridian eastern valley emergency beacon loses secondary anchor |
| 3 | TOB review track opens; six-week calendar; Orison window before enforcement | Civilian pump station, 60-year-old charter, 500m notice not filed | Filter-residue analysis + sample correlation queued | Station on grid southern distribution arm; Orison has surface rights but not utility easement | Undocumented Asterite path changes concession tier if confirmed | Meridian ridge settlements lose heat in 6 hours if station locked |

### Mechanics Status

Mechanics introduced in these three missions and their dependency on
current prototype features:

| Mission | New mechanic | Dependency |
| ------- | ------------ | ---------- |
| 1 | Rescue (friendly unit moves onto Scout-7 tile) | Already implemented in prototype |
| 2 | Property capture (2-turn adjacent action) | Needs capture rule implementation |
| 2 | Engineer Repair action (2 HP, adjacent, no attack) | Needs Engineer unit + action menu |
| 2 | Sapper bonus damage to support and property | Needs matchup modifier entry |
| 3 | Crawler escort units (move, no attack, escort-loss defeat) | Needs escort-unit type |
| 3 | AT Lancer matchup bonus vs Armor | Needs matchup modifier entry |
| 3 | Hunter Bike speed + support-hunter matchup bonus | Needs Striker unit class entry |

The rescue mechanic is already playable. Property capture and Engineer
actions are the Mission 2 blockers. Escort units and Lancer matchup entries
are the Mission 3 blockers. All three missions can be partially playable
with the existing prototype if capture, escort, and Lancer rules are
stubbed as placeholder objectives until full implementation.
