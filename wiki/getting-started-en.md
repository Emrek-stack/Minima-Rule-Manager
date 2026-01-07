# Getting Started

This page provides the minimal steps to start using RuleEngine packages.

## Prerequisites

- .NET SDK 8.0 or later
- C# 12+ (recommended)

## 1) Install RuleEngine.Core

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRuleEngine();
```

## 2) SQLite persistence (optional)

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

```csharp
using RuleEngine.Sqlite.Extensions;

builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");
```

## 3) Campaign engine (optional)

```bash
dotnet add package Minima.CampaignEngine.Core
```

```csharp
using CampaignEngine.Core.Extensions;

builder.Services.AddCampaignEngine();
```

## Simple example

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

## Next steps

- Rule authoring: [rule-authoring-en](rule-authoring-en)
- Architecture: [architecture-en](architecture-en)
- Package details: [packages-en](packages-en)
