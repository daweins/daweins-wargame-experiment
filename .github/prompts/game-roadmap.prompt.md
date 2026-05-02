---
description: "Turn the high-level tactical combat game idea into milestones, playable slices, and acceptance criteria"
agent: Product Strategist
tools: [read, search, edit, todo]
argument-hint: "[horizon=prototype|vertical-slice|alpha] [constraints=...]"
---

# Game Roadmap

## Inputs

* ${input:horizon:prototype}: Optional roadmap horizon.
* ${input:constraints:Steam Deck and laptop playable}: Optional product or
  technical constraints.

## Requirements

1. Read the game technical direction and current ecosystem blueprint.
2. Produce a milestone roadmap for the requested horizon.
3. Break each milestone into playable, agent-sized slices.
4. Include acceptance criteria and verification ideas for each slice.
5. Keep controller-first, 1280x800, deterministic simulation, replay, and
   Steam Deck iteration in scope.
6. Do not request or store private Deck details or credentials.

Create or update a public-safe roadmap document if the user asks for a saved
artifact; otherwise return the roadmap in chat.
