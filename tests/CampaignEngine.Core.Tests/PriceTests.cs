using CampaignEngine.Core.Models;
using FluentAssertions;
using Xunit;

namespace CampaignEngine.Core.Tests.Models;

public class PriceTests
{
    [Fact]
    public void Constructor_ShouldCreatePrice()
    {
        var price = new Price(100, "TRY");
        price.Value.Should().Be(100);
        price.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Add_ShouldAddPrices()
    {
        var p1 = new Price(100, "TRY");
        var p2 = new Price(50, "TRY");
        var result = p1 + p2;
        result.Value.Should().Be(150);
    }

    [Fact]
    public void Subtract_ShouldSubtractPrices()
    {
        var p1 = new Price(100, "TRY");
        var p2 = new Price(30, "TRY");
        var result = p1 - p2;
        result.Value.Should().Be(70);
    }

    [Fact]
    public void Multiply_ShouldMultiplyPrice()
    {
        var price = new Price(100, "TRY");
        var result = price * 0.2m;
        result.Value.Should().Be(20);
    }

    [Fact]
    public void Divide_ShouldDividePrice()
    {
        var price = new Price(100, "TRY");
        var result = price / 2;
        result.Value.Should().Be(50);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var price = new Price(100.50m, "USD");
        price.ToString().Should().Be("USD100.50");
    }

    [Fact]
    public void FromString_ShouldParsePrice()
    {
        var price = Price.FromString("USD100.50");
        price.Currency.Should().Be("USD");
        price.Value.Should().Be(100.50m);
    }

    [Fact]
    public void Zero_ShouldReturnZeroPrice()
    {
        var zero = Price.Zero;
        zero.Value.Should().Be(0);
    }
}
