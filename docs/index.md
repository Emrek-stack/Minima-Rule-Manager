---
title: RuleEngine
layout: default
---

![RuleEngine](assets/rulemanager.png)

# RuleEngine & CampaignEngine

RuleEngine, .NET uygulamalarinda dinamik kural calistirma ve kampanya yonetimi icin tasarlanmis, kurumsal kullanima uygun bir NuGet paket ailesidir. Roslyn tabanli C# expression derleme, versiyonlama, audit logging ve SQLite kaliciligi ile gelir.

## Neden RuleEngine?

- Yuksek performansli derleme ve cache
- Versiyonlama, audit log ve geri alma
- Tasarlanmis metadata (rule editor entegrasyonu)
- DI uyumlu, coklu framework hedefleme (.NET 8/9/10)
- Kampanya altyapisi (CampaignEngine.Core)

## Paketler

- `Minima.RuleEngine.Core` - Kural motoru cekirdegi
- `Minima.RuleEngine.Sqlite` - SQLite kalicilik ve audit
- `Minima.CampaignEngine.Core` - Kampanya motoru

Detaylar icin `packages.html` sayfasina bakin.

## Hizli Baslangic

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRuleEngine();
```

Daha fazlasi: `getting-started.html`

## Dokumantasyon Haritasi

- Baslangic ve kurulum: `getting-started.html`
- Paketler ve surumleme: `packages.html`
- Mimari: `architecture.html`
- RuleEngine.Core: `ruleengine-core.html`
- CampaignEngine.Core: `campaignengine-core.html`
- SQLite saglayici: `sqlite-provider.html`
- Kural yazimi: `rule-authoring.html`
- Operasyon ve performans: `operations.html`
- Guvenlik: `security.html`
- Surumleme ve release sureci: `release-process.html`
- Katkida bulunma: `contributing.html`
- Ornekler: `examples.html`

## Kurumsal Hazirlik

- Surumleme politikasi ve release otomasyonu
- Audit logging ve performans metrikleri
- Tasarim zamani metadata katalogu
- Guvenlik politikasi ve acik hata bildirim kanallari

Sonraki adim: `architecture.html`
