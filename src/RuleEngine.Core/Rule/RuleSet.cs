using System.Runtime.ExceptionServices;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Rule set composed of predicate and result rules. Produces a single result for a single match.
/// </summary>
/// <remarks>If you do not derive a custom ruleset, use helper methods under <see cref="RuleSet"/> to create one.</remarks>
/// <typeparam name="TInput">Input model.</typeparam>
/// <typeparam name="TOutput">Output model.</typeparam>
public class RuleSet<TInput, TOutput> where TInput : RuleInputModel
{
    /// <summary>
    /// Each rule must have a unique ID.
    /// </summary>
    public string Code { get; internal set; }

    /// <summary>
    /// Compiled predicate rule.
    /// </summary>
    public CompiledRule<TInput, bool> PredicateRule { get; internal set; }

    /// <summary>
    /// Compiled result rule.
    /// </summary>
    public CompiledRule<TInput, TOutput> ResultRule { get; set; }

    /// <summary>
    /// Priority.
    /// </summary>
    public int Priority { get; internal set; }

    /// <summary>
    /// Parameterless constructor used by factories; do not call directly.
    /// </summary>
    public RuleSet()
    {
        Code = null!;
        PredicateRule = null!;
        ResultRule = null!;
    }
    /// <summary>
    /// Base constructor for rule sets.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="predicateRule"></param>
    /// <param name="resultRule"></param>
    /// <param name="priority"></param>
    protected RuleSet(string code, CompiledRule<TInput, bool> predicateRule, CompiledRule<TInput, TOutput> resultRule, int priority)
    {
        if (code == null)
            throw new ArgumentNullException(nameof(code));
        if (predicateRule == null)
            throw new ArgumentNullException(nameof(predicateRule));
        if (resultRule == null)
            throw new ArgumentNullException(nameof(resultRule));

        Code = code;
        PredicateRule = predicateRule;
        ResultRule = resultRule;
        Priority = priority;
    }

    /// <summary>
    /// Executes the predicate rule with the provided <paramref name="input"/>.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual bool Predicate(TInput input)
    {
        try
        {
            return PredicateRule.Invoke(input);
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, PredicateRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
            throw;
        }
    }

    /// <summary>
    /// Executes the result rule with the provided <paramref name="input"/>.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual TOutput GetResult(TInput input)
    {
        try
        {
            return ResultRule.Invoke(input);
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, ResultRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
            throw;
        }
    }

    internal static RuleSet<TInput, TOutput> Create(string code,
        CompiledRule<TInput, bool> predicateRule,
        CompiledRule<TInput, TOutput> resultRule,
        int priority)
    {
        return new RuleSet<TInput, TOutput>(code, predicateRule, resultRule, priority);
    }
    internal static readonly RuleCompiler<TInput, bool> DefaultPredicateCompiler = new RuleCompiler<TInput, bool>();
    internal static readonly RuleCompiler<TInput, TOutput> DefaultResultCompiler = new RuleCompiler<TInput, TOutput>(useExpressionTreeTemplate: false);
}

