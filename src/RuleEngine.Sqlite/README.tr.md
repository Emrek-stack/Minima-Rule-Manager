# RuleEngine.Sqlite

RuleEngine.Core icin SQLite kalicilik paketi. EF Core ile migration, audit logging ve versioning sunar.

## Ozellikler

- **SQLite kalicilik** ve audit logging
- **Versioning** ve rollback
- **EF Core** entegrasyonu
- **Dependency Injection** destegi
- **Custom evaluator** uyumlulugu

## Kurulum

```bash
dotnet add package Minima.RuleEngine.Sqlite
```

```csharp
using RuleEngine.Sqlite.Extensions;

builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");
```

## Migration

```bash
dotnet ef migrations add InitialCreate --project RuleEngine.Sqlite --startup-project YourApp

dotnet ef database update --project RuleEngine.Sqlite --startup-project YourApp
```

## Dokumantasyon

- SQLite provider: `../../docs/sqlite-provider.md`
- Operasyon: `../../docs/operations.md`
- Guvenlik: `../../docs/security.md`
