---
title: Release Process
layout: default
---

# Release Process

RuleEngine paketleri icin release sureci otomatiklestirilmis scriptler ile yonetilir.

## Onkosullar

- .NET SDK 8.0+
- NuGet API Key
- GitHub Personal Access Token

## Release Script

```bash
export NUGET_API_KEY="your-nuget-api-key"
export GITHUB_TOKEN="your-github-token"

./release.sh
```

PowerShell icin `release.ps1` kullanilabilir.

## Neler Yapilir?

1. Versiyon artirilir (MINOR)
2. Paketler build ve pack edilir
3. NuGet'e publish edilir
4. GitHub release olusturulur

Detayli adimlar repositorydeki `RELEASE.md` dokumaninda bulunur.
