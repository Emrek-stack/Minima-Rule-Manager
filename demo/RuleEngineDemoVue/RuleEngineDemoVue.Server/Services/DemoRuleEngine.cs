using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;

namespace RuleEngineDemoVue.Server.Services;

public sealed class DemoRuleEngine : IRuleEngine
{
    private readonly IRuleRepository _repository;
    private readonly IRuleEvaluator _evaluator;
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<DemoRuleEngine> _logger;

    public DemoRuleEngine(
        IRuleRepository repository,
        IRuleEvaluator evaluator,
        IAuditRepository auditRepository,
        ILogger<DemoRuleEngine> logger)
    {
        _repository = repository;
        _evaluator = evaluator;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public Task<RuleExecutionResult> EvaluateAsync(string ruleId, object input, CancellationToken cancellationToken = default)
    {
        return EvaluateInternalAsync(ruleId, input, cancellationToken);
    }

    public Task<RuleExecutionResult> EvaluateAsync<TInput>(string ruleId, TInput input, CancellationToken cancellationToken = default)
    {
        return EvaluateInternalAsync(ruleId, input!, cancellationToken);
    }

    private async Task<RuleExecutionResult> EvaluateInternalAsync(string ruleId, object input, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetActiveVersionAsync(ruleId);
        if (rule == null)
        {
            return new RuleExecutionResult
            {
                Success = false,
                ErrorMessage = "Rule not found."
            };
        }

        var stopwatch = Stopwatch.StartNew();
        var result = await _evaluator.EvaluateAsync(rule, input, cancellationToken);
        stopwatch.Stop();

        await _auditRepository.LogExecutionAsync(new RuleExecutionAudit
        {
            Id = Guid.NewGuid().ToString("N"),
            RuleId = rule.Id,
            RuleVersion = rule.Version,
            Input = JsonSerializer.Serialize(input),
            Output = JsonSerializer.Serialize(result.Result),
            Success = result.Success,
            ErrorMessage = result.ErrorMessage,
            Duration = stopwatch.Elapsed,
            ExecutedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Rule {RuleId} executed in {Duration}ms", rule.Id, stopwatch.Elapsed.TotalMilliseconds);
        result.Duration = stopwatch.Elapsed;
        return result;
    }
}
