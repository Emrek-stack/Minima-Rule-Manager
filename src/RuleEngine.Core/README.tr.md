# RuleEngine.Core

Roslyn tabanli C# expression derleme, design-time metadata ve genisletilebilir calistirma akisi sunan core kural motoru. Kurumsal kullanim icin versioning ve audit logging destekler.

## Ozellikler

- **Kural tanimi**: metadata, versioning, parametreler, durum yonetimi
- **C# Expression**: Roslyn scripting ile calistirma
- **Design-Time Metadata**: kural editoru metadata/kategori/parametre
- **Versioning**: coklu versiyon ve aktivasyon
- **Audit Logging**: calistirma gecmisi
- **RuleManager Flow**: `IRuleManager` + `IRuleProvider`
- **DEBUG_RULES**: PDB olusturma (debug)
- **Extensible**: custom evaluator destegi

## Kurulum

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

builder.Services.AddRuleEngine();
builder.Services.AddRuleEngineDesignTime();
```

## Ornek

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

## Dokumantasyon

- Baslangic: `../../docs/getting-started.md`
- Mimari: `../../docs/architecture.md`
- Kural yazimi: `../../docs/rule-authoring.md`
