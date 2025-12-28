# RuleEngine'e Katkıda Bulunma

RuleEngine'e katkıda bulunma ilginiz için teşekkür ederiz! Bu belge, katkıda bulunanlar için yönergeler ve bilgiler sağlar.

## 🚀 Başlarken

### Ön Gereksinimler

- .NET 8.0 SDK veya üzeri
- Visual Studio 2022, VS Code veya JetBrains Rider
- Git

### Geliştirme Ortamını Kurma

1. **Fork ve Clone**
   ```bash
   git clone https://github.com/your-username/RuleEngine.git
   cd RuleEngine
   ```

2. **Bağımlılıkları Yükleyin**
   ```bash
   dotnet restore
   ```

3. **Çözümü Derleyin**
   ```bash
   dotnet build
   ```

4. **Testleri Çalıştırın**
   ```bash
   dotnet test
   ```

## 🏗️ Proje Yapısı

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/          # Temel kural motoru işlevselliği
│   ├── RuleEngine.Sqlite/        # SQLite persistence katmanı
│   └── RuleEngine.Mvc/           # Örnek MVC uygulaması
├── tests/
│   ├── RuleEngine.Core.Tests/    # Temel işlevsellik için unit testler
│   └── RuleEngine.Integration.Tests/ # Integration testler
├── docs/                         # Dokümantasyon
└── samples/                      # Örnek uygulamalar
```

## 🧪 Test

### Testleri Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Belirli test projesini çalıştır
dotnet test tests/RuleEngine.Core.Tests/

# Coverage ile çalıştır
dotnet test --collect:"XPlat Code Coverage"
```

### Test Yazma

- **xUnit** kullanın
- **FluentAssertions** kullanın
- **AAA pattern** takip edin (Arrange, Act, Assert)
- Hem başarı hem de hata senaryolarını test edin

Örnek:
```csharp
[Fact]
public async Task RuleCompiler_ShouldCompileValidRule()
{
    // Arrange
    var compiler = new RuleCompiler<TestInput, bool>();
    var ruleString = "Input.Value > 10";

    // Act
    var result = compiler.CheckSyntax(ruleString);

    // Assert
    result.Should().BeEmpty();
}
```

## 📝 Kod Stili

### C# Kodlama Standartları

- **Microsoft C# Coding Conventions** takip edin
- Public üyeler için **PascalCase** kullanın
- Private field'lar için **camelCase** kullanın
- Asenkron operasyonlar için **async/await** kullanın
- Uygun yerlerde **nullable reference types** kullanın

### İsimlendirme Kuralları

- **Sınıflar**: `PascalCase` (örn. `RuleCompiler`)
- **Metodlar**: `PascalCase` (örn. `CompileAsync`)
- **Property'ler**: `PascalCase` (örn. `IsActive`)
- **Field'lar**: Private için `camelCase` ve alt çizgi öneki (örn. `_ruleRepository`)
- **Sabitler**: `PascalCase` (örn. `DefaultTimeout`)

### Dokümantasyon

- Public API'ler için **XML dokümantasyonu** kullanın
- Dokümantasyona **örnekler** ekleyin
- **Parametreleri** ve **dönüş değerlerini** belgeleyin
- **Anlamlı commit mesajları** kullanın

Örnek:
```csharp
/// <summary>
/// Bir C# kural string'ini çalıştırılabilir bir fonksiyona derler.
/// </summary>
/// <typeparam name="TInput">Kural için input tipi</typeparam>
/// <typeparam name="TReturn">Kuralın dönüş tipi</typeparam>
/// <param name="ruleName">Kuralın adı</param>
/// <param name="ruleString">Derlenecek C# kodu</param>
/// <returns>Çalıştırılabilir derlenmiş kural</returns>
/// <example>
/// <code>
/// var compiler = new RuleCompiler&lt;CustomerInput, bool&gt;();
/// var rule = await compiler.CompileAsync("age-check", "Input.Age > 18");
/// var result = rule.Invoke(new CustomerInput { Age = 20 });
/// </code>
/// </example>
public async Task<CompiledRule<TInput, TReturn>> CompileAsync(string ruleName, string ruleString)
{
    // Implementasyon
}
```

## 🐛 Hata Raporları

### Göndermeden Önce

1. **Mevcut issue'ları arayın** - tekrardan kaçının
2. **En son sürümle test edin**
3. **Main branch'te düzeltilip düzeltilmediğini kontrol edin**

### Hata Raporu Şablonu

