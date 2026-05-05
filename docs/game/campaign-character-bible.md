---
title: Campaign Character Bible
description: Character backgrounds, arcs, commander doctrine, voice, and candidate mechanics for the tactical combat campaign
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Design Purpose

Character identity should make missions feel authored without turning combat
units into persistent named party members. The campaign is still an
army-command tactics game: individual units are disposable battlefield pieces,
while commanders, staff, rivals, and faction leaders provide continuity,
motivation, humor, and tactical personality.

Use this document as a character design bible, not as an implementation lock.
Backgrounds, motivations, relationships, and voice patterns are campaign canon
unless later story work changes them. Commander powers, signature units, and
special mechanics are candidates that need forecast, UI, AI, replay, and
mission-load validation before implementation.

The compact implementation guardrails for those candidate powers are defined in
[CO Power Rule Budget](co-power-rule-budget.md).

Grounded history, political factions, Asterite limits, and the formal definition
of the Basin Stabilization Grid are defined in
[Universe Backstory](universe-backstory.md).

## Mechanical Maturity Labels

| Label | Meaning |
| --- | --- |
| Canon | Safe story, voice, or relationship direction for briefings, banter, and debriefs |
| Doctrine | Tactical preference that can shape missions, AI priorities, and player guidance without new rules |
| Candidate | Possible CO power, signature unit, or special rule that needs validation before implementation |
| Deferred | Later-campaign idea that should wait for supporting systems |
| Rejected | Idea that conflicts with readability, determinism, replay, or campaign constraints |

## Global Character Rules

* Keep character continuity in commanders, staff, rival COs, recurring bases,
  and named places, not persistent named combat units.
* Express personality first through mission framing, objective pressure, AI
  doctrine, map layout, briefing voice, power barks, and debriefs.
* Keep powers deterministic, inspectable, forecast-visible, and replayable.
* Make every power fit inside a compact controller-first inspect panel.
* Treat unique units as signature unit candidates with counters and fallbacks,
  not guaranteed unlocks.
* Give every major character a contradiction, so their tactical instinct can
  help in one mission and create pressure in another.
* Use catch phrases sparingly. Prefer voice patterns and recurring habits over
  repeated slogans.

## Core Cast Overview

| Character | Faction role | Battlefield verb | Arc pressure | Mechanical status |
| --- | --- | --- | --- | --- |
| Dr. Elara Venn | Kestrel expedition director | Stabilize | Humane scientist authorizing force | Doctrine now, power candidate later |
| Major Jonah Rusk | Kestrel security liaison | Hold | Doctrine must learn improvisation | Doctrine now, early power candidate |
| Chief Engineer Priya Nayar | Kestrel logistics lead | Repair | Fixes that keep escalation possible | Doctrine now, support candidate later |
| Lt. Sera Holt | Kestrel recon lead | Reveal | Rescue instinct versus operational risk | Doctrine now, fog candidate later |
| Director Cassian Sloane | Orison antagonist | Exploit | Law as weapon meets consequences | Enemy doctrine, power candidate |
| Colonel Amara Rhee | Sable rival and ally | Coordinate | Orders versus evidence | Enemy doctrine, ally candidate later |
| Major Lev Kravic | Sable hardliner | Escalate | Control mistaken for safety | Enemy doctrine, late antagonist candidate |
| Marshal Inez Calder | Meridian protector | Outmaneuver | Suspicion versus coalition trust | Doctrine now, ally candidate later |

## Dr. Elara Venn

### Venn Background

Elara Venn built her career on field geology that embarrassed cleaner stories.
She is the sort of scientist who trusts ugly measurements over elegant theory,
and that habit made her valuable to Kestrel's sponsors when Asterite residue
started appearing in places the official maps said were disconnected from any
active utility corridor. She accepted the Aster Basin assignment because it
looked like the rare chance to study a material before industry turned it into
doctrine.

She has directed dangerous field seasons before, but never a battlefield. Her
authority at the start of the campaign is logistical and scientific: site
safety, sample control, evacuation plans, and grant accountability. Mission 1
forces her to give orders that can destroy vehicles and kill people.

### Venn Personality And Motivation

Venn is calm, exacting, and dry under pressure. She dislikes theatrical
certainty and becomes stubborn when someone uses legality or rank to smother
evidence. Her motivation is to get her people home without turning the
expedition into the same armed extraction machine she came to study.

