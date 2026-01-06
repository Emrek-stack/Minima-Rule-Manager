namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Represents a part of a rule statement tree.
/// </summary>
public abstract class Statement
{
    /// <summary>
    /// Raw expression string.
    /// </summary>
    public string ExpressionString { get; set; } = string.Empty;

    /// <summary>
    /// Statement type name.
    /// </summary>
    public string Type => GetType().Name;
}
