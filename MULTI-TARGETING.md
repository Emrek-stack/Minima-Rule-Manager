# Multi-Targeting Support

Tüm NuGet paketleri artık .NET 8.0, .NET 9.0 ve .NET 10.0 desteği ile geliyor.

## 📦 Desteklenen Framework'ler

| Paket | .NET 8.0 | .NET 9.0 | .NET 10.0 | Versiyon |
|-------|----------|----------|-----------|----------|
| **Minima.RuleEngine.Core** | ✅ | ✅ | ✅ | v1.0.3 |
| **Minima.RuleEngine.Sqlite** | ✅ | ✅ | ✅ | v1.0.3 |
| **Minima.CampaignEngine.Core** | ✅ | ✅ | ✅ | v1.0.2 |

## 🚀 Kullanım

### .NET 8.0 Projesi
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Minima.RuleEngine.Core" Version="1.0.2" />
    <PackageReference Include="Minima.CampaignEngine.Core" Version="1.0.1" />
  </ItemGroup>
</Project>
```

### .NET 9.0 Projesi
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Minima.RuleEngine.Core" Version="1.0.2" />
    <PackageReference Include="Minima.CampaignEngine.Core" Version="1.0.1" />
  </ItemGroup>
</Project>
```

### .NET 10.0 Projesi
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Minima.RuleEngine.Core" Version="1.0.3" />
    <PackageReference Include="Minima.CampaignEngine.Core" Version="1.0.2" />
  </ItemGroup>
</Project>
```

## 📊 NuGet Paket İçeriği

### Minima.RuleEngine.Core v1.0.3
```
lib/
├── net8.0/
│   └── RuleEngine.Core.dll
├── net9.0/
│   └── RuleEngine.Core.dll
└── net10.0/
    └── RuleEngine.Core.dll
```

### Minima.CampaignEngine.Core v1.0.2
```
lib/
├── net8.0/
│   └── CampaignEngine.Core.dll
├── net9.0/
│   └── CampaignEngine.Core.dll
└── net10.0/
    └── CampaignEngine.Core.dll
```

### Minima.RuleEngine.Sqlite v1.0.3
```
lib/
├── net8.0/
│   └── RuleEngine.Sqlite.dll
├── net9.0/
│   └── RuleEngine.Sqlite.dll
└── net10.0/
    └── RuleEngine.Sqlite.dll
```

## ✅ Test Sonuçları

Tüm testler .NET 8.0, 9.0 ve 10.0 için başarılı:

```
✅ RuleEngine.Core.Tests: 5/5 passed
✅ RuleEngine.Integration.Tests: 2/2 passed
✅ CampaignEngine.Core.Tests: 26/26 passed
```

## 🔧 Build

```bash
# Tüm framework'ler için build
dotnet build --configuration Release

# Belirli framework için build
dotnet build --configuration Release --framework net8.0
dotnet build --configuration Release --framework net9.0
dotnet build --configuration Release --framework net10.0
```

## 📦 NuGet Pack

```bash
# Paketler otomatik oluşturulur
dotnet build --configuration Release

# Manuel pack
dotnet pack --configuration Release
```

## 🎯 Avantajlar

- ✅ Tek paket, çoklu framework desteği
- ✅ Geriye dönük uyumluluk (.NET 8.0)
- ✅ İleriye dönük uyumluluk (.NET 9.0 & 10.0)
- ✅ Otomatik framework seçimi
- ✅ Aynı API, farklı runtime'lar

## 📝 Notlar

- ✅ .NET 10.0 desteği eklendi!
- Tüm paketler aynı API'yi kullanır
- Framework-specific kod yok
- Tüm özellikler her üç framework'te de çalışır
