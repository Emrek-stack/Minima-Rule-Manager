using System.Collections.Concurrent;
using RuleEngine.Core.Models;
using RuleEngine.Core.Extensions;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Base class for provider caching and background processing.
/// </summary>
public abstract class ProviderWorker
{
    protected int _initialized = 0;

    public abstract Task ProcessAsync();

    public void WaitInitialization()
    {
        while (_initialized == 0)
        {
            Thread.Sleep(10);
        }
    }

    private static readonly SemaphoreSlim _waitSemaphore = new SemaphoreSlim(1);

    public void WaitInitializationWithSemaphore()
    {
        _waitSemaphore.Wait();
        try
        {
            while (_initialized == 0)
            {
                Thread.Sleep(10);
            }
        }
        finally
        {
            _waitSemaphore.Release();
        }
    }
}

/// <summary>
/// Generic class for provider caching and background processing.
/// </summary>
/// <typeparam name="TRuleSet"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public class ProviderWorker<TRuleSet, TInput, TOutput> : ProviderWorker
    where TRuleSet : RuleSet<TInput, TOutput>
    where TInput : RuleInputModel
{
    private new int _initialized;

    public bool Initialized => _initialized > 0;

    private readonly IRuleProvider<TRuleSet, TInput, TOutput> _provider;
    public readonly ConcurrentDictionary<string, TRuleSet> RuleSets = new ConcurrentDictionary<string, TRuleSet>();

    public ProviderWorker(IRuleProvider<TRuleSet, TInput, TOutput> provider)
    {
        _provider = provider;
        _providerTypeName = provider.GetType().GetFriendlyName();
    }

    private readonly string _providerTypeName;

    public override async Task ProcessAsync()
    {
        try
        {
            var lastCompileTime = RuleSets.Count > 0 ?
                RuleSets.Values.Max(
                    rs => rs.PredicateRule == null ? rs.ResultRule.CompileTime :
                        (
                            rs.ResultRule == null ? rs.PredicateRule.CompileTime :
                                (
                                    rs.PredicateRule.CompileTime > rs.ResultRule.CompileTime
                                        ? rs.PredicateRule.CompileTime
                                        : rs.ResultRule.CompileTime
                                )
                        )
                ) : default(DateTime);

            // Remove deleted rule sets.
            var isExistDic = await _provider.IsExistsAsync(RuleSets.Keys.ToArray());
            foreach (var deletedKey in isExistDic.Where(kv => !kv.Value).Select(kv => kv.Key))
            {
                RuleSets.TryRemove(deletedKey, out _);
            }

            // Fetch updated rule sets.
            var newRuleSets = await _provider.GenerateRuleSetsAsync(lastCompileTime);
            foreach (var newRuleSet in newRuleSets)
            {
                RuleSets[newRuleSet.Key] = newRuleSet.Value;
            }
        }
        catch (Exception e)
        {
            // Log exception
            Console.WriteLine($"Error in ProviderWorker.ProcessAsync: {e.Message}");
        }
        finally
        {
            Interlocked.CompareExchange(ref _initialized, 1, 0);
        }
    }

    private static readonly SemaphoreSlim _waitSemaphore = new SemaphoreSlim(1);

    public new void WaitInitialization()
    {
        if (Initialized)
            return;

        _waitSemaphore.Wait();
        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var initializeTask = Task.Run(async () =>
            {
                while (!Initialized && !cancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(50, cancellationTokenSource.Token);
                }
            }, cancellationTokenSource.Token);
            if (!initializeTask.Wait(25500, cancellationTokenSource.Token))
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException($"The rule sets of the {_provider.GetType().GetFriendlyName()} haven't initialized yet.");
            }
        }
        finally
        {
            _waitSemaphore.Release();
        }
    }

    public IEnumerable<TOutput> Execute(TInput input)
    {
        return RuleSets.Values
            .Where(rs => rs.Predicate(input))
            .OrderBy(rs => rs.Priority)
            .Select(rs => rs.GetResult(input));
    }

    public async Task<IEnumerable<TOutput>> ExecuteAsync(TInput input)
    {
        return await Task.Run(() => Execute(input));
    }

    public IEnumerable<TOutput> Execute(TInput input, IEnumerable<TOutput> availableResults)
    {
        return RuleSets.Values
            .Where(rs => rs.Predicate(input))
            .OrderBy(rs => rs.Priority)
            .Select(rs => rs is MultiResultRuleSet<TInput, TOutput> multiResult ? 
                multiResult.GetResult(input, availableResults) : 
                rs.GetResult(input));
    }

    public async Task<IEnumerable<TOutput>> ExecuteAsync(TInput input, IEnumerable<TOutput> availableResults)
    {
        return await Task.Run(() => Execute(input, availableResults));
    }
}
