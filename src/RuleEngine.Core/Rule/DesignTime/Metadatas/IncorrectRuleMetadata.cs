using RuleEngine.Core.Extensions;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Represents a rule line that could not be parsed or added via the UI.
/// </summary>
public sealed class IncorrectRuleMetadata : Metadata<IncorrectRuleStatement>
{
    /// <summary>
    /// Creates incorrect rule metadata.
    /// </summary>
    public IncorrectRuleMetadata()
    {
        DisplayFormat = "!Hatalı Kural Satırı: {0}";
    }

    public override string GetDisplay(IncorrectRuleStatement statement)
    {
        return string.Format(DisplayFormat, statement.ExpressionString.Elapsis(50));
    }

    public override string Description => "Bu kural satırı tanımlanamamıştır.";

    public override string GenerateExpressionString(IncorrectRuleStatement statement, int depth, string indent)
    {
        return statement.ExpressionString;
    }
}
