---
title: Autonomous Coding Agents State of the Art
description: Public research notes on highly autonomous coding agents and agentic programming products as of 2026-05-02
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: research
---

## Research Questions

* What are the public capabilities and workflow models of leading autonomous coding agents as of 2026-05-02?
* How does each system handle planning, execution, long-running tasks, tests, PRs, and human approval?
* What tradeoffs emerge between local and cloud execution models?
* What lessons apply to designing a repo-local multi-agent programming ecosystem in VS Code?
* What direct public source URLs support the findings?

## Scope

Primary systems: GitHub Copilot coding agent/workspaces, OpenAI Codex/cloud coding agent, Anthropic Claude Code, Google Jules, Devin/Cognition, Cursor Background Agents, Windsurf/Codeium Cascade, Replit Agent, Aider, OpenHands, SWE-agent, and notable adjacent systems.

## Findings

### High-level synthesis

As of 2026-05-02, highly autonomous coding agents have converged on a common
workflow: accept a bounded task, inspect repository context, form a plan,
execute edits in an isolated workspace, run tests or other verification, expose
logs/diffs/artifacts, and hand control back to a human through a PR, task board,
or local diff review. The main product differences are where the workspace
runs, how much autonomy is allowed by default, how persistent memory and team
instructions are modeled, and how approvals are enforced.

For this repository, the implementation decision is narrower than the research
space: use GitHub Copilot, VS Code custom agents, prompt files, hooks, GitHub
branches, pull requests, GitHub Actions, and local scripts as the active stack.
Other systems are research references, not dependencies. Do not design flows
that require external model-provider tokens, external coding-agent services, or
public exposure of credentials, auth tokens, private hostnames, Steam Deck
details, local MCP secrets, or other sensitive data.

### Product observations

* GitHub Copilot cloud agent runs in GitHub Actions-powered ephemeral
  environments. It researches, plans, edits, runs tests and linters, pushes
  commits to agent branches, opens or updates one PR per task, shows session
  logs, supports custom agents, MCP, hooks, skills, repository instructions, and
  Copilot Memory. Copilot Workspace was an earlier agentic dev environment and
  its technical preview ended 2025-05-30.
* OpenAI Codex spans CLI, IDE extension, app, and cloud. Cloud tasks run in
  isolated containers, can run in parallel, use setup scripts and cached
  environments, run terminal/edit/test loops, produce diffs, support PRs and
  GitHub code review, and rely on AGENTS.md, skills, hooks, MCP, subagents,
  sessions, worktrees, automations, and approval/sandbox controls.
* Claude Code is local-first across terminal, VS Code, JetBrains, desktop, and
  web. It edits files, runs commands, creates commits and PRs, supports plan
  mode, resumable sessions, worktrees, subagents, schedules/routines,
  non-interactive CI usage, GitHub Actions, MCP, skills/plugins, hooks,
  permissions, managed settings, and optional sandboxing.
* Google Jules is asynchronous and cloud-native. It clones GitHub repositories
  into Google Cloud VMs, shows a plan and reasoning, supports user steering
  before/during/after execution, runs tests or creates them, produces diffs and
  PRs, supports GitHub issue labels, CLI/API, and high concurrency tiers.
* Devin is a commercial autonomous software engineer for teams. It provides a
  conversational workspace with shell, browser, and embedded IDE, real-time
  progress, take-over controls, issue/ticket/Slack/Linear workflows, PRs, CI
  feedback loops, multi-repo and multi-agent project execution, automations, and
  enterprise-oriented knowledge and customization.
* Cursor Cloud Agents, formerly Background Agents, run in isolated cloud VMs,
  can run many agents in parallel, clone GitHub/GitLab repos to separate
  branches, push PRs, use MCP, hooks, desktop/browser computer use, screenshots,
  videos, logs, automatic CI-failure fixing, network controls, signed commits,
  secrets controls, and team follow-ups.
* Windsurf Cascade is IDE-local and interactive, with Code and Chat modes,
  continuously refined plans/todos, queued messages, checkpoints/reverts,
  problem-panel and linter integration, terminal/tool/MCP/web-search usage,
  memories, simultaneous Cascades, and worktree guidance for parallelism.
* Replit Agent targets end-to-end app creation in Replit's cloud workspace. It
  plans, builds, sets up infrastructure, tests, deploys, supports Plan/Build
  modes, Lite/Economy/Power modes, task boards, background tasks, isolated task
  copies, review/apply/dismiss gates, live previews, work logs, test results,
  conflict handling, connected services, and multi-artifact output.
