using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule
{
    /// <summary>
    /// Seçim ve sonuç kurallarından oluşan kural kümesi. Tek bir seçime karşılık ve tek bir sonuç çıktı verir.
    /// </summary>
    /// <remarks>Eğer ruleset'i türetip özelleştirmemişseniz oluşturmak için <see cref="RuleSet"/> altındaki yardımcı metodlarını kullanabilirsiniz.</remarks>
    /// <typeparam name="TInput">Giriş modeli</typeparam>
    /// <typeparam name="TOutput">Çıkış modeli</typeparam>
    public class RuleSet<TInput, TOutput> where TInput : RuleInputModel
    {
        /// <summary>
        /// Her bir kuralın unique bir idsi bulunmalı.
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Derlenmiş seçim kuralı
        /// </summary>
        public CompiledRule<TInput, bool> PredicateRule { get; internal set; }

        /// <summary>
        /// Derlenmiş sonuç kuralı
        /// </summary>
        public CompiledRule<TInput, TOutput> ResultRule { get; set; }

        /// <summary>
        /// Öncelik
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Yeni bir ruleset tipi üretilirken ruleset'e ait diğer propları setlememek adına eklendi. Harici çağrılmaması gerekli.
        /// </summary>
        public RuleSet()
        {

        }
        /// <summary>
        /// Kural setini oluşturan base ctor.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="predicateRule"></param>
        /// <param name="resultRule"></param>
        /// <param name="priority"></param>
        protected RuleSet(string code, CompiledRule<TInput, bool> predicateRule, CompiledRule<TInput, TOutput> resultRule, int priority)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (predicateRule == null)
                throw new ArgumentNullException(nameof(predicateRule));
            if (resultRule == null)
                throw new ArgumentNullException(nameof(resultRule));

            Code = code;
            PredicateRule = predicateRule;
            ResultRule = resultRule;
            Priority = priority;
        }

        /// <summary>
        /// Seçim kuralını <paramref name="input"/> parametresine göre çalıştırır.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool Predicate(TInput input)
        {
            try
            {
                return PredicateRule.Invoke(input);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, PredicateRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
                throw;
            }
        }

        /// <summary>
        /// Sonuç kuralını <paramref name="input"/> parametresine göre çalıştırır.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual TOutput GetResult(TInput input)
        {
            try
            {
                return ResultRule.Invoke(input);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, ResultRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
                throw;
            }
        }

        internal static RuleSet<TInput, TOutput> Create(string code,
            CompiledRule<TInput, bool> predicateRule,
            CompiledRule<TInput, TOutput> resultRule,
            int priority)
        {
            return new RuleSet<TInput, TOutput>(code, predicateRule, resultRule, priority);
        }
        internal static readonly RuleCompiler<TInput, bool> DefaultPredicateCompiler = new RuleCompiler<TInput, bool>();
        internal static readonly RuleCompiler<TInput, TOutput> DefaultResultCompiler = new RuleCompiler<TInput, TOutput>(useExpressionTreeTemplate: false);
    }

    /// <summary>
    /// Çoklu sonuçlar içinden seçim yapabilen kural seti.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class MultiResultRuleSet<TInput, TOutput> : RuleSet<TInput, TOutput> where TInput : RuleInputModel
    {
        /// <summary>
        /// Çoklu sonuçlar içinden seçim yapabilen kural seti.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="predicateRule"></param>
        /// <param name="resultRule"></param>
        /// <param name="priority"></param>
        public MultiResultRuleSet(string code, CompiledRule<TInput, bool> predicateRule, CompiledRule<TInput, TOutput> resultRule, int priority)
            : base(code, predicateRule, resultRule, priority)
        {
        }

        /// <summary>
        /// Çoklu sonuçlar içinden seçim yapabilen kural seti.
        /// </summary>
        public MultiResultRuleSet()
        {
        }

        /// <summary>
        /// Sonuç kuralını <paramref name="input"/> parametresine göre çalıştırır ve çoklu sonuçlar içinden birini seçer.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual TOutput GetResult(TInput input, IEnumerable<TOutput> availableResults)
        {
            try
            {
                var result = ResultRule.Invoke(input);
                return availableResults.FirstOrDefault(r => r.Equals(result)) ?? result;
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(new RuleRuntimeException(e, ResultRule.RuleString, System.Text.Json.JsonSerializer.Serialize(input), Code) { Priority = Priority }).Throw();
                throw;
            }
        }
    }

    /// <summary>
    /// <c>RuleSet</c> oluşturmak için yardımcı metodlar
    /// </summary>
    public static class RuleSet
    {
        /// <summary>
        /// Önceden derlenmiş kurallar ile tek sonuç üreten rule set oluşturur.
        /// </summary>
        /// <typeparam name="TInput">Seçim ve sonuç için giriş modeli</typeparam>
        /// <typeparam name="TOutput">Sonuç kuralının çıkış modeli</typeparam>
        /// <param name="code">Her bir kuralın unique bir idsi bulunmalı.</param>
        /// <param name="predicateRule">Derlenmiş seçim kuralı</param>
        /// <param name="resultRule">Derlenmiş sonuç kuralı</param>
        /// <param name="priority">Öncelik</param>
        /// <returns></returns>
        public static RuleSet<TInput, TOutput> Create<TInput, TOutput>(
            string code,
            CompiledRule<TInput, bool> predicateRule,
            CompiledRule<TInput, TOutput> resultRule,
            int priority)
            where TInput : RuleInputModel
        {
            return RuleSet<TInput, TOutput>.Create(code, predicateRule, resultRule, priority);
        }

        public static TRuleSet Create<TRuleSet, TInput, TOutput>(
           string code,
           CompiledRule<TInput, bool> compiledPredicateRule,
           CompiledRule<TInput, TOutput> compiledResultRule,
           int priority)
           where TInput : RuleInputModel
           where TRuleSet : RuleSet<TInput, TOutput>, new()
        {
            return new TRuleSet
            {
                Code = code,
                PredicateRule = compiledPredicateRule,
                ResultRule = compiledResultRule,
                Priority = priority
            };
        }

        /// <summary>
        /// Önceden derlenmiş kurallar ile çoklu sonuçlar içinden bir sonucu seçen kural seti oluşturur.
        /// </summary>
        /// <typeparam name="TInput">Seçim ve sonuç için giriş modeli</typeparam>
        /// <typeparam name="TOutput">Sonuç kuralının çıkış modeli</typeparam>
        /// <param name="code">Her bir kuralın unique bir idsi bulunmalı.</param>
        /// <param name="predicateRule">Derlenmiş seçim kuralı</param>
        /// <param name="resultRule">Derlenmiş sonuç kuralı</param>
        /// <param name="priority">Öncelik</param>
        /// <returns></returns>
        public static MultiResultRuleSet<TInput, TOutput> CreateMultiResult<TInput, TOutput>(
            string code,
            CompiledRule<TInput, bool> predicateRule,
            CompiledRule<TInput, TOutput> resultRule,
            int priority)
            where TInput : RuleInputModel
        {
            return new MultiResultRuleSet<TInput, TOutput>(code, predicateRule, resultRule, priority);
        }

        /// <summary>
        /// Derlenmemiş kurallar ile tek sonuç üreten rule set oluşturur.
        /// </summary>
        /// <typeparam name="TInput">Seçim ve sonuç için giriş modeli</typeparam>
        /// <typeparam name="TOutput">Sonuç kuralının çıkış modeli</typeparam>
        /// <param name="code">Her bir kuralın unique bir idsi bulunmalı.</param>
        /// <param name="predicateRuleString">Derlenmemiş seçim kuralı</param>
        /// <param name="resultRuleString">Derlenmemiş sonuç kuralı</param>
        /// <param name="priority">Öncelik</param>
        /// <returns></returns>
        public static async Task<RuleSet<TInput, TOutput>> CreateAsync<TInput, TOutput>(
            string code,
            string predicateRuleString,
            string resultRuleString,
            int priority)
            where TInput : RuleInputModel
        {
            var predicateRule = await RuleSet<TInput, TOutput>.DefaultPredicateCompiler.CompileAsync(code, predicateRuleString);
            var resultRule = await RuleSet<TInput, TOutput>.DefaultResultCompiler.CompileAsync(code, resultRuleString);
            return Create(code, predicateRule, resultRule, priority);
        }

        /// <summary>
        /// Derlenmemiş kurallar ile çoklu sonuçlar içinden bir sonucu seçen kural seti oluşturur.
        /// </summary>
        /// <typeparam name="TInput">Seçim ve sonuç için giriş modeli</typeparam>
        /// <typeparam name="TOutput">Sonuç kuralının çıkış modeli</typeparam>
        /// <param name="code">Her bir kuralın unique bir idsi bulunmalı.</param>
        /// <param name="predicateRuleString">Derlenmemiş seçim kuralı</param>
        /// <param name="resultRuleString">Derlenmemiş sonuç kuralı</param>
        /// <param name="priority">Öncelik</param>
        /// <returns></returns>
        public static async Task<MultiResultRuleSet<TInput, TOutput>> CreateMultiResultAsync<TInput, TOutput>(
            string code,
            string predicateRuleString,
            string resultRuleString,
            int priority)
            where TInput : RuleInputModel
        {
            var predicateRule = await RuleSet<TInput, TOutput>.DefaultPredicateCompiler.CompileAsync(code, predicateRuleString);
            var resultRule = await RuleSet<TInput, TOutput>.DefaultResultCompiler.CompileAsync(code, resultRuleString);
            return CreateMultiResult(code, predicateRule, resultRule, priority);
        }
    }
}
