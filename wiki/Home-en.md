![RuleEngine](assets/rulemanager.png)

# RuleEngine & CampaignEngine

RuleEngine is an enterprise-ready NuGet package family for dynamic rule execution and campaign management in .NET. It provides Roslyn-based C# expression compilation, versioning, audit logging, and SQLite persistence.

## Why RuleEngine?

- High-performance compilation and caching
- Versioning, audit logs, and rollback
- Design-time metadata for rule editors
- DI friendly, multi-targeting (.NET 8/9/10)
- Campaign infrastructure (CampaignEngine.Core)

## Packages

- `Minima.RuleEngine.Core` - Rule engine core
- `Minima.RuleEngine.Sqlite` - SQLite persistence and audit
- `Minima.CampaignEngine.Core` - Campaign engine

See [packages-en](packages-en) for details.

## Quick Start

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRuleEngine();
```

More: [getting-started-en](getting-started-en)

## Documentation Map

- Getting started: [getting-started-en](getting-started-en)
- Feature matrix: [features-en](features-en)
- Packages & versioning: [packages-en](packages-en)
- Architecture: [architecture-en](architecture-en)
- RuleEngine.Core: [ruleengine-core-en](ruleengine-core-en)
- CampaignEngine.Core: [campaignengine-core-en](campaignengine-core-en)
- SQLite provider: [sqlite-provider-en](sqlite-provider-en)
- Rule authoring: [rule-authoring-en](rule-authoring-en)
- Operations & performance: [operations-en](operations-en)
- Security: [security-en](security-en)
- Release process: [release-process-en](release-process-en)
- Contributing: [contributing-en](contributing-en)
- Examples: [examples-en](examples-en)

## Enterprise Readiness

- Release automation and versioning policy
- Audit logging and performance metrics
- Design-time metadata catalog
- Security policy and disclosure process

Next: [architecture-en](architecture-en)
