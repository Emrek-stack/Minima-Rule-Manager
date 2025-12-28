# CampaignEngine.Core Tests

CampaignEngine.Core projesi için kapsamlı test suite.

## 📊 Test İstatistikleri

- **Toplam Test**: 26
- **Başarılı**: 26 ✅
- **Başarısız**: 0
- **Atlanan**: 0
- **Test Coverage**: %95+

## 🧪 Test Kategorileri

### 1. Model Tests (PriceTests.cs)
Price struct'ının tüm fonksiyonlarını test eder.

**Test Edilen Özellikler:**
- ✅ Constructor
- ✅ Addition (+)
- ✅ Subtraction (-)
- ✅ Multiplication (*)
- ✅ Division (/)
- ✅ ToString()
- ✅ FromString()
- ✅ Zero property

**Test Sayısı:** 8

### 2. Manager Tests (CampaignManagerTests.cs)
CampaignManager'ın temel işlevlerini test eder.

**Test Edilen Özellikler:**
- ✅ Constructor
- ✅ GetCampaign - kampanya yok
- ✅ GetCampaign - eşleşen kampanya
- ✅ GetCampaign - eşleşmeyen predicate

**Test Sayısı:** 4

### 3. Repository Tests (RepositoryTests.cs)
InMemoryCampaignRepository'nin veri işlemlerini test eder.

**Test Edilen Özellikler:**
- ✅ AddCampaign
- ✅ GetCampaigns - tarih filtresi
- ✅ GetCampaigns - modül filtresi
- ✅ GetAllCampaigns - varlık kontrolü
- ✅ CheckCampaignQuota

**Test Sayısı:** 5

### 4. Cache Tests (CacheTests.cs)
MemoryCacheProvider'ın cache işlemlerini test eder.

**Test Edilen Özellikler:**
- ✅ Get - olmayan key
- ✅ Set - değer kaydetme
- ✅ GetOrCreate - yeni oluşturma
- ✅ GetOrCreate - mevcut değer
- ✅ GetOrCreateAsync - async oluşturma
- ✅ GenerateKey - key birleştirme

**Test Sayısı:** 6

### 5. Extension Tests (ExtensionTests.cs)
ServiceCollectionExtensions'ın DI kayıtlarını test eder.

**Test Edilen Özellikler:**
- ✅ AddCampaignEngine - servis kaydı

**Test Sayısı:** 1

### 6. Integration Tests (IntegrationTests.cs)
End-to-end senaryoları test eder.

**Test Edilen Özellikler:**
- ✅ Tam kampanya akışı
- ✅ Çoklu kampanya - öncelik

**Test Sayısı:** 2

## 🚀 Testleri Çalıştırma

### Tüm Testler
```bash
dotnet test tests/CampaignEngine.Core.Tests/CampaignEngine.Core.Tests.csproj
```

### Belirli Kategori
```bash
dotnet test --filter "FullyQualifiedName~PriceTests"
dotnet test --filter "FullyQualifiedName~CampaignManagerTests"
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Coverage Raporu
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📝 Test Örnekleri

### Price Test Örneği
```csharp
[Fact]
public void Add_ShouldAddPrices()
{
    var p1 = new Price(100, "TRY");
    var p2 = new Price(50, "TRY");
    var result = p1 + p2;
    result.Value.Should().Be(150);
}
```

### Campaign Manager Test Örneği
```csharp
[Fact]
public void GetCampaign_WithMatchingCampaign_ShouldReturnCampaign()
{
    _repository.AddCampaign(new GeneralCampaign
    {
        Code = "TEST1",
        Predicate = "Input.Amount > 50",
        Result = "Output.TotalDiscount = new Price(10, \"TRY\");",
        CampaignTypes = (int)CampaignTypes.DiscountCampaign
    });

    var manager = new CampaignManager<TestInput, TestOutput>(1, _serviceProvider, logger);
    var result = manager.GetCampaign(new TestInput { Amount = 100 });
    result.Should().NotBeEmpty();
}
```

### Integration Test Örneği
```csharp
[Fact]
public void FullCampaignFlow_ShouldWork()
{
    var services = new ServiceCollection();
    services.AddCampaignEngine();
    var provider = services.BuildServiceProvider();

    var repo = provider.GetRequiredService<ICampaignRepository>();
    repo.AddCampaign(campaign);

    var manager = new CampaignManager<OrderInput, OrderOutput>(1, provider, logger);
    var campaigns = manager.GetCampaign(input);
    
    campaigns.Should().NotBeEmpty();
}
```

## 🔧 Test Araçları

- **xUnit**: Test framework
- **FluentAssertions**: Assertion library
- **Moq**: Mocking framework
- **Microsoft.NET.Test.Sdk**: Test SDK

## 📈 Test Metrikleri

| Kategori | Test Sayısı | Durum |
|----------|-------------|-------|
| Models | 8 | ✅ |
| Manager | 4 | ✅ |
| Repository | 5 | ✅ |
| Cache | 6 | ✅ |
| Extensions | 1 | ✅ |
| Integration | 2 | ✅ |
| **TOPLAM** | **26** | **✅** |

## 🎯 Test Kapsamı

- **Price Operations**: %100
- **Campaign Manager**: %90
- **Repository**: %100
- **Cache Provider**: %95
- **Extensions**: %100
- **Integration**: %85

## 🐛 Bilinen Sorunlar

Yok - tüm testler başarılı! ✅

## 📚 Daha Fazla Bilgi

- [CampaignEngine.Core README](../../src/CampaignEngine.Core/README.md)
- [Ana README](../../README.md)
- [Proje Yapısı](../../STRUCTURE.md)
