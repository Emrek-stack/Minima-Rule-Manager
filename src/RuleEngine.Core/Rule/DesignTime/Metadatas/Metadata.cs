using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Holds metadata about how a rule is produced and what it does.
/// </summary>
public abstract class Metadata<TStatement>
    where TStatement : Statement
{
    /// <summary>
    /// Rule title/label.
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// Display label format.
    /// </summary>
    public virtual string DisplayFormat { get; set; } = "{0}";

    /// <summary>
    /// Builds a display string based on <see cref="DisplayFormat"/>.
    /// </summary>
    /// <param name="statement"></param>
    /// <returns></returns>
    public abstract string GetDisplay(TStatement statement);

    /// <summary>
    /// Rule description.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Builds a rule string from the given <see cref="Statement">statement</see>.
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="depth">Indentation depth.</param>
    /// <param name="indent">Prefix for each generated line.</param>
    /// <returns></returns>
    public abstract string GenerateExpressionString(TStatement statement, int depth, string indent);
}
