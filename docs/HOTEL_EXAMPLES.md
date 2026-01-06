---
title: Hotel Examples (TR)
layout: default
---

# Otel Kurallari Ornekleri

Bu ornekler, konaklama sektorunde tipik kurallari RuleEngine ile modellemeyi gosterir.

## Giris/Çikis Modelleri

```csharp
public class HotelBookingInput : RuleInputModel
{
    public int GuestAge { get; set; }
    public string Gender { get; set; } = string.Empty; // "Male", "Female"
    public bool IsSingleGuest { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string CountryCode { get; set; } = string.Empty;
}

public class HotelBookingOutput
{
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
}
```

## 18 Yas Alti Konaklama Yasagi

```csharp
// Predicate
"Input.GuestAge >= 18"

// Result
"Output.IsAllowed = Input.GuestAge >= 18;\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"18 yas alti konaklama kabul edilmez\";"
```

## Tek Erkek Konaklama Yasagi

```csharp
// Predicate
"!(Input.IsSingleGuest && Input.Gender == \"Male\")"

// Result
"Output.IsAllowed = !(Input.IsSingleGuest && Input.Gender == \"Male\");\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"Tek erkek konaklama kabul edilmez\";"
```

## Ulke Bazli Kisit

```csharp
// Predicate
"Input.CountryCode != \"XX\""

// Result
"Output.IsAllowed = Input.CountryCode != \"XX\";\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"Bu ulkeden rezervasyon kabul edilmez\";"
```