Her contradiction is that she opposes militarizing science while repeatedly
using scientific authority to make military decisions. She does not want
command, but she is hard to move once she accepts responsibility.

### Venn Campaign Arc

* Missions 1-5: Venn treats combat as an emergency evacuation problem and tries
  to preserve evidence, people, and options.
* Missions 6-10: She accepts that survival requires repeatable military
  infrastructure, then defeats Orison without adopting Sloane's ownership logic.
* Missions 11-25: She learns coalition command by arguing evidence across
  factions that do not trust one another.
* Missions 26-40: Prototype escalation tempts her to solve uncertainty with
  stronger tools. Kravic becomes a warning about that impulse.
* Missions 41-50: She chooses containment over possession and frames the final
  victory as a responsibility, not a conquest.

### Venn Interactions

* Rusk challenges her to make orders executable instead of merely correct.
* Priya keeps her honest about what the expedition can actually sustain.
* Holt pushes her hardest on rescue decisions when incomplete information makes
  withdrawal look rational.
* Sloane treats her ethics as naivete, which makes her more dangerous to him.
* Rhee respects her evidence and distrusts her lack of military chain of
  command.
* Calder tests whether Venn can protect civilians when victory points in
  another direction.

### Venn Voice And Sample Lines

Venn uses lab procedure, field notes, and hypothesis language in tactical
contexts. She is funniest when she is precise about absurd circumstances.

* "Let's test that assumption before it becomes policy."
* "This is not a retreat. It is a controlled relocation of our remaining good
  ideas."
* "Apparently we are editing the rules of engagement live."
* "The map is giving us confidence, which is not the same as truth."

### Venn Doctrine And Candidate Mechanics

Canon doctrine: Flexible objective play, terrain reads, evidence gathering,
and defensive stabilization. Venn should reward players who preserve options
instead of committing every unit to one damage race.

Candidate CO power, Field Hypothesis: For one player turn, friendly units on
cover or properties gain a small defense bonus, and objective or terrain
inspect panels expose all active modifiers. If support actions exist, the first
adjacent repair this turn restores +1 HP.

Candidate charge pattern: Objective progress, rescues, captures, and surviving
enemy attacks while on defensive terrain.

Signature unit candidate: Survey Analyst, a Field Tech variant with weaker
attack and stronger capture, inspect, or objective actions. Fallback if cut:
make this a mission tag for normal Field Techs in investigation missions.

Counterplay and risk: Venn powers can become passive safety blankets. Keep the
bonus small, temporary, and tied to position. Do not let her erase the need for
Armor, Lancers, or proper screens.

## Major Jonah Rusk

### Rusk Background

Jonah Rusk is a regular officer assigned to Kestrel as security liaison because
the project crossed enough strategic categories to worry several committees.
He expected access control, convoy drills, and contractor disputes. He did not
expect a civilian expedition director to become his commanding counterpart
during a live attack.

Rusk has seen what happens when clever people confuse improvisation with a plan.
He is not anti-science. He is anti-panic, anti-wishful thinking, and anti-order
sets that sound better in briefings than they behave under fire.

### Rusk Personality And Motivation

Rusk is disciplined, terse, protective, and more patient than his first
impression suggests. His motivation is to keep Kestrel alive long enough for
the expedition's better ideas to matter.

His contradiction is that doctrine is the tool he trusts most, but the campaign
keeps proving that Kestrel survives by changing doctrine faster than official
systems can approve.

### Rusk Campaign Arc

* Missions 1-6: Rusk teaches the expedition how to hold ground, screen support,
  and stop treating every problem as a lab accident.
* Missions 7-15: Meridian and Orison pressure force him to distinguish
  restraint from hesitation.
* Missions 16-25: Rhee becomes his professional mirror, showing what good
  doctrine looks like when attached to imperfect orders.
* Missions 26-40: Kravic tempts Rusk's control instincts, then proves that
  discipline without judgment becomes escalation.
* Missions 41-50: Rusk becomes the coalition's defensive backbone and trusts
  field improvisation without abandoning standards.

### Rusk Interactions

* Venn and Rusk argue in verbs: she asks what is true, he asks what is
  executable.
