using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Scripting;

namespace RuleEngine.Core.Models;

/// <summary>
/// Compiled rule.
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TReturn"></typeparam>
public class CompiledRule<TInput, TReturn>
{
    public CompiledRule()
    {
        Invoke = null!;
        Expression = null!;
        RuleString = null!;
    }

    public CompiledRule(ScriptRunner<TReturn> scriptRunner)
    {
        Expression = null!;
        RuleString = null!;
        Invoke = input =>
        {
            var globals = new GlobalRuleParams<TInput> { Model = input };
            return scriptRunner(globals).ConfigureAwait(false).GetAwaiter().GetResult();
        };
    }

    public CompiledRule(Func<TInput, TReturn> invoke, DateTime compileTime)
    {
        Expression = null!;
        RuleString = null!;
        Invoke = invoke;
        CompileTime = compileTime;
    }

    /// <summary>
    /// Compile time.
    /// </summary>
    public DateTime CompileTime { get; set; }

    /// <summary>
    /// Executes the rule.
    /// </summary>
    public Func<TInput, TReturn> Invoke { get; set; }

    /// <summary>
    /// Returns the expression when supported. Currently only predicate rules expose expressions.
    /// </summary>
    public Expression<Func<TInput, TReturn>> Expression {get; set;}

    public string RuleString { get; set; }
}

/// <summary>
/// Global rule model.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public class GlobalRuleParams<TInput>
{
    public TInput Model = default!;
}
