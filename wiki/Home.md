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

Detaylar icin [packages](packages) sayfasina bakin.

## Hizli Baslangic

```bash
dotnet add package Minima.RuleEngine.Core
```

```csharp
using RuleEngine.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRuleEngine();
```

Daha fazlasi: [getting-started](getting-started)

## Dokumantasyon Haritasi

- Baslangic ve kurulum: [getting-started](getting-started)
- Feature matrix: [features](features)
- Paketler ve surumleme: [packages](packages)
- Mimari: [architecture](architecture)
- RuleEngine.Core: [ruleengine-core](ruleengine-core)
- CampaignEngine.Core: [campaignengine-core](campaignengine-core)
- SQLite saglayici: [sqlite-provider](sqlite-provider)
- Kural yazimi: [rule-authoring](rule-authoring)
- Operasyon ve performans: [operations](operations)
- Guvenlik: [security](security)
- Surumleme ve release sureci: [release-process](release-process)
- Katkida bulunma: [contributing](contributing)
- Ornekler: [examples](examples)

## Kurumsal Hazirlik

- Surumleme politikasi ve release otomasyonu
- Audit logging ve performans metrikleri
- Tasarim zamani metadata katalogu
- Guvenlik politikasi ve acik hata bildirim kanallari

Sonraki adim: [architecture](architecture)