* Priya frustrates him by making broken machines tactically relevant before he
  has a manual for them.
* Holt forces him to protect scouts who are already beyond the line he would
  prefer to hold.
* Rhee earns his respect quickly, which makes her later disobedience of bad
  orders matter more.
* Kravic is what Rusk fears he might become if certainty outranks judgment.

### Rusk Voice And Sample Lines

Rusk uses clipped command language and dry operational summaries. His humor is
buried in restraint.

* "Hold what matters. Trade the rest deliberately."
* "That is not a plan. That is optimism with grid coordinates."
* "If you move the line, tell the line first."
* "I can work with improvised. I cannot work with surprised."

### Rusk Doctrine And Candidate Mechanics

Canon doctrine: Line holding, counterattacks, infantry screens, and deliberate
trades. Rusk is a good early player-facing commander because his instincts map
onto the first prototype's chokepoint lesson.

Candidate CO power, Lock The Line: Until the next player turn, friendly units
that did not move this turn gain +1 defense and a small counterattack bonus.
The effect should display as a shield icon and forecast delta.

Candidate charge pattern: Damage taken, successful counterattacks, and turns
where enemy units fail to capture or enter protected objective zones.

Signature unit candidate: Guard Armor, a slower Utility Armor variant that
gains defense when adjacent to infantry. Fallback if cut: add a Rusk mission
modifier that rewards adjacency with score or optional objective credit instead
of adding a unit.

Counterplay and risk: A defensive CO can slow the game if overpowered. The
power should reward planned holds, not turtling forever. Keep duration short
and avoid stacking with every terrain bonus.

## Chief Engineer Priya Nayar

### Nayar Background

Priya Nayar runs Kestrel logistics because nobody else can keep survey crawlers,
relay masts, pump systems, fuel manifests, and human optimism functioning in
the same week. She knows where every missing bolt should have been, who signed
for it, and whether the replacement can be made from a coffee grinder and two
unethical brackets.

Before the campaign, Priya's battlefield was procurement. Once Orison attacks,
she becomes the person who turns survey tools into repeatable military
infrastructure while remaining personally offended that this is necessary.

### Nayar Personality And Motivation

Priya is practical, funny, impatient with waste, and ferociously protective of
systems that keep people alive. She sees war as a catastrophic maintenance
failure with uniforms.

Her contradiction is that every brilliant fix helps Kestrel survive and also
makes escalation easier. She hates that her competence keeps expanding the
possible war.

### Nayar Campaign Arc

* Missions 1-5: Priya keeps the camp alive with field repairs, inventory
  tricks, and equipment that was not built for combat.
* Missions 6-15: She turns emergency fabrication and depots into infrastructure
  while worrying that Kestrel now has a war economy.
* Missions 16-30: Prototype escalation makes her the expert everyone needs and
  the person most aware of what the machines cost.
* Missions 31-45: The Basin Stabilization Grid reframes her work from repair to
  public-safety operation. Her diagrams become the coalition's survival plans.
* Missions 46-50: She insists final victory protect coolant, heat, medical
  power, and settlement systems, not only military objectives.

### Nayar Interactions

* Venn relies on Priya to convert principles into usable resources.
* Rusk and Priya bicker over safety margins, then repeatedly prove each other
  right.
* Holt treats Priya's repairs as rescue promises, which Priya finds both moving
  and operationally unreasonable.
* Sloane is everything Priya hates about accounting for damage without caring
  what broke.
* Calder and Priya bond over infrastructure as lived reality, not map dressing.

### Nayar Voice And Sample Lines

Priya converts battlefield chaos into maintenance, inventory, and procurement
language. She is the main source of grounded bureaucratic comedy on the player
side.

* "I can fix it, but I am absolutely putting it in the report."
* "Ammunition is now a rapidly depreciating research consumable."
* "That bridge did not fail. It resigned under hostile management."
* "If anyone asks, this was already a field modification."

### Nayar Doctrine And Candidate Mechanics

Canon doctrine: Repair, resupply, rotating damaged units, escorting support,
and protecting mission infrastructure. Priya teaches that logistics is a
tactical system, not flavor text.

Candidate CO power, Expedited Maintenance: For one player turn, Engineers and
Field Rigs repair +1 HP, and the first repaired vehicle can still move if it
has not acted. If action restoration is too complex, replace it with a fixed
+1 HP repair bonus only.

