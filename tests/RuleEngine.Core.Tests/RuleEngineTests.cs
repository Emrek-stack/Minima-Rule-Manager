using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;
using Xunit;

namespace RuleEngine.Core.Tests
{
    public class RuleEngineTests
    {
        [Fact]
        public void RuleCompiler_ShouldCompileSimpleRule()
        {
            // Arrange
            var compiler = new RuleCompiler<TestInput, bool>();
            var ruleString = "Input.Value > 10";

            // Act
            var result = compiler.CheckSyntax(ruleString);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void RuleCompiler_ShouldDetectSyntaxError()
        {
            // Arrange
            var compiler = new RuleCompiler<TestInput, bool>();
            var ruleString = "Input.Value >"; // Invalid syntax

            // Act
            var result = compiler.CheckSyntax(ruleString);

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task RuleSet_ShouldCreateAndExecute()
        {
            // Arrange
            var predicateRule = "Input.Value > 10";
            var resultRule = "Output = Input.Value * 2;";

            // Act
            var ruleSet = await RuleSet.CreateAsync<TestInput, int>("test-rule", predicateRule, resultRule, 1);

            // Assert
            ruleSet.Should().NotBeNull();
            ruleSet.Code.Should().Be("test-rule");
            ruleSet.Priority.Should().Be(1);
        }

    }

    public class TestInput : RuleInputModel
    {
        public int Value { get; set; }
    }
}