/// <summary>
/// Rule set that can select among multiple results.
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public class MultiResultRuleSet<TInput, TOutput> : RuleSet<TInput, TOutput> where TInput : RuleInputModel
{
    /// <summary>
    /// Creates a multi-result rule set.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="predicateRule"></param>
    /// <param name="resultRule"></param>
    /// <param name="priority"></param>
    public MultiResultRuleSet(string code, CompiledRule<TInput, bool> predicateRule, CompiledRule<TInput, TOutput> resultRule, int priority)
        : base(code, predicateRule, resultRule, priority)
    {
    }

    /// <summary>
    /// Creates a multi-result rule set.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="predicateRule"></param>
    /// <param name="resultRules"></param>
    /// <param name="priority"></param>
    public MultiResultRuleSet(
        string code,
        CompiledRule<TInput, bool> predicateRule,
        IEnumerable<KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>> resultRules,
        int priority)
        : base(code, predicateRule, BuildResultRule(resultRules), priority)
    {
    }

    /// <summary>
    /// Creates a multi-result rule set.
    /// </summary>
    public MultiResultRuleSet()
    {
    }

    /// <summary>
    /// Executes the result rule and selects from available results.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual TOutput GetResult(TInput input, IEnumerable<TOutput> availableResults)
    {
        try
        {
            var result = ResultRule.Invoke(input);
            return availableResults.FirstOrDefault(r => r!.Equals(result)) ?? result;
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, ResultRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
            throw;
        }
    }

    private static CompiledRule<TInput, TOutput> BuildResultRule(
        IEnumerable<KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>> resultRules)
    {
        var rulePairs = resultRules?.ToList() ??
                        new List<KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>>();
        var compileTime = rulePairs.Count == 0
            ? DateTime.MinValue
            : rulePairs.Max(rr => rr.Key.CompileTime > rr.Value.CompileTime ? rr.Key.CompileTime : rr.Value.CompileTime);
        return new CompiledRule<TInput, TOutput>(
            input =>
            {
                var resultRule = rulePairs.FirstOrDefault(r => r.Key.Invoke(input)).Value;
                return resultRule == null ? default! : resultRule.Invoke(input);
            },
            compileTime);
    }

    internal static MultiResultRuleSet<TInput, TOutput> Create(
        string code,
        CompiledRule<TInput, bool> predicateRule,
        IEnumerable<KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>> resultRules,
        int priority)
    {
        return new MultiResultRuleSet<TInput, TOutput>(code, predicateRule, resultRules, priority);
    }
}

/// <summary>
/// Helper methods for creating <c>RuleSet</c> instances.
/// </summary>
public static class RuleSet
{
    /// <summary>
    /// Creates a single-result rule set from compiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRule">Compiled predicate rule.</param>
    /// <param name="resultRule">Compiled result rule.</param>
    /// <param name="priority">Priority.</param>
    /// <returns></returns>
    public static RuleSet<TInput, TOutput> Create<TInput, TOutput>(
        string code,
        CompiledRule<TInput, bool> predicateRule,
        CompiledRule<TInput, TOutput> resultRule,
        int priority)
        where TInput : RuleInputModel
    {
        return RuleSet<TInput, TOutput>.Create(code, predicateRule, resultRule, priority);
    }

    public static TRuleSet Create<TRuleSet, TInput, TOutput>(
        string code,
        CompiledRule<TInput, bool> compiledPredicateRule,
        CompiledRule<TInput, TOutput> compiledResultRule,
        int priority)
        where TInput : RuleInputModel
        where TRuleSet : RuleSet<TInput, TOutput>, new()
    {
        return new TRuleSet
        {
            Code = code,
            PredicateRule = compiledPredicateRule,
            ResultRule = compiledResultRule,
            Priority = priority
        };
    }

    /// <summary>
    /// Creates a multi-result rule set from compiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRule">Compiled predicate rule.</param>
    /// <param name="resultRule">Compiled result rule.</param>
    /// <param name="priority">Priority.</param>
    /// <returns></returns>
    public static MultiResultRuleSet<TInput, TOutput> CreateMultiResult<TInput, TOutput>(
        string code,
        CompiledRule<TInput, bool> predicateRule,
        CompiledRule<TInput, TOutput> resultRule,
        int priority)
        where TInput : RuleInputModel
    {
        return new MultiResultRuleSet<TInput, TOutput>(code, predicateRule, resultRule, priority);
    }

