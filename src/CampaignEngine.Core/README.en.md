# CampaignEngine.Core

Modern campaign management system built on RuleEngine.Core. Designed for enterprise scenarios with prioritization, quota, and audit-focused flows.

## Features

- **Rule-based system** integrated with RuleEngine.Core
- **Discount campaigns** (percentage/fixed)
- **Product gift campaigns**
- **Quota management** and usage tracking
- **Priority ordering**
- **Cache support** (memory cache)
- **Dependency Injection** friendly
- **Basket integration** via `ITravelProduct`
- **Available campaigns** per product

## Installation

```bash
dotnet add package Minima.CampaignEngine.Core
```

## Quick Start

### 1. Service Registration

```csharp
services.AddCampaignEngine();
services.AddLogging();
```

### 2. Model Definitions

```csharp
public class MyCampaignInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public string Country { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}

public class MyCampaignOutput : CampaignOutput
{
}
```

### 3. Create Campaign Manager

```csharp
var campaignManager = new CampaignManager<MyCampaignInput, MyCampaignOutput>(
    moduleId: 1,
    serviceProvider: serviceProvider,
    logger: logger,
    typeof(Price)
);
```

### 4. Define a Campaign

```csharp
var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Summer Sale",
    ModulId = 1,
    Priority = 100,
    StartDate = DateTime.Now,
    EndDate = DateTime.Now.AddMonths(3),

    Predicate = "Input.TotalAmount > 500 && Input.Country == \"US\"",
    Result = "Output.TotalDiscount = Input.TotalAmount * 0.2m;",
    Usage = "Input.UsageCount < 10",

    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Quota = 1000
};

repository.AddCampaign(campaign);
```

### 5. Apply Campaigns

```csharp
var input = new MyCampaignInput
{
    TotalAmount = 600,
    Country = "US",
    UsageCount = 5
};

var campaigns = campaignManager.GetCampaign(input);

foreach (var campaign in campaigns)
{
    Console.WriteLine($"Discount: {campaign.TotalDiscount}");
}
```

## Campaign Types

### DiscountCampaign (0)
Highest priority campaign is applied.

### ProductGiftCampaign (1)
All eligible campaigns are applied.

## Rule Writing

### Predicate
```csharp
"Input.TotalAmount > 500"
"Input.TotalAmount > 500 && Input.Country == \"US\""
```

### Result
```csharp
"Output.TotalDiscount = new Price(100, \"USD\");"
```

### Usage
```csharp
"Input.UsageCount < 5"
```

## Additional Info

- RuleEngine.Core docs: `../RuleEngine.Core/README.md`
- Examples: `../../examples/`
- Documentation: `../../docs/index.md`

## NuGet Notes

- Package: `Minima.CampaignEngine.Core`
- Recent updates: available campaign resolution, use/delete helpers, demo seed helper
