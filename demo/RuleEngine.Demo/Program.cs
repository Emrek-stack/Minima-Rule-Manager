using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Managers;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule.DesignTime;
using RuleEngine.Core.Rule.DesignTime.Parameters;
using RuleEngine.Core.Rule.DesignTime.Statements;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddRuleEngine();
services.AddRuleEngineDesignTime();
services.AddSingleton<IRuleRepository, InMemoryRuleRepository>();
services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
services.AddSingleton<IRuleEvaluator, DemoRuleEvaluator>();
services.AddSingleton<IRuleEngine, DemoRuleEngine>();
services.AddSingleton<IRuleManager, RuleManager>();

var provider = services.BuildServiceProvider();

var ruleRepository = provider.GetRequiredService<IRuleRepository>();
var auditRepository = provider.GetRequiredService<IAuditRepository>();
var evaluator = provider.GetRequiredService<IRuleEvaluator>();
var ruleEngine = provider.GetRequiredService<IRuleEngine>();
var ruleManager = provider.GetRequiredService<IRuleManager>();

var discountRule = await ruleRepository.CreateAsync(new CreateRuleRequest
{
    Name = "Order Discount",
    Description = "Applies a discount for high-value US orders",
    Tags = new[] { "pricing", "orders" },
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount >= 100m && Input.Country == \"US\"",
        ResultExpression = "Input.Amount * 0.9m",
        Metadata = new Dictionary<string, object>
        {
            ["DesignTime"] = new DesignTimeMetadata
            {
                Name = "OrderDiscount",
                Title = "Order Discount",
                Description = "Discount rule for US orders",
                ExpressionFormat = "Input.Amount {0} {1} && Input.Country == {2}",
                DisplayFormat = "{0} {1} {2}",
                IsPredicate = true,
                Parameters = new List<ParameterDefinition>
                {
                    new EqualityListParameter("Operator"),
                    new NumericParameter("Amount"),
                    new StringParameter("Country")
                },
                Categories = new List<RuleCategoryMetadata>
                {
                    new RuleCategoryMetadata { Id = 1, Title = "Pricing", Status = 1 }
                }
            }
        }
    }
});

await ruleRepository.ActivateVersionAsync(discountRule.Id, 1);

await MetadataManager.RefreshAsync();

var namedRule = new NamedRuleStatement
{
    Name = "OrderDiscount",
    ParameterValues = new List<string> { ">=", "100", "US" }
};
var generatedRule = RuleGenerator.Generate(namedRule, namedRules: MetadataManager.NamedRuleMetadatas);
var parsedTree = RuleParser.Parse(generatedRule);

Console.WriteLine("Design-time generated rule:");
Console.WriteLine(generatedRule);
Console.WriteLine("Parsed tree type: " + parsedTree.Type);

var order = new OrderInput
{
    OrderId = "ORD-1001",
    Amount = 150m,
    Country = "US",
    CustomerTier = "Gold"
};

var engineResult = await ruleEngine.EvaluateAsync(discountRule.Id, order);
Console.WriteLine($"IRuleEngine result: {engineResult.Result}");

ruleManager.RegisterProvider("discount", new DemoRuleProvider(ruleRepository, evaluator, discountRule.Id));
var managerResult = await ruleManager.ExecuteRuleAsync("discount", order);
Console.WriteLine($"IRuleManager result: {managerResult.Result}");

var history = await auditRepository.GetExecutionHistoryAsync(discountRule.Id, limit: 5);
Console.WriteLine("Audit history:");
foreach (var audit in history)
{
    Console.WriteLine($"- {audit.ExecutedAt:u} success={audit.Success} duration={audit.Duration.TotalMilliseconds}ms output={audit.Output}");
}

var newVersion = await ruleRepository.CreateVersionAsync(discountRule.Id, new CreateVersionRequest
{
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount >= 200m && Input.Country == \"US\"",
        ResultExpression = "Input.Amount * 0.85m"
    },
    Activate = true
});

var updatedResult = await ruleEngine.EvaluateAsync(newVersion.Id, order);
Console.WriteLine($"After version update result: {updatedResult.Result}");

var invalidRule = new RuleDefinition
{
    Id = "invalid",
    Name = "InvalidRule",
    Version = 1,
    Status = RuleStatus.Active,
    Content = new RuleContent
    {
        PredicateExpression = "Input.Amount >=",
        ResultExpression = "Input.Amount *"
    }
};

var validation = await evaluator.ValidateAsync(invalidRule, order);
Console.WriteLine($"Validation success: {validation.IsValid}");
if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
        Console.WriteLine($"- {error}");
}

public class OrderInput : RuleInputModel
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Country { get; set; } = string.Empty;
    public string CustomerTier { get; set; } = string.Empty;
}

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

public sealed class DemoRuleEvaluator : IRuleEvaluator
{
    private readonly ConcurrentDictionary<string, (RuleEngine.Core.Models.CompiledRule<OrderInput, bool> Predicate, RuleEngine.Core.Models.CompiledRule<OrderInput, decimal> Result)> _cache
        = new ConcurrentDictionary<string, (RuleEngine.Core.Models.CompiledRule<OrderInput, bool>, RuleEngine.Core.Models.CompiledRule<OrderInput, decimal>)>();

    private readonly RuleEngine.Core.Rule.RuleCompiler<OrderInput, bool> _predicateCompiler = new RuleEngine.Core.Rule.RuleCompiler<OrderInput, bool>();
    private readonly RuleEngine.Core.Rule.RuleCompiler<OrderInput, decimal> _resultCompiler = new RuleEngine.Core.Rule.RuleCompiler<OrderInput, decimal>(useExpressionTreeTemplate: true);

    public async Task<RuleExecutionResult> EvaluateAsync(RuleDefinition rule, object input, CancellationToken cancellationToken = default)
    {
        if (input is not OrderInput order)
            throw new ArgumentException("Input must be OrderInput", nameof(input));

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

    private async Task<(RuleEngine.Core.Models.CompiledRule<OrderInput, bool> Predicate, RuleEngine.Core.Models.CompiledRule<OrderInput, decimal> Result)> GetCompiledAsync(RuleDefinition rule)
    {
        var cacheKey = $"{rule.Id}:{rule.Version}";
        if (_cache.TryGetValue(cacheKey, out var compiled))
            return compiled;

        var predicate = await _predicateCompiler.CompileAsync(rule.Name + ".Predicate", rule.Content.PredicateExpression);
        var result = await _resultCompiler.CompileAsync(rule.Name + ".Result", rule.Content.ResultExpression);

        compiled = (predicate, result);
        _cache[cacheKey] = compiled;
        return compiled;
    }
}

public sealed class DemoRuleProvider : IRuleProvider
{
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEvaluator _evaluator;
    private readonly string _ruleId;

    public DemoRuleProvider(IRuleRepository ruleRepository, IRuleEvaluator evaluator, string ruleId)
    {
        _ruleRepository = ruleRepository;
        _evaluator = evaluator;
        _ruleId = ruleId;
    }

    public Task<RuleDefinition?> GetRuleDefinitionAsync(object input)
    {
        return _ruleRepository.GetActiveVersionAsync(_ruleId);
    }

    public async Task<bool> ValidateRuleAsync(RuleDefinition rule, object input)
    {
        var validation = await _evaluator.ValidateAsync(rule, input);
        return validation.IsValid;
    }
}

public sealed class InMemoryRuleRepository : IRuleRepository
{
    private readonly ConcurrentDictionary<string, RuleEntry> _rules = new ConcurrentDictionary<string, RuleEntry>();

    public Task<RuleDefinition?> GetByIdAsync(string id)
    {
        return Task.FromResult(_rules.TryGetValue(id, out var entry) ? entry.BuildActiveDefinition() : null);
    }

    public Task<RuleDefinition?> GetActiveVersionAsync(string ruleId)
    {
        return Task.FromResult(_rules.TryGetValue(ruleId, out var entry) ? entry.BuildActiveDefinition() : null);
    }