    /// <summary>
    /// Creates a multi-result rule set from compiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRule">Compiled predicate rule.</param>
    /// <param name="resultRules">Compiled predicate/result rule pairs.</param>
    /// <param name="priority">Priority.</param>
    /// <returns></returns>
    public static MultiResultRuleSet<TInput, TOutput> CreateMultiResult<TInput, TOutput>(
        string code,
        CompiledRule<TInput, bool> predicateRule,
        IEnumerable<KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>> resultRules,
        int priority)
        where TInput : RuleInputModel
    {
        return MultiResultRuleSet<TInput, TOutput>.Create(code, predicateRule, resultRules, priority);
    }

    /// <summary>
    /// Creates a single-result rule set from uncompiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRuleString">Uncompiled predicate rule.</param>
    /// <param name="resultRuleString">Uncompiled result rule.</param>
    /// <param name="priority">Priority.</param>
    /// <returns></returns>
    public static async Task<RuleSet<TInput, TOutput>> CreateAsync<TInput, TOutput>(
        string code,
        string predicateRuleString,
        string resultRuleString,
        int priority)
        where TInput : RuleInputModel
    {
        var predicateRule = await RuleSet<TInput, TOutput>.DefaultPredicateCompiler.CompileAsync(code, predicateRuleString);
        var resultRule = await RuleSet<TInput, TOutput>.DefaultResultCompiler.CompileAsync(code, resultRuleString);
        return Create(code, predicateRule, resultRule, priority);
    }

    /// <summary>
    /// Creates a multi-result rule set from uncompiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRuleString">Uncompiled predicate rule.</param>
    /// <param name="resultRuleString">Uncompiled result rule.</param>
    /// <param name="priority">Priority.</param>
    /// <returns></returns>
    public static async Task<MultiResultRuleSet<TInput, TOutput>> CreateMultiResultAsync<TInput, TOutput>(
        string code,
        string predicateRuleString,
        string resultRuleString,
        int priority)
        where TInput : RuleInputModel
    {
        var predicateRule = await RuleSet<TInput, TOutput>.DefaultPredicateCompiler.CompileAsync(code, predicateRuleString);
        var resultRule = await RuleSet<TInput, TOutput>.DefaultResultCompiler.CompileAsync(code, resultRuleString);
        return CreateMultiResult(code, predicateRule, resultRule, priority);
    }

    /// <summary>
    /// Creates a multi-result rule set from uncompiled rules.
    /// </summary>
    /// <typeparam name="TInput">Input model for predicate and result.</typeparam>
    /// <typeparam name="TOutput">Output model for the result rule.</typeparam>
    /// <param name="code">Unique rule ID.</param>
    /// <param name="predicateRuleString">Uncompiled predicate rule.</param>
    /// <param name="resultRules">Uncompiled predicate/result rule pairs.</param>
    /// <param name="priority">Priority.</param>
    /// <param name="extraTypes">Additional types for this compilation only.</param>
    /// <returns></returns>
    public static async Task<MultiResultRuleSet<TInput, TOutput>> CreateMultiResultAsync<TInput, TOutput>(
        string code,
        string predicateRuleString,
        IDictionary<string, string> resultRules,
        int priority,
        params Type[] extraTypes)
        where TInput : RuleInputModel
    {
        var compiledPredicate = await RuleSet<TInput, TOutput>.DefaultPredicateCompiler
            .CompileAsync($"{code}.Predicate", predicateRuleString, extraTypes);
        var compiledResultPredicates = await RuleSet<TInput, TOutput>
            .DefaultPredicateCompiler.CompileAsync($"{code}.ResultPredicates", resultRules.Keys, extraTypes);
        var compiledResults = await RuleSet<TInput, TOutput>
            .DefaultResultCompiler.CompileAsync($"{code}.Results", resultRules.Values, extraTypes);
        var compiledResultsWithPredicates = compiledResultPredicates.Zip(compiledResults,
            (k, v) => new KeyValuePair<CompiledRule<TInput, bool>, CompiledRule<TInput, TOutput>>(k, v));
        return MultiResultRuleSet<TInput, TOutput>.Create(code, compiledPredicate, compiledResultsWithPredicates, priority);
    }
}
