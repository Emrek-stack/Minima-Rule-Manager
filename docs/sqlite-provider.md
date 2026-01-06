---
title: SQLite Provider
layout: default
---

# RuleEngine.Sqlite

RuleEngine.Sqlite, RuleEngine.Core icin kalicilik ve audit logging saglar. EF Core ile migration ve seeding destekler.

## Kurulum

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

```csharp
using RuleEngine.Sqlite.Extensions;

builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");
```

## Tablo Yapisi

- Rules
- RuleVersions
- RuleParameters
- RuleExecutionAudits

## Migration

```bash
dotnet ef migrations add InitialCreate --project RuleEngine.Sqlite --startup-project YourApp

dotnet ef database update --project RuleEngine.Sqlite --startup-project YourApp
```

## Audit Logging

Audit kayitlari input/output, sure ve hata bilgisi icerir. Kurumsal ortamlarda log retention politikasi ile birlikte kullanilmasi onerilir.

Detaylar: `operations.html`
