using RuleEngine.Core.Models;

namespace RuleEngine.Core.Rule;

/// <summary>
/// <see cref="RuleManager"/>'ın ihtiyaç duyduğu metodları sağlar. <see cref="RuleManager"/> tarafından singleton gibi çağrılır, yani aynı tipteki birden fazla provider instance'ı <see cref="RuleManager"/> için tektir.
/// </summary>
/// <typeparam name="TRuleSet"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IRuleProvider<TRuleSet, TInput, TOutput> : IRuleProvider
    where TRuleSet: RuleSet<TInput, TOutput>
    where TInput : RuleInputModel
{
    /// <summary>
    /// <paramref name="after"/> tarihinden sonraki kuralları derler ve <typeparamref name="TRuleSet"/> oluşturarak çıktı verir.
    /// </summary>
    /// <param name="after"></param>
    /// <returns></returns>
    Task<IDictionary<string, TRuleSet>> GenerateRuleSetsAsync(DateTime after);

    /// <summary>
    /// RuleSetlerin mevcut ve aktif olup olmadığını döner.
    /// </summary>
    /// <param name="keys">Kontrol edilecek olan keyler</param>
    /// <returns></returns>
    Task<IDictionary<string, bool>> IsExistsAsync(params string[] keys);
}

/// <summary>
/// Cache işlemleri için kullanılan base sınıftır. Bunun yerine <see cref="IRuleProvider{TRuleSet, TInput, TOutput}"/> arayüzünü kullanın.
/// </summary>
public interface IRuleProvider { }