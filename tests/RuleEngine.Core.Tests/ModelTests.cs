using FluentAssertions;
using RuleEngine.Core.Models;
using Xunit;

namespace RuleEngine.Core.Tests;

public class ModelTests
{
    [Fact]
    public void RuleInputModel_ShouldBeAbstract()
    {
        typeof(RuleInputModel).IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void CompiledRule_ShouldStoreMetadata()
    {
        var rule = new CompiledRule<TestInput, bool>
        {
            CompileTime = DateTime.Now,
            RuleString = "Input.Value > 10"
        };
        rule.CompileTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        rule.RuleString.Should().Be("Input.Value > 10");
    }

    [Fact]
    public void RuleSyntaxError_ShouldContainErrorInfo()
    {
        var error = new RuleSyntaxError
        {
            Line = 1,
            ChracterAt = 5,
            Title = "Syntax Error",
            Description = "Invalid syntax"
        };
        error.Line.Should().Be(1);
        error.Title.Should().Be("Syntax Error");
    }

    public class TestInput : RuleInputModel
    {
        public int Value { get; set; }
    }
}
