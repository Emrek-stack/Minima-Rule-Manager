# CampaignEngine.Core

RuleEngine.Core uzerine insa edilmis modern kampanya yonetim sistemi. Kurumsal senaryolar icin onceliklendirme, kota ve audit odakli calisir.

## 🎯 Özellikler

- **Kural Tabanlı Sistem**: RuleEngine.Core ile entegre
- **İndirim Kampanyaları**: Yüzde veya sabit tutar indirimleri
- **Ürün Hediye Kampanyaları**: Sepet bazlı hediye ürünler
- **Kota Yönetimi**: Kampanya kullanım limitleri
- **Öncelik Sistemi**: Kampanya önceliklendirme
- **Cache Desteği**: Memory cache ile performans optimizasyonu
- **Dependency Injection**: Modern .NET DI pattern
- **Sepet Entegrasyonu**: `ITravelProduct` ile kampanya kullanım/geri alma
- **Uygun Kampanyalar**: Ürün bazında available campaign hesaplama

## 📦 Kurulum

```bash
dotnet add package Minima.CampaignEngine.Core
```

## 🚀 Hızlı Başlangıç

### 1. Service Registration

```csharp
services.AddCampaignEngine();
services.AddLogging();
```

### 2. Model Tanımlama

```csharp
public class MyCampaignInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public string Country { get; set; }
    public int UsageCount { get; set; }
}

public class MyCampaignOutput : CampaignOutput
{
    // CampaignOutput'tan TotalDiscount ve CampaignProductDiscount gelir
}
```

### 3. Campaign Manager Oluşturma

```csharp
var campaignManager = new CampaignManager<MyCampaignInput, MyCampaignOutput>(
    moduleId: 1,
    serviceProvider: serviceProvider,
    logger: logger,
    typeof(Price) // Extra types for rule compilation
);
```

### 4. Kampanya Tanımlama

```csharp
var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Yaz İndirimi",
    ModulId = 1,
    Priority = 100,
    StartDate = DateTime.Now,
    EndDate = DateTime.Now.AddMonths(3),
    
    // Seçim kuralı - Kampanya ne zaman uygulanır?
    Predicate = "Input.TotalAmount > 500 && Input.Country == \"TR\"",
    
    // Sonuç kuralı - Ne kadar indirim yapılır?
    Result = @"Output.TotalDiscount = Input.TotalAmount * 0.2m;",
    
    // Kullanım kuralı - Kimler kullanabilir?
    Usage = "Input.UsageCount < 10",
    
    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Quota = 1000
};

repository.AddCampaign(campaign);
```

### 5. Kampanya Uygulama

```csharp
var input = new MyCampaignInput 
{ 
    TotalAmount = 600,
    Country = "TR",
    UsageCount = 5
};

var campaigns = campaignManager.GetCampaign(input);

foreach (var campaign in campaigns)
{
    Console.WriteLine($"Discount: {campaign.TotalDiscount}");
}
```

### 6. Kampanya Setlerini ve Uygun Kampanyalari Alma

```csharp
var campaignResults = campaignManager.GetCampaign(input, out var ruleSets);

var available = campaignManager.GetAvailableCampaigns(
    productKey: "PRD-001",
    productsInTransaction: products,
    input: input);
```

### 7. Sepet Kampanya Kullan / Geri Al

```csharp
products = campaignManager.UseCampaign(
    productKey: "PRD-001",
    campaignCode: "CITYGIFT50",
    productsInTransaction: products);

campaignManager.DeleteCampaign("CITYGIFT50", products);
```

### 8. Demo Seed (Hazir Kampanyalar)

```csharp
using CampaignEngine.Core.Demo;
using CampaignEngine.Core.Repositories;

if (campaignRepository is InMemoryCampaignRepository memoryRepo)
{
    CampaignSeed.SeedToRepository(memoryRepo, moduleId: 1);
}
```

## 📋 Kampanya Tipleri

### DiscountCampaign (0)
İndirim kampanyaları - En yüksek öncelikli kampanya uygulanır

```csharp
Predicate = "Input.TotalAmount > 1000",
Result = "Output.TotalDiscount = new Price(200, \"TRY\");",
CampaignTypes = (int)CampaignTypes.DiscountCampaign
```

### ProductGiftCampaign (1)
Ürün hediye kampanyaları - Tüm uygun kampanyalar uygulanır

```csharp
Predicate = "Input.ProductCount >= 3",
Result = "Output.TotalDiscount = new Price(50, \"TRY\");",
CampaignTypes = (int)CampaignTypes.ProductGiftCampaign
```

## 🔧 Kural Yazımı

### Predicate (Seçim) Kuralı
Kampanyanın ne zaman uygulanacağını belirler:

```csharp
// Basit koşul
"Input.TotalAmount > 500"

// Çoklu koşul
"Input.TotalAmount > 500 && Input.Country == \"TR\""

// Tarih kontrolü
"Input.OrderDate >= DateTime.Now.AddDays(-7)"

// Liste kontrolü
"Input.Categories.Contains(\"Electronics\")"
```

