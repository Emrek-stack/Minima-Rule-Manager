using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Provides methods required by <see cref="RuleManager"/>. RuleManager treats providers as singletons per type.
/// </summary>
/// <typeparam name="TRuleSet"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IRuleProvider<TRuleSet, TInput, TOutput> : IRuleProvider
    where TRuleSet: RuleSet<TInput, TOutput>
    where TInput : RuleInputModel
{
    /// <summary>
    /// Compiles rules updated after <paramref name="after"/> and returns <typeparamref name="TRuleSet"/> instances.
    /// </summary>
    /// <param name="after"></param>
    /// <returns></returns>
    Task<IDictionary<string, TRuleSet>> GenerateRuleSetsAsync(DateTime after);

    /// <summary>
    /// Returns whether the specified rule sets exist and are active.
    /// </summary>
    /// <param name="keys">Keys to check.</param>
    /// <returns></returns>
    Task<IDictionary<string, bool>> IsExistsAsync(params string[] keys);
}

/// <summary>
/// Marker interface for provider caching. Prefer <see cref="IRuleProvider{TRuleSet, TInput, TOutput}"/>.
/// </summary>
public interface IRuleProvider { }
