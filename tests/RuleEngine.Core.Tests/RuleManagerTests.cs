using FluentAssertions;
using RuleEngine.Core.Rule;

namespace RuleEngine.Core.Tests;

public class RuleManagerTests
{
    private static readonly RuleCompiler<TestRuleInput, bool> PredicateCompiler = new();
    private static readonly RuleCompiler<TestRuleInput, TestRuleOutput> ResultCompiler = new();

    [Fact]
    public async Task Register_ShouldExposeRuleSets()
    {
        var provider = new TestRuleProvider(new Dictionary<string, TestResultRuleSet>
        {
            {
                "Rule1", new TestResultRuleSet(
                    "Rule1",
                    await PredicateCompiler.CompileAsync("Rule1.Predicate", ""),
                    await ResultCompiler.CompileAsync("Rule1.Result", ""),
                    0)
            },
            {
                "Rule2", new TestResultRuleSet(
                    "Rule2",
                    await PredicateCompiler.CompileAsync("Rule2.Predicate", ""),
                    await ResultCompiler.CompileAsync("Rule2.Result", ""),
                    0)
            }
        });

        var ruleSets = provider.GetRuleSets();

        ruleSets.Should().HaveCount(2);
        ruleSets.Should().ContainKey("Rule1");
        ruleSets.Should().ContainKey("Rule2");
    }

    [Fact]
    public async Task ProviderSingularity_ShouldReuseWorkerByType()
    {
        var provider1 = new TestRuleProvider(new Dictionary<string, TestResultRuleSet>
        {
            {
                "Rule1", new TestResultRuleSet(
                    "Rule1",
                    await PredicateCompiler.CompileAsync("Provider1.Rule1.Predicate", ""),
                    await ResultCompiler.CompileAsync("Provider1.Rule1.Result", ""),
                    0)
            },
            {
                "Rule2", new TestResultRuleSet(
                    "Rule2",
                    await PredicateCompiler.CompileAsync("Provider1.Rule2.Predicate", ""),
                    await ResultCompiler.CompileAsync("Provider1.Rule2.Result", ""),
                    0)
            }
        });

        var provider2 = new TestRuleProvider(new Dictionary<string, TestResultRuleSet>());

        var ruleSets1 = provider1.GetRuleSets();
        var ruleSets2 = provider2.GetRuleSets();

        ruleSets2.Keys.Should().BeEquivalentTo(ruleSets1.Keys);
    }
}
