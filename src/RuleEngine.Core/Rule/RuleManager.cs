using System.Collections.Concurrent;
using System.Reflection;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Manages rule sets by provider.
/// </summary>
public static class RuleManager
{
    // We key by provider type because workers are tied to the provider type, not instance.
    private static readonly ConcurrentDictionary<Type, ProviderWorker> ProviderWorkers = new ConcurrentDictionary<Type, ProviderWorker>();
        
    private static ProviderWorker<TRuleSet, TInput, TOutput> GetProviderWorker<TRuleSet, TInput, TOutput>(IRuleProvider<TRuleSet, TInput, TOutput> provider)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = ProviderWorkers.GetOrAdd(provider.GetType(),
                p =>
                {
                    var newProviderWorker = new ProviderWorker<TRuleSet, TInput, TOutput>(provider);
                    _ = newProviderWorker.ProcessAsync();
                    return newProviderWorker;
                })
            as ProviderWorker<TRuleSet, TInput, TOutput>;

        if (providerWorker == null)
            throw new ArgumentException("Unsupported RuleSet type.", nameof(provider));

        providerWorker.WaitInitialization();
        return providerWorker;
    }

    /// <summary>
    /// Evaluates predicate rules using <paramref name="input"/> and returns matching rule sets.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <param name="input">Input passed to predicate rules.</param>
    /// <returns>Dictionary of rule code to <typeparamref name="TRuleSet"/>.</returns>
    public static IDictionary<string, TRuleSet> PredicateRuleSets<TRuleSet, TInput, TOutput>(
        this IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);

        // TODO: consider parallel execution when the rule count exceeds a threshold.
        return providerWorker.RuleSets.Where(rs => rs.Value.Predicate(input))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    /// <summary>
    /// Returns all rule sets provided by the provider.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <returns>Dictionary of rule code to <typeparamref name="TRuleSet"/>.</returns>
    public static IDictionary<string, TRuleSet> GetRuleSets<TRuleSet, TInput, TOutput>(
        this IRuleProvider<TRuleSet, TInput, TOutput> provider)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);

        return providerWorker.RuleSets;
    }

    /// <summary>
    /// Executes rule sets matching the predicate for <paramref name="input"/>.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static IEnumerable<TOutput> Execute<TRuleSet, TInput, TOutput>(
        IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);
        return providerWorker.Execute(input);
    }

    /// <summary>
    /// Executes rule sets matching the predicate for <paramref name="input"/>.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<TOutput>> ExecuteAsync<TRuleSet, TInput, TOutput>(
        IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);
        return await providerWorker.ExecuteAsync(input);
    }

    /// <summary>
    /// Executes multi-result rule sets matching the predicate for <paramref name="input"/>.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <param name="input"></param>
    /// <param name="availableResults"></param>
    /// <returns></returns>
    public static IEnumerable<TOutput> Execute<TRuleSet, TInput, TOutput>(
        IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input, IEnumerable<TOutput> availableResults)
        where TRuleSet : MultiResultRuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);
        return providerWorker.Execute(input, availableResults);
    }

    /// <summary>
    /// Executes multi-result rule sets matching the predicate for <paramref name="input"/>.
    /// </summary>
    /// <typeparam name="TRuleSet"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="provider"></param>
    /// <param name="input"></param>
    /// <param name="availableResults"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<TOutput>> ExecuteAsync<TRuleSet, TInput, TOutput>(
        IRuleProvider<TRuleSet, TInput, TOutput> provider, TInput input, IEnumerable<TOutput> availableResults)
        where TRuleSet : MultiResultRuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        var providerWorker = GetProviderWorker(provider);
        return await providerWorker.ExecuteAsync(input, availableResults);
    }

    private static readonly MethodInfo _genericGetProviderWorkerMethod = typeof(RuleManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
        .First(m => m.Name == "GetProviderWorker" && m.IsGenericMethod);
    private static readonly ConcurrentDictionary<Type[], MethodInfo> _getProviderWorkerMethods = new ConcurrentDictionary<Type[], MethodInfo>();
        
    private static ProviderWorker GetProviderWorker(IRuleProvider ruleProvider)
    {
        var genericTypes = ruleProvider.GetType().GetInterfaces().First(i => i.IsGenericType).GetGenericArguments();
        var method = _getProviderWorkerMethods.GetOrAdd(genericTypes, types =>
        {
            return _genericGetProviderWorkerMethod.MakeGenericMethod(types);
        });
        return method.Invoke(null, new[] { ruleProvider }) as ProviderWorker ?? throw new InvalidOperationException("Failed to get provider worker");
    }

    public static void WaitInitialization(this IRuleProvider ruleProvider)
    {
        GetProviderWorker(ruleProvider);
    }

    public static void WaitInitialization<TRuleSet, TInput, TOutput>(this IRuleProvider<TRuleSet, TInput, TOutput> provider)
        where TRuleSet : RuleSet<TInput, TOutput>
        where TInput : RuleInputModel
    {
        GetProviderWorker(provider);
    }

    private static readonly CancellationTokenSource CancelToken = new CancellationTokenSource();

    private static readonly Thread BackgroundThread = new Thread(Worker)
    {
        IsBackground = true,
        Name = "RuleManager"
    };

    static RuleManager()
    {
        BackgroundThread.Start();
    }

    private static void Worker()
    {
        // Run on background thread to allow async processing.
        Task.Run(async () =>
            {
                while (!CancelToken.IsCancellationRequested)
                {
                    await Task.WhenAll(ProviderWorkers.Values.Select(pw => pw.ProcessAsync()));
                    await Task.Delay(TimeSpan.FromSeconds(30), CancelToken.Token);
                }
            }, CancelToken.Token)
            .Wait(CancelToken.Token);
    }
}
