---
title: Agentic Programming Security Model
description: Security boundaries for Copilot-driven autonomous development in this repository
author: GitHub Copilot
ms.date: 2026-05-02
ms.topic: concept
---

## Security Posture

The repository is treated as publishable by default. Agentic automation must not
depend on secrets being hidden in source control, chat history, generated logs,
screenshots, local MCP files, or repo work tracking.

The safe default is no credentials in the repo and no credentials in prompts.
When credentials become necessary, use GitHub Secrets, GitHub environment
secrets, local ignored `.env.local` files, SSH agent forwarding controlled by
the user, or the operating system credential store. Do not print, commit, or
summarize actual values.

## Threats

* Accidental commit of credentials or private configuration
* Prompt or tool output containing secrets that then get copied into docs
* MCP servers exposing credentialed tools to autonomous agents
* GitHub Actions logs revealing transformed secrets
* Self-hosted runners or local deployment scripts reading more machine state
  than they need
* Cloud agent seeing files that were assumed to be excluded from Copilot context
* Public issues, pull requests, or research artifacts containing sensitive
  local details

## Controls

* `.gitignore` blocks local secrets, private agent runtime logs, local MCP
  configuration, and common build output.
* `.github/copilot-instructions.md` gives Copilot always-on secret-safety rules.
* `.github/hooks/secret-guard.json` can run a local pre-tool guard that blocks
  obvious secret-like content before tool execution.
* `scripts/security/Test-SecretPatterns.ps1` scans stdin for hook use and can
  scan the repo before commits or before publishing.
* GitHub secret scanning and push protection should be enabled on GitHub. Public
  repositories receive secret scanning automatically, and user push protection is
  on by default for public repositories.
* GitHub Actions should use least-privilege `GITHUB_TOKEN` permissions and avoid
  passing secrets on command lines.

## GitHub Copilot Cloud Agent Rules

Copilot cloud agent runs in a GitHub Actions-powered environment. It can plan,
edit, test, commit, and open pull requests. It can also use configured MCP tools
autonomously.

Use the following policy for cloud-agent work:

* Start with no custom MCP servers.
* If MCP is needed, allowlist specific read-only tools rather than `*`.
* Store required MCP secrets only in the GitHub `copilot` environment using the
  required `COPILOT_MCP_` prefix.
* Do not configure personal access tokens with broader access unless the user
  explicitly approves the exact scope.
* Avoid self-hosted runners unless they are ephemeral, single-use, and network
  restricted.
* Do not rely on Copilot content exclusions as a secret boundary for cloud-agent
  work.

## Local Steam Deck Deployment Rules

Steam Deck deployment will eventually require local machine details. Keep them
out of the repo:

* Steam Deck host, IP, username, and SSH key paths stay in ignored local files.
* Deployment scripts should read configuration from environment variables or
  explicit parameters.
* Logs should say that deployment succeeded or failed without printing private
  hosts, usernames, key paths, or tokens.

## Incident Response

If a secret is exposed:

1. Stop using the value immediately.
2. Rotate or revoke the credential at the provider.
3. Remove the value from files and generated artifacts.
4. If it was committed, treat Git history and PR discussion as compromised and
   clean or rotate accordingly.
5. Add a regression guard so the same leak path is blocked next time.
