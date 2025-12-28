# Proje Yapısı

## 📁 Dizin Yapısı

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/              # Kural motoru çekirdeği
│   │   ├── Rule/                     # Kural yönetimi (RuleManager, RuleCompiler, RuleSet)
│   │   ├── Models/                   # Veri modelleri (CompiledRule, RuleInputModel)
│   │   ├── Abstractions/             # Interface'ler (IRuleProvider, IRuleRepository)
│   │   └── Extensions/               # Extension metodlar
│   │
│   ├── CampaignEngine.Core/          # ⭐ Kampanya motoru (YENİ)
│   │   ├── Models/                   # Kampanya modelleri (GeneralCampaign, Price)
│   │   ├── Abstractions/             # Interface'ler (ICampaignRepository, ITravelProduct)
│   │   ├── Cache/                    # Önbellek sağlayıcıları (MemoryCacheProvider)
│   │   ├── Repositories/             # Veri erişim (InMemoryCampaignRepository)
│   │   ├── Extensions/               # Extension metodlar (ServiceCollectionExtensions)
│   │   ├── CampaignManager.cs        # Ana kampanya yöneticisi
│   │   └── README.md                 # Detaylı dokümantasyon
│   │
│   ├── RuleEngine.Sqlite/            # SQLite implementasyonu
│   └── RuleEngine.Mvc/               # Web UI
│
├── examples/
│   └── CampaignEngine.Example/       # ⭐ Kampanya örneği (YENİ)
│
├── tests/
│   ├── RuleEngine.Core.Tests/
│   ├── RuleEngine.Integration.Tests/
│   └── CampaignEngine.Core.Tests/      # ⭐ Kampanya testleri (YENİ)
│       ├── PriceTests.cs               # Price model testleri
│       ├── CampaignManagerTests.cs     # Manager testleri
│       ├── RepositoryTests.cs          # Repository testleri
│       ├── CacheTests.cs               # Cache testleri
│       ├── ExtensionTests.cs           # Extension testleri
│       ├── IntegrationTests.cs         # Integration testleri
│       └── README.md                   # Test dokümantasyonu
│
└── README.md                          # Ana dokümantasyon
```

## 🔗 Proje Bağımlılıkları

```
CampaignEngine.Core
    └── RuleEngine.Core
        └── Microsoft.CodeAnalysis.CSharp.Scripting
        └── Microsoft.Extensions.*

CampaignEngine.Example
    └── CampaignEngine.Core
```

## 📦 NuGet Paketleri

### Üretilen Paketler
- `Minima.RuleEngine.Core` (v1.0.3) - .NET 8.0, 9.0 & 10.0 ⭐
- `Minima.RuleEngine.Sqlite` (v1.0.3) - .NET 8.0, 9.0 & 10.0 ⭐
- `Minima.CampaignEngine.Core` (v1.0.2) - .NET 8.0, 9.0 & 10.0 ⭐

### Bağımlılıklar
- Microsoft.CodeAnalysis.CSharp.Scripting 4.8.0
- Microsoft.Extensions.DependencyInjection 8.0.0
- Microsoft.Extensions.Logging 8.0.0
- Microsoft.Extensions.Caching.Memory 8.0.0
- Newtonsoft.Json 13.0.3

## 🎯 Temel Sınıflar

### RuleEngine.Core

#### RuleManager
- Kural setlerini provider bazında yönetir
- Background processing ile otomatik güncelleme
- Thread-safe operasyonlar

#### RuleCompiler<TInput, TReturn>
- C# expression'larını derler
- Roslyn kullanarak runtime compilation
- Syntax kontrolü

#### RuleSet<TInput, TOutput>
- Predicate (seçim) kuralı
- Result (sonuç) kuralı
- Priority (öncelik)

### CampaignEngine.Core ⭐

#### CampaignManager<TInput, TOutput>
- Kampanya yönetimi
- RuleEngine.Core entegrasyonu
- Kural bazlı kampanya seçimi

#### GeneralCampaign
- Kampanya entity modeli
- Predicate, Result, Usage kuralları
- Kota ve öncelik yönetimi

#### Price
- Para birimi desteği (ISO 4217)
- Matematiksel operatörler
- JSON serialization

## 🔄 Veri Akışı

### RuleEngine
```
Input → RuleProvider → RuleManager → RuleSet → CompiledRule → Output
```

### CampaignEngine
```
CampaignInput → CampaignManager → RuleProvider → RuleSet → CampaignOutput
                                        ↓
                                  ICampaignRepository
```

## 🛠️ Genişletme Noktaları

### Custom Repository
```csharp
public class MyCampaignRepository : ICampaignRepository
{
    // Kendi veri kaynağınızı kullanın
}
```

### Custom Cache Provider
```csharp
public class RedisCacheProvider : ICacheProvider
{
    // Redis veya başka cache sistemi
}
```

### Custom Rule Provider
```csharp
public class MyRuleProvider : IRuleProvider<MyRuleSet, MyInput, MyOutput>
{
    // Özel kural sağlayıcı
}
```

## 📊 Performans Özellikleri

- **Compilation Cache**: Kurallar bir kez derlenir
- **Memory Cache**: Sık kullanılan veriler cache'lenir
- **Background Processing**: Kural güncellemeleri arka planda
- **Thread-Safe**: ConcurrentDictionary kullanımı
- **Lazy Loading**: İhtiyaç anında yükleme

## 🔐 Güvenlik

- Input validation
- SQL injection koruması (parametreli sorgular)
- Expression injection koruması
- Kural syntax kontrolü

## 📝 Lisans

MIT License
