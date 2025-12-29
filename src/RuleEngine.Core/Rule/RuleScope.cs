using System.Collections.Concurrent;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Kural yapılarına gönderilecek olan parametereleri setlemek için kullanılır.
/// </summary>
public sealed class RuleScope : IDisposable
{
    private static readonly AsyncLocal<RuleScopeWrapper> ScopeContext = new AsyncLocal<RuleScopeWrapper>();

    private static RuleScope? CurrentScope
    {
        get
        {
            var wrapper = ScopeContext.Value;
            return wrapper?.RuleScope;
        }
        set
        {
            ScopeContext.Value = value == null ? null : new RuleScopeWrapper(value);
        }
    }

    private sealed class RuleScopeWrapper
    {
        public readonly RuleScope RuleScope;

        public RuleScopeWrapper(RuleScope scope)
        {
            RuleScope = scope;
        }
    }

    private readonly RuleScope? _parentScope;
    private readonly ConcurrentDictionary<Type, RuleInputModel> _scopeInputs = new ConcurrentDictionary<Type, RuleInputModel>();

    private RuleScope()
    {
        _parentScope = CurrentScope;
        CurrentScope = this;
    }

    /// <summary>
    /// Mevcut kural scope'unu bitirir.
    /// </summary>
    public void Dispose()
    {
        CurrentScope = _parentScope;
    }

    /// <summary>
    /// Yeni bir kural scope'u oluşturur. Dispose olana kadar geçerlidir. using ile kullanınız.
    /// </summary>
    /// <returns></returns>
    public static RuleScope Begin()
    {
        return new RuleScope();
    }

    /// <summary>
    /// Kurala gönderilecek olan parametreleri setler. Bu scope dispose olana kadar bu input parametreleri geçerlidir.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleInput"></param>
    /// <returns></returns>
    public RuleScope Set<T>(T ruleInput)
        where T : RuleInputModel
    {
        _scopeInputs[typeof(T)] = ruleInput;
        return this;
    }

    /// <summary>
    /// Mevcut scope'tan <typeparamref name="T"/> tipinde kural girişi döner.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Get<T>()
        where T : RuleInputModel
    {
        var type = typeof(T);
        var currentScope = CurrentScope;
        if (currentScope == null || !currentScope._scopeInputs.ContainsKey(type))
            return null;
        return (T)currentScope._scopeInputs[type];
    }
}