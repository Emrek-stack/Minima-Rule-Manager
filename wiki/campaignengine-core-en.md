# CampaignEngine.Core

CampaignEngine.Core is built on RuleEngine.Core and provides campaign selection with priority, quota, predicate/result/usage rules.

## Key Features

- Campaign selection and prioritization
- Discount, ProductGift, GiftCoupon types
- Usage and quota checks
- Available campaign calculation
- UseCampaign / DeleteCampaign basket flow
- Price model (ISO 4217 compatible)

All features: [features-en](features-en)

## Installation

```bash
dotnet add package Minima.CampaignEngine.Core
```

```csharp
using CampaignEngine.Core.Extensions;

builder.Services.AddCampaignEngine();
```

## Simple Example

```csharp
public class CampaignInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public string Country { get; set; } = string.Empty;
}

public class CampaignOutput : CampaignEngine.Core.Models.CampaignOutput
{
}

var campaignManager = new CampaignManager<CampaignInput, CampaignOutput>(
    moduleId: 1,
    serviceProvider: serviceProvider,
    logger: logger,
    typeof(Price)
);

var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Summer Sale",
    ModulId = 1,
    Priority = 100,
    Predicate = "Input.TotalAmount > 500 && Input.Country == \"US\"",
    Result = "Output.TotalDiscount = new Price(100, \"USD\");",
    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Quota = 1000
};

repository.AddCampaign(campaign);
```

Details: [rule-authoring-en](rule-authoring-en)
