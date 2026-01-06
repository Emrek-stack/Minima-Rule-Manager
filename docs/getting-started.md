---
title: Getting Started
layout: default
---

# Getting Started

Bu sayfa, RuleEngine paketlerini hizla kullanmaya baslamak icin asgari adimlari sunar.

## Onkosullar

- .NET SDK 8.0 veya ustu
- C# 12+ (onerilen)

## 1) RuleEngine.Core kurulumu

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRuleEngine();
```

## 2) SQLite ile kalicilik (opsiyonel)

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

```csharp
using RuleEngine.Sqlite.Extensions;

builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");
```

## 3) Kampanya motoru (opsiyonel)

```bash
dotnet add package Minima.CampaignEngine.Core
```

```csharp
using CampaignEngine.Core.Extensions;

builder.Services.AddCampaignEngine();
```

## Basit ornek

```csharp
using RuleEngine.Core.Rule;
using RuleEngine.Core.Models;

public class OrderInput : RuleInputModel
{
    public decimal Amount { get; set; }
    public string CustomerType { get; set; } = string.Empty;
}

var compiler = new RuleCompiler<OrderInput, bool>();
var rule = await compiler.CompileAsync(
    "vip-check",
    "Input.Amount > 500 && Input.CustomerType == \"VIP\""
);

var result = rule.Invoke(new OrderInput { Amount = 750, CustomerType = "VIP" });
```

## Sonraki adimlar

- Kural yazimi: `rule-authoring.html`
- Mimari: `architecture.html`
- Paket detaylari: `packages.html`
