---
title: Agent Orchestration and Long-Running Autonomous Development Loop Research
description: Public research notes on agent orchestration architecture, control loops, gates, durability, and traceability
ms.date: 2026-05-02
ms.topic: research
---

## Research questions

* What public architectures and framework features support long-running autonomous development loops as of May 2, 2026?
* Which multi-agent topologies are most relevant for supervisor, worker, critic, evaluator, and human-approval patterns?
* How do LangGraph, Microsoft AutoGen AgentChat/Core, CrewAI Flows, OpenAI Agents SDK and Responses API, Anthropic tool-use/computer-use guidance, and GitHub Copilot custom agents/hooks expose planning, orchestration, durability, tracing, and control primitives?
* What design recommendations follow for durable queues, event logs, state machines, approval gates, eval gates, traceability, MCP integration, GitHub Actions or local CI integration, hooks, and safe stop/start control?

## Findings

### Durable loop architecture

The strongest public pattern for long-running autonomous software work is a
durable state machine, not one free-running chat. The orchestrator owns the
queue, phase state, stop and resume controls, approvals, artifact index,
verification gates, and final handoff. Agents own bounded reasoning and tool use
inside explicit phases.

The reference flow is:

```text
Intake -> Clarify -> Plan -> Approve -> Execute -> Validate -> Critic -> Security Gate -> PR -> CI Gate -> Human Review
```

Each work item needs a stable run identifier, current phase, objective,
assumptions, selected context, agent assignments, changed files, verification
evidence, review findings, approval events, and next safe action. The ledger is
the recovery mechanism when a model session ends, context compacts, a command
fails, or the human pauses work.

### Multi-agent topology

The most useful topology is supervisor plus bounded specialists:

* Supervisor or orchestrator: owns decomposition, routing, and phase gates.
* Product or requirements agent: converts ambiguous goals into scoped work.
* Architecture agent: chooses data boundaries, technical tradeoffs, and seams.
* Implementation agent: edits code and runs focused commands.
* Test or evaluator agent: designs objective checks and verification evidence.
* Critic or reviewer agent: reviews correctness and maintainability without
  editing.
* Security agent: reviews secrets, tool access, dependency risk, and public
  exposure.
* Release or platform agent: handles target platform constraints and deployment
  readiness.

Free-form group chat is not a strong system of record. The orchestrator should
route artifacts and decisions, not rely on consensus among personalities.

### Gates and approvals

Gates should sit between phases rather than only at the end:

* Planning gates prevent mutation before scope is understood.
* Implementation gates keep changes small and owned.
* Verification gates run deterministic checks before review.
* Critic gates find behavior, design, and test gaps.
* Security gates stop credential exposure and unsafe tool expansion.
* CI and PR gates provide external evidence before merge.

LLM critics are useful for review and risk discovery, but tests, linters,
builds, replay checks, and deterministic simulations should be authoritative
when they exist.

### Tool policy

MCP and other tools are side-effect surfaces. Public guidance from GitHub's MCP
cloud-agent docs is especially important: configured MCP tools can be used
autonomously, so allowlist specific read-only tools before considering broader
access. Tool access should be capability-based, narrow, logged, and gated by
human approval when it can mutate state or requires credentials.

### Copilot-specific design implication

For this repo, the implementation should use GitHub Copilot custom agents,
prompt files, hooks, GitHub branches, pull requests, Actions, and local scripts.
LangGraph, AutoGen, CrewAI, OpenAI Agents SDK, and Anthropic guidance are useful
research references, but they should not become runtime dependencies unless the
user explicitly changes the rule.

The permanent-loop requirement is best approximated safely as repeated bounded
Copilot loop invocations with a durable ledger, plus optional Copilot cloud-agent
tasks on GitHub issues. A self-scheduling loop that runs indefinitely without a
human-visible phase gate would be harder to audit and riskier around secrets.

## Sources

