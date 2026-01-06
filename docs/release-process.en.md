---
title: Release Process
layout: default
---

# Release Process

RuleEngine packages are released via automated scripts.

## Prerequisites

- .NET SDK 8.0+
- NuGet API Key
- GitHub Personal Access Token

## Release Script

```bash
export NUGET_API_KEY="your-nuget-api-key"
export GITHUB_TOKEN="your-github-token"

./release.sh
```

Use `release.ps1` for PowerShell.

## What Happens

1. Version is incremented (minor)
2. Packages are built and packed
3. NuGet publish
4. GitHub release creation

Detailed steps are in `RELEASE.md`.