* Aider is an open-source terminal pair programmer for local git repos. It uses
  repo maps, code/ask/architect/help modes, model-agnostic editing, automatic
  commits, lint/test loops, /run and /test feedback, watch mode for IDE use,
  configuration files, and local human approval through the terminal workflow.
* OpenHands is an open-source plus hosted/self-hostable agent platform. It
  offers SDK, CLI, local GUI, cloud, and enterprise deployments; Docker/process/
  remote sandboxing; GitHub/GitLab/Bitbucket/Slack/Jira/Linear integrations;
  issue-label and @mention PR flows; GitHub Action resolver; automations;
  skills, MCP, custom agents, hooks, persistence, pause/resume, and security
  confirmation modes.
* SWE-agent and mini-SWE-agent are research-first open-source agents centered on
  fixing GitHub issues and SWE-bench-style tasks. SWE-agent demonstrated the
  importance of agent-computer interfaces; mini-SWE-agent reduces the agent core
  to roughly 100 lines while retaining strong SWE-bench performance, with local
  and container environment options, batch mode, inspector/trajectory tooling,
  and broad model support.
* Notable adjacent systems include Amp, Amazon Q Developer, JetBrains Junie, and
  GitLab Duo Agent Platform. Amp is a terminal/editor agent with threads,
  AGENTS.md, handoff, subagents, skills, toolboxes, code review checks, MCP, and
  permissions. Amazon Q Developer spans IDE, CLI, AWS Console, Slack/Teams,
  GitLab, and GitHub preview for agentic development and app transformation.
  Junie is JetBrains' IDE-native coding agent with enterprise AI subscription
  and ACP support for other agents. GitLab Duo Agent Platform embeds multiple
  agents and flows throughout GitLab, including Developer Flow, Code Review
  Flow, CI/CD fixing, security flows, custom agents, MCP clients, and external
  agents.

### Local versus cloud tradeoffs

* Local agents preserve access to uncommitted state, private tools, debuggers,
  local databases, and low-latency steering, but they depend on the user's
  machine, can block the developer, and need stricter per-tool approvals.
* Cloud agents excel at background execution, concurrency, mobile/browser
  initiation, clean branches, PR-centered review, standardized runners, and team
  audit logs, but they require source-host integration, network/secrets design,
  remote environment setup, and explicit safeguards for prompt injection and
  exfiltration.
* The strongest systems offer both: local interactive pairing plus cloud
  delegation, shared instructions, shared session/task history, and a clean path
  to move work between local and cloud.

### Lessons for a repo-local VS Code multi-agent ecosystem

* Treat repository instructions as first-class artifacts: support AGENTS.md or
  equivalent scoped instructions, task-specific skills, review checklists, and
  tool manifests.
* Use branch/worktree/session isolation for every autonomous task; make
  collision avoidance explicit when agents run in parallel.
* Split planning from mutation. Plan mode should be read-only until approval,
  with editable plans and done criteria.
* Make verification a contract: agents should discover build/test/lint commands,
  run the smallest meaningful checks, store evidence, and report skipped checks.
* Keep approval boundaries visible: edits, shell commands, network access, PR
  creation, pushes, secret access, and MCP/tool execution should each have
  configurable policy.
* Prefer high-level, deterministic repo-local tools over raw shell when possible.
  Expose test, build, environment, issue, and game/domain operations as tools
  with narrow schemas.
* Preserve auditable trajectories: plans, tool calls, command output summaries,
  diffs, screenshots, and final evidence should be inspectable and resumable.
* Design for multiple tempos: foreground pair programming, background tasks,
  scheduled maintenance, issue/PR comment triggers, and batch runs all need
  different UX and permissions.
* Make recovery easy: checkpoints, reversible patches, worktree cleanup,
  interruption, pause/resume, and human takeover should be core primitives.
* Manage context explicitly: use subagents for bounded research, compact or
  hand off long threads, and store durable lessons only after repeated evidence.
* Build around PR-quality output even for local tasks: summary, risk notes,
  changed files, tests run, residual risk, and reviewer-ready diffs.

## Sources

