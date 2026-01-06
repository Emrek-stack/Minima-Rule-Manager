using System.Collections.Concurrent;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// Used to set scoped rule inputs passed to rule executions.
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
    /// Ends the current rule scope.
    /// </summary>
    public void Dispose()
    {
        CurrentScope = _parentScope;
    }

    /// <summary>
    /// Creates a new rule scope valid until disposed. Use with a using block.
    /// </summary>
    /// <returns></returns>
    public static RuleScope Begin()
    {
        return new RuleScope();
    }

    /// <summary>
    /// Sets inputs to be used within this scope until disposed.
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
    /// Gets a scoped rule input of type <typeparamref name="T"/>.
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
