# E-Commerce Examples for RuleEngine

This documentation shows how to use the RuleEngine library in e-commerce systems. Examples are ordered from simple to complex rules.

## Table of Contents

- [Basic Setup](#basic-setup)
- [Simple Rules](#simple-rules)
- [Intermediate Rules](#intermediate-rules)
- [Complex Rules](#complex-rules)
- [Performance Optimizations](#performance-optimizations)
- [Real-World Scenarios](#real-world-scenarios)

## Basic Setup

### 1. Install Packages

```bash
dotnet add package Minima.RuleEngine.Core
dotnet add package Minima.RuleEngine.Sqlite
```

### 2. Create Basic Models

```csharp
using Minima.RuleEngine.Core.Rule;
using Minima.RuleEngine.Core.Models;

// E-commerce input model
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

// E-commerce output model
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

## Simple Rules

### 1. VIP Customer Discount

```csharp
// Rule: 20% discount for VIP customers
var vipRule = "Input.CustomerType == \"VIP\"";
var vipResult = @"
    Output.DiscountPercentage = 20;
    Output.FinalAmount = Input.OrderAmount * 0.8;
    Output.Message = ""VIP customer discount applied!"";
    Output.AppliedPromotions.Add(""VIP Discount"");
";

// Usage
var compiler = new RuleCompiler<EcommerceOrderInput, EcommercePricingOutput>();
var compiledRule = await compiler.CompileAsync("vip-discount", vipResult);

var order = new EcommerceOrderInput
{
    OrderAmount = 500,
    CustomerType = "VIP"
};

var result = compiledRule.Invoke(order);
// Result: 20% discount, FinalAmount = 400
```

### 2. Bulk Order Discount

```csharp
// Rule: 15% discount for 10 or more items
var bulkRule = "Input.ItemCount >= 10";
var bulkResult = @"
    Output.DiscountPercentage = 15;
    Output.FinalAmount = Input.OrderAmount * 0.85;
    Output.Message = ""Bulk order discount applied!"";
    Output.AppliedPromotions.Add(""Bulk Order Discount"");
";
```

### 3. First-Time Customer

```csharp
// Rule: 10% discount for first-time customers
var firstTimeRule = "Input.IsFirstTimeCustomer == true";
var firstTimeResult = @"
    Output.DiscountPercentage = 10;
    Output.FinalAmount = Input.OrderAmount * 0.9;
    Output.Message = ""Welcome discount applied!"";
    Output.AppliedPromotions.Add(""Welcome Discount"");
";
```

### 4. Free Shipping

```csharp
// Rule: Free shipping for orders over $200
var freeShippingRule = "Input.OrderAmount >= 200";
var freeShippingResult = @"
    Output.ShippingCost = 0;
    Output.FreeShipping = true;
    Output.ShippingMethod = ""Standard"";
    Output.EstimatedDeliveryDays = 3;
    Output.Message = ""You've earned free shipping!"";
    Output.AppliedPromotions.Add(""Free Shipping"");
";
```

## Intermediate Rules

### 1. Multi-Condition Rule

```csharp
// Rule: VIP customer + Electronics + Over $1000
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
    Output.Message = ""VIP electronics super discount!"";
";
```

### 2. Loyalty Points Rule

```csharp
// Rule: Discount based on loyalty points
var loyaltyRule = "Input.CustomerLoyaltyPoints > 1000";
var loyaltyResult = @"
    var loyaltyDiscount = Math.Min(Input.CustomerLoyaltyPoints / 10000.0, 0.3);
    Output.DiscountPercentage = loyaltyDiscount * 100;
    Output.FinalAmount = Input.OrderAmount * (1 - loyaltyDiscount);
    Output.AppliedPromotions.Add(""Loyalty Points Discount"");
    Output.Message = $""You've earned {loyaltyDiscount:P} discount with your loyalty points!"";
";
```

### 3. Seasonal Campaign

```csharp
// Rule: 30% discount on electronics in December
var seasonalRule = "Input.OrderDate.Month == 12 && Input.ProductCategory == \"Electronics\"";
var seasonalResult = @"
    Output.DiscountPercentage = 30;
    Output.FinalAmount = Input.OrderAmount * 0.7;
    Output.AppliedPromotions.Add(""Christmas Electronics Sale"");
    Output.Message = ""Christmas electronics campaign!"";
";
```

### 4. Coupon System

```csharp
// Rule: Valid coupon code check
var couponRule = "Input.HasCoupon == true && Input.CouponCode == \"SAVE20\"";
var couponResult = @"
    Output.DiscountPercentage = 20;
    Output.FinalAmount = Input.OrderAmount * 0.8;
    Output.AppliedPromotions.Add(""Coupon: SAVE20"");
    Output.Message = ""Coupon discount applied!"";
";
```

## Complex Rules

### 1. Dynamic Pricing

```csharp
// Rule: Dynamic pricing based on demand, season, and customer type
var dynamicPricingRule = @"
    var basePrice = Input.OrderAmount;
    var demandMultiplier = Input.ItemCount < 5 ? 1.2 : 1.0; // Low stock = higher price
    var seasonMultiplier = Input.OrderDate.Month == 12 ? 1.5 : 1.0; // December = higher price
    var customerMultiplier = Input.CustomerType == ""VIP"" ? 0.8 : 1.0; // VIP = lower price
    
    var finalPrice = basePrice * demandMultiplier * seasonMultiplier * customerMultiplier;
    
    Output.FinalAmount = Math.Round(finalPrice, 2);
    Output.Message = $""Dynamic price: Base({basePrice}) × Demand({demandMultiplier}) × Season({seasonMultiplier}) × Customer({customerMultiplier})"";
";
```

### 2. Cart Analysis and Cross-Selling

```csharp
// Rule: Analyze cart contents and make suggestions
var cartAnalysisRule = @"
    var electronicsCount = Input.CartItems.Count(i => i.Category == ""Electronics"");
    var clothingCount = Input.CartItems.Count(i => i.Category == ""Clothing"");
    var totalWeight = Input.CartItems.Sum(i => i.Weight);
    var averagePrice = Input.CartItems.Average(i => i.Price);
    
    var suggestions = new List<string>();
    var crossSellDiscount = 0.0;
    
    // Cross-sell suggestions
    if(electronicsCount > 0 && !Input.CartItems.Any(i => i.Category == ""Accessories""))
    {
        suggestions.Add(""We recommend accessories for your electronics"");
        crossSellDiscount = 0.1;
    }
    
    if(clothingCount > 0 && !Input.CartItems.Any(i => i.Category == ""Shoes""))
    {
        suggestions.Add(""We recommend shoes for your clothing"");
        crossSellDiscount = Math.Max(crossSellDiscount, 0.08);
    }
    
    // Shipping calculation
    var shippingCost = 0.0;
    if(totalWeight > 5)
    {
        shippingCost = 25.0; // Heavy shipping
    }
    else if(Input.OrderAmount < 200)
    {
        shippingCost = 15.0; // Standard shipping
    }
    
    Output.Suggestions = suggestions;
    Output.ShippingCost = shippingCost;
    Output.FinalAmount = Input.OrderAmount * (1 - crossSellDiscount) + shippingCost;
";
```

### 3. Complex Coupon Analysis

```csharp
// Rule: Regex-based coupon format validation and multi-coupon support
var couponAnalysisRule = @"
    var isValidCoupon = false;
    var couponDiscount = 0.0;
    var couponMessage = """";
    
    if(!string.IsNullOrEmpty(Input.CouponCode))
    {
        var code = Input.CouponCode.ToUpper();
        
        // Regex coupon format check
        if(System.Text.RegularExpressions.Regex.IsMatch(code, ""^SAVE[0-9]+$""))
        {
            var discountValue = int.Parse(code.Substring(4));
            if(discountValue <= 50) // Max 50%
            {
                isValidCoupon = true;
                couponDiscount = discountValue / 100.0;
                couponMessage = $""{discountValue}% coupon discount applied"";
            }
        }
        else if(code.StartsWith(""VIP""))
        {
            if(Input.CustomerType == ""VIP"")
            {
                isValidCoupon = true;
                couponDiscount = 0.25;
                couponMessage = ""VIP special coupon discount"";
            }
        }
        else if(code == ""WELCOME2024"")
        {
            if(Input.IsFirstTimeCustomer)
            {
                isValidCoupon = true;
                couponDiscount = 0.15;
                couponMessage = ""Welcome 2024 coupon"";
            }
        }
        else if(code == ""BLACKFRIDAY"")
        {
            if(Input.OrderDate.Month == 11 && Input.OrderDate.Day >= 24)
            {
                isValidCoupon = true;
                couponDiscount = 0.40;
                couponMessage = ""Black Friday mega discount"";
            }
        }
    }
    
    Output.FinalAmount = Input.OrderAmount * (1 - couponDiscount);
";
```

## Performance Optimizations

### 1. Rule Caching

```csharp
public class CachedEcommerceRuleService
{
    private readonly MemoryCache _cache;
    private readonly RuleCompiler<EcommerceOrderInput, EcommercePricingOutput> _compiler;

    public async Task<EcommercePricingOutput> ExecuteRuleAsync(string ruleId, EcommerceOrderInput input)
    {
        // Check cache first
        if (_cache.TryGetValue($"rule_{ruleId}", out CompiledRule<EcommerceOrderInput, EcommercePricingOutput>? cachedRule))
        {
            return cachedRule.Invoke(input); // Very fast!
        }

        // If not in cache, compile and cache
        var rule = await _ruleRepository.GetAsync(ruleId);
        var compiledRule = await _compiler.CompileAsync(ruleId, rule.Content);
        
        _cache.Set($"rule_{ruleId}", compiledRule, TimeSpan.FromHours(1));
        return compiledRule.Invoke(input);
    }
}
```

### 2. Parallel Rule Execution

```csharp
public async Task<List<EcommercePricingOutput>> ExecuteMultipleRulesAsync(List<string> ruleIds, EcommerceOrderInput input)
{
    var tasks = ruleIds.Select(async ruleId =>
    {
        var rule = await GetCachedRuleAsync(ruleId);
        return rule.Invoke(input);
    });

    // Execute all rules in parallel
    var results = await Task.WhenAll(tasks);
    return results.ToList();
}
```

## Real-World Scenarios

### 1. E-Commerce Pricing Engine

```csharp
public class EcommercePricingEngine
{
    private readonly CachedEcommerceRuleService _ruleService;

    public async Task<EcommercePricingOutput> CalculatePriceAsync(EcommerceOrderInput order)
    {
        var rules = new[] { "vip-discount", "bulk-discount", "first-time-discount", "seasonal-campaign" };
        var results = await _ruleService.ExecuteMultipleRulesAsync(rules, order);
        
        // Select the best result
        return results.OrderByDescending(r => r.DiscountPercentage).First();
    }
}
```

### 2. ASP.NET Core MVC Integration

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

### 3. Frontend JavaScript Integration

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
        
        // Display results
        document.getElementById('discountPercentage').textContent = result.discountPercentage + '%';
        document.getElementById('finalAmount').textContent = '$' + result.finalAmount.toFixed(2);
        document.getElementById('shippingCost').textContent = result.freeShipping ? 'Free' : '$' + result.shippingCost;
        document.getElementById('message').textContent = result.message;
        
        // Display suggestions
        if (result.suggestions && result.suggestions.length > 0) {
            const suggestionsDiv = document.getElementById('suggestions');
            suggestionsDiv.innerHTML = result.suggestions.map(s => `<li>${s}</li>`).join('');
        }
    } catch (error) {
        console.error('Price calculation error:', error);
    }
}
```

## Performance Metrics

### Expected Performance:
- **Initial compilation**: ~50-100ms
- **Cached execution**: ~0.1-1ms
- **Parallel execution**: 5-10x speedup
- **Memory usage**: Minimal overhead

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

## Conclusion

This documentation shows how to use the RuleEngine library in e-commerce systems. You can find examples at every level, from simple to complex rules.

### Main Advantages:
- **Dynamic Pricing**: Price rules can be updated without code changes
- **A/B Testing**: Test different rules
- **Personalization**: Customer-specific rules
- **Performance**: Compiled rules run very fast
- **Versioning**: Safe rule updates
- **Audit**: All rule executions are logged

With this library, you can build a powerful pricing engine for your e-commerce system!