    public Task<IEnumerable<RuleDefinition>> GetAllAsync()
    {
        var values = _rules.Values.Select(entry => entry.BuildActiveDefinition());
        return Task.FromResult(values);
    }

    public Task<RuleDefinition> CreateAsync(CreateRuleRequest request)
    {
        var ruleId = Guid.NewGuid().ToString("N");
        var now = DateTime.UtcNow;

        var entry = new RuleEntry
        {
            Id = ruleId,
            Name = request.Name,
            Description = request.Description,
            Tags = request.Tags,
            Status = RuleStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now,
            Parameters = request.Parameters,
            Versions = new List<RuleContent> { request.Content },
            ActiveVersion = 1
        };

        _rules[ruleId] = entry;
        return Task.FromResult(entry.BuildActiveDefinition());
    }

    public Task<RuleDefinition> UpdateAsync(string id, UpdateRuleRequest request)
    {
        if (!_rules.TryGetValue(id, out var entry))
            throw new ArgumentException("Rule not found.", nameof(id));

        if (request.Name != null)
            entry.Name = request.Name;
        if (request.Description != null)
            entry.Description = request.Description;
        if (request.Tags != null)
            entry.Tags = request.Tags;
        if (request.Status.HasValue)
            entry.Status = request.Status.Value;
        if (request.Parameters != null)
            entry.Parameters = request.Parameters;

        if (request.Content != null)
        {
            entry.Versions.Add(request.Content);
            entry.ActiveVersion = entry.Versions.Count;
        }

        entry.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(entry.BuildActiveDefinition());
    }

    public Task DeleteAsync(string id)
    {
        _rules.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<RuleDefinition> CreateVersionAsync(string ruleId, CreateVersionRequest request)
    {
        if (!_rules.TryGetValue(ruleId, out var entry))
            throw new ArgumentException("Rule not found.", nameof(ruleId));

        entry.Versions.Add(request.Content);
        entry.UpdatedAt = DateTime.UtcNow;
        if (request.Parameters.Count > 0)
            entry.Parameters = request.Parameters;

        if (request.Activate)
            entry.ActiveVersion = entry.Versions.Count;

        return Task.FromResult(entry.BuildActiveDefinition());
    }

    public Task<RuleDefinition> ActivateVersionAsync(string ruleId, int version)
    {
        if (!_rules.TryGetValue(ruleId, out var entry))
            throw new ArgumentException("Rule not found.", nameof(ruleId));
        if (version < 1 || version > entry.Versions.Count)
            throw new ArgumentOutOfRangeException(nameof(version), "Version out of range.");

        entry.ActiveVersion = version;
        entry.Status = RuleStatus.Active;
        entry.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(entry.BuildActiveDefinition());
    }

    private sealed class RuleEntry
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
        public RuleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public List<RuleContent> Versions { get; set; } = new();
        public int ActiveVersion { get; set; } = 1;

        public RuleDefinition BuildActiveDefinition()
        {
            var versionIndex = Math.Max(1, ActiveVersion) - 1;
            var content = Versions.Count > versionIndex ? Versions[versionIndex] : new RuleContent();

            return new RuleDefinition
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Tags = Tags,
                Status = Status == RuleStatus.Draft ? RuleStatus.Active : Status,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                Version = ActiveVersion,
                Content = content,
                Parameters = Parameters
            };
        }
    }
}

public sealed class InMemoryAuditRepository : IAuditRepository
{
    private readonly ConcurrentQueue<RuleExecutionAudit> _audits = new ConcurrentQueue<RuleExecutionAudit>();

    public Task LogExecutionAsync(RuleExecutionAudit audit)
    {
        _audits.Enqueue(audit);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<RuleExecutionAudit>> GetExecutionHistoryAsync(string ruleId, int limit = 100)
    {
        var history = _audits.Where(audit => audit.RuleId == ruleId)
            .OrderByDescending(audit => audit.ExecutedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult<IEnumerable<RuleExecutionAudit>>(history);
    }
}
