using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule.DesignTime.Metadatas;
using RuleEngine.Core.Rule.DesignTime.Parameters;

namespace RuleEngine.Core.Rule.DesignTime;

/// <summary>
/// Provides access to design-time metadata.
/// </summary>
public static class MetadataManager
{
    private static readonly object SyncRoot = new object();
    private static CancellationTokenSource? _cancelToken;
    private static Task? _backgroundTask;
    private static TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);
    private static IRuleRepository? _ruleRepository;
    private static ILogger? _logger;
    private static int _initialized;

    public static bool Initialized => _initialized > 0;

    /// <summary>
    /// Cached rule metadata.
    /// </summary>
    public static ConcurrentDictionary<string, NamedRuleMetadata> NamedRuleMetadatas { get; } = new ConcurrentDictionary<string, NamedRuleMetadata>();

    /// <summary>
    /// Rule names by category.
    /// </summary>
    public static ConcurrentDictionary<string, List<string>> RulesByCategory { get; } = new ConcurrentDictionary<string, List<string>>();

    /// <summary>
    /// Category IDs and titles.
    /// </summary>
    public static ConcurrentDictionary<int, string> Categories { get; } = new ConcurrentDictionary<int, string>();

        private static readonly ReaderWriterLockSlim LastUpdateTimeLocker = new ReaderWriterLockSlim();
        private static DateTime LastUpdateTime;

    /// <summary>
    /// Configures repository and logger dependencies for MetadataManager.
    /// </summary>
    public static void Configure(IRuleRepository ruleRepository, ILogger logger, TimeSpan? refreshInterval = null)
    {
        if (ruleRepository == null)
            throw new ArgumentNullException(nameof(ruleRepository));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        lock (SyncRoot)
        {
            _ruleRepository = ruleRepository;
            _logger = logger;
            if (refreshInterval.HasValue)
                _refreshInterval = refreshInterval.Value;

            if (_backgroundTask == null || _backgroundTask.IsCompleted)
            {
                _cancelToken?.Cancel();
                _cancelToken = new CancellationTokenSource();
                _backgroundTask = Task.Run(() => WorkerAsync(_cancelToken.Token), _cancelToken.Token);
            }
        }
    }

    /// <summary>
    /// Reloads current rule metadata.
    /// </summary>
        public static async Task RefreshAsync()
        {
            if (_ruleRepository == null)
                return;

            try
            {
                var rules = (await _ruleRepository.GetAllAsync()).ToList();
                var maxUpdatedAt = rules.Count == 0 ? DateTime.MinValue : rules.Max(r => r.UpdatedAt);

                LastUpdateTimeLocker.EnterReadLock();
                var lastUpdated = LastUpdateTime;
                LastUpdateTimeLocker.ExitReadLock();

                if (maxUpdatedAt <= lastUpdated && Initialized)
                    return;

                var activeRules = rules.Where(r => r.Status == RuleStatus.Active).ToList();

                var newNamedRules = new ConcurrentDictionary<string, NamedRuleMetadata>();
                var newCategories = new ConcurrentDictionary<int, string>();
                var newRulesByCategory = new ConcurrentDictionary<string, List<string>>();

            foreach (var rule in activeRules)
            {
                var metadata = BuildNamedRuleMetadata(rule);
                if (metadata == null)
                    continue;

                var metadataValue = metadata.Value;
                newNamedRules[metadataValue.Key] = metadataValue.Value;

                if (metadataValue.ValueCategories != null)
                {
                    foreach (var category in metadataValue.ValueCategories.Where(c => c.Status == 1))
                    {
                        newCategories[category.Id] = category.Title;
                        var categoryKey = category.Id.ToString();
                        var list = newRulesByCategory.GetOrAdd(categoryKey, _ => new List<string>());
                        list.Add(metadataValue.Key);
                    }
                }
            }

            NamedRuleMetadatas.Clear();
            foreach (var pair in newNamedRules)
                NamedRuleMetadatas[pair.Key] = pair.Value;

            Categories.Clear();
            foreach (var pair in newCategories)
                Categories[pair.Key] = pair.Value;

            RulesByCategory.Clear();
            foreach (var pair in newRulesByCategory)
                RulesByCategory[pair.Key] = pair.Value;

            Interlocked.CompareExchange(ref _initialized, 1, 0);
                LastUpdateTimeLocker.EnterWriteLock();
                try
                {
                    LastUpdateTime = maxUpdatedAt == DateTime.MinValue ? DateTime.UtcNow : maxUpdatedAt;
                }
                finally
                {
                    LastUpdateTimeLocker.ExitWriteLock();
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error while refreshing design-time rule metadata.");
        }
    }

    /// <summary>
    /// Blocks until metadata is initialized.
    /// </summary>
    public static void WaitInitialization()
    {
        if (Initialized)
            return;
        var cancellationTokenSource = new CancellationTokenSource();
        var initializeTask = Task.Run(async () =>
        {
            while (!Initialized && !cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay(50, cancellationTokenSource.Token);
            }
        }, cancellationTokenSource.Token);
        if (!initializeTask.Wait(15500, cancellationTokenSource.Token))
        {
            cancellationTokenSource.Cancel();
            throw new TimeoutException($"The {nameof(MetadataManager)} haven't initialized yet.");
        }
    }

    /// <summary>
    /// Returns metadata list filtered by category.
    /// </summary>
    /// <param name="isPredicate">Filters metadata as predicate or result metadata.</param>
    /// <param name="categories">Filters metadata by category IDs.</param>
    /// <returns></returns>
    public static Dictionary<string, NamedRuleMetadata> GetMetadaByCategory(bool isPredicate = true, params int[] categories)
    {
        var result = new Dictionary<string, NamedRuleMetadata>();
        do
        {
        } while (!Initialized);

        if (categories == null || categories.Length == 0)
            result = NamedRuleMetadatas.Where(c => c.Value.IsPredicate == isPredicate)
                .ToDictionary(k => k.Key, v => v.Value);
        else
        {
            var categoryMetadatas = RulesByCategory.Where(c => categories.Any(x => x.ToString() == c.Key)).SelectMany(c => c.Value);
            result = NamedRuleMetadatas.Where(
                    c => c.Value.IsPredicate == isPredicate && categoryMetadatas.Any(x => x == c.Key))
                .ToDictionary(k => k.Key, v => v.Value);
        }
        return result;
    }

    /// <summary>
    /// Predefined rule metadata, available as static accessors.
    /// </summary>
    public static class PredefinedMetadatas
    {
        private static readonly Lazy<AndOperatorMetadata> AndOperatorMetadata = new Lazy<AndOperatorMetadata>(() => new AndOperatorMetadata());

        public static AndOperatorMetadata AndOperator => AndOperatorMetadata.Value;

        private static readonly Lazy<OrOperatorMetadata> OrOperatorMetadata = new Lazy<OrOperatorMetadata>(() => new OrOperatorMetadata());

        public static OrOperatorMetadata OrOperator => OrOperatorMetadata.Value;

        private static readonly Lazy<ComplexRuleMetadata> ComplexRuleMetadata = new Lazy<ComplexRuleMetadata>(() => new ComplexRuleMetadata());

        public static ComplexRuleMetadata ComplexRule => ComplexRuleMetadata.Value;

        private static readonly Lazy<IncorrectRuleMetadata> IncorrectRuleMetadata = new Lazy<IncorrectRuleMetadata>(() => new IncorrectRuleMetadata());

        public static IncorrectRuleMetadata IncorrectRule => IncorrectRuleMetadata.Value;

        private static readonly Lazy<RuleTreeMetadata> RuleTreeMetadata = new Lazy<RuleTreeMetadata>(() => new RuleTreeMetadata());

        public static RuleTreeMetadata RuleTree => RuleTreeMetadata.Value;
    }

    private static async Task WorkerAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await RefreshAsync();
            await Task.Delay(_refreshInterval, cancellationToken);
        }
    }

    private static (string Key, NamedRuleMetadata Value, List<RuleCategoryMetadata>? ValueCategories)? BuildNamedRuleMetadata(RuleDefinition rule)
    {
        if (rule.Content == null)
            return null;

        var metadata = ExtractDesignTimeMetadata(rule);
        var name = metadata.Name ?? rule.Name;
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var isPredicate = metadata.IsPredicate ?? true;
        var expressionFormat = metadata.ExpressionFormat ??
                               (isPredicate ? rule.Content.PredicateExpression : rule.Content.ResultExpression);

        var namedRule = new NamedRuleMetadata(metadata.Title ?? rule.Name)
        {
            Id = rule.Id,
            Description = metadata.Description ?? rule.Description,
            ExpressionFormat = expressionFormat ?? string.Empty,
            IsPredicate = isPredicate,
            DisplayFormat = metadata.DisplayFormat ?? "{0}",
            ParameterDefinations = metadata.Parameters ?? new List<ParameterDefinition>()
        };

        return (name, namedRule, metadata.Categories);
    }

    private static DesignTimeMetadata ExtractDesignTimeMetadata(RuleDefinition rule)
    {
        var contentMetadata = rule.Content.Metadata ?? new Dictionary<string, object>();

        if (MetadataValueReader.TryGet(contentMetadata, "DesignTime", out DesignTimeMetadata? designTime) && designTime != null)
            return designTime;
        if (MetadataValueReader.TryGet(contentMetadata, "designTime", out designTime) && designTime != null)
            return designTime;

        var metadata = new DesignTimeMetadata
        {
            Name = MetadataValueReader.GetString(contentMetadata, "DesignTime.Name") ?? rule.Name,
            Title = MetadataValueReader.GetString(contentMetadata, "DesignTime.Title") ?? rule.Name,
            Description = MetadataValueReader.GetString(contentMetadata, "DesignTime.Description") ?? rule.Description,
            ExpressionFormat = MetadataValueReader.GetString(contentMetadata, "DesignTime.ExpressionFormat"),
            DisplayFormat = MetadataValueReader.GetString(contentMetadata, "DesignTime.DisplayFormat"),
            IsPredicate = MetadataValueReader.GetBoolean(contentMetadata, "DesignTime.IsPredicate")
        };

        if (MetadataValueReader.TryGet(contentMetadata, "DesignTime.Parameters", out List<ParameterDefinition>? parameters))
            metadata.Parameters = parameters;

        if (MetadataValueReader.TryGet(contentMetadata, "DesignTime.Categories", out List<RuleCategoryMetadata>? categories))
            metadata.Categories = categories;
        else if (MetadataValueReader.TryGet(contentMetadata, "DesignTime.CategoryId", out int categoryId) &&
                 MetadataValueReader.TryGet(contentMetadata, "DesignTime.CategoryTitle", out string? categoryTitle))
        {
            metadata.Categories = new List<RuleCategoryMetadata>
            {
                new RuleCategoryMetadata { Id = categoryId, Title = categoryTitle ?? string.Empty, Status = 1 }
            };
        }

        return metadata;
    }
}
