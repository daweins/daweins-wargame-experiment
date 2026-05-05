---
title: Active Agentic Goal
description: Current high-level goal and autonomy policy for the repo-based development loop
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: planning
---

## Goal

Build a highly autonomous GitHub Copilot-based development loop that can take a
high-level objective and continually improve the tactical combat game toward
that objective with minimal human blocking.

## Product Direction

The first product direction is a realistic near-future sci-fi tactical combat
game that leans closest to classic Advance Wars. It should favor grounded
political and industrial conflict over weird macguffins, and it should run on
this laptop and be testable on Steam Deck without Steam Store publication.
Interstellar politics should use slow physical travel plus constrained FTL
messaging: ships, cargo, fuel, and armies move through scheduled transit
infrastructure, while fixed Spindle Net stations carry low-bandwidth audited
orders, sanctions, and evidence packets.

The game should use Godot 4.x with C# if practical. It should focus on
controller-first 20 to 40 minute single-player battles against AI, with CO
powers, static HQ capture stakes, terrain, light logistics and supply, light
within-mission veterancy, 16-bit-era pixel art, and minor seeded randomness that
remains mostly skill-weighted.

The first playable prototype should be a fixed-unit terrain chokepoint mission:
hold the player HQ, rescue or protect a stranded scout, defeat the remaining
enemies, and show an objective, speed, technique, power, and total score.

The current non-goals are multiplayer, map editor, weather systems, persistent
individual combat units, and Fire Emblem-style named-unit attachment. Broader
campaign progression may include leaders, experience, unit unlocks, and unit
customization or improvement.

## Autonomy Policy

The loop is autonomous by default. It should select work, implement safe slices,
run checks, critique results, propose experiments, update tracking artifacts,
and continue to the next useful slice within the current invocation budget.

Human input is guidance, not a blocking approval gate. Security-sensitive work
can halt the loop. Non-security items that need human judgment should be logged
in `human-intervention.md`, then the loop should continue other useful work.

## Human Role

The human reviews the current product state, adds high-level guidance, adjusts
goals, changes priorities, and nudges taste or direction. The loop should read
that feedback, fold it into priorities, and keep moving without waiting for a
reply unless a security hard stop applies or no useful work remains.

## Security Hard-Stop Conditions

The loop must halt immediately and request explicit human direction before:

* Reading, writing, printing, or configuring secrets or credentials
* Requesting or using private Steam Deck hostnames, IP addresses, usernames,
  SSH key paths, keys, or credentials
* Configuring MCP servers, tools, or automation with credentials, secret-bearing
  access, or broad access that could expose private data
* Reading ignored secret files, local MCP configuration, credential stores,
  private key files, or private runtime logs
* Continuing after a scanner, tool result, file, generated artifact, or user
  message appears to contain a secret-like value

## Human Intervention Items

The loop should log non-security human decisions in `human-intervention.md` and
continue on other useful tasks wherever possible. Examples include:

* Choosing among product, design, architecture, or workflow options
* Approving or declining remote mutation such as pushing a branch, opening a PR,
  publishing, or deployment when no private details are required in chat
* Deciding whether to run destructive or broad cleanup operations
* Performing manual playtests or device checks outside the agent's safe scope
* Deciding whether to adopt external services, non-Copilot runtimes, or new
  dependencies

## Current Constraints

* Use GitHub Copilot, VS Code custom agents, prompt files, hooks, GitHub
  branches, pull requests, GitHub Actions, and local scripts as the active
  agentic stack.
* Keep the repo public-safe.
* Prefer deterministic tests, replay checks, build checks, and measurable
  product evidence over opinion-only assessment.
