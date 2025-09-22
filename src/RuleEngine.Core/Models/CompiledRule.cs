using System;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Scripting;

namespace RuleEngine.Core.Models
{
    /// <summary>
    /// Derlenmiş olan bir kural
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public class CompiledRule<TInput, TReturn>
    {
        public CompiledRule()
        {

        }

        public CompiledRule(ScriptRunner<TReturn> scriptRunner)
        {
            Invoke = input =>
            {
                var globals = new GlobalRuleParams<TInput> { Model = input };
                return scriptRunner(globals).ConfigureAwait(false).GetAwaiter().GetResult();
            };
        }

        public CompiledRule(Func<TInput, TReturn> invoke, DateTime compileTime)
        {
            Invoke = invoke;
            CompileTime = compileTime;
        }

        /// <summary>
        /// Derlenme zamanı
        /// </summary>
        public DateTime CompileTime { get; set; }

        /// <summary>
        /// Kuralı çalıştırır.
        /// </summary>
        public Func<TInput, TReturn> Invoke { get; set; }

        /// <summary>
        /// Eğer kural destekliyorsa expression'nı döner. Şu an için sadece predicate rule'lar expression olarak dönebilir.
        /// </summary>
        public Expression<Func<TInput, TReturn>> Expression {get; set;}

        public string RuleString { get; set; }
    }

    /// <summary>
    /// Global kural modeli
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public class GlobalRuleParams<TInput>
    {
        /// <summary>
        /// kural içinden erişilen model özelliği
        /// </summary>
        public TInput Model;
    }
}
