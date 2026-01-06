# Rule Manager & Campaign Engine

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Core.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Core/)
[![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Sqlite.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Sqlite/)
[![NuGet](https://img.shields.io/nuget/v/Minima.CampaignEngine.Core.svg)](https://www.nuget.org/packages/Minima.CampaignEngine.Core/)

Modern, extensible, and high-performance rule engine and campaign management system. Create dynamic business rules with Roslyn-based C# expression evaluation, persist with SQLite or custom repositories, and build campaign systems.

## 🌟 Why RuleEngine?

- **🚀 High Performance**: Compiled rules, caching mechanism, background processing
- **🔧 Easy Integration**: Dependency Injection, ASP.NET Core support
- **📦 Multi-Targeting**: .NET 8.0, 9.0, and 10.0 support
- **🎯 Flexible Architecture**: Provider pattern, custom repository support
- **🔒 Secure**: Thread-safe operations, input validation
- **📊 Traceable**: Audit logging, execution history

## 📦 Projects

### RuleEngine.Core
Modern rule engine with Roslyn-based C# expression evaluation.

**Features:**
- ✅ C# expression support (Roslyn Scripting API)
- ✅ Dynamic rule compilation and caching
- ✅ Thread-safe concurrent operations
- ✅ Extensible architecture with provider pattern
- ✅ Background processing with automatic updates
- ✅ Memory cache support
- ✅ Syntax validation and error handling
- ✅ Generic input/output models
- ✅ Design-time metadata catalog (parameters + categories)
- ✅ RuleManager/IRuleProvider orchestration flow
- ✅ DEBUG_RULES PDB debugging support

### RuleEngine.Sqlite
SQLite-based persistence layer.

**Features:**
- ✅ Entity Framework Core integration
- ✅ Rule versioning and rollback
- ✅ Execution audit logging
- ✅ Migration and seeding support
- ✅ CRUD operations

### CampaignEngine.Core ⭐ NEW
Campaign management system built on top of RuleEngine.Core.

**Features:**
- ✅ Rule-based campaign system
- ✅ Discount campaigns (percentage/fixed amount)
- ✅ Product gift campaigns
- ✅ Quota management and usage tracking
- ✅ Priority-based campaign selection
- ✅ Memory cache support
- ✅ Dependency Injection
- ✅ Custom repository support
- ✅ Available campaign resolution per product
- ✅ Basket apply/remove helpers (UseCampaign/DeleteCampaign)
- ✅ Demo seed helper for quick testing

## 🚀 Quick Start

### RuleEngine Usage

```csharp
using RuleEngine.Core.Rule;
using RuleEngine.Core.Models;

// 1. Define input/output models
public class OrderInput : RuleInputModel
{
    public decimal Amount { get; set; }
    public string CustomerType { get; set; }
    public int Age { get; set; }
}

public class DiscountOutput
{
    public decimal DiscountAmount { get; set; }
    public string Message { get; set; }
}

// 2. Create rule compiler
var compiler = new RuleCompiler<OrderInput, bool>();

// 3. Compile the rule
var rule = await compiler.CompileAsync(
    "vip-check", 
    "Input.Age > 18 && Input.CustomerType == \"VIP\""
);

// 4. Execute the rule
var input = new OrderInput 
{ 
    Age = 25, 
    CustomerType = "VIP",
    Amount = 1000 
};
var result = rule.Invoke(input); // true

Console.WriteLine($"Is VIP Adult: {result}");
```

### CampaignEngine Usage ⭐

```csharp
using CampaignEngine.Core;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Extensions;

// 1. Add to service collection
services.AddCampaignEngine();
services.AddLogging();
services.AddMemoryCache();

// 2. Define input/output models
public class CampaignInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public string Country { get; set; }
    public int UsageCount { get; set; }
}

public class CampaignOutput : CampaignEngine.Core.Models.CampaignOutput
{
    // TotalDiscount and CampaignProductDiscount are inherited
}

// 3. Create campaign manager
var campaignManager = new CampaignManager<CampaignInput, CampaignOutput>(
    moduleId: 1,
    serviceProvider: serviceProvider,
    logger: logger,
    typeof(Price) // Extra types for compilation
);

// 4. Define campaign
var campaign = new GeneralCampaign
{
    Code = "SUMMER2024",
    Name = "Summer Sale",
    ModulId = 1,
    Priority = 100,
    StartDate = DateTime.Now,
    EndDate = DateTime.Now.AddMonths(3),
    
    // Predicate rule - When to apply campaign?
    Predicate = "Input.TotalAmount > 500 && Input.Country == \"US\"",
    
    // Result rule - How much discount?
    Result = @"Output.TotalDiscount = new Price(100, ""USD"");",
    
    // Usage rule - Who can use it?
    Usage = "Input.UsageCount < 10",
    
    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Quota = 1000
};

repository.AddCampaign(campaign);

// 5. Get and apply campaigns
var input = new CampaignInput
{
    TotalAmount = 600,
    Country = "US",
    UsageCount = 5
};

var campaigns = campaignManager.GetCampaign(input);

foreach (var result in campaigns)
{
    Console.WriteLine($"Campaign: {result.Code}");
    Console.WriteLine($"Discount: {result.TotalDiscount}");
}
```

### SQLite Persistence Usage

```csharp
using RuleEngine.Sqlite.Data;
using Microsoft.EntityFrameworkCore;

// 1. Configure DbContext
services.AddDbContext<RuleDbContext>(options =>
    options.UseSqlite("Data Source=ruleengine.db"));

// 2. Use repositories
public class RuleService
{
    private readonly RuleDbContext _context;
    
    public async Task<RuleEntity> CreateRuleAsync(string name, string predicate, string result)
    {
        var rule = new RuleEntity
        {
            Name = name,
            IsActive = true,
            CreateDate = DateTime.UtcNow
        };
        
        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();
        
        var version = new RuleVersionEntity
        {
            RuleId = rule.Id,
            Version = 1,
            Predicate = predicate,
            Result = result,
            IsActive = true,
            CreateDate = DateTime.UtcNow
        };
        
        _context.RuleVersions.Add(version);
        await _context.SaveChangesAsync();
        
        return rule;
    }
    
    public async Task<List<RuleEntity>> GetActiveRulesAsync()
    {
        return await _context.Rules
            .Include(r => r.Versions)
            .Where(r => r.IsActive)
            .ToListAsync();
    }
}
```

## 📦 Requirements

- .NET 8.0, .NET 9.0, or .NET 10.0
- Microsoft.CodeAnalysis.CSharp.Scripting 4.14.0
- Microsoft.Extensions.DependencyInjection 8.0.0+
- Microsoft.Extensions.Logging 8.0.0+
- Microsoft.Extensions.Caching.Memory 8.0.1+

## 🏗️ Architecture

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/          # Rule engine core
│   │   ├── Rule/                 # Rule management
│   │   ├── Models/               # Data models
│   │   ├── Abstractions/         # Interfaces
│   │   └── Services/             # Services
│   │
│   ├── RuleEngine.Sqlite/        # SQLite persistence
│   │   ├── Data/                 # DbContext & Entities
│   │   └── Repositories/         # Repository implementations
│   │
│   └── CampaignEngine.Core/      # Campaign engine
│       ├── Models/               # Campaign models
│       ├── Abstractions/         # Interfaces
│       ├── Cache/                # Cache providers
│       ├── Repositories/         # Data access
│       └── Extensions/           # Extension methods
│
├── tests/                        # Test projects
├── demo/                         # Demo applications
└── docs/                         # Documentation
```

## 📝 Rule Writing

### Predicate (Selection) Rule
Determines when the campaign should be applied:

```csharp
// Simple condition
"Input.TotalPrice.Value > 1000"

// Multiple conditions
"Input.TotalPrice.Value > 1000 && Input.Country == \"US\""

// Date check
"Input.OrderDate >= DateTime.Now.AddDays(-7)"

// List check
"Input.Categories.Contains(\"Electronics\")"

// Complex condition
"Input.CustomerType == \"VIP\" && Input.TotalOrders > 10 && Input.LastOrderDate > DateTime.Now.AddMonths(-1)"
```

### Result (Action) Rule
Calculates the discount amount:

```csharp
// Fixed amount discount
"Output.TotalDiscount = new Price(100, \"USD\");"

// Percentage calculation
"Output.TotalDiscount = Input.TotalPrice * 0.2m;"

// Conditional calculation
@"if (Input.TotalPrice.Value > 1000)
    Output.TotalDiscount = Input.TotalPrice * 0.25m;
  else
    Output.TotalDiscount = Input.TotalPrice * 0.15m;"

// Product gift
@"Output.TotalDiscount = new Price(100, ""USD"");
  Output.CampaignProductDiscount = new CampaignProductDiscount 
  { 
      ProductKey = ""GIFT-001"",
      DiscountAmount = new Price(50, ""USD"")
  };"
```

### Usage (Eligibility) Rule
Determines who can use the campaign:

```csharp
// Usage count check
"Input.UsageCount < 5"

// First purchase check
"Input.IsFirstPurchase == true"

// Membership level check
"Input.MembershipLevel >= 2 && Input.UsageCount < 10"
```

## 🎯 Campaign Types

### DiscountCampaign (0)
Discount campaigns - Highest priority campaign is applied

```csharp
var campaign = new GeneralCampaign
{
    Code = "VIP20",
    Name = "VIP Customer Discount",
    Predicate = "Input.CustomerType == \"VIP\" && Input.TotalAmount > 500",
    Result = "Output.TotalDiscount = Input.TotalAmount * 0.2m;",
    CampaignTypes = (int)CampaignTypes.DiscountCampaign,
    Priority = 100
};
```

### ProductGiftCampaign (1)
Product gift campaigns - All eligible campaigns are applied

```csharp
var campaign = new GeneralCampaign
{
    Code = "GIFT3",
    Name = "Buy 3 Pay 2",
    Predicate = "Input.ProductCount >= 3",
    Result = @"Output.CampaignProductDiscount = new CampaignProductDiscount 
               { 
                   ProductKey = Input.ProductKey,
                   DiscountAmount = new Price(Input.ProductPrice.Value / 3, ""USD"")
               };",
    CampaignTypes = (int)CampaignTypes.ProductGiftCampaign,
    Priority = 50
};
```

### GiftCoupon (2)
Gift coupon campaigns

```csharp
var campaign = new GeneralCampaign
{
    Code = "COUPON50",
    Name = "$50 Gift Coupon",
    Predicate = "Input.TotalAmount > 1000",
    Result = "Output.GiftCoupon = new Price(50, \"USD\");",
    CampaignTypes = (int)CampaignTypes.GiftCoupon,
    Priority = 30
};
```

## 🔍 Example Scenario: E-Commerce Pricing

```csharp
// 1. Define campaigns
var campaigns = new[]
{
    // VIP customer discount
    new GeneralCampaign
    {
        Code = "VIP25",
        Name = "VIP Special Discount",
        Priority = 100,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddMonths(12),
        Predicate = "Input.CustomerType == \"VIP\" && Input.TotalAmount > 500",
        Result = "Output.TotalDiscount = Input.TotalAmount * 0.25m;",
        Usage = "Input.UsageCount < 100",
        CampaignTypes = (int)CampaignTypes.DiscountCampaign,
        Quota = 10000
    },
    
    // Bulk order discount
    new GeneralCampaign
    {
        Code = "BULK15",
        Name = "Bulk Order Discount",
        Priority = 80,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddMonths(6),
        Predicate = "Input.ItemCount >= 10",
        Result = "Output.TotalDiscount = Input.TotalAmount * 0.15m;",
        CampaignTypes = (int)CampaignTypes.DiscountCampaign,
        Quota = 5000
    },
    
    // First purchase discount
    new GeneralCampaign
    {
        Code = "WELCOME10",
        Name = "Welcome Discount",
        Priority = 60,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddMonths(12),
        Predicate = "Input.IsFirstPurchase == true",
        Result = "Output.TotalDiscount = Input.TotalAmount * 0.10m;",
        Usage = "Input.UsageCount == 0",
        CampaignTypes = (int)CampaignTypes.DiscountCampaign,
        Quota = 1000
    },
    
    // Free shipping
    new GeneralCampaign
    {
        Code = "FREESHIP",
        Name = "Free Shipping",
        Priority = 40,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddMonths(12),
        Predicate = "Input.TotalAmount >= 200",
        Result = "Output.FreeShipping = true; Output.ShippingDiscount = new Price(15, \"USD\");",
        CampaignTypes = (int)CampaignTypes.DiscountCampaign
    }
};

// 2. Add to repository
foreach (var campaign in campaigns)
{
    repository.AddCampaign(campaign);
}

// 3. Use campaigns
var input = new CampaignInput 
{ 
    TotalAmount = 600,
    CustomerType = "VIP",
    ItemCount = 5,
    IsFirstPurchase = false,
    UsageCount = 3
};

var results = campaignManager.GetCampaign(input);

// 4. Process results
foreach (var result in results)
{
    Console.WriteLine($"Campaign: {result.Code} - {result.Name}");
    Console.WriteLine($"Discount: {result.TotalDiscount}");
    Console.WriteLine($"Priority: {result.Priority}");
    Console.WriteLine();
}

// Output:
// Campaign: VIP25 - VIP Special Discount
// Discount: 150 USD (25% of 600)
// Priority: 100
//
// Campaign: FREESHIP - Free Shipping
// Discount: 15 USD
// Priority: 40
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/CampaignEngine.Core.Tests/
dotnet test tests/RuleEngine.Core.Tests/
dotnet test tests/RuleEngine.Integration.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

**Test Statistics:**
- ✅ CampaignEngine.Core.Tests: 26/26 passed
- ✅ RuleEngine.Core.Tests: 5/5 passed
- ✅ RuleEngine.Integration.Tests: 2/2 passed
- 📊 Total Coverage: 95%+

### Test Example

```csharp
using Xunit;
using FluentAssertions;

public class CampaignManagerTests
{
    [Fact]
    public void Should_Apply_VIP_Discount()
    {
        // Arrange
        var input = new CampaignInput
        {
            TotalAmount = 1000,
            CustomerType = "VIP"
        };
        
        var campaign = new GeneralCampaign
        {
            Code = "VIP20",
            Predicate = "Input.CustomerType == \"VIP\"",
            Result = "Output.TotalDiscount = Input.TotalAmount * 0.2m;",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign
        };
        
        // Act
        var results = campaignManager.GetCampaign(input);
        
        // Assert
        results.Should().NotBeEmpty();
        results.First().TotalDiscount.Value.Should().Be(200);
    }
}
```

## 📦 NuGet Packages

### Installation

```bash
# RuleEngine.Core
dotnet add package Minima.RuleEngine.Core --version 1.1.11

# RuleEngine.Sqlite
dotnet add package Minima.RuleEngine.Sqlite --version 1.1.11

# CampaignEngine.Core
dotnet add package Minima.CampaignEngine.Core --version 1.1.11
```

### Package Information

| Package | Version | .NET Support | Download |
|---------|---------|--------------|----------|
| Minima.RuleEngine.Core | 1.1.11 | 8.0, 9.0, 10.0 | [![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Core.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Core/) |
| Minima.RuleEngine.Sqlite | 1.1.11 | 8.0, 9.0, 10.0 | [![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Sqlite.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Sqlite/) |
| Minima.CampaignEngine.Core | 1.1.11 | 8.0, 9.0, 10.0 | [![NuGet](https://img.shields.io/nuget/v/Minima.CampaignEngine.Core.svg)](https://www.nuget.org/packages/Minima.CampaignEngine.Core/) |

### Recent NuGet Updates

- `Minima.RuleEngine.Core`: design-time metadata catalog, RuleManager/IRuleProvider flow, DEBUG_RULES PDB support
- `Minima.RuleEngine.Sqlite`: System.Text.Json persistence for rule metadata/parameters, design-time metadata integration
- `Minima.CampaignEngine.Core`: available campaign resolution, basket apply/remove helpers, demo seed helper

## 🔧 Advanced Usage

### Custom Repository

```csharp
public class SqlServerCampaignRepository : ICampaignRepository
{
    private readonly ApplicationDbContext _context;
    
    public SqlServerCampaignRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<GeneralCampaign> GetCampaigns(DateTime after, int moduleId)
    {
        return _context.Campaigns
            .Where(c => c.CreateDate > after && c.ModulId == moduleId)
            .Where(c => c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now)
            .OrderByDescending(c => c.Priority)
            .ToList();
    }
    
    public IDictionary<string, bool> GetAllCampaigns(IDictionary<string, bool> keys)
    {
        var codes = keys.Keys.ToList();
        var existing = _context.Campaigns
            .Where(c => codes.Contains(c.Code))
            .Select(c => c.Code)
            .ToList();
            
        return keys.ToDictionary(k => k.Key, k => existing.Contains(k.Key));
    }
    
    public bool CheckCampaignQuota(int quota, int campaignId)
    {
        var usageCount = _context.CampaignUsages
            .Count(u => u.CampaignId == campaignId);
        return usageCount < quota;
    }
}

// DI Registration
services.AddScoped<ICampaignRepository, SqlServerCampaignRepository>();
```

### Custom Cache Provider

```csharp
public class RedisCacheProvider : ICacheProvider
{
    private readonly IConnectionMultiplexer _redis;
    
    public RedisCacheProvider(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public T Get<T>(string key)
    {
        var db = _redis.GetDatabase();
        var value = db.StringGet(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
    }
    
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var db = _redis.GetDatabase();
        var serialized = JsonSerializer.Serialize(value);
        db.StringSet(key, serialized, expiration);
    }
    
    public void Remove(string key)
    {
        var db = _redis.GetDatabase();
        db.KeyDelete(key);
    }
}

// DI Registration
services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
services.AddSingleton<ICacheProvider, RedisCacheProvider>();
```

### ASP.NET Core API Integration

```csharp
[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly CampaignManager<CampaignInput, CampaignOutput> _campaignManager;
    private readonly ILogger<CampaignController> _logger;
    
    public CampaignController(
        CampaignManager<CampaignInput, CampaignOutput> campaignManager,
        ILogger<CampaignController> logger)
    {
        _campaignManager = campaignManager;
        _logger = logger;
    }
    
    [HttpPost("check")]
    public IActionResult CheckCampaigns([FromBody] CampaignInput input)
    {
        try
        {
            var campaigns = _campaignManager.GetCampaign(input);
            
            return Ok(new
            {
                success = true,
                campaigns = campaigns.Select(c => new
                {
                    code = c.Code,
                    name = c.Name,
                    discount = c.TotalDiscount,
                    priority = c.Priority
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Campaign check failed");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
    
    [HttpGet("active")]
    public IActionResult GetActiveCampaigns()
    {
        var campaigns = _campaignManager.GetAllActiveCampaigns();
        return Ok(campaigns);
    }
}
```

## 📊 Performance Tips

### 1. Rule Caching

```csharp
// Rules are automatically cached
var rule = await compiler.CompileAsync("rule1", ruleString);
// First compilation: ~50-100ms

var result1 = rule.Invoke(input1); // ~0.1-1ms
var result2 = rule.Invoke(input2); // ~0.1-1ms
var result3 = rule.Invoke(input3); // ~0.1-1ms
```

### 2. Parallel Rule Execution

```csharp
public async Task<List<CampaignOutput>> ExecuteMultipleCampaignsAsync(
    List<GeneralCampaign> campaigns, 
    CampaignInput input)
{
    var tasks = campaigns.Select(async campaign =>
    {
        return await ExecuteCampaignAsync(campaign, input);
    });
    
    var results = await Task.WhenAll(tasks);
    return results.ToList();
}
```

### 3. Background Processing

```csharp
// RuleManager automatically updates rules in the background
RuleManager.StartBackgroundProcessing(TimeSpan.FromMinutes(5));
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

## 👥 Authors

- Emre Karahan

## 🔗 Links

- [Documentation (GitHub Pages-ready)](docs/)
- [Examples](examples/)
- [Changelog](CHANGELOG.md)
- [Contributing Guide](CONTRIBUTING.md)
- [Security Policy](SECURITY.md)

## 📚 Documentation

- [STRUCTURE.md](STRUCTURE.md) - Project structure and architecture
- [MULTI-TARGETING.md](MULTI-TARGETING.md) - Multi-framework support
- [ECOMMERCE_EXAMPLES.md](docs/ECOMMERCE_EXAMPLES.md) - E-commerce examples
- [CONTRIBUTING.md](CONTRIBUTING.md) - Contributing guide
- [CHANGELOG.md](CHANGELOG.md) - Version history
- [SECURITY.md](SECURITY.md) - Security policy
