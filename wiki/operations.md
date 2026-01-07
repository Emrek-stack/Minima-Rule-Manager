# Operasyon ve Performans

Kurumsal kullanim senaryolari icin operasyonel konulari burada topladik.

## Performans

- Derlenen kurallar cache edilir ve tekrar kullanilir.
- Background rule refresh ile rule set guncellemeleri yapilabilir.
- Memory cache ile repository cikislari optimize edilir.

## Audit ve Gozlemlenebilirlik

- Rule execution audit kayitlari input/output ve sure bilgisini icerir.
- Loglama icin `Microsoft.Extensions.Logging` kullanilir.
- Kurumsal log altyapilarina (Serilog, Application Insights, ELK) kolayca entegre olur.

## Risk Kontrolleri

- Expression dogrulama ile hatali kurallar engellenir.
- Usage/Quota kontrolleri ile kampanya kotasi korunur.

## Background Processing

```csharp
RuleManager.StartBackgroundProcessing(TimeSpan.FromMinutes(5));
```

Detaylar: [security](security)
