# E-ticaret İçin RuleEngine Kullanım Örnekleri

Bu dokümantasyon, RuleEngine kütüphanesini e-ticaret sistemlerinde nasıl kullanacağınızı gösterir. Örnekler basit kurallardan karmaşık kurallara doğru sıralanmıştır.

## İçindekiler

- [Temel Kurulum](#temel-kurulum)
- [Basit Kurallar](#basit-kurallar)
- [Orta Seviye Kurallar](#orta-seviye-kurallar)
- [Karmaşık Kurallar](#karmaşık-kurallar)
- [Performans Optimizasyonları](#performans-optimizasyonları)
- [Gerçek Dünya Senaryoları](#gerçek-dünya-senaryoları)

## Temel Kurulum

### 1. Paketleri Yükleyin

```bash
dotnet add package Minima.RuleEngine.Core
dotnet add package Minima.RuleEngine.Sqlite
```

### 2. Temel Modelleri Oluşturun

```csharp
using Minima.RuleEngine.Core.Rule;
using Minima.RuleEngine.Core.Models;

// E-ticaret için input modeli
public class EcommerceOrderInput : RuleInputModel
{
    public decimal OrderAmount { get; set; }
    public string CustomerType { get; set; } = string.Empty; // "VIP", "Regular", "Premium"
    public int ItemCount { get; set; }
    public bool IsFirstTimeCustomer { get; set; }
    public int CustomerLoyaltyPoints { get; set; }
    public string ProductCategory { get; set; } = string.Empty; // "Electronics", "Clothing", "Books"
    public DateTime OrderDate { get; set; }
    public string CustomerCountry { get; set; } = string.Empty;
    public bool HasCoupon { get; set; }
    public string CouponCode { get; set; } = string.Empty;
    public List<CartItem> CartItems { get; set; } = new();
}

public class CartItem
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
}

// E-ticaret için output modeli
public class EcommercePricingOutput
{
    public decimal DiscountPercentage { get; set; }
    public decimal FinalAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string ShippingMethod { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> AppliedPromotions { get; set; } = new();
    public bool FreeShipping { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public decimal GiftVoucher { get; set; }
    public List<string> Suggestions { get; set; } = new();
}
```

## Basit Kurallar

### 1. VIP Müşteri İndirimi

```csharp
// Kural: VIP müşteriler için %20 indirim
var vipRule = "Input.CustomerType == \"VIP\"";
var vipResult = @"
    Output.DiscountPercentage = 20;
    Output.FinalAmount = Input.OrderAmount * 0.8;
    Output.Message = ""VIP müşteri indirimi uygulandı!"";
    Output.AppliedPromotions.Add(""VIP Discount"");
";

// Kullanım
var compiler = new RuleCompiler<EcommerceOrderInput, EcommercePricingOutput>();
var compiledRule = await compiler.CompileAsync("vip-discount", vipResult);

var order = new EcommerceOrderInput
{
    OrderAmount = 500,
    CustomerType = "VIP"
};

var result = compiledRule.Invoke(order);
// Sonuç: %20 indirim, FinalAmount = 400
```

### 2. Toplu Sipariş İndirimi

```csharp
// Kural: 10 veya daha fazla ürün için %15 indirim
var bulkRule = "Input.ItemCount >= 10";
var bulkResult = @"
    Output.DiscountPercentage = 15;
    Output.FinalAmount = Input.OrderAmount * 0.85;
    Output.Message = ""Toplu sipariş indirimi uygulandı!"";
    Output.AppliedPromotions.Add(""Bulk Order Discount"");
";
```

### 3. İlk Kez Alışveriş Yapan Müşteri

```csharp
// Kural: İlk kez alışveriş yapan müşteriler için %10 indirim
var firstTimeRule = "Input.IsFirstTimeCustomer == true";
var firstTimeResult = @"
    Output.DiscountPercentage = 10;
    Output.FinalAmount = Input.OrderAmount * 0.9;
    Output.Message = ""Hoş geldin indirimi uygulandı!"";
    Output.AppliedPromotions.Add(""Welcome Discount"");
";
```

### 4. Ücretsiz Kargo

```csharp
// Kural: 200 TL ve üzeri siparişlerde ücretsiz kargo
var freeShippingRule = "Input.OrderAmount >= 200";
var freeShippingResult = @"
    Output.ShippingCost = 0;
    Output.FreeShipping = true;
    Output.ShippingMethod = ""Standard"";
    Output.EstimatedDeliveryDays = 3;
    Output.Message = ""Ücretsiz kargo kazandınız!"";
    Output.AppliedPromotions.Add(""Free Shipping"");
";
```

## Orta Seviye Kurallar

### 1. Çoklu Koşul Kuralı

```csharp
// Kural: VIP müşteri + Elektronik ürün + 1000 TL üzeri
var multiConditionRule = @"
    Input.CustomerType == ""VIP"" && 
    Input.ProductCategory == ""Electronics"" && 
    Input.OrderAmount > 1000
";

var multiConditionResult = @"
    Output.DiscountPercentage = 25;
    Output.FinalAmount = Input.OrderAmount * 0.75;
    Output.ShippingCost = 0;
    Output.FreeShipping = true;
    Output.ShippingMethod = ""Express"";
    Output.EstimatedDeliveryDays = 1;
    Output.AppliedPromotions.Add(""VIP Electronics Discount"");
    Output.Message = ""VIP elektronik ürün süper indirimi!"";
";
```

### 2. Sadakat Puanı Kuralı

```csharp
// Kural: Sadakat puanına göre indirim
var loyaltyRule = "Input.CustomerLoyaltyPoints > 1000";
var loyaltyResult = @"
    var loyaltyDiscount = Math.Min(Input.CustomerLoyaltyPoints / 10000.0, 0.3);
    Output.DiscountPercentage = loyaltyDiscount * 100;
    Output.FinalAmount = Input.OrderAmount * (1 - loyaltyDiscount);
    Output.AppliedPromotions.Add(""Loyalty Points Discount"");
    Output.Message = $""Sadakat puanınızla {loyaltyDiscount:P} indirim kazandınız!"";
";
```

### 3. Sezonluk Kampanya

```csharp
// Kural: Aralık ayında elektronik ürünler için %30 indirim
var seasonalRule = "Input.OrderDate.Month == 12 && Input.ProductCategory == \"Electronics\"";
var seasonalResult = @"
    Output.DiscountPercentage = 30;
    Output.FinalAmount = Input.OrderAmount * 0.7;
    Output.AppliedPromotions.Add(""Christmas Electronics Sale"");
    Output.Message = ""Yılbaşı elektronik kampanyası!"";
";
```

### 4. Kupon Sistemi

```csharp
// Kural: Geçerli kupon kodu kontrolü
var couponRule = "Input.HasCoupon == true && Input.CouponCode == \"SAVE20\"";
var couponResult = @"
    Output.DiscountPercentage = 20;
    Output.FinalAmount = Input.OrderAmount * 0.8;
    Output.AppliedPromotions.Add(""Coupon: SAVE20"");
    Output.Message = ""Kupon indirimi uygulandı!"";
";
```

## Karmaşık Kurallar

### 1. Dinamik Fiyat Hesaplama

```csharp
// Kural: Talep, sezon ve müşteri tipine göre dinamik fiyat
var dynamicPricingRule = @"
    var basePrice = Input.OrderAmount;
    var demandMultiplier = Input.ItemCount < 5 ? 1.2 : 1.0; // Az stok = yüksek fiyat
    var seasonMultiplier = Input.OrderDate.Month == 12 ? 1.5 : 1.0; // Aralık = yüksek fiyat
    var customerMultiplier = Input.CustomerType == ""VIP"" ? 0.8 : 1.0; // VIP = düşük fiyat
    
    var finalPrice = basePrice * demandMultiplier * seasonMultiplier * customerMultiplier;
    
    Output.FinalAmount = Math.Round(finalPrice, 2);
    Output.Message = $""Dinamik fiyat: Base({basePrice}) × Demand({demandMultiplier}) × Season({seasonMultiplier}) × Customer({customerMultiplier})"";
";
```

### 2. Sepet Analizi ve Çapraz Satış

```csharp
// Kural: Sepet içeriğini analiz et ve öneriler yap
var cartAnalysisRule = @"
    var electronicsCount = Input.CartItems.Count(i => i.Category == ""Electronics"");
    var clothingCount = Input.CartItems.Count(i => i.Category == ""Clothing"");
    var totalWeight = Input.CartItems.Sum(i => i.Weight);
    var averagePrice = Input.CartItems.Average(i => i.Price);
    
    var suggestions = new List<string>();
    var crossSellDiscount = 0.0;
    
    // Çapraz satış önerileri
    if(electronicsCount > 0 && !Input.CartItems.Any(i => i.Category == ""Accessories""))
    {
        suggestions.Add(""Elektronik ürünleriniz için aksesuar öneriyoruz"");
        crossSellDiscount = 0.1;
    }
    
    if(clothingCount > 0 && !Input.CartItems.Any(i => i.Category == ""Shoes""))
    {
        suggestions.Add(""Kıyafetleriniz için ayakkabı öneriyoruz"");
        crossSellDiscount = Math.Max(crossSellDiscount, 0.08);
    }
    
    // Kargo hesaplama
    var shippingCost = 0.0;
    if(totalWeight > 5)
    {
        shippingCost = 25.0; // Ağır kargo
    }
    else if(Input.OrderAmount < 200)
    {
        shippingCost = 15.0; // Standart kargo
    }
    
    Output.ElectronicsCount = electronicsCount;
    Output.ClothingCount = clothingCount;
    Output.TotalWeight = totalWeight;
    Output.AveragePrice = averagePrice;
    Output.Suggestions = suggestions;
    Output.CrossSellDiscount = crossSellDiscount;
    Output.ShippingCost = shippingCost;
    Output.FinalAmount = Input.OrderAmount * (1 - crossSellDiscount) + shippingCost;
";
```

### 3. Karmaşık Kupon ve Kod Analizi

```csharp
// Kural: Regex ile kupon formatı kontrolü ve çoklu kupon desteği
var couponAnalysisRule = @"
    var isValidCoupon = false;
    var couponDiscount = 0.0;
    var couponMessage = """";
    
    if(!string.IsNullOrEmpty(Input.CouponCode))
    {
        var code = Input.CouponCode.ToUpper();
        
        // Regex ile kupon formatı kontrolü
        if(System.Text.RegularExpressions.Regex.IsMatch(code, ""^SAVE[0-9]+$""))
        {
            var discountValue = int.Parse(code.Substring(4));
            if(discountValue <= 50) // Max %50
            {
                isValidCoupon = true;
                couponDiscount = discountValue / 100.0;
                couponMessage = $""{discountValue}% kupon indirimi uygulandı"";
            }
        }
        else if(code.StartsWith(""VIP""))
        {
            if(Input.CustomerType == ""VIP"")
            {
                isValidCoupon = true;
                couponDiscount = 0.25;
                couponMessage = ""VIP özel kupon indirimi"";
            }
        }
        else if(code == ""WELCOME2024"")
        {
            if(Input.IsFirstTimeCustomer)
            {
                isValidCoupon = true;
                couponDiscount = 0.15;
                couponMessage = ""Hoş geldin 2024 kuponu"";
            }
        }
        else if(code == ""BLACKFRIDAY"")
        {
            if(Input.OrderDate.Month == 11 && Input.OrderDate.Day >= 24)
            {
                isValidCoupon = true;
                couponDiscount = 0.40;
                couponMessage = ""Black Friday mega indirimi"";
            }
        }
    }
    
    Output.IsValidCoupon = isValidCoupon;
    Output.CouponDiscount = couponDiscount;
    Output.CouponMessage = couponMessage;
    Output.FinalAmount = Input.OrderAmount * (1 - couponDiscount);
";
```

### 4. Zaman Bazlı Karmaşık Kural

```csharp
// Kural: Müşteri yaşı, son sipariş tarihi ve saat bazlı indirimler
var timeBasedRule = @"
    var now = DateTime.Now;
    var customerAge = (now - Input.CustomerRegistrationDate).TotalDays;
    var lastOrderDays = (now - Input.LastOrderDate).TotalDays;
    
    var loyaltyScore = 0;
    
    // Sadakat puanı hesaplama
    if(customerAge > 365) loyaltyScore += 50;
    if(customerAge > 730) loyaltyScore += 30;
    if(Input.TotalOrders > 10) loyaltyScore += 20;
    if(Input.TotalOrders > 50) loyaltyScore += 30;
    if(lastOrderDays < 30) loyaltyScore += 10;
    if(lastOrderDays < 7) loyaltyScore += 20;
    
    // Saat bazlı indirim
    var hourDiscount = 0.0;
    if(now.Hour >= 22 || now.Hour <= 6)
    {
        hourDiscount = 0.05; // Gece indirimi
    }
    
    // Hafta sonu indirimi
    var weekendDiscount = 0.0;
    if(now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
    {
        weekendDiscount = 0.03;
    }
    
    // Bayram indirimi
    var holidayDiscount = 0.0;
    if(IsHoliday(now))
    {
        holidayDiscount = 0.10;
    }
    
    var totalDiscount = (loyaltyScore / 100.0) + hourDiscount + weekendDiscount + holidayDiscount;
    
    Output.LoyaltyScore = loyaltyScore;
    Output.TotalDiscount = Math.Min(totalDiscount, 0.5); // Max %50 indirim
    Output.FinalAmount = Input.OrderAmount * (1 - Output.TotalDiscount);
    Output.DiscountBreakdown = $""Loyalty: {loyaltyScore/100.0:P}, Hour: {hourDiscount:P}, Weekend: {weekendDiscount:P}, Holiday: {holidayDiscount:P}"";
    
    // Yardımcı fonksiyon
    bool IsHoliday(DateTime date)
    {
        var holidays = new[] { new DateTime(2024, 1, 1), new DateTime(2024, 4, 23), new DateTime(2024, 5, 1) };
        return holidays.Any(h => h.Date == date.Date);
    }
";
```

### 5. Çoklu Kural Zinciri

```csharp
// Kural: Birden fazla kuralı sırayla uygula ve en iyi sonucu seç
var multiRuleChain = @"
    var results = new List<EcommercePricingOutput>();
    
    // Kural 1: VIP indirimi
    if(Input.CustomerType == ""VIP"" && Input.OrderAmount > 100)
    {
        results.Add(new EcommercePricingOutput 
        { 
            DiscountPercentage = 20, 
            FinalAmount = Input.OrderAmount * 0.8,
            Message = ""VIP indirimi""
        });
    }
    
    // Kural 2: Toplu sipariş indirimi
    if(Input.ItemCount >= 10)
    {
        results.Add(new EcommercePricingOutput 
        { 
            DiscountPercentage = 15, 
            FinalAmount = Input.OrderAmount * 0.85,
            Message = ""Toplu sipariş indirimi""
        });
    }
    
    // Kural 3: İlk kez müşteri indirimi
    if(Input.IsFirstTimeCustomer)
    {
        results.Add(new EcommercePricingOutput 
        { 
            DiscountPercentage = 10, 
            FinalAmount = Input.OrderAmount * 0.9,
            Message = ""Hoş geldin indirimi""
        });
    }
    
    // En iyi sonucu seç (en yüksek indirim)
    var bestResult = results.OrderByDescending(r => r.DiscountPercentage).FirstOrDefault();
    
    if(bestResult != null)
    {
        Output.DiscountPercentage = bestResult.DiscountPercentage;
        Output.FinalAmount = bestResult.FinalAmount;
        Output.Message = bestResult.Message;
    }
    else
    {
        Output.DiscountPercentage = 0;
        Output.FinalAmount = Input.OrderAmount;
        Output.Message = ""Standart fiyat"";
    }
";
```

## Performans Optimizasyonları

### 1. Kural Önbellekleme

```csharp
public class CachedEcommerceRuleService
{
    private readonly MemoryCache _cache;
    private readonly RuleCompiler<EcommerceOrderInput, EcommercePricingOutput> _compiler;

    public async Task<EcommercePricingOutput> ExecuteRuleAsync(string ruleId, EcommerceOrderInput input)
    {
        // Önce cache'den kontrol et
        if (_cache.TryGetValue($"rule_{ruleId}", out CompiledRule<EcommerceOrderInput, EcommercePricingOutput>? cachedRule))
        {
            return cachedRule.Invoke(input); // Çok hızlı!
        }

        // Cache'de yoksa derle ve cache'e koy
        var rule = await _ruleRepository.GetAsync(ruleId);
        var compiledRule = await _compiler.CompileAsync(ruleId, rule.Content);
        
        _cache.Set($"rule_{ruleId}", compiledRule, TimeSpan.FromHours(1));
        return compiledRule.Invoke(input);
    }
}
```

### 2. Paralel Kural Çalıştırma

```csharp
public async Task<List<EcommercePricingOutput>> ExecuteMultipleRulesAsync(List<string> ruleIds, EcommerceOrderInput input)
{
    var tasks = ruleIds.Select(async ruleId =>
    {
        var rule = await GetCachedRuleAsync(ruleId);
        return rule.Invoke(input);
    });

    // Tüm kuralları paralel çalıştır
    var results = await Task.WhenAll(tasks);
    return results.ToList();
}
```

## Gerçek Dünya Senaryoları

### 1. E-ticaret Fiyatlandırma Motoru

```csharp
public class EcommercePricingEngine
{
    private readonly CachedEcommerceRuleService _ruleService;

    public async Task<EcommercePricingOutput> CalculatePriceAsync(EcommerceOrderInput order)
    {
        var rules = new[] { "vip-discount", "bulk-discount", "first-time-discount", "seasonal-campaign" };
        var results = await _ruleService.ExecuteMultipleRulesAsync(rules, order);
        
        // En iyi sonucu seç
        return results.OrderByDescending(r => r.DiscountPercentage).First();
    }
}
```

### 2. ASP.NET Core MVC Entegrasyonu

```csharp
[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly EcommercePricingEngine _pricingEngine;

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculatePrice([FromBody] EcommerceOrderInput order)
    {
        try
        {
            var result = await _pricingEngine.CalculatePriceAsync(order);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
```

### 3. Frontend JavaScript Entegrasyonu

```javascript
async function calculatePrice() {
    const orderData = {
        orderAmount: parseFloat(document.getElementById('totalAmount').value),
        customerType: document.getElementById('customerType').value,
        itemCount: parseInt(document.getElementById('itemCount').value),
        isFirstTimeCustomer: document.getElementById('isFirstTime').checked,
        customerLoyaltyPoints: parseInt(document.getElementById('loyaltyPoints').value),
        productCategory: document.getElementById('category').value,
        couponCode: document.getElementById('couponCode').value
    };

    try {
        const response = await fetch('/api/pricing/calculate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(orderData)
        });

        const result = await response.json();
        
        // Sonuçları göster
        document.getElementById('discountPercentage').textContent = result.discountPercentage + '%';
        document.getElementById('finalAmount').textContent = '₺' + result.finalAmount.toFixed(2);
        document.getElementById('shippingCost').textContent = result.freeShipping ? 'Ücretsiz' : '₺' + result.shippingCost;
        document.getElementById('message').textContent = result.message;
        
        // Önerileri göster
        if (result.suggestions && result.suggestions.length > 0) {
            const suggestionsDiv = document.getElementById('suggestions');
            suggestionsDiv.innerHTML = result.suggestions.map(s => `<li>${s}</li>`).join('');
        }
    } catch (error) {
        console.error('Fiyat hesaplama hatası:', error);
    }
}
```

## Performans Metrikleri

### Beklenen Performans:
- **İlk derleme**: ~50-100ms
- **Önbellekli çalıştırma**: ~0.1-1ms
- **Paralel çalıştırma**: 5-10x hızlanma
- **Memory kullanımı**: Minimal overhead

### Monitoring:
```csharp
public class EcommerceRuleMetrics
{
    public long TotalExecutions { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double AverageExecutionTime { get; set; }
    public double CacheHitRatio => (double)CacheHits / TotalExecutions;
    public Dictionary<string, long> RuleExecutionCounts { get; set; } = new();
}
```

## Sonuç

Bu dokümantasyon, RuleEngine kütüphanesini e-ticaret sistemlerinde nasıl kullanacağınızı gösterir. Basit kurallardan karmaşık kurallara kadar her seviyede örnekler bulabilirsiniz.

### Ana Avantajlar:
- **Dinamik Fiyatlandırma**: Kod değişikliği olmadan fiyat kuralları güncellenebilir
- **A/B Testing**: Farklı kuralları test edebilirsiniz
- **Kişiselleştirme**: Müşteri bazlı özel kurallar
- **Performans**: Derlenmiş kurallar çok hızlı çalışır
- **Versiyonlama**: Güvenli kural güncellemeleri
- **Audit**: Tüm kural çalıştırmaları loglanır

Bu kütüphane ile e-ticaret sisteminizde güçlü bir fiyatlandırma motoru oluşturabilirsiniz!
