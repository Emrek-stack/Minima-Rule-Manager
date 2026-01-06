# Release Scriptleri

RuleEngine icin otomatik release scriptleri.

## Ozellikler

- Minor versiyonu otomatik arttirir
- Tum .csproj dosyalarini gunceller
- NuGet paketlerini build eder ve pack'ler
- NuGet.org'a publish eder
- GitHub release olusturur

## Onkosullar

- .NET SDK 8.0+
- NuGet API Key (publish icin)
- GitHub Personal Access Token (release icin)

## Kullanim

### PowerShell (Windows/macOS/Linux)

```powershell
$env:NUGET_API_KEY = "your-nuget-api-key"
$env:GITHUB_TOKEN = "your-github-token"

./release.ps1
```

### Bash (macOS/Linux)

```bash
export NUGET_API_KEY="your-nuget-api-key"
export GITHUB_TOKEN="your-github-token"

./release.sh
```

## Neler Yapar

1. **Versiyon Artirma**: `RuleEngine.Core.csproj` icindeki versiyonu okur ve minor arttirir
2. **Projeleri Guncelleme**: Uc projede surumu gunceller
3. **Build & Pack**: Release build ve paketleme
4. **NuGet Publish**: Paketleri NuGet.org'a gonderir
5. **GitHub Release**: Tag ve release notlari olusturur

## Ortam Degiskenleri

| Degisken | Zorunlu | Aciklama |
| --- | --- | --- |
| `NUGET_API_KEY` | Opsiyonel | NuGet publish icin API key |
| `GITHUB_TOKEN` | Opsiyonel | GitHub release icin token |

Not: Degiskenler yoksa ilgili adimlar atlanir.

## Sorun Giderme

### Permission Denied (Bash)

```bash
chmod +x release.sh
```

### NuGet Push Hata

- API key'in gecerli oldugunu kontrol edin
- Paket surumunun mevcut olmadigindan emin olun
- Baglanti sorunlarini kontrol edin

### GitHub Release Hata

- Token scope (repo) dogru mu?
- Git remote URL dogru mu?
- Repo uzerinde yetkiniz var mi?

## Manuel Adimlar

```bash
# 1) Surumleri manuel guncelleyin
# 2) Build ve pack
 dotnet build --configuration Release
 dotnet pack --configuration Release --no-build

# 3) NuGet push
 dotnet nuget push src/RuleEngine.Core/bin/Release/*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json

# 4) GitHub release'i manuel olusturun
```