### Result (Sonuç) Kuralı
İndirim miktarını hesaplar:

```csharp
// Sabit tutar
"Output.TotalDiscount = new Price(100, \"TRY\");"

// Yüzde hesaplama
"Output.TotalDiscount = Input.TotalAmount * 0.15m;"

// Koşullu hesaplama
@"if (Input.TotalAmount > 1000)
    Output.TotalDiscount = Input.TotalAmount * 0.2m;
  else
    Output.TotalDiscount = Input.TotalAmount * 0.1m;"
```

### Usage (Kullanım) Kuralı
Kampanyayı kimlerin kullanabileceğini belirler:

```csharp
// Kullanım sayısı kontrolü
"Input.UsageCount < 5"

// İlk alışveriş kontrolü
"Input.IsFirstPurchase == true"

// Üyelik kontrolü
"Input.MembershipLevel >= 2"
```

## 💡 İleri Seviye Örnekler

### Kademeli İndirim

```csharp
Predicate = "Input.TotalAmount > 100",
Result = @"
    if (Input.TotalAmount > 2000)
        Output.TotalDiscount = Input.TotalAmount * 0.25m;
    else if (Input.TotalAmount > 1000)
        Output.TotalDiscount = Input.TotalAmount * 0.15m;
    else
        Output.TotalDiscount = Input.TotalAmount * 0.10m;"
```

### Kategori Bazlı İndirim

```csharp
Predicate = "Input.Categories.Any(c => c == \"Electronics\")",
Result = @"
    var electronicsTotal = Input.Products
        .Where(p => p.Category == ""Electronics"")
        .Sum(p => p.Price.Value);
    Output.TotalDiscount = new Price(electronicsTotal * 0.2m, ""TRY"");"
```

### Tarih Bazlı Kampanya

```csharp
Predicate = @"
    var now = DateTime.Now;
    now.DayOfWeek == DayOfWeek.Friday && 
    now.Hour >= 18 && 
    now.Hour < 22",
Result = "Output.TotalDiscount = Input.TotalAmount * 0.30m;"
```

## 🎨 Custom Repository

```csharp
public class SqlCampaignRepository : ICampaignRepository
{
    private readonly DbContext _context;
    
    public IEnumerable<GeneralCampaign> GetCampaigns(DateTime after, int moduleId)
    {
        return _context.Campaigns
            .Where(c => c.CreateDate > after && c.ModulId == moduleId)
            .ToList();
    }
    
    public IDictionary<string, bool> GetAllCampaigns(IDictionary<string, bool> keys)
    {
        var codes = keys.Keys.ToList();
        var existing = _context.Campaigns
            .Where(c => codes.Contains(c.Code))
            .Select(c => c.Code)
            .ToList();
            
        return keys.ToDictionary(k => k.Key, k => existing.Contains(k.Key));
    }
    
    public bool CheckCampaignQuota(int quota, int campaignId)
    {
        var usageCount = _context.CampaignUsages
            .Count(u => u.CampaignId == campaignId);
        return usageCount < quota;
    }
}

// Register
services.AddSingleton<ICampaignRepository, SqlCampaignRepository>();
```

## 🔍 Debugging

Kural hatalarını görmek için:

```csharp
try
{
    var campaigns = campaignManager.GetCampaign(input);
}
catch (RuleRuntimeException ex)
{
    Console.WriteLine($"Rule Code: {ex.Code}");
    Console.WriteLine($"Input: {ex.Input}");
    Console.WriteLine($"Priority: {ex.Priority}");
    Console.WriteLine($"Error: {ex.Message}");
}
```

## 📊 Performans

- **Cache**: Memory cache ile tekrarlayan sorgular optimize edilir
- **Compilation**: Kurallar bir kez derlenir, cache'lenir
- **Background Processing**: Kural güncellemeleri arka planda işlenir
- **Thread-Safe**: Concurrent dictionary ile güvenli erişim

## 🧪 Test

```csharp
[Fact]
public void Should_Apply_Discount_Campaign()
{
    // Arrange
    var input = new CampaignInput { TotalAmount = 600 };
    
    // Act
    var campaigns = campaignManager.GetCampaign(input);
    
    // Assert
    Assert.NotEmpty(campaigns);
    Assert.True(campaigns.First().TotalDiscount.Value > 0);
}
```

## 📚 Daha Fazla Bilgi

- [RuleEngine.Core Dokumantasyonu](../RuleEngine.Core/README.md)
- [Ornekler](../../examples/)
- [Genel Dokumantasyon](../../docs/index.md)

## 🧾 NuGet Notları

- Paket: `Minima.CampaignEngine.Core`
- Bu paketle gelen yeni eklemeler: available campaign hesaplama, kullanım/geri alma akisi, demo seed yardimcisi