Candidate charge pattern: Repairs performed, structures stabilized, convoy or
support units preserved, and turns completed with no vehicle losses.

Signature unit candidate: Patch Rig, a fragile Field Rig variant that repairs
vehicles and stabilizes mission objects. Fallback if cut: give standard Field
Rigs a mission-specific stabilize command in Priya-focused missions.

Counterplay and risk: Repair powers can create stalemates or undo meaningful
damage. Keep repair values small, make support units vulnerable, and ensure
Sappers, Strikers, or objective clocks pressure the sustain plan.

## Lt. Sera Holt

### Holt Background

Sera Holt leads Scout-7, the recon team that goes missing before Mission 1.
She knows the basin by tire marks, dead relay zones, and the places where the
official map becomes suspiciously polite. Her rescue gives the campaign its
first personal promise: Kestrel does not leave people in the fog because the
map stopped making sense.

Holt was trained to gather incomplete information and live with the discomfort.
That skill becomes central once old grid corridors, Sable jamming, and Orison
power siphons start distorting sensors, maps, and everyone else's confidence.

### Holt Personality And Motivation

Holt is observant, restless, sardonic, and brave in the way people get when
they have already been afraid and survived. Her motivation is to make sure no
one dies because command waited for perfect visibility.

Her contradiction is that rescue instinct saves people, but it can also push
operations into danger. Holt must learn when revealing the map creates
responsibility beyond moving faster than everyone else.

### Holt Campaign Arc

* Missions 1-5: Holt moves from rescued objective to recon voice, teaching that
  information is part of survival.
* Missions 6-15: She becomes the player's guide to roads, flanks, and enemy
  intent beyond the current screen.
* Missions 16-25: Fog and civilian terrain force her to balance scouting with
  protection.
* Missions 26-40: Prototype power draws and grid-control signals make her doubt
  every clean sensor answer, sharpening her judgment.
* Missions 41-50: She leads the final recon route into the old grid control
  district and brings the campaign back to the promise of Mission 1.

### Holt Interactions

* Venn trusts Holt's field observations even when they are not yet evidence.
* Rusk worries Holt outruns support, and Holt worries Rusk calls caution a plan.
* Priya is Holt's favorite person after every ugly extraction.
* Calder recognizes Holt as someone who reads lived terrain instead of only
  military maps.
* Rhee respects Holt's discipline once Holt stops treating all formal doctrine
  as slow.

### Holt Voice And Sample Lines

Holt uses sensor, map, and field-route language with nervous humor. She often
names what the map is hiding.

* "The fog is lying. Badly."
* "I found the road. It is making poor choices."
* "Scout-7 is not late. Scout-7 is conducting aggressive punctuality research."
* "If the map says empty, assume it has a hobby."

### Holt Doctrine And Candidate Mechanics

Canon doctrine: Recon, soft fog, flank routes, baiting, marks, and extraction.
Holt should make scouts and sensor objectives feel valuable without turning fog
into unfair surprise.

Candidate CO power, Clean Signal: For one player turn, friendly Scouts,
properties, or sensor posts reveal a small radius and mark visible enemies.
Marked enemies lose concealment bonuses and show forecast deltas clearly.

Candidate charge pattern: Tiles revealed, enemies marked, scout survival,
rescues completed, and sensor posts captured.

Signature unit candidate: Signal Scout, a Survey Scout variant that can mark or
reveal instead of attacking. Fallback if cut: add a temporary mark command to
standard Scouts in Holt-led fog missions.

Counterplay and risk: Information powers can trivialize fog. Keep reveal radii
short, duration fixed, and enemy counterplay visible through jammers, screens,
or objective clocks rather than hidden immunity.

## Director Cassian Sloane

### Sloane Background

Cassian Sloane is Orison Resource Combine's basin director, though he prefers
titles that sound less temporary. He arrived with extraction licenses, private
security, compliance decks, and a talent for making violence appear as a
contractual misunderstanding.

Sloane understands Asterite's value before most people understand its danger.
He is not a battlefield genius, but he knows how to turn production, property,
and paperwork into pressure until opponents mistake delay for defeat.

### Sloane Personality And Motivation

