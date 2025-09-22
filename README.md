# RuleEngine

[![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Core.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Core/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)

**RuleEngine** is a powerful, lightweight rule engine for .NET 8 that allows you to define, compile, and execute business rules dynamically using C# code. It's a complete port of the Gordios RuleEngine with modern .NET features and SQLite persistence.

## 🚀 Features

- **Dynamic Rule Compilation**: Compile C# rule strings at runtime using Roslyn
- **Type-Safe**: Full type safety with generic input/output models
- **SQLite Persistence**: Store and manage rules with Entity Framework Core
- **Syntax Validation**: Built-in syntax checking for rule strings
- **Async Support**: Full async/await support for rule execution
- **Versioning**: Rule versioning and rollback capabilities
- **Auditing**: Complete execution audit trail
- **Import/Export**: JSON-based rule import/export
- **MVC Integration**: Ready-to-use ASP.NET Core MVC sample application
- **KnockoutJS Frontend**: Complete rule builder UI (ported from Gordios)

## 📦 Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| `Minima.RuleEngine.Core` | Core rule engine functionality | [![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Core.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Core/) |
| `Minima.RuleEngine.Sqlite` | SQLite persistence layer | [![NuGet](https://img.shields.io/nuget/v/Minima.RuleEngine.Sqlite.svg)](https://www.nuget.org/packages/Minima.RuleEngine.Sqlite/) |

## 🏃‍♂️ Quick Start

### 1. Install the Package

```bash
dotnet add package Minima.RuleEngine.Core
```

### 2. Basic Usage

```csharp
using Minima.RuleEngine.Core.Rule;
using Minima.RuleEngine.Core.Models;

// Define your input model
public class CustomerInput : RuleInputModel
{
    public int Age { get; set; }
    public bool HasLicense { get; set; }
    public string CustomerType { get; set; } = string.Empty;
}

// Create a rule compiler
var compiler = new RuleCompiler<CustomerInput, bool>();

// Define a rule string
string ruleString = "Input.Age > 18 && Input.HasLicense == true";

// Check syntax
var syntaxErrors = compiler.CheckSyntax(ruleString);
if (syntaxErrors.Count > 0)
{
    foreach (var error in syntaxErrors)
    {
        Console.WriteLine($"Error: {error.Title} - {error.Description}");
    }
    return;
}

// Compile the rule
var compiledRule = await compiler.CompileAsync("age-check-rule", ruleString);

// Execute the rule
var customer = new CustomerInput 
{ 
    Age = 20, 
    HasLicense = true, 
    CustomerType = "VIP" 
};

var result = compiledRule.Invoke(customer);
Console.WriteLine($"Rule result: {result}"); // True
```

### 3. Using RuleSet (Advanced)

```csharp
// Define predicate and result rules
var predicateRule = "Input.Age >= 21";
var resultRule = "Output.CanDrink = true; Output.Message = \"Customer can purchase alcohol\";";

// Create a rule set
var ruleSet = await RuleSet.CreateAsync<CustomerInput, CustomerOutput>(
    "drinking-rule", 
    predicateRule, 
    resultRule, 
    priority: 1
);

// Execute the rule set
var result = await ruleSet.ExecuteAsync(customer);
if (result.Success)
{
    Console.WriteLine($"Result: {result.Result}");
}
```

## 🗄️ SQLite Persistence

### 1. Install SQLite Package

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

### 2. Configure Services

```csharp
using Minima.RuleEngine.Sqlite.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add RuleEngine with SQLite
builder.Services.AddRuleEngine()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
```

### 3. Create and Manage Rules

```csharp
using Minima.RuleEngine.Core.Abstractions;

public class RuleService
{
    private readonly IRuleRepository _ruleRepository;

    public RuleService(IRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    public async Task<string> CreateRuleAsync()
    {
        var request = new CreateRuleRequest
        {
            Name = "VIP Customer Discount",
            Description = "Applies 20% discount for VIP customers",
            Tags = new[] { "discount", "vip", "pricing" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.CustomerType == \"VIP\" && Input.OrderAmount > 100",
                ResultExpression = "Output.DiscountPercentage = 20; Output.Message = \"VIP discount applied\";",
                Language = "csharp"
            }
        };

        var rule = await _ruleRepository.CreateAsync(request);
        await _ruleRepository.ActivateVersionAsync(rule.Id, 1);
        
        return rule.Id;
    }
}
```

## 🎯 Real-World Examples

### E-commerce Pricing Rules

```csharp
public class OrderInput : RuleInputModel
{
    public decimal OrderAmount { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public bool IsFirstTimeCustomer { get; set; }
}

public class PricingOutput
{
    public decimal DiscountPercentage { get; set; }
    public decimal FinalAmount { get; set; }
    public string Message { get; set; } = string.Empty;
}

// VIP Customer Rule
var vipRule = "Input.CustomerType == \"VIP\" && Input.OrderAmount > 100";
var vipResult = "Output.DiscountPercentage = 20; Output.FinalAmount = Input.OrderAmount * 0.8; Output.Message = \"VIP discount applied\";";

// Bulk Order Rule
var bulkRule = "Input.ItemCount >= 10";
var bulkResult = "Output.DiscountPercentage = 15; Output.FinalAmount = Input.OrderAmount * 0.85; Output.Message = \"Bulk order discount\";";

// First Time Customer Rule
var firstTimeRule = "Input.IsFirstTimeCustomer == true";
var firstTimeResult = "Output.DiscountPercentage = 10; Output.FinalAmount = Input.OrderAmount * 0.9; Output.Message = \"Welcome discount\";";
```

### Loan Approval Rules

```csharp
public class LoanApplication : RuleInputModel
{
    public decimal Income { get; set; }
    public decimal RequestedAmount { get; set; }
    public int CreditScore { get; set; }
    public int EmploymentYears { get; set; }
}

public class LoanDecision
{
    public bool Approved { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal InterestRate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// High Credit Score Rule
var highCreditRule = "Input.CreditScore >= 750 && Input.Income > Input.RequestedAmount * 0.3";
var highCreditResult = "Output.Approved = true; Output.ApprovedAmount = Input.RequestedAmount; Output.InterestRate = 3.5; Output.Reason = \"Excellent credit profile\";";

// Medium Credit Score Rule
var mediumCreditRule = "Input.CreditScore >= 650 && Input.CreditScore < 750 && Input.EmploymentYears >= 2";
var mediumCreditResult = "Output.Approved = true; Output.ApprovedAmount = Input.RequestedAmount * 0.8; Output.InterestRate = 5.5; Output.Reason = \"Good credit with stable employment\";";
```

## 🎨 MVC Sample Application

The repository includes a complete ASP.NET Core MVC application with:

- **Rule Management UI**: Create, edit, and manage rules
- **Rule Builder**: Visual rule builder with KnockoutJS (ported from Gordios)
- **Execution History**: View rule execution logs
- **Version Management**: Manage rule versions and rollbacks

### Running the Sample

```bash
git clone https://github.com/yourusername/RuleEngine.git
cd RuleEngine
dotnet run --project src/RuleEngine.Mvc
```

Navigate to `http://localhost:5000` to see the application in action.

## 🔧 Advanced Features

### Custom Rule Providers

```csharp
public class CustomRuleProvider : IRuleProvider<RuleSet<MyInput, MyOutput>, MyInput, MyOutput>
{
    public async Task<RuleSet<MyInput, MyOutput>?> GetRuleSetAsync(string ruleCode)
    {
        // Custom logic to load rules from any source
        return await LoadRuleFromCustomSource(ruleCode);
    }
}
```

### Rule Execution Auditing

```csharp
public class AuditService
{
    private readonly IAuditRepository _auditRepository;

    public async Task LogExecutionAsync(string ruleId, object input, object output, bool success)
    {
        var audit = new RuleExecutionAudit
        {
            RuleId = ruleId,
            Input = JsonSerializer.Serialize(input),
            Output = JsonSerializer.Serialize(output),
            Success = success,
            ExecutedAt = DateTime.UtcNow
        };

        await _auditRepository.LogExecutionAsync(audit);
    }
}
```

### Import/Export Rules

```csharp
// Export rules to JSON
var rules = await _ruleRepository.GetAllAsync();
var json = JsonSerializer.Serialize(rules, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText("rules.json", json);

// Import rules from JSON
var json = File.ReadAllText("rules.json");
var rules = JsonSerializer.Deserialize<List<Rule>>(json);
foreach (var rule in rules)
{
    await _ruleRepository.CreateAsync(rule);
}
```

## 🧪 Testing

```csharp
[Fact]
public async Task RuleCompiler_ShouldCompileAndExecuteRule()
{
    // Arrange
    var compiler = new RuleCompiler<TestInput, bool>();
    var ruleString = "Input.Value > 10";

    // Act
    var syntaxErrors = compiler.CheckSyntax(ruleString);
    var compiledRule = await compiler.CompileAsync("test-rule", ruleString);
    var result = compiledRule.Invoke(new TestInput { Value = 15 });

    // Assert
    Assert.Empty(syntaxErrors);
    Assert.NotNull(compiledRule);
    Assert.True(result);
}
```

## 📊 Performance

- **Compilation**: ~50-100ms for typical business rules
- **Execution**: ~0.1-1ms per rule execution
- **Memory**: Minimal overhead, rules are compiled to efficient IL
- **Concurrency**: Thread-safe, supports concurrent rule execution

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Gordios Project**: Original RuleEngine implementation
- **Roslyn**: Microsoft's C# compiler platform
- **Entity Framework Core**: Data access technology
- **KnockoutJS**: MVVM framework for the rule builder UI

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/RuleEngine/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/RuleEngine/discussions)
- **Documentation**: [Wiki](https://github.com/yourusername/RuleEngine/wiki)

---

**Made with ❤️ for the .NET community**