# Degisim Gunlugu

RuleEngine icin tum onemli degisiklikler bu dosyada belgelenir.

Format, [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) temeline dayanir ve bu proje [SemVer](https://semver.org/spec/v2.0.0.html) kurallarini izler.

## [Yayinlanmadi]

### Eklendi
- RuleEngine ilk surumu
- Core kural derleme ve calistirma motoru
- SQLite kalicilik katmani
- ASP.NET Core MVC ornek uygulamasi
- KnockoutJS kural builder arayuzu
- Kapsamli test paketi

## [1.0.0] - 2025-09-22

### Eklendi
- **Core Rule Engine**
  - Dinamik C# kural derleme icin `RuleCompiler<TInput, TReturn>`
  - Karmasik kural calistirma icin `RuleSet<TInput, TOutput>`
  - Kural yonetimi icin `RuleManager` static sinifi
  - Ayrintili hata raporlama ile syntax dogrulama
  - Tum islemler icin async/await destegi
  - Tip-guvenli generic input/output modelleri

- **SQLite Persistence**
  - Kural CRUD islemleri icin `IRuleRepository`
  - Calistirma audit kayitlari icin `IAuditRepository`
  - Entity Framework Core entegrasyonu
  - Migration ve seeding
  - Kural versiyonlama ve rollback
  - Import/export destegi

- **MVC Ornek Uygulamasi**
  - Tam ASP.NET Core MVC uygulamasi
  - CRUD ile kural yonetimi arayuzu
  - KnockoutJS tabanli gorsel kural builder
  - Surukle-birak kural olusturma
  - C# kod duzenleme icin ACE Editor
  - Calistirma gecmisi ve izleme
  - Bootstrap tabanli responsive tasarim

- **Rule Builder UI**
  - KnockoutJS tabanli kural builder
  - Surukle-birak kural olusturma
  - Dinamik parametre yonetimi
  - Gercek zamanli dogrulama
  - JSON tabanli kural serilestirme
  - Select2 dropdown ve Sortable.js entegrasyonu

- **Testing**
  - Kapsamli unit test paketi
  - SQLite ile integration testleri
  - xUnit ve FluentAssertions
  - Core fonksiyonlar icin test coverage
  - Repository’ler icin mock tabanli testler

- **Documentation**
  - Orneklerle README
  - XML comment tabanli API dokumani
  - Katki rehberi
  - Davranis kurallari
  - Lisans (MIT)

### Teknik Detaylar
- **Hedef Framework**: .NET 8.0
- **Bagimliliklar**:
  - Microsoft.CodeAnalysis.CSharp.Scripting 4.8.0
  - Microsoft.EntityFrameworkCore.Sqlite 8.0.0
  - Microsoft.Extensions.DependencyInjection 8.0.0
- **Performans**:
  - Kural derleme: ~50-100ms
  - Kural calistirma: ~0.1-1ms
  - Thread-safe es zamanli calistirma
- **Ozellikler**:
  - Roslyn tabanli C# derleme
  - EF Core ile SQLite veritabani
  - RESTful API endpointleri
  - Gercek zamanli kural dogrulama
  - Kapsamli hata yonetimi

### Ornekler
- E-ticaret fiyatlama kurallari
- Kredi onay akislari
- Musteri dogrulama mantigi
- Dinamik is kurallari
- Kosullu mantik calistirma

### NuGet Paketleri
- `RuleEngine.Core` - Core kural motoru
- `RuleEngine.Sqlite` - SQLite kalicilik katmani

---

## Surum Gecmisi

### 1.0.0 (2025-09-22)
- Ilk surum
- Tam kural motoru implementasyonu
- SQLite kalicilik
- MVC ornek uygulamasi
- Kapsamli dokumantasyon

---

## Gecis Rehberi

### Gecis Notlari

RuleEngine, eski bir kural motorunun .NET 8 ile yeniden yazilmis halidir:

1. **Static RuleManager**: Uyumluluk icin static yapisi korunur
2. **RuleCompiler**: Daha iyi hata yonetimi ve async destek
3. **RuleSet**: Tip guvenligi ve performans iyilestirmeleri
4. **Persistence**: EF Core ile SQLite destegi
5. **UI**: KnockoutJS arayuzu modernleştirildi

### Kiran Degisiklikler
- 1.0.0 icin yok (yeni proje)

### Yeni Ozellikler
- SQLite kalicilik katmani
- ASP.NET Core MVC entegrasyonu
- Gelismis hata yonetimi
- Daha iyi performans
- Modern .NET 8 ozellikleri

---

## Destek

- **GitHub Issues**: [Hata bildirimi ve istek](https://github.com/yourusername/RuleEngine/issues)
- **GitHub Discussions**: [Soru/Paylasim](https://github.com/yourusername/RuleEngine/discussions)
- **Dokumantasyon**: [Tam dokumantasyon](https://github.com/yourusername/RuleEngine/wiki)