Sloane is polished, charming, contemptuous, and funny because he says monstrous
things in the cadence of a sponsor call. His motivation is to convert Asterite
into monopoly power before governments, locals, or scientists can slow the
claim.

His contradiction is that he treats legality as reality until reality stops
honoring paperwork. The Basin Stabilization Grid is his final humiliation
because public infrastructure cannot be fully owned, invoiced, or flattered
once its history becomes visible.

### Sloane Campaign Arc

* Missions 1-6: Sloane appears through Orison pressure, stolen schedules, and
  armored claim teams before the player fights his local plan directly.
* Missions 7-10: He becomes the first major antagonist defeat when Kestrel
  captures the refinery and burns his local legal strategy.
* Missions 11-30: Orison remnants and Sloane's mobile refinery keep him
  dangerous as a resource opportunist.
* Missions 31-40: Sloane tries to exploit the grid by treating a public-safety
  system like a concession asset.
* Missions 41-50: He becomes either a contained liability or a bitter source of
  useful Orison data, depending on how much presence later arcs need.

### Sloane Interactions

* Venn is his philosophical enemy because she refuses to confuse access with
  ownership.
* Priya loathes him because his cost models erase repair, injury, and civic
  damage.
* Calder needles him with the directness of someone who has paid his invoices
  in real life.
* Rhee distrusts him professionally, even before their objectives conflict.
* Kravic finds Sloane unserious, which makes their temporary alignments brittle.

### Sloane Voice And Sample Lines

Sloane speaks in corporate legal language, asset framing, and polished threats.
He should sound plausible enough to be dangerous.

* "Ownership is courage with paperwork."
* "This is not an attack. It is accelerated dispute resolution."
* "The basin has stakeholders. Some of them are better organized."
* "I admire your principles. They are expensive, but very photogenic."

### Sloane Doctrine And Candidate Mechanics

Canon doctrine: Economy pressure, property grabs, production tempo, contractor
expendability, and armor-backed claims. Sloane should force players to fight
the map economy, not only his units.

Candidate enemy power, Emergency Appropriation: Once telegraphed, Orison can
discount one production batch or repair units on owned properties by a fixed
amount. The map must show which properties are affected before activation.

Candidate charge pattern: Properties held, income gained, units produced, and
damage dealt near extraction nodes.

Signature unit candidate: Claim Armor, a Line Armor variant that gains a small
benefit while near Orison-owned properties or extraction nodes. Fallback if
cut: use normal Line Armor with property-centered AI priorities.

Counterplay and risk: Free money feels like cheating if not exposed. Sloane's
advantages must be visible on the map and answerable through captures,
blocking, or raid pressure.

## Colonel Amara Rhee

### Rhee Background

Amara Rhee commands the Sable Accord expeditionary force sent into the basin
under treaty authority. She is not a villain pretending to be professional. She
is professional, which makes her early opposition more dangerous. Her units are
disciplined, her claims are documented, and her opening assumptions are wrong
in ways she is prepared to investigate.

Rhee has built her career on preventing strategic surprises from becoming wars.
Asterite looks like exactly that kind of surprise, and Kestrel initially looks
like an uncontrolled actor near the fuse.

### Rhee Personality And Motivation

Rhee is formal, controlled, perceptive, and quietly willing to change her mind
when evidence survives pressure. Her motivation is to prevent the basin from
becoming a strategic imbalance that drags states into open conflict.

Her contradiction is that she values lawful order, but the campaign forces her
to decide when lawful orders are built on false maps.

### Rhee Campaign Arc

* Missions 5-10: Rhee appears as a state-backed rival with better maps and a
  clean claim to authority.
* Missions 16-20: She becomes the main opponent across the Fog Line, then
  respects Kestrel when the evidence stops fitting her orders.
* Missions 21-30: Rhee tests limited cooperation while managing Sable politics
  and Kravic's hardline pressure.
* Missions 31-35: She commits Sable units to coalition defense when grid
  failures threaten civilians and military forces alike.
* Missions 36-50: Rhee breaks with Kravic and helps build the final coalition
  plan without pretending trust is simple.

### Rhee Interactions

* Venn and Rhee clash over whether evidence or authority moves first.
* Rusk and Rhee speak the same tactical language, which makes their
  disagreements precise.
* Holt gradually earns Rhee's respect by turning incomplete information into
  disciplined action.
