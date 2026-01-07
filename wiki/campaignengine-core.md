# CampaignEngine.Core

CampaignEngine.Core, RuleEngine.Core uzerine insa edilmis kampanya yonetim katmanidir. Kampanyalari oncelik, kota, secim (predicate), sonuc (result) ve kullanim (usage) kurallariyla calistirir.

## Temel Ozellikler

- Kampanya secimi ve onceliklendirme
- Discount, ProductGift, GiftCoupon tipleri
- Usage ve quota kontrolleri
- Available campaign hesaplama
- UseCampaign / DeleteCampaign sepet akisi
- Price modeli (ISO 4217 uyumlu)

Tum ozellikler: [features](features)

## Kurulum

```bash
dotnet add package Minima.CampaignEngine.Core
```

```csharp
using CampaignEngine.Core.Extensions;

builder.Services.AddCampaignEngine();
```

## Basit Ornek

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

## Kampanya Tipleri

- DiscountCampaign: En yuksek oncelik uygulanir
- ProductGiftCampaign: Tum uygun kampanyalar uygulanir
- GiftCoupon: Kupon kampanyalari

## Sepet Entegrasyonu

Kampanya kullan/geri al akisi icin `UseCampaign` ve `DeleteCampaign` yardimcilari bulunur.

Detaylar: [rule-authoring](rule-authoring)
