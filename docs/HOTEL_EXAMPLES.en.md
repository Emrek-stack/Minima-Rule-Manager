---
title: Hotel Examples (EN)
layout: default
---

# Hotel Rule Examples

These examples show how to model common hospitality rules with RuleEngine.

## Input/Output Models

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

## No Guests Under 18

```csharp
// Predicate
"Input.GuestAge >= 18"

// Result
"Output.IsAllowed = Input.GuestAge >= 18;\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"Guests under 18 are not allowed\";"
```

## No Single Male Guest

```csharp
// Predicate
"!(Input.IsSingleGuest && Input.Gender == \"Male\")"

// Result
"Output.IsAllowed = !(Input.IsSingleGuest && Input.Gender == \"Male\");\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"Single male guests are not allowed\";"
```

## Country Restriction

```csharp
// Predicate
"Input.CountryCode != \"XX\""

// Result
"Output.IsAllowed = Input.CountryCode != \"XX\";\nOutput.Reason = Output.IsAllowed ? \"OK\" : \"Bookings from this country are not allowed\";"
```
