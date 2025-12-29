using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Managers
{
    /// <summary>
    /// Rule execution manager based on Gordios.RuleEngine flow.
    /// </summary>
    public class RuleManager : IRuleManager
    {
        private readonly ConcurrentDictionary<string, IRuleProvider> _providers = new ConcurrentDictionary<string, IRuleProvider>();
        private readonly IRuleEvaluator _evaluator;
        private readonly ILogger<RuleManager> _logger;

        public RuleManager(IRuleEvaluator evaluator, ILogger<RuleManager> logger)
        {
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void RegisterProvider(string ruleType, IRuleProvider provider)
        {
            if (string.IsNullOrWhiteSpace(ruleType))
                throw new ArgumentException("Rule type cannot be null or empty.", nameof(ruleType));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _providers[ruleType] = provider;
        }

        public async Task<RuleExecutionResult> ExecuteRuleAsync(string ruleType, object input)
        {
            if (!_providers.TryGetValue(ruleType, out var provider))
            {
                return new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = $"No provider registered for rule type '{ruleType}'."
                };
            }

            RuleDefinition? rule;
            try
            {
                rule = await provider.GetRuleDefinitionAsync(input);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get rule definition for rule type {RuleType}", ruleType);
                return new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Failed to get rule definition."
                };
            }

            if (rule == null)
            {
                return new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Rule definition not found."
                };
            }

            try
            {
                var isValid = await provider.ValidateRuleAsync(rule, input);
                if (!isValid)
                {
                    return new RuleExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Rule validation failed."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rule validation failed for rule type {RuleType}", ruleType);
                return new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Rule validation failed."
                };
            }

            try
            {
                return await _evaluator.EvaluateAsync(rule, input);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rule execution failed for rule type {RuleType}", ruleType);
                return new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = "Rule execution failed."
                };
            }
        }
    }
}