```markdown
**Hatayı Açıklayın**
Hatanın ne olduğuna dair açık ve öz bir açıklama.

**Yeniden Üretme**
Davranışı yeniden üretme adımları:
1. '...' gidin
2. '....' tıklayın
3. '....' kaydırın
4. Hatayı görün

**Beklenen Davranış**
Ne olmasını beklediğinize dair açık ve öz bir açıklama.

**Ekran Görüntüleri**
Uygunsa, sorununuzu açıklamaya yardımcı olacak ekran görüntüleri ekleyin.

**Ortam:**
- İşletim Sistemi: [örn. Windows 10, macOS 12.0, Ubuntu 20.04]
- .NET Sürümü: [örn. 8.0.0]
- RuleEngine Sürümü: [örn. 1.0.0]

**Ek Bağlam**
Sorun hakkında başka bir bağlam ekleyin.
```

## ✨ Özellik İstekleri

### Göndermeden Önce

1. **Mevcut özellik isteklerini kontrol edin**
2. **Proje kapsamına uygun olup olmadığını düşünün**
3. **Açık bir kullanım senaryosu sağlayın**

### Özellik İsteği Şablonu

```markdown
**Özellik isteğiniz bir sorunla mı ilgili? Lütfen açıklayın.**
Sorunun ne olduğuna dair açık ve öz bir açıklama.

**İstediğiniz çözümü açıklayın**
Ne olmasını istediğinize dair açık ve öz bir açıklama.

**Düşündüğünüz alternatifleri açıklayın**
Düşündüğünüz alternatif çözümlerin veya özelliklerin açık ve öz bir açıklaması.

**Ek bağlam**
Özellik isteği hakkında başka bir bağlam veya ekran görüntüsü ekleyin.
```

## 🔄 Pull Request Süreci

### Göndermeden Önce

1. **Main'den bir feature branch oluşturun**
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Kodlama standartlarını takip ederek değişikliklerinizi yapın**

3. **Yeni işlevsellik için testler ekleyin**

4. **Gerekirse dokümantasyonu güncelleyin**

5. **Hiçbir şeyin bozulmadığından emin olmak için tüm testleri çalıştırın**
   ```bash
   dotnet test
   ```

6. **Açık bir mesajla değişikliklerinizi commit edin**
   ```bash
   git commit -m "feat: Add amazing feature"
   ```

7. **Fork'unuza push edin**
   ```bash
   git push origin feature/amazing-feature
   ```

### Pull Request Şablonu

```markdown
**Açıklama**
Değişikliklerin kısa açıklaması.

**Değişiklik Tipi**
- [ ] Hata düzeltme (mevcut işlevselliği bozmayan değişiklik)
- [ ] Yeni özellik (mevcut işlevselliği bozmayan değişiklik)
- [ ] Breaking change (mevcut işlevselliğin çalışmamasına neden olacak düzeltme veya özellik)
- [ ] Dokümantasyon güncellemesi

**Test**
- [ ] Unit testler geçti
- [ ] Integration testler geçti
- [ ] Manuel test tamamlandı

**Kontrol Listesi**
- [ ] Kod projenin kodlama standartlarını takip ediyor
- [ ] Self-review tamamlandı
- [ ] Dokümantasyon güncellendi
- [ ] Testler eklendi/güncellendi
```

## 🏷️ Sürüm Süreci

### Versiyonlama

[Semantic Versioning](https://semver.org/) takip ediyoruz:
- **MAJOR**: Breaking değişiklikler
- **MINOR**: Yeni özellikler (geriye dönük uyumlu)
- **PATCH**: Hata düzeltmeleri (geriye dönük uyumlu)

### Sürüm Kontrol Listesi

- [ ] Tüm testler geçti
- [ ] Dokümantasyon güncellendi
- [ ] Versiyon numaraları güncellendi
- [ ] CHANGELOG.md güncellendi
- [ ] NuGet paketleri oluşturuldu
- [ ] Sürüm notları hazırlandı

## 📚 Kaynaklar

- [.NET Dokümantasyonu](https://docs.microsoft.com/tr-tr/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/tr-tr/ef/core/)
- [Roslyn Dokümantasyonu](https://docs.microsoft.com/tr-tr/dotnet/csharp/roslyn-sdk/)
- [xUnit Testing](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)

## 🤝 Topluluk Kuralları

### Davranış Kuralları

- **Saygılı** ve kapsayıcı olun
- Geri bildirimlerde **yapıcı** olun
- Yeni gelenlere **sabırlı** olun
- Tartışmalarda **işbirlikçi** olun

### Yardım Alma

- **GitHub Discussions** sorular ve fikirler için
- **GitHub Issues** hatalar ve özellik istekleri için
- **Pull Requests** kod katkıları için

## 📞 İletişim

- **Maintainer**: [Your Name](mailto:your.email@example.com)
- **GitHub**: [@yourusername](https://github.com/yourusername)
- **Twitter**: [@yourusername](https://twitter.com/yourusername)

---

RuleEngine'e katkıda bulunduğunuz için teşekkür ederiz! 🚀