* LangGraph overview: <https://docs.langchain.com/oss/python/langgraph/overview>
* LangGraph persistence: <https://docs.langchain.com/oss/python/langgraph/persistence>
* LangGraph human in the loop: <https://docs.langchain.com/oss/python/langgraph/human-in-the-loop>
* LangGraph time travel: <https://docs.langchain.com/oss/python/langgraph/time-travel>
* Microsoft AutoGen AgentChat guide: <https://microsoft.github.io/autogen/stable/user-guide/agentchat-user-guide/index.html>
* AutoGen selector group chat: <https://microsoft.github.io/autogen/stable/user-guide/agentchat-user-guide/selector-group-chat.html>
* AutoGen Magentic-One: <https://microsoft.github.io/autogen/stable/user-guide/agentchat-user-guide/magentic-one.html>
* AutoGen Core guide: <https://microsoft.github.io/autogen/stable/user-guide/core-user-guide/index.html>
* CrewAI crews: <https://docs.crewai.com/concepts/crews>
* CrewAI tasks: <https://docs.crewai.com/concepts/tasks>
* CrewAI flows: <https://docs.crewai.com/concepts/flows>
* OpenAI Agents SDK: <https://openai.github.io/openai-agents-python/>
* OpenAI Agents handoffs: <https://openai.github.io/openai-agents-python/handoffs/>
* OpenAI Agents guardrails: <https://openai.github.io/openai-agents-python/guardrails/>
* OpenAI Responses API guide: <https://platform.openai.com/docs/guides/responses>
* Anthropic building effective agents: <https://www.anthropic.com/engineering/building-effective-agents>
* Anthropic writing tools for agents: <https://www.anthropic.com/engineering/writing-tools-for-agents>
* Anthropic effective harnesses: <https://www.anthropic.com/engineering/effective-harnesses>
* Model Context Protocol architecture: <https://modelcontextprotocol.io/docs/concepts/architecture>
* Model Context Protocol tools: <https://modelcontextprotocol.io/docs/concepts/tools>
* Model Context Protocol security best practices: <https://modelcontextprotocol.io/specification/2025-06-18/basic/security_best_practices>
* GitHub Copilot cloud agent overview: <https://docs.github.com/en/copilot/concepts/agents/cloud-agent/about-cloud-agent>
* GitHub Copilot custom agents: <https://docs.github.com/en/copilot/how-tos/use-copilot-agents/cloud-agent/create-custom-agents>
* GitHub Copilot MCP: <https://docs.github.com/en/copilot/how-tos/use-copilot-agents/cloud-agent/extend-cloud-agent-with-mcp>
* GitHub Copilot setup steps: <https://docs.github.com/en/copilot/how-tos/use-copilot-agents/cloud-agent/customize-the-agent-environment>
* VS Code custom agents: <https://code.visualstudio.com/docs/copilot/customization/custom-agents>
* VS Code prompt files: <https://code.visualstudio.com/docs/copilot/customization/prompt-files>
* GitHub Actions secure use: <https://docs.github.com/en/actions/security-for-github-actions/security-guides/security-hardening-for-github-actions>
* GitHub Actions secrets: <https://docs.github.com/en/actions/security-for-github-actions/security-guides/using-secrets-in-github-actions>
* GitHub secret scanning: <https://docs.github.com/en/code-security/secret-scanning/about-secret-scanning>
* GitHub push protection: <https://docs.github.com/en/code-security/secret-scanning/push-protection-for-repositories-and-organizations>

## Recommendations

* Use GitHub Copilot and VS Code customizations as the active orchestration
  surface.
* Use one Strategic Orchestrator agent plus bounded specialist agents.
* Use prompt files as start, iterate, and assess controls.
* Store runtime ledgers in ignored `.copilot-tracking/agentic/runs/` paths.
* Promote only sanitized decisions to public docs.
* Keep MCP disabled at first. If needed later, allowlist read-only tools and use
  GitHub environment secrets with explicit user approval.
* Add deterministic checks before broadening autonomy.
* Treat stop, pause, and resume as first-class workflow states.

## Clarifying questions

None.
