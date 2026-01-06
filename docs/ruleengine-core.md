---
title: RuleEngine.Core
layout: default
---

# RuleEngine.Core

RuleEngine.Core, Roslyn tabanli C# expression derleme, versioning, audit logging ve design-time metadata destegi sunar.

## Temel Ozellikler

- C# expression derleme ve cache
- Rule versioning ve active version
- Audit logging (input/output ve sure)
- RuleManager / RuleProvider akisi
- Design-time metadata katalogu
- DEBUG_RULES ile PDB olusturma
- MultiResultRuleSet ile coklu sonuc secimi
- Syntax dogrulama (RuleSyntaxError)

Tum ozellikler: `features.html`

## Kurulum

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

builder.Services.AddRuleEngine();
builder.Services.AddRuleEngineDesignTime();
```

## Ornek Kural

```csharp
var createRequest = new CreateRuleRequest
{
    Name = "Discount Rule",
    Description = "Amount bazli indirim",
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

## RuleManager Akisi

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

Metadata katalogu, kural editoru UI'leri icin metadata ve parametre tanimlari saglar.

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

## Performans ve Hata Yakalama

- Kurallar ilk derlemede yavas, sonra cache uzerinden hizli calisir.
- `RuleExecutionResult` ile hata mesaji ve sure olcumleri alinabilir.

Detaylar: `operations.html`
