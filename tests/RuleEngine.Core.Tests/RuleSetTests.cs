using FluentAssertions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;

namespace RuleEngine.Core.Tests;

public class RuleSetTests
{
    [Fact]
    public async Task DefaultCompilingSingleResult_ShouldReturnExpectedOutput()
    {
        var ruleSet = await RuleSet.CreateAsync<TestRuleInput, TestRuleOutput>(
            "SingleResult",
            "Input.StringList.Contains(\"TK\") && Input.IntProp == 5",
            "Output.TotalPrice = Input.Price + 5;",
            0);

        var input = new TestRuleInput
        {
            StringList = new List<string> { "TK" },
            IntProp = 5,
            Price = 1.2M
        };

        ruleSet.Predicate(input).Should().BeTrue();
        ruleSet.GetResult(input).TotalPrice.Should().Be(6.2M);
    }

    [Fact]
    public async Task PreCompiledSingleResult_ShouldReturnExpectedOutput()
    {
        var predicate = await new RuleCompiler<TestRuleInput, bool>()
            .CompileAsync("PreCompiled.Predicate", "Input.StringList.Contains(\"TK\") && Input.IntProp == 5");
        var result = await new RuleCompiler<TestRuleInput, TestRuleOutput>(useExpressionTreeTemplate: false)
            .CompileAsync("PreCompiled.Result", "Output.TotalPrice = Input.Price + 5;");

        var ruleSet = RuleSet.Create("PreCompiled", predicate, result, 0);

        ruleSet.Predicate(new TestRuleInput { StringList = new List<string> { "TK" }, IntProp = 5 })
            .Should().BeTrue();
        ruleSet.GetResult(new TestRuleInput { Price = 1.2M }).TotalPrice.Should().Be(6.2M);
    }

    [Fact]
    public async Task DefaultCompilingMultipleResult_ShouldSelectExpectedOutput()
    {
        var ruleSet = await RuleSet.CreateMultiResultAsync<TestRuleInput, TestRuleOutput>(
            "MultiResult",
            "Input.StringList.Contains(\"TK\") && Input.IntProp == 5",
            new Dictionary<string, string>
            {
                { "Input.ClassType == 1", "Output.Selected = \"Economy\";" },
                { "Input.ClassType == 2", "Output.Selected = \"Business\";" },
                { "Input.ClassType == 3", "Output.Selected = \"First\";" },
                { "Input.ClassType == 4", "Output.Selected = \"Comfort\";" }
            },
            0);

        ruleSet.Predicate(new TestRuleInput { StringList = new List<string> { "TK" }, IntProp = 5 })
            .Should().BeTrue();
        ruleSet.GetResult(new TestRuleInput { ClassType = 1 }).Selected.Should().Be("Economy");
        ruleSet.GetResult(new TestRuleInput { ClassType = 2 }).Selected.Should().Be("Business");
        ruleSet.GetResult(new TestRuleInput { ClassType = 3 }).Selected.Should().Be("First");
        ruleSet.GetResult(new TestRuleInput { ClassType = 4 }).Selected.Should().Be("Comfort");
    }

    [Fact]
    public async Task PreCompiledMultipleResult_ShouldSelectExpectedOutput()
    {
        var predicateCompiler = new RuleCompiler<TestRuleInput, bool>();
        var resultPredicateCompiler = new RuleCompiler<TestRuleInput, bool>();
        var resultCompiler = new RuleCompiler<TestRuleInput, TestRuleOutput>(useExpressionTreeTemplate: false);

        var predicate = await predicateCompiler.CompileAsync(
            "MultiResult.Predicate",
            "Input.StringList.Contains(\"TK\") && Input.IntProp == 5");

        var resultPredicates = await resultPredicateCompiler.CompileAsync(
            "MultiResult.ResultPredicates",
            new[]
            {
                "Input.ClassType == 1",
                "Input.ClassType == 2",
                "Input.ClassType == 3",
                "Input.ClassType == 4"
            });

        var results = await resultCompiler.CompileAsync(
            "MultiResult.Results",
            new[]
            {
                "Output.Selected = \"Economy\";",
                "Output.Selected = \"Business\";",
                "Output.Selected = \"First\";",
                "Output.Selected = \"Comfort\";"
            });

        var pairs = resultPredicates.Zip(
            results,
            (k, v) => new KeyValuePair<CompiledRule<TestRuleInput, bool>, CompiledRule<TestRuleInput, TestRuleOutput>>(k, v));

        var ruleSet = RuleSet.CreateMultiResult("MultiResult", predicate, pairs, 0);

        ruleSet.Predicate(new TestRuleInput { StringList = new List<string> { "TK" }, IntProp = 5 })
            .Should().BeTrue();
        ruleSet.GetResult(new TestRuleInput { ClassType = 1 }).Selected.Should().Be("Economy");
        ruleSet.GetResult(new TestRuleInput { ClassType = 2 }).Selected.Should().Be("Business");
        ruleSet.GetResult(new TestRuleInput { ClassType = 3 }).Selected.Should().Be("First");
        ruleSet.GetResult(new TestRuleInput { ClassType = 4 }).Selected.Should().Be("Comfort");
    }
}
