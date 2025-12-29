using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Bir kuralın nasıl üretildiği ve ne işe yaradığı gibi bilgileri tutar
/// </summary>
public abstract class Metadata<TStatement>
    where TStatement : Statement
{
    /// <summary>
    /// Bu kuralın başlığı ya da etiketi
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// Bir kuralın etiketi, başlığı
    /// </summary>
    public virtual string DisplayFormat { get; set; } = "{0}";

    /// <summary>
    /// <see cref="DisplayFormat"/>'a göre gösteriş şekli oluşturur.
    /// </summary>
    /// <param name="statement"></param>
    /// <returns></returns>
    public abstract string GetDisplay(TStatement statement);

    /// <summary>
    /// Bir kuralın açıklaması
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Verilen <see cref="Statement">statement</see> eşliğinde kural stringini oluşturur.
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="depth">Girinti derinliği</param>
    /// <param name="indent">Üretilen her satır kodun başına eklenir.</param>
    /// <returns></returns>
    public abstract string GenerateExpressionString(TStatement statement, int depth, string indent);
}