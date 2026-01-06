using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;

namespace RuleEngine.Core.Tests;

public class TestRuleInput : RuleInputModel
{
    public int IntProp { get; set; }
    public string StringProp { get; set; } = string.Empty;
    public List<string> StringList { get; set; } = new();
    public DateTime DateTime { get; set; }
    public int ClassType { get; set; }
    public decimal Price { get; set; }
}

public class TestRuleOutput
{
    public decimal TotalPrice { get; set; }
    public string Selected { get; set; } = string.Empty;
}

public class TestResultRuleSet : RuleSet<TestRuleInput, TestRuleOutput>
{
    public TestResultRuleSet(
        string code,
        CompiledRule<TestRuleInput, bool> predicateRule,
        CompiledRule<TestRuleInput, TestRuleOutput> resultRule,
        int priority)
        : base(code, predicateRule, resultRule, priority)
    {
    }

    public object? ExtraDtoModel { get; set; }
}

public class TestRuleProvider : IRuleProvider<TestResultRuleSet, TestRuleInput, TestRuleOutput>
{
    private readonly IDictionary<string, TestResultRuleSet> _ruleSets;

    public TestRuleProvider(IDictionary<string, TestResultRuleSet>? ruleSets)
    {
        _ruleSets = ruleSets ?? new Dictionary<string, TestResultRuleSet>();
    }

    public Task<IDictionary<string, TestResultRuleSet>> GenerateRuleSetsAsync(DateTime after)
    {
        var updated = _ruleSets
            .Where(kv => kv.Value.PredicateRule.CompileTime > after || kv.Value.ResultRule.CompileTime > after)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        return Task.FromResult<IDictionary<string, TestResultRuleSet>>(updated);
    }

    public Task<IDictionary<string, bool>> IsExistsAsync(params string[] keys)
    {
        var result = keys.ToDictionary(key => key, key => _ruleSets.ContainsKey(key));
        return Task.FromResult<IDictionary<string, bool>>(result);
    }
}
