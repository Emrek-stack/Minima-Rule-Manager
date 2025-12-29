using RuleEngine.Core.Models;

namespace RuleEngine.Core.Abstractions;

/// <summary>
/// Main interface for the rule engine
/// </summary>
public interface IRuleEngine
{
    /// <summary>
    /// Evaluates a rule with the given input
    /// </summary>
    /// <param name="ruleId">ID of the rule to evaluate</param>
    /// <param name="input">Input data for the rule</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the rule evaluation</returns>
    Task<RuleExecutionResult> EvaluateAsync(string ruleId, object input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates a rule with strongly-typed input
    /// </summary>
    /// <typeparam name="TInput">Type of the input</typeparam>
    /// <param name="ruleId">ID of the rule to evaluate</param>
    /// <param name="input">Input data for the rule</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of the rule evaluation</returns>
    Task<RuleExecutionResult> EvaluateAsync<TInput>(string ruleId, TInput input, CancellationToken cancellationToken = default);
}