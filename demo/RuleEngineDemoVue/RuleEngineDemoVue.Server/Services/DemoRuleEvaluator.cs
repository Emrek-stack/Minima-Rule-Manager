using Microsoft.Extensions.Caching.Memory;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;
using RuleEngineDemoVue.Server.Models;

namespace RuleEngineDemoVue.Server.Services;

public sealed class DemoRuleEvaluator : IRuleEvaluator
{
    private sealed class CompiledRules
    {
        public CompiledRules(
            CompiledRule<OrderRuleInput, bool> predicate,
            CompiledRule<OrderRuleInput, object> result)
        {
            Predicate = predicate;
            Result = result;
        }

        public CompiledRule<OrderRuleInput, bool> Predicate { get; }
        public CompiledRule<OrderRuleInput, object> Result { get; }
    }

    private static readonly MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromMinutes(30)
    };

    private readonly IMemoryCache _cache;

    private readonly RuleCompiler<OrderRuleInput, bool> _predicateCompiler = new RuleCompiler<OrderRuleInput, bool>();
    private readonly RuleCompiler<OrderRuleInput, object> _resultCompiler = new RuleCompiler<OrderRuleInput, object>();

    public DemoRuleEvaluator(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<RuleExecutionResult> EvaluateAsync(RuleDefinition rule, object input, CancellationToken cancellationToken = default)
    {
        if (input is not OrderRuleInput order)
            throw new ArgumentException("Input must be OrderRuleInput", nameof(input));

        var compiled = await GetCompiledAsync(rule);
        var matched = compiled.Predicate.Invoke(order);
        if (!matched)
        {
            return new RuleExecutionResult
            {
                Success = true,
                Result = null,
                Metadata = new Dictionary<string, object> { ["PredicateMatched"] = false }
            };
        }

        var result = compiled.Result.Invoke(order);
        return new RuleExecutionResult
        {
            Success = true,
            Result = result,
            Metadata = new Dictionary<string, object> { ["PredicateMatched"] = true }
        };
    }

    public Task<ValidationResult> ValidateAsync(RuleDefinition rule, object input)
    {
        var errors = new List<string>();

        errors.AddRange(_predicateCompiler.CheckSyntax(rule.Content.PredicateExpression)
            .Select(err => $"Predicate: {err.Description}"));
        errors.AddRange(_resultCompiler.CheckSyntax(rule.Content.ResultExpression)
            .Select(err => $"Result: {err.Description}"));

        return Task.FromResult(errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors.ToArray()));
    }

    private async Task<CompiledRules> GetCompiledAsync(RuleDefinition rule)
    {
        var cacheKey = $"{rule.Id}:{rule.Version}";
        if (_cache.TryGetValue(cacheKey, out CompiledRules? compiled) && compiled != null)
            return compiled;

        var predicate = await _predicateCompiler.CompileAsync(rule.Name + ".Predicate", rule.Content.PredicateExpression);
        var result = await _resultCompiler.CompileAsync(rule.Name + ".Result", rule.Content.ResultExpression);

        compiled = new CompiledRules(predicate, result);
        _cache.Set(cacheKey, compiled, CacheOptions);
        return compiled;
    }
}
