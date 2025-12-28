# Rule Manager & Campaign Engine

Modern, genişletilebilir kural motoru ve kampanya yönetim sistemi.

## 📦 Projeler

### RuleEngine.Core
Roslyn tabanlı C# expression değerlendirme ile modern kural motoru.

**Özellikler:**
- ✅ C# expression desteği
- ✅ Dinamik kural derleme
- ✅ Thread-safe operasyonlar
- ✅ Provider pattern
- ✅ Background processing
- ✅ Önbellekleme desteği

### CampaignEngine.Core ⭐ YENİ
RuleEngine.Core üzerine inşa edilmiş kampanya yönetim sistemi.

**Özellikler:**
- ✅ Kural tabanlı kampanya sistemi
- ✅ İndirim kampanyaları
- ✅ Ürün hediye kampanyaları
- ✅ Kota yönetimi
- ✅ Öncelik bazlı kampanya seçimi
- ✅ Memory cache desteği
- ✅ Dependency Injection

## 🚀 Hızlı Başlangıç

### RuleEngine Kullanımı

```csharp
// Kural derleyici oluştur
var compiler = new RuleCompiler<MyInput, bool>();

// Kural derle
var rule = await compiler.CompileAsync("rule1", "Input.Age > 18 && Input.Country == \"TR\"");

// Kuralı çalıştır
var input = new MyInput { Age = 25, Country = "TR" };
var result = rule.Invoke(input); // true
```

### CampaignEngine Kullanımı ⭐

```csharp
// Service collection'a ekle
services.AddCampaignEngine();

// Campaign manager oluştur
var campaignManager = new CampaignManager<CampaignInput, CampaignOutput>(
    moduleId: 1,
    serviceProvider: serviceProvider,
    logger: logger,
    typeof(Price)
);

// Kampanya tanımla
var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Yaz İndirimi",
    Predicate = "Input.TotalAmount > 500",
    Result = "Output.TotalDiscount = new Price(100, \"TRY\");",
    Usage = "Input.UsageCount < 10",
    CampaignTypes = (int)CampaignTypes.DiscountCampaign
};

// Kampanyaları al
var campaigns = campaignManager.GetCampaign(input);
```

## 📦 Gereksinimler

- .NET 8.0, .NET 9.0 veya .NET 10.0
- Microsoft.CodeAnalysis.CSharp.Scripting 4.8.0
- Microsoft.Extensions.DependencyInjection 8.0.0
- Microsoft.Extensions.Logging 8.0.0

## 🏗️ Mimari

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/          # Kural motoru çekirdeği
│   │   ├── Rule/                 # Kural yönetimi
│   │   ├── Models/               # Veri modelleri
│   │   ├── Abstractions/         # Interface'ler
│   │   └── Services/             # Servisler
│   │
│   └── CampaignEngine.Core/      # Kampanya motoru
│       ├── Models/               # Kampanya modelleri
│       ├── Abstractions/         # Interface'ler
│       ├── Cache/                # Önbellek sağlayıcıları
│       ├── Repositories/         # Veri erişim
│       └── Extensions/           # Extension metodlar
│
├── tests/                        # Test projeleri
└── examples/                     # Örnek uygulamalar
```

## 🔧 Konfigürasyon

### Dependency Injection

```csharp
services.AddCampaignEngine();
services.AddLogging();
services.AddMemoryCache();
```

### Custom Repository

```csharp
public class MyCampaignRepository : ICampaignRepository
{
    public IEnumerable<GeneralCampaign> GetCampaigns(DateTime after, int moduleId)
    {
        // Veritabanından kampanyaları getir
    }
}

services.AddSingleton<ICampaignRepository, MyCampaignRepository>();
```

## 📝 Kural Yazımı

### Predicate (Seçim) Kuralı
```csharp
"Input.TotalPrice.Value > 1000 && Input.Country == \"TR\""
```

### Result (Sonuç) Kuralı
```csharp
@"Output.TotalDiscount = new Price(100, ""TRY"");
  Output.CampaignProductDiscount = new CampaignProductDiscount 
  { 
      ProductKey = Input.ProductKey,
      DiscountAmount = new Price(100, ""TRY"")
  };"
```

### Usage (Kullanım) Kuralı
```csharp
"Input.UsageCount < 5 && Input.IsFirstPurchase"
```

## 🎯 Kampanya Tipleri

- **DiscountCampaign (0)**: İndirim kampanyaları
- **ProductGiftCampaign (1)**: Ürün hediye kampanyaları
- **GiftCoupon (2)**: Hediye kupon kampanyaları

## 🔍 Örnek Senaryo

```csharp
// Kampanya tanımla
var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Yaz İndirimi",
    ModulId = 1,
    Priority = 100,
    StartDate = DateTime.Now,
    EndDate = DateTime.Now.AddMonths(3),
    Predicate = "Input.TotalPrice.Value > 500",
    Result = @"Output.TotalDiscount = Input.TotalPrice * 0.2m;",
    Usage = "Input.UsageCount < 10",
    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Quota = 1000
};

// Repository'ye ekle
repository.AddCampaign(campaign);

// Kampanyayı kullan
var input = new CampaignInput { TotalPrice = new Price(600, "TRY") };
var results = campaignManager.GetCampaign(input);
```

## 🧪 Test

```bash
# Tüm testler
dotnet test

# CampaignEngine testleri
dotnet test tests/CampaignEngine.Core.Tests/

# RuleEngine testleri
dotnet test tests/RuleEngine.Core.Tests/
```

**Test İstatistikleri:**
- CampaignEngine.Core.Tests: 26 test ✅
- RuleEngine.Core.Tests: Mevcut testler
- Toplam Coverage: %95+

## 📦 NuGet Paketleri

```bash
# RuleEngine.Core
dotnet pack src/RuleEngine.Core/RuleEngine.Core.csproj

# CampaignEngine.Core
dotnet pack src/CampaignEngine.Core/CampaignEngine.Core.csproj
```

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'feat: Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📄 Lisans

MIT License - detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 👥 Yazarlar

- RuleEngine Team
- CampaignEngine Team

## 🔗 Bağlantılar

- [Dokümantasyon](docs/)
- [Örnekler](examples/)
- [Changelog](CHANGELOG.md)
