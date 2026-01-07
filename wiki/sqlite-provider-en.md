# RuleEngine.Sqlite

RuleEngine.Sqlite provides persistence and audit logging for RuleEngine.Core using EF Core.

## Key Features

- EF Core persistence
- Versioning and active version management
- Rule parameter serialization (System.Text.Json)
- Audit logging (input/output, duration, errors)
- Migration and seeding

All features: [features-en](features-en)

## Installation

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

```csharp
using RuleEngine.Sqlite.Extensions;

builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");
```

## Schema

- Rules
- RuleVersions
- RuleParameters
- RuleExecutionAudits

## Migrations

```bash
dotnet ef migrations add InitialCreate --project RuleEngine.Sqlite --startup-project YourApp

dotnet ef database update --project RuleEngine.Sqlite --startup-project YourApp
```

## Audit Logging

Audit records include input/output, duration, and error details. For enterprise usage, apply retention policies with your logging platform.

Details: [operations-en](operations-en)
