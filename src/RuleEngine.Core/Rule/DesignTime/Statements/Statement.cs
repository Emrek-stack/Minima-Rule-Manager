namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Kural cümleciklerindeki her bir parça
/// </summary>
public abstract class Statement
{
    /// <summary>
    /// Parse edilmemiş kural stringi
    /// </summary>
    public string ExpressionString { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string Type => GetType().Name;
}