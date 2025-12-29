using FluentAssertions;
using RuleEngine.Core.Models;

namespace RuleEngine.Integration.Tests;

public class BasicIntegrationTests
{
    [Fact]
    public void RuleInputModel_CanBeInherited()
    {
        var input = new TestInput { Value = 10 };
        input.Value.Should().Be(10);
    }

    [Fact]
    public void CompiledRule_CanStoreMetadata()
    {
        var rule = new CompiledRule<TestInput, bool>
        {
            CompileTime = DateTime.Now,
            RuleString = "test"
        };
        rule.RuleString.Should().Be("test");
    }

    public class TestInput : RuleInputModel
    {
        public int Value { get; set; }
    }
}
