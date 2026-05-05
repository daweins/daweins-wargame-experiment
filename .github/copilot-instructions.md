---
title: Repository Copilot Instructions
description: Always-on instructions for GitHub Copilot in this repository
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: reference
---

## Core Direction

Use GitHub Copilot, VS Code custom agents, prompt files, hooks, GitHub Actions,
GitHub issues, branches, pull requests, and local scripts as the default
agentic programming stack. Do not introduce external coding-agent services,
third-party model-provider APIs, paid agent runtimes, or provider-specific auth
tokens unless the user explicitly asks for them.

## Secret Safety

Assume this repository may become public.

Never ask the user to paste credentials, auth tokens, API keys, personal access
tokens, SSH private keys, private hostnames, Steam Deck credentials, cloud
credentials, connection strings, or secrets into chat, source files, markdown,
logs, screenshots, prompts, generated artifacts, commits, issues, or pull
requests.

Never generate placeholder text that looks like a real token. Use names such as
`YOUR_TOKEN_HERE` only in documentation examples, and prefer references to
GitHub Secrets, GitHub environment secrets, local `.env.local` files, or OS
credential stores.

Do not read `.env`, `.env.*`, private key files, credential stores, local MCP
configuration, or ignored runtime logs unless the user explicitly asks and the
task cannot be completed safely without that file. If a file may contain
sensitive data, summarize the needed schema without revealing values.

If a tool result, terminal output, file, or user message appears to contain a
secret, stop propagating the value. Tell the user to rotate the credential and
remove it from history or logs as appropriate.

## Copilot Cloud Agent Constraints

GitHub Copilot cloud agent runs in an ephemeral GitHub Actions-powered
environment and can research, plan, edit, run tests, and open pull requests.
When MCP servers are configured for the cloud agent, it can use allowed MCP
tools autonomously. Configure only the narrowest read-only tools needed unless
the user explicitly approves broader access.

Content exclusions are not a reliable safety boundary for Copilot cloud agent.
Do not commit sensitive files expecting Copilot to ignore them.

## Agentic Development Rules

Use `.copilot-tracking/agentic/` as the repo-based work tracking system for
long-running autonomous work. Each meaningful agentic pass should record the
objective, current state, backlog changes, delegated work, development log,
verification evidence, adversarial critique, experiment updates, decisions,
metrics, risks, and next actions.

Operate autonomously by default when work is safe. Treat human input as
non-blocking guidance for goals, priorities, product judgment, and nudging.
Stop immediately only for security-sensitive conditions: credentials, private
device details, MCP credentials or broad sensitive-data access, ignored secret
files, credential stores, private key files, or suspected secret exposure in
files, prompts, logs, screenshots, tool results, or generated artifacts.
For non-security items that need human judgment or action, create or update an
entry in `.copilot-tracking/agentic/human-intervention.md`, skip the blocked
item, and continue other useful safe work wherever possible.

Split work into ingest, prioritize, implementation, verification, adversarial
critique, experiment planning, security review, human-intervention routing,
recording, and continuation.
Planning and review agents should use the smallest tool set possible.
Implementation agents may edit and execute commands, but they must respect the
secret-safety rules above.

Prefer deterministic checks over opinion-only review. Run tests, linters,
formatters, replay checks, build checks, and targeted simulations when they
exist. If a check cannot be run, record why.

Run periodic code review as part of autonomous work. Reviews are triggered by
meaningful code slices, three to five completed autonomous slices, changes to
core rules or architecture boundaries, dependency or security changes,
generated artifacts, failed checks, pre-commit work, and pull request prep.
Classify suggestions before fixing them. Autofix accepted bounded, local,
public-safe, verifiable suggestions immediately; require independent acceptance
for semantic gameplay, replay, save schema, dependency, security, or
architecture changes. Do not create cosmetic churn from a no-issue review, and
do not apply speculative cleanup unless it is accepted and tied to a concrete
bug, risk, or simplification. Follow the project constitution in
`docs/game/code-quality-architecture-constitution.md`.

For the tactical combat game, favor deterministic simulation, command logs,
seeded random number generation, controller-first UX, Steam Deck 1280x800
validation, and fast laptop-to-Deck iteration.
