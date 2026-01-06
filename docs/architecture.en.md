---
title: Architecture
layout: default
---

# Architecture Overview

RuleEngine has a modular architecture composed of core rule execution, persistence, and campaign layers.

## Components

- RuleEngine.Core: rule compilation, execution, metadata, versioning
- RuleEngine.Sqlite: EF Core persistence and audit logging
- CampaignEngine.Core: campaign selection, prioritization, application

## Data Flow

```text
Input -> RuleProvider -> RuleManager -> RuleSet -> CompiledRule -> Output
```

For campaigns:

```text
CampaignInput -> CampaignManager -> RuleProvider -> RuleSet -> CampaignOutput
```

## Module Boundaries

- Core uses evaluator/repository abstractions.
- Sqlite provides repository implementations and migrations.
- CampaignEngine builds on RuleEngine.Core with its own repository abstraction.

## Extension Points

- `IRuleEvaluator` for custom expression engines
- `IRuleRepository` and `IAuditRepository` for custom persistence
- `ICampaignRepository` for campaign data sources

## Design-Time Metadata

Metadata powers rule editor catalogs, parameter definitions, and categories.

More: `ruleengine-core.en.html`