* Kravic is her internal antagonist: capable, loyal to control, and dangerous.
* Calder distrusts Rhee until Rhee proves civilians are not merely terrain.

### Rhee Voice And Sample Lines

Rhee uses formal command language, doctrine, and carefully chosen concessions.
She should rarely joke, which makes her dry moments land harder.

* "Discipline is what remains when the map changes."
* "Your evidence is inconvenient. Continue."
* "I will not mistake urgency for authority."
* "We can be wrong without becoming careless."

### Rhee Doctrine And Candidate Mechanics

Canon doctrine: Disciplined captures, layered defense, artillery lanes, recon
screens, and coordinated advances. Rhee missions should feel fair but exacting.

Candidate enemy or ally power, Measured Advance: For one turn, Sable infantry
capture more efficiently, and protected support units gain a setup benefit if
adjacent screens are intact. If artillery is not implemented, use capture and
defense benefits only.

Candidate charge pattern: Captures completed, support units protected, turns
where formations remain intact, and enemy advances blocked without losses.

Signature unit candidate: Accord Spotter, an infantry or recon hybrid that
improves attacks against marked visible targets. Fallback if cut: use Survey
Scout or Field Tech equivalents with mission-specific marked-target rules.

Counterplay and risk: Rhee should not feel like Sable gets to be better at
everything. Her doctrine should be strong when formations hold and vulnerable
when players break screens or disrupt capture timing.

## Major Lev Kravic

### Kravic Background

Lev Kravic is Rhee's hardline counterpart inside the Sable force. He is
capable, brave, and far more frightening because his worst decisions come from
a sincere belief that delay kills. He reads ambiguity as hostile action and
treats control as the only humane answer to strategic uncertainty.

Kravic does not begin as a rogue caricature. He begins as the officer many
institutions reward during crisis: decisive, severe, and willing to carry blame
if history calls him correct.

### Kravic Personality And Motivation

Kravic is intense, brilliant, brittle, and contemptuous of hesitation. His
motivation is to control Asterite, then grid authority, before anyone less
disciplined misuses it.

His contradiction is that he wants safety badly enough to endanger everyone. He
becomes the campaign's clearest human warning that control is not the same as
containment.

### Kravic Campaign Arc

* Missions 16-20: Kravic pressures Rhee to destroy evidence rather than let
  Kestrel reinterpret the conflict.
* Missions 21-30: He escalates around prototype competition and sees coalition
  restraint as weakness.
* Missions 36-40: Kravic becomes the main antagonist by trying to seize grid
  authority uplinks before anyone can stop him.
* Missions 41-50: His defeat leaves the coalition with the harder job of
  containing what he wanted to command.

### Kravic Interactions

* Rhee is his ideological and professional counterweight.
* Rusk recognizes Kravic's competence and rejects what it costs.
* Venn frustrates him because her uncertainty is disciplined rather than weak.
* Holt sees his plans fail when fog punishes assumed control.
* Sloane irritates him because economic opportunism looks undisciplined even
  when their goals temporarily align.

### Kravic Voice And Sample Lines

Kravic speaks in control language, moral urgency, and clipped judgments. He
should sound persuasive enough that his danger is uncomfortable.

* "Control is mercy delivered early."
* "Delay is a decision. Own it."
* "Uncertainty does not absolve command."
* "If the map changes, secure the hand that draws it."

### Kravic Doctrine And Candidate Mechanics

Canon doctrine: Forced tempo, artillery pressure, zone denial, and punishing
hesitation. Kravic should telegraph danger, then make staying still costly.

Candidate enemy power, Fire Authorization: At the end of Kravic's turn, a
visible zone is marked for next-turn strike or suppression. Units still inside
the zone at resolution suffer fixed damage or a fixed combat penalty. The
warning must be clear before the player responds.

Candidate charge pattern: Damage dealt, objectives threatened, zones held, and
turns where the opponent fails to move out of pressure.

Signature unit candidate: Siege Coordinator, a support unit that improves
artillery or strike-zone effects while protected. Fallback if cut: use normal
support units and scripted warning zones in Kravic missions.

Counterplay and risk: Kravic's kit can feel punitive if warning zones are hard
to read or if the player lacks movement options. Every strike must have visible
counterplay through movement, blocking, disruption, or sacrificing a known
trade.

