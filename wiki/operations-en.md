# Operations and Performance

Operational considerations for enterprise usage.

## Performance

- Compiled rules are cached and reused.
- Background refresh can update rule sets.
- Memory cache can optimize repository reads.

## Audit and Observability

- Rule execution audits include input/output and duration.
- `Microsoft.Extensions.Logging` is used for logging.
- Integrates with Serilog, Application Insights, ELK, etc.

## Risk Controls

- Expression validation blocks invalid rules.
- Usage/quota checks protect campaign limits.

## Background Processing

```csharp
RuleManager.StartBackgroundProcessing(TimeSpan.FromMinutes(5));
```

Details: [security-en](security-en)
