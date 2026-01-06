# Project Structure

## 📁 Directory Structure

```
RuleEngine/
├── src/
│   ├── RuleEngine.Core/              # Rule engine core
│   │   ├── Rule/                     # Rule management (RuleManager, RuleCompiler, RuleSet)
│   │   ├── Models/                   # Data models (CompiledRule, RuleInputModel)
│   │   ├── Abstractions/             # Interfaces (IRuleProvider, IRuleRepository)
│   │   └── Extensions/               # Extension methods
│   │
│   ├── CampaignEngine.Core/          # ⭐ Campaign engine (NEW)
│   │   ├── Models/                   # Campaign models (GeneralCampaign, Price)
│   │   ├── Abstractions/             # Interfaces (ICampaignRepository, ITravelProduct)
│   │   ├── Cache/                    # Cache providers (MemoryCacheProvider)
│   │   ├── Repositories/             # Data access (InMemoryCampaignRepository)
│   │   ├── Extensions/               # Extension methods (ServiceCollectionExtensions)
│   │   ├── CampaignManager.cs        # Main campaign manager
│   │   └── README.md                 # Detailed documentation
│   │
│   ├── RuleEngine.Sqlite/            # SQLite implementation
│   └── RuleEngine.Mvc/               # Web UI
│
├── examples/
│   └── CampaignEngine.Example/       # ⭐ Campaign example (NEW)
│
├── tests/
│   ├── RuleEngine.Core.Tests/
│   ├── RuleEngine.Integration.Tests/
│   └── CampaignEngine.Core.Tests/      # ⭐ Campaign tests (NEW)
│       ├── PriceTests.cs               # Price model tests
│       ├── CampaignManagerTests.cs     # Manager tests
│       ├── RepositoryTests.cs          # Repository tests
│       ├── CacheTests.cs               # Cache tests
│       ├── ExtensionTests.cs           # Extension tests
│       ├── IntegrationTests.cs         # Integration tests
│       └── README.md                   # Test documentation
│
└── README.md                          # Main documentation
```

## 🔗 Project Dependencies

```
CampaignEngine.Core
    └── RuleEngine.Core
        └── Microsoft.CodeAnalysis.CSharp.Scripting
        └── Microsoft.Extensions.*

CampaignEngine.Example
    └── CampaignEngine.Core
```

## 📦 NuGet Packages

### Published Packages
- `Minima.RuleEngine.Core` (v1.1.11) - .NET 8.0, 9.0 & 10.0 ⭐
- `Minima.RuleEngine.Sqlite` (v1.1.11) - .NET 8.0, 9.0 & 10.0 ⭐
- `Minima.CampaignEngine.Core` (v1.1.11) - .NET 8.0, 9.0 & 10.0 ⭐

### Dependencies
- Microsoft.CodeAnalysis.CSharp.Scripting 4.14.0
- Microsoft.Extensions.DependencyInjection 8.0.0+
- Microsoft.Extensions.Logging 8.0.0+
- Microsoft.Extensions.Caching.Memory 8.0.1+
- Newtonsoft.Json 13.0.3

## 🎯 Core Classes

### RuleEngine.Core

#### RuleManager
- Manages rule sets by provider
- Background processing with automatic updates
- Thread-safe operations

#### RuleCompiler<TInput, TReturn>
- Compiles C# expressions
- Runtime compilation using Roslyn
- Syntax validation

#### RuleSet<TInput, TOutput>
- Predicate (selection) rule
- Result (action) rule
- Priority

### CampaignEngine.Core ⭐

#### CampaignManager<TInput, TOutput>
- Campaign management
- RuleEngine.Core integration
- Rule-based campaign selection

#### GeneralCampaign
- Campaign entity model
- Predicate, Result, Usage rules
- Quota and priority management

#### Price
- Currency support (ISO 4217)
- Mathematical operators
- JSON serialization

## 🔄 Data Flow

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

## 🛠️ Extension Points

### Custom Repository
```csharp
public class MyCampaignRepository : ICampaignRepository
{
    // Use your own data source
}
```

### Custom Cache Provider
```csharp
public class RedisCacheProvider : ICacheProvider
{
    // Redis or other cache system
}
```

### Custom Rule Provider
```csharp
public class MyRuleProvider : IRuleProvider<MyRuleSet, MyInput, MyOutput>
{
    // Custom rule provider
}
```

## 📊 Performance Features

- **Compilation Cache**: Rules are compiled once
- **Memory Cache**: Frequently used data is cached
- **Background Processing**: Rule updates in the background
- **Thread-Safe**: ConcurrentDictionary usage
- **Lazy Loading**: Load on demand

## 🔐 Security

- Input validation
- SQL injection protection (parameterized queries)
- Expression injection protection
- Rule syntax validation

## 📝 License

MIT License
