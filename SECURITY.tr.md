# Güvenlik Politikası

## Desteklenen Sürümler

Aşağıdaki sürümlerde güvenlik açıkları için yamalar yayınlıyoruz:

| Sürüm | Destekleniyor          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Güvenlik Açığı Bildirme

Güvenlik hatalarını ciddiye alıyoruz. Bulgularınızı sorumlu bir şekilde açıkladığınız için çabalarınızı takdir ediyoruz ve katkılarınızı kabul etmek için her türlü çabayı göstereceğiz.

### Güvenlik Açığı Nasıl Bildirilir

**Lütfen güvenlik açıklarını genel GitHub issue'ları aracılığıyla bildirmeyin.**

Bunun yerine, lütfen e-posta yoluyla bildirin: [security@yourdomain.com](mailto:security@yourdomain.com)

48 saat içinde bir yanıt almalısınız. Herhangi bir nedenle almazsanız, orijinal mesajınızı aldığımızdan emin olmak için lütfen e-posta yoluyla takip edin.

Lütfen raporunuza aşağıdaki bilgileri ekleyin:

- Sorun türü (örn. buffer overflow, SQL injection, cross-site scripting, vb.)
- Sorunun ortaya çıkmasıyla ilgili kaynak dosya(lar)ın tam yolları
- Etkilenen kaynak kodun konumu (tag/branch/commit veya doğrudan URL)
- Sorunu yeniden üretmek için gereken özel yapılandırma
- Sorunu yeniden üretmek için adım adım talimatlar
- Proof-of-concept veya exploit kodu (mümkünse)
- Sorunun etkisi, bir saldırganın bunu nasıl istismar edebileceği dahil

### Ne Beklemeli

Bir rapor gönderdikten sonra:

1. 48 saat içinde güvenlik açığı raporunuzun alındığını onaylayacağız
2. İlerlememiz hakkında düzenli güncellemeler sağlayacağız
3. Güvenlik danışma belgelerimizde size kredi vereceğiz (anonim kalmayı tercih etmediğiniz sürece)

### Güvenlik En İyi Uygulamaları

RuleEngine kullanırken lütfen şu güvenlik en iyi uygulamalarını izleyin:

#### Kural Derleme Güvenliği

- **Input Doğrulama**: Kural input verilerini her zaman doğrulayın ve temizleyin
- **Sandboxing**: Güvenilmeyen kurallar için kural derlemesini sandbox ortamında çalıştırmayı düşünün
- **Kaynak Limitleri**: Kural yürütme için uygun timeout ve bellek limitleri ayarlayın

```csharp
// Örnek: Timeout ile güvenli kural derleme
var compiler = new RuleCompiler<MyInput, bool>();
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var rule = await compiler.CompileAsync("safe-rule", ruleString, cts.Token);
```

#### Veritabanı Güvenliği

- **Connection String'ler**: Veritabanı connection string'lerini asla hardcode etmeyin
- **SQL Injection**: Parametreli sorgular kullanın (EF Core bunu otomatik olarak halleder)
- **Erişim Kontrolü**: Uygun kimlik doğrulama ve yetkilendirme uygulayın

```csharp
// Örnek: Güvenli connection string yapılandırması
builder.Services.AddRuleEngine()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
```

#### API Güvenliği

- **Kimlik Doğrulama**: Kural yönetimi API'leri için uygun kimlik doğrulama uygulayın
- **Yetkilendirme**: Kural operasyonları için rol tabanlı erişim kontrolü kullanın
- **Rate Limiting**: Kötüye kullanımı önlemek için rate limiting uygulayın

```csharp
// Örnek: Güvenli API endpoint
[Authorize(Roles = "RuleManager")]
[HttpPost]
public async Task<IActionResult> CreateRule([FromBody] CreateRuleRequest request)
{
    // Implementasyon
}
```

### Bilinen Güvenlik Hususları

#### Kural Derleme

- **Kod Enjeksiyonu**: Kural string'leri C# kodu olarak derlenir. Yalnızca güvenilir kullanıcıların kural oluşturmasına izin verin
- **Kaynak Tükenmesi**: Kötü niyetli kurallar aşırı CPU veya bellek tüketebilir
- **Assembly Yükleme**: Derlenmiş kurallar uygulama domain'ine yüklenir

#### Öneriler

1. **Input Doğrulama**: Kural input verilerini her zaman doğrulayın
2. **Kullanıcı İzinleri**: Kural oluşturmayı yalnızca yetkili kullanıcılarla sınırlayın
3. **İzleme**: Olağandışı desenler için kural yürütmeyi izleyin
4. **Düzenli Güncellemeler**: Güvenlik yamaları için bağımlılıkları güncel tutun

### Güvenlik Güncellemeleri

Gerektiğinde güvenlik güncellemeleri yayınlayacağız. Bir güvenlik açığı keşfedildiğinde:

1. Bir güvenlik danışma belgesi oluşturacağız
2. Mümkün olan en kısa sürede yamalı bir sürüm yayınlayacağız
3. Kullanıcıları GitHub sürümleri ve güvenlik danışma belgeleri aracılığıyla bilgilendireceğiz

### İletişim

Güvenlikle ilgili sorular veya endişeler için lütfen bizimle iletişime geçin:
- E-posta: [security@yourdomain.com](mailto:security@yourdomain.com)
- GitHub Security Advisories: [Danışma belgelerini görüntüle](https://github.com/yourusername/RuleEngine/security/advisories)

### Teşekkürler

Güvenlik açıklarını sorumlu bir şekilde açıklayan aşağıdaki güvenlik araştırmacılarına teşekkür etmek isteriz:

- [Güvenlik araştırmacılarını buraya listeleyin]

---

**Son güncelleme**: Eylül 2025
