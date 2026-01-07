# Kural Yazimi Rehberi

RuleEngine, C# expression tabanli predicate ve result kurallarini derler ve calistirir. Kampanya kurallari icin ayni syntax kullanilir.

## Predicate Kurallari

```csharp
"Input.TotalAmount > 1000"
"Input.Country == \"US\" && Input.CustomerType == \"VIP\""
"Input.OrderDate >= DateTime.Now.AddDays(-7)"
```

## Result Kurallari

```csharp
"Output.TotalDiscount = Input.TotalAmount * 0.2m;"
"Output.TotalDiscount = new Price(100, \"USD\");"
```

## Usage Kurallari (Kampanya)

```csharp
"Input.UsageCount < 5"
"Input.IsFirstPurchase == true"
```

## Otel Kurallari Ornekleri

```csharp
// 18 yas alti konaklama
"Input.GuestAge >= 18"

// Tek erkek konaklama
"!(Input.IsSingleGuest && Input.Gender == \"Male\")"
```

## Best Practices

- Kurallari kucuk ve test edilebilir parcalara bolun
- Payload modeli (Input/Output) net olmali
- Rule versioning ile degisiklikleri kontrollu yonetin

Ornekler: [examples](examples)