* GitHub Copilot cloud agent overview: <https://docs.github.com/en/copilot/concepts/agents/cloud-agent/about-cloud-agent>
* GitHub Copilot cloud agent sessions: <https://docs.github.com/en/copilot/how-tos/use-copilot-agents/cloud-agent/start-copilot-sessions>
* GitHub Copilot cloud agent environment: <https://docs.github.com/en/copilot/how-tos/copilot-on-github/customize-copilot/customize-cloud-agent/customize-the-agent-environment>
* GitHub Copilot coding agent announcement: <https://github.blog/news-insights/product-news/github-copilot-meet-the-new-coding-agent/>
* GitHub Copilot Workspace: <https://githubnext.com/projects/copilot-workspace>
* OpenAI Codex announcement: <https://openai.com/index/introducing-codex/>
* OpenAI Codex overview: <https://developers.openai.com/codex/>
* OpenAI Codex cloud: <https://developers.openai.com/codex/cloud>
* OpenAI Codex cloud environments: <https://developers.openai.com/codex/cloud/environments>
* OpenAI Codex internet access: <https://developers.openai.com/codex/cloud/internet-access>
* OpenAI Codex IDE features: <https://developers.openai.com/codex/ide/features>
* OpenAI Codex GitHub integration: <https://developers.openai.com/codex/integrations/github>
* OpenAI Codex GitHub Action: <https://developers.openai.com/codex/github-action>
* OpenAI Codex best practices: <https://developers.openai.com/codex/learn/best-practices>
* Claude Code overview: <https://code.claude.com/docs/en/overview>
* Claude Code common workflows: <https://code.claude.com/docs/en/common-workflows>
* Claude Code GitHub Actions: <https://code.claude.com/docs/en/github-actions>
* Claude Code settings and permissions: <https://code.claude.com/docs/en/settings>
* Google Jules product page: <https://jules.google/>
* Google Jules public beta announcement: <https://blog.google/technology/google-labs/jules/>
* Devin docs: <https://docs.devin.ai/>
* Devin product page: <https://devin.ai/>
* Devin launch announcement: <https://www.cognition.ai/introducing-devin>
* Cursor Cloud Agents: <https://cursor.com/docs/cloud-agent>
* Cursor Cloud Agent capabilities: <https://cursor.com/docs/cloud-agent/capabilities>
* Cursor Cloud Agent security and network: <https://cursor.com/docs/cloud-agent/security-network>
* Cursor Cloud Agent settings: <https://cursor.com/docs/cloud-agent/settings>
* Windsurf Cascade overview: <https://docs.windsurf.com/windsurf/cascade/cascade>
* Replit Agent: <https://docs.replit.com/core-concepts/agent>
* Replit Agent task system: <https://docs.replit.com/core-concepts/agent/task-system>
* Replit Agent plan mode: <https://docs.replit.com/core-concepts/agent/plan-mode>
* Aider docs: <https://aider.chat/docs/>
* Aider chat modes: <https://aider.chat/docs/usage/modes.html>
* Aider linting and testing: <https://aider.chat/docs/usage/lint-test.html>
* OpenHands docs: <https://docs.openhands.dev/>
* OpenHands CLI: <https://docs.openhands.dev/openhands/usage/run-openhands/cli-mode>
* OpenHands Cloud: <https://docs.openhands.dev/openhands/usage/cloud/openhands-cloud>
* OpenHands GitHub integration: <https://docs.openhands.dev/openhands/usage/cloud/github-installation>
* OpenHands GitHub Action: <https://docs.openhands.dev/openhands/usage/run-openhands/github-action>
* OpenHands automations: <https://docs.openhands.dev/openhands/usage/automations/overview>
* OpenHands repository: <https://github.com/OpenHands/OpenHands>
* SWE-agent docs: <https://swe-agent.com/latest/>
* SWE-agent repository: <https://github.com/SWE-agent/SWE-agent>
* SWE-agent paper: <https://arxiv.org/abs/2405.15793>
* mini-SWE-agent docs: <https://mini-swe-agent.com/latest/>
* mini-SWE-agent repository: <https://github.com/SWE-agent/mini-swe-agent>
* Amp manual: <https://ampcode.com/manual>
* Amazon Q Developer: <https://aws.amazon.com/q/developer/>
* JetBrains Junie: <https://www.jetbrains.com/junie/>
* GitLab Duo Agent Platform: <https://docs.gitlab.com/user/duo_agent_platform/>

## Follow-On Questions

* Which systems have production-quality VS Code extension APIs for third-party
  orchestration rather than only first-party UI integration?
* Which systems publish detailed retention, sandbox, and model-training policies
  for private enterprise code beyond product-level claims?

## Clarifying Questions

None yet.
