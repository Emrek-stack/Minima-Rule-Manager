# RuleEngine Katki Rehberi

RuleEngine'e katki saglamak istediginiz icin tesekkurler! Bu belge, katilimcilar icin rehber ve bilgiler sunar.

## 🚀 Baslangic

### Onkosullar

- .NET 8.0 SDK veya ustu
- Visual Studio 2022, VS Code veya JetBrains Rider
- Git

### Gelistirme Ortami Kurulumu

1. **Fork ve Clone**
   ```bash
   git clone https://github.com/your-username/RuleEngine.git
   cd RuleEngine
   ```

2. **Bagimliliklari Yukle**
   ```bash
   dotnet restore
   ```

3. **Solution Build**
   ```bash
   dotnet build
   ```

4. **Testleri Calistir**
   ```bash
   dotnet test
   ```

## 🏗️ Proje Yapisi

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/          # Core kural motoru
│   ├── RuleEngine.Sqlite/        # SQLite kalicilik katmani
│   └── RuleEngine.Mvc/           # MVC ornek uygulama
├── tests/
│   ├── RuleEngine.Core.Tests/    # Core unit testleri
│   └── RuleEngine.Integration.Tests/ # Integration testler
├── docs/                         # Dokumantasyon
└── samples/                      # Ornek uygulamalar
```

## 🧪 Test

### Test Calistirma

```bash
# Tum testler
 dotnet test

# Belirli test projesi
 dotnet test tests/RuleEngine.Core.Tests/

# Coverage
 dotnet test --collect:"XPlat Code Coverage"
```

### Test Yazimi

- Unit test icin **xUnit** kullanin
- Assertion icin **FluentAssertions** kullanin
- **AAA** pattern izleyin (Arrange, Act, Assert)
- Basarili ve basarisiz senaryolari test edin

Ornek:
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

## 📝 Kod Standartlari

### C# Standartlari

- **Microsoft C# Coding Conventions** takip edin
- Public uyeler icin **PascalCase**
- Private alanlar icin **camelCase**
- Asenkron islemler icin **async/await**
- Uygun yerde nullable reference types

### Isimlendirme

- **Siniflar**: `PascalCase`
- **Metodlar**: `PascalCase`
- **Ozellikler**: `PascalCase`
- **Alanlar**: `_camelCase`
- **Sabitler**: `PascalCase`

### Dokumantasyon

- Public API'ler icin **XML doc**
- Dokumanda **ornek** kullanin
- **Parametre** ve **return** aciklamalari ekleyin
- **Anlamli commit mesajlari** kullanin

Ornek:
```csharp
/// <summary>
/// Compiles a C# rule string into an executable function.
/// </summary>
/// <typeparam name="TInput">The input type for the rule</typeparam>
/// <typeparam name="TReturn">The return type of the rule</typeparam>
/// <param name="ruleName">The name of the rule</param>
/// <param name="ruleString">The C# code to compile</param>
/// <returns>A compiled rule that can be executed</returns>
/// <example>
/// <code>
/// var compiler = new RuleCompiler&lt;CustomerInput, bool&gt;();
/// var rule = await compiler.CompileAsync("age-check", "Input.Age > 18");
/// var result = rule.Invoke(new CustomerInput { Age = 20 });
/// </code>
/// </example>
public async Task<CompiledRule<TInput, TReturn>> CompileAsync(string ruleName, string ruleString)
{
    // Implementation
}
```

## 🐛 Hata Bildirimi

### Gondermeden Once

1. Mevcut issue'lari kontrol edin
2. En yeni surumle test edin
3. Ana branch'te duzeltilip duzeltilmedigine bakin

### Hata Bildirim Sablonu

```markdown
**Hatanin tanimi**
Hatanin acik ve kisa tanimi.

**Tekrar etme adimlari**
1. ...
2. ...
3. ...

**Beklenen davranis**
Beklenen sonuc.

**Ekran goruntuleri**
Uygunsa ekleyin.

**Ortam:**
- OS: [or. Windows 10, macOS 12.0, Ubuntu 20.04]
- .NET Version: [or. 8.0.0]
- RuleEngine Version: [or. 1.0.0]

**Ek notlar**
Ek baglam.
```

## ✨ Ozellik Istegi

### Gondermeden Once

1. Mevcut ozellik isteklerini kontrol edin
2. Proje kapsamına uygunlugunu degerlendirin
3. Net bir use case verin

### Ozellik Istegi Sablonu

```markdown
**Sorunla ilgili mi?**
Sorunun tanimi.

**Istenen cozum**
Istenen davranis.

**Alternatifler**
Dusunulen alternatifler.

**Ek notlar**
Ek baglam veya ekran goruntuleri.
```

## 🔄 Pull Request Sureci

### Gondermeden Once

- Branch’in guncel oldugundan emin olun
- Testleri calistirin
- Dokumantasyon guncellemelerini ekleyin

### Review Sureci

- Kod kalite kontrolu
- Testlerin basarili olmasi
- Dokumantasyon kontrolu

Tesekkurler!
