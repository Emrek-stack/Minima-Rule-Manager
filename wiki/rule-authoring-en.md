# Rule Authoring

RuleEngine compiles and runs C# expression-based predicate and result rules. Campaign rules use the same syntax.

## Predicate Rules

```csharp
"Input.TotalAmount > 1000"
"Input.Country == \"US\" && Input.CustomerType == \"VIP\""
"Input.OrderDate >= DateTime.Now.AddDays(-7)"
```

## Result Rules

```csharp
"Output.TotalDiscount = Input.TotalAmount * 0.2m;"
"Output.TotalDiscount = new Price(100, \"USD\");"
```

## Usage Rules (Campaign)

```csharp
"Input.UsageCount < 5"
"Input.IsFirstPurchase == true"
```

## Hotel Rule Examples

```csharp
// No guests under 18
"Input.GuestAge >= 18"

// No single male guest
"!(Input.IsSingleGuest && Input.Gender == \"Male\")"
```

## Best Practices

- Keep rules small and testable
- Define clear Input/Output models
- Use rule versioning for controlled changes

Examples: [examples-en](examples-en)
