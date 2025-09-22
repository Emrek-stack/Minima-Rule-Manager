# RuleEngine

A modern, extensible rule engine for .NET 8 with SQLite persistence, built with Microsoft.Extensions.DependencyInjection and Entity Framework Core.

## Features

- **Rule Definition**: Create rules with metadata, versioning, and status management
- **C# Expression Evaluation**: Execute C# expressions using Roslyn scripting
- **SQLite Persistence**: Store rules and execution history in SQLite database
- **Versioning**: Create multiple versions of rules and activate specific versions
- **Audit Logging**: Track all rule executions with input/output and performance metrics
- **Dependency Injection**: Built-in support for Microsoft.Extensions.DependencyInjection
- **Extensible**: Plugin architecture for custom rule evaluators

## Quick Start

### 1. Install the Package

```bash
dotnet add package RuleEngine.Sqlite
```

### 2. Register Services

```csharp
using RuleEngine.Sqlite.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add RuleEngine with SQLite persistence
builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");

var app = builder.Build();
```

### 3. Create and Execute Rules

```csharp
// Create a rule
var createRequest = new CreateRuleRequest
{
    Name = "Discount Rule",
    Description = "Applies discount based on order amount",
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount > 100",
        ResultExpression = "Input.Amount * 0.9" // 10% discount
    }
};

var rule = await ruleRepository.CreateAsync(createRequest);

// Activate the rule
await ruleRepository.ActivateVersionAsync(rule.Id, 1);

// Execute the rule
var result = await ruleEngine.EvaluateAsync(rule.Id, new { Amount = 150 });
// Result: 135 (150 * 0.9)
```

## Architecture Overview

### Core Components

- **IRuleEngine**: Main interface for rule execution
- **IRuleRepository**: Manages rule storage and retrieval
- **IRuleEvaluator**: Executes rule logic (C# expressions by default)
- **IAuditRepository**: Logs rule execution history

### Data Model

```csharp
public class RuleDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
    public RuleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string[] Tags { get; set; }
    public string Description { get; set; }
    public RuleContent Content { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public class RuleContent
{
    public string PredicateExpression { get; set; }
    public string ResultExpression { get; set; }
    public string Language { get; set; } = "csharp";
    public Dictionary<string, object> Metadata { get; set; }
}
```

## Usage Examples

### Creating Rules

```csharp
// Simple rule
var simpleRule = new CreateRuleRequest
{
    Name = "Age Check",
    Description = "Checks if person is adult",
    Content = new RuleContent
    {
        PredicateExpression = "Input.Age >= 18",
        ResultExpression = "true"
    }
};

// Complex rule with parameters
var complexRule = new CreateRuleRequest
{
    Name = "Price Calculator",
    Description = "Calculates price with tax and discount",
    Content = new RuleContent
    {
        PredicateExpression = "Input.BasePrice > 0",
        ResultExpression = "Input.BasePrice * (1 + Input.TaxRate) * (1 - Input.DiscountRate)"
    },
    Parameters = new Dictionary<string, object>
    {
        ["TaxRate"] = 0.08,
        ["DiscountRate"] = 0.05
    }
};
```

### Rule Versioning

```csharp
// Create a new version
var newVersion = new CreateVersionRequest
{
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount > 200", // Changed threshold
        ResultExpression = "Input.Amount * 0.85"    // Better discount
    },
    Activate = true // Activate immediately
};

await ruleRepository.CreateVersionAsync(ruleId, newVersion);

// Rollback to previous version
await ruleRepository.ActivateVersionAsync(ruleId, 1);
```

### Execution History

```csharp
// Get execution history
var history = await auditRepository.GetExecutionHistoryAsync(ruleId, limit: 50);

foreach (var audit in history)
{
    Console.WriteLine($"Executed at: {audit.ExecutedAt}");
    Console.WriteLine($"Success: {audit.Success}");
    Console.WriteLine($"Duration: {audit.Duration.TotalMilliseconds}ms");
    Console.WriteLine($"Input: {audit.Input}");
    Console.WriteLine($"Output: {audit.Output}");
}
```

## Database Schema

The SQLite database contains the following tables:

- **Rules**: Rule metadata (name, description, status, tags)
- **RuleVersions**: Rule content and versioning information
- **RuleParameters**: Additional parameters for rules
- **RuleExecutionAudits**: Execution history and performance metrics

## Migration and Seeding

### Running Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate --project RuleEngine.Sqlite --startup-project YourApp

# Update database
dotnet ef database update --project RuleEngine.Sqlite --startup-project YourApp
```

### Seeding Sample Data

```csharp
// Add sample rules during application startup
var sampleRules = new[]
{
    new CreateRuleRequest
    {
        Name = "VIP Customer Discount",
        Description = "Special discount for VIP customers",
        Content = new RuleContent
        {
            PredicateExpression = "Input.IsVip == true",
            ResultExpression = "Input.Amount * 0.8"
        }
    },
    new CreateRuleRequest
    {
        Name = "Bulk Order Discount",
        Description = "Discount for bulk orders",
        Content = new RuleContent
        {
            PredicateExpression = "Input.Quantity >= 10",
            ResultExpression = "Input.Amount * 0.9"
        }
    }
};

foreach (var ruleRequest in sampleRules)
{
    var rule = await ruleRepository.CreateAsync(ruleRequest);
    await ruleRepository.ActivateVersionAsync(rule.Id, 1);
}
```

## Custom Evaluators

You can create custom rule evaluators by implementing `IRuleEvaluator`:

```csharp
public class JavaScriptRuleEvaluator : IRuleEvaluator
{
    public async Task<RuleExecutionResult> EvaluateAsync(RuleDefinition rule, object input, CancellationToken cancellationToken = default)
    {
        // Implement JavaScript evaluation logic
        // using libraries like Jint or V8
    }

    public async Task<ValidationResult> ValidateAsync(RuleDefinition rule, object input)
    {
        // Implement validation logic
    }
}

// Register custom evaluator
services.AddRuleEngineWithSqlite<JavaScriptRuleEvaluator>("Data Source=ruleengine.db");
```

## Performance Considerations

- **Caching**: Rule definitions are cached in memory for fast access
- **Async Operations**: All operations are async for better scalability
- **Connection Pooling**: Entity Framework handles connection pooling automatically
- **Audit Logging**: Can be disabled or configured for specific rules if needed

## Error Handling

The rule engine provides comprehensive error handling:

```csharp
var result = await ruleEngine.EvaluateAsync(ruleId, input);

if (!result.Success)
{
    Console.WriteLine($"Rule execution failed: {result.ErrorMessage}");
    Console.WriteLine($"Duration: {result.Duration.TotalMilliseconds}ms");
}
```

## Testing

The library includes comprehensive unit tests and integration tests:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/RuleEngine.Core.Tests/
dotnet test tests/RuleEngine.Integration.Tests/
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Changelog

### v1.0.0
- Initial release
- C# expression evaluation using Roslyn
- SQLite persistence with Entity Framework Core
- Rule versioning and activation
- Audit logging
- Microsoft.Extensions.DependencyInjection integration