## Marshal Inez Calder

### Calder Background

Inez Calder leads the Free Meridian Compact, a practical alliance of settlers,
haulers, mechanics, prospectors, medics, and local defenders who have been
described by outside authorities as whatever term best justifies ignoring them.
She knows which Asterite taps heat homes, which roads wash out, and which
promises arrive in uniforms before leaving in invoices.

Calder is not anti-outsider by ideology. She is anti-abandonment by experience.
Kestrel earns her attention only when it proves the drill site, settlement grid,
and civilian power nodes matter as much as tactical victory.

### Calder Personality And Motivation

Calder is warm with civilians, sharp with outsiders, witty, suspicious, and
absolutely unwilling to let strategic language erase people. Her motivation is
to keep Meridian communities from becoming collateral under cleaner terms.

Her contradiction is that suspicion protects her people early, but it can also
delay the coalition trust needed to save them later.

### Calder Campaign Arc

* Missions 7-10: Calder appears as a hostile local protector who treats
  Kestrel as another extraction force until proven otherwise.
* Missions 21-25: Meridian becomes central to the campaign's civilian stakes,
  and Calder joins a loose coalition.
* Missions 26-35: She forces prototype and grid-control plans to account for
  convoys, heat taps, and settlements.
* Missions 36-50: Calder becomes the coalition conscience and the commander who
  keeps final victory tied to what happens after the map is won.

### Calder Interactions

* Venn earns Calder's respect through restraint and evidence, not speeches.
* Priya and Calder quickly recognize each other as infrastructure people.
* Holt understands Calder's routes and ambush logic before the formal command
  staff does.
* Sloane is a personal kind of enemy because Calder has seen how companies make
  local damage sound temporary.
* Rhee must prove treaty authority can protect civilians instead of only
  controlling territory.

### Calder Voice And Sample Lines

Calder uses local knowledge, road language, and sharp reframing of official
terms. Her humor should feel lived-in rather than performative.

* "You call it irregular because you never had to live here."
* "Tourists with artillery and better stationery."
* "That empty land has a grandmother, two heat taps, and a bad well."
* "If your plan cannot find the clinic, your plan is lost."

### Calder Doctrine And Candidate Mechanics

Canon doctrine: Mobility, ambush, convoy protection, sabotage, backroads, light
units, and terrain tricks. Calder should make the player care about routes that
conventional maps undervalue.

Candidate CO power, Backroad Network: For one player turn, light units gain
reduced movement cost through one readable terrain family and can ignore one
soft zone-of-control rule if such a rule exists. If zones of control are not
implemented, use movement cost only.

Candidate charge pattern: Convoy progress, ambush attacks from cover, civilian
objectives preserved, and light units surviving near enemy pressure.

Signature unit candidate: Hauler Striker, a fast light unit that can carry
mission cargo or supplies but loses direct fights. Fallback if cut: use normal
Strikers with convoy marker objectives in Calder missions.

Counterplay and risk: Mobility powers can break map design. Limit affected
unit tags, terrain types, and duration. Avoid letting Calder skip the tactical
question a map was built to ask.

## Treaty Oversight Bureau

### Bureau Narrative Role

The Treaty Oversight Bureau is sponsor, regulator, procedural antagonist, and
comedy engine. It is not a battlefield faction at first. Its pressure should
arrive through audits, sealed files, authorization delays, scoring constraints,
objective complications, and debrief consequences.

### Representative Character Candidate

Auditor Mara Quill can serve as the recurring Bureau face if the campaign needs
one named representative. She is not stupid or cowardly. She is very good at a
job whose forms were not designed for armored raids, sealed grid districts, or
field fabricators being reclassified during live fire.

Voice examples:

* "Does the battle have a sign-in sheet?"
* "I am required to ask whether the bridge was like that before."
* "Your emergency authority is legible, which is not the same as comforting."

Gameplay use:

* Add optional objectives that reward restraint, evidence preservation, or
  infrastructure protection.
* Gate sealed information through mission outcomes rather than long exposition.
* Influence score categories or briefing pressure without adding a Bureau unit
  roster.

Risk: Bureau comedy can undercut stakes if it appears during losses. Keep the
humor pointed at systems, not casualties.

## Basin Stabilization Grid

### Grid Narrative Role

The Basin Stabilization Grid is not a person. The Loom is only a field nickname
for its interlaced power, pump, road-lock, depot, and emergency isolation
routes. It should read as human-built infrastructure with old authority rules,
not as a mysterious character or ancient machine.

### Operational Personality

The grid does not hate anyone. It verifies authority, routes power, isolates
faults, locks depots, closes gates, and applies emergency safety rules written
after the Lattice Incident. It fails dangerously because its records are
fragmented and the basin has changed around it, not because it has intentions.

Voice examples for alerts and objective text:

* "Authority conflict detected. Holding last valid route."
* "Unregistered extraction draw exceeds safety threshold."
* "Substation isolation pending. Manual override required."
* "Civilian load detected on restricted line. Confirm before shutdown."

Gameplay use:

* Start with existing mechanics wearing grid-control presentation: maintenance
  drones, depot lockouts, substation shutdowns, route objectives, and barrier
  landmarks.
* Add automated production or route locking only after replay and UI can show
  the state changes clearly.
* Make grid pressure predictable enough for planning, even when the politics are
  ugly.

Risk: Infrastructure systems can become a hidden rules engine. Every grid
lockout, route change, production change, or drone activation needs a visible
warning, deterministic timing, and replay data.

## Relationship Matrix

| Pair | Tension | Campaign use |
| --- | --- | --- |
| Venn and Rusk | Evidence versus executable doctrine | Turns early survival choices into command growth |
| Venn and Priya | Principles versus resource reality | Grounds big decisions in what the camp can sustain |
| Venn and Holt | Rescue ethics versus operational risk | Keeps Mission 1's promise alive through the finale |
| Venn and Sloane | Stewardship versus ownership | Defines the first antagonist conflict |
| Venn and Rhee | Evidence versus authority | Converts rivalry into coalition command |
| Rusk and Rhee | Shared discipline, different orders | Makes Sable a professional rival rather than a cartoon enemy |
| Rusk and Kravic | Doctrine versus control | Pressures Rusk's arc in the hardliner offensive |
| Priya and Calder | Infrastructure as lived stakes | Makes civilian systems tactically meaningful |
| Holt and Calder | Field routes versus official maps | Supports ambush, convoy, and scout mission identity |
| Rhee and Kravic | Professional duty versus escalation | Drives the Sable split without making Rhee naive |
| Sloane and Calder | Asset claims versus local survival | Keeps Orison's harm concrete |

## Power And Unit Validation Checklist

Use this checklist before moving any candidate mechanic into implementation:

* The effect fits in one compact inspect panel with duration and affected tags.
* Forecasts show the numeric before-and-after difference before commitment.
* Replay can derive the outcome from command data, current state, seed, and
  rules version.
* The AI uses the same public rule or receives a clearly telegraphed scenario
  rule.
* The effect has counterplay through movement, capture, disruption, screening,
  terrain, objective timing, or resource denial.
* The mechanic does not compete with the mission's main lesson, especially in
  Missions 1-6.
* A no-new-unit fallback exists if the signature unit adds too much UI, AI, or
  replay burden.

## First Implementation Candidates

The safest first playable commander slice is not the full cast. Prototype a
small set only after Missions 1-3 briefs and unit profile metadata are stable.

Recommended first candidates:

1. Rusk, because Lock The Line reinforces the first prototype's chokepoint and
   counterattack lessons.
2. Venn, because Field Hypothesis can teach objective and terrain clarity
   without requiring new unit families.
3. Holt, once soft fog or sensor posts exist.
4. Sloane, once capture economy and production pressure exist.

Priya should wait until Engineer or Field Rig support actions exist. Rhee
should wait until captures, sensors, and layered defense are validated. Calder
should wait until convoy or light-unit mobility missions exist. Kravic should
wait until telegraphed zones, support fire, or artillery-style pressure can be
shown fairly.

## Rejected Early Shortcuts

* Do not give commanders hidden passive bonuses that only appear after damage
  resolves.
* Do not introduce character-specific persistent named combat units.
* Do not make every principal cast member playable before enemy doctrine and
  mission framing have done cheaper narrative work.
* Do not use catch phrases as substitute characterization.
* Do not let the grid change terrain, production, or routes without visible
  warnings and deterministic replay representation.
