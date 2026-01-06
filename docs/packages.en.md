---
title: Packages
layout: default
---

# Packages and Support

The RuleEngine family is published as multiple NuGet packages. Each targets .NET 8/9/10 and follows a SemVer-based release policy.

## Package List

| Package | Purpose | NuGet |
| --- | --- | --- |
| `Minima.RuleEngine.Core` | Rule engine core | https://www.nuget.org/packages/Minima.RuleEngine.Core/ |
| `Minima.RuleEngine.Sqlite` | SQLite persistence and audit | https://www.nuget.org/packages/Minima.RuleEngine.Sqlite/ |
| `Minima.CampaignEngine.Core` | Campaign engine | https://www.nuget.org/packages/Minima.CampaignEngine.Core/ |

## Versioning and Compatibility

- Current package version: `1.1.11`
- Target frameworks: `net8.0`, `net9.0`, `net10.0`
- Minimum dependencies:
  - Microsoft.CodeAnalysis.CSharp.Scripting 4.14.0
  - Microsoft.Extensions.* (8/9/10 compatible)

## Version Policy

- SemVer: `MAJOR.MINOR.PATCH`
- MINOR versions include new features; PATCH versions include fixes.
- Releases are automated via `release.sh` and `release.ps1`.

## Package Selection

- In-memory rule execution only: `Minima.RuleEngine.Core`
- SQLite persistence and audit: `Minima.RuleEngine.Sqlite`
- Campaign rules: `Minima.CampaignEngine.Core`

All features: `features.en.html`

## Upgrade Notes

- Validate migrations and audit tables in staging before upgrading MINOR versions.
- If new campaign fields were added, review custom repository implementations.

More: `release-process.en.html`
