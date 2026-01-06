---
title: RuleEngine.Core
layout: default
---

# RuleEngine.Core

RuleEngine.Core provides Roslyn-based C# expression compilation, versioning, audit logging, and design-time metadata.

## Key Features

- C# expression compilation and caching
- Rule versioning and active version management
- Audit logging (input/output and duration)
- RuleManager / RuleProvider flow
- Design-time metadata catalog
- DEBUG_RULES for PDB generation
- MultiResultRuleSet for multi-result selection
- Syntax validation (RuleSyntaxError)

All features: `features.en.html`

## Installation

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

builder.Services.AddRuleEngine();
builder.Services.AddRuleEngineDesignTime();
```

## Example Rule

```csharp
var createRequest = new CreateRuleRequest
{
    Name = "Discount Rule",
    Description = "Amount-based discount",
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount > 100",
        ResultExpression = "Input.Amount * 0.9"
    }
};

var rule = await ruleRepository.CreateAsync(createRequest);
await ruleRepository.ActivateVersionAsync(rule.Id, 1);

var result = await ruleEngine.EvaluateAsync(rule.Id, new { Amount = 150 });
```

## RuleManager Flow

```csharp
public sealed class MyRuleProvider : IRuleProvider
{
    public Task<RuleDefinition?> GetRuleDefinitionAsync(object input)
        => _repository.GetActiveVersionAsync("my_rule_id");

    public Task<bool> ValidateRuleAsync(RuleDefinition rule, object input)
        => Task.FromResult(true);
}
```

## Design-Time Metadata

```csharp
Content = new RuleContent
{
    PredicateExpression = "Input.TotalAmount > 100m",
    ResultExpression = "true",
    Metadata = new Dictionary<string, object>
    {
        ["DesignTime"] = new DesignTimeMetadata
        {
            Name = "Order.MinTotal",
            Title = "Minimum Order Total",
            ExpressionFormat = "Input.TotalAmount > {0}",
            Parameters = new List<ParameterDefinition>
            {
                new NumericParameter("Minimum Total")
            }
        }
    }
}
```

## Performance and Errors

- First compilation is slower; subsequent calls use cache.
- `RuleExecutionResult` provides error details and timings.

Details: `operations.en.html`
