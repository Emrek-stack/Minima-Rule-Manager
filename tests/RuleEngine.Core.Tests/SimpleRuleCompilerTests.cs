using FluentAssertions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;

namespace RuleEngine.Core.Tests;

public class SimpleRuleCompilerTests
{
    [Fact]
    public void CheckSyntax_WithValidRule_ShouldReturnEmpty()
    {
        var compiler = new RuleCompiler<TestInput, bool>();
        var errors = compiler.CheckSyntax("Input.Value > 10");
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CheckSyntax_WithInvalidRule_ShouldReturnErrors()
    {
        var compiler = new RuleCompiler<TestInput, bool>();
        var errors = compiler.CheckSyntax("Input.Value >");
        errors.Should().NotBeEmpty();
    }

    public class TestInput : RuleInputModel
    {
        public int Value { get; set; }
    }
}
