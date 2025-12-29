using System.Text;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Kompleks kuralları ifade eder.
/// </summary>
public sealed class ComplexRuleMetadata : Metadata<ComplexRuleStatement>
{
    /// <summary>
    /// Kompleks kural gösterimi oluşturur.
    /// </summary>
    public ComplexRuleMetadata()
    {
        DisplayFormat = "Kompleks ({0}): {1}";
    }

    public override string GetDisplay(ComplexRuleStatement statement)
    {
        return string.Format(DisplayFormat, statement.Name, statement.ExpressionString.Elapsis(50));
    }

    public override string Description => "Mevcut tanımlı kurallar yeterli olmadığında gelişmiş kurallar yazılabilmektedir.";

    public override string GenerateExpressionString(ComplexRuleStatement statement, int depth, string indent)
    {
        var builder = new StringBuilder();
        builder.Append(indent.Repeat(depth));
        builder.AppendLine("/**ComplexRule_Start**/");

        if (!string.IsNullOrEmpty(statement.Name))
        {
            builder.AppendFormat("{0}/**{1}**/", indent.Repeat(depth), statement.Name);
            builder.AppendLine();
        }

        builder.AppendLine((statement.ExpressionString ?? string.Empty)
            .Replace("/**ComplexRule_Start**/", string.Empty)
            .Replace("/**ComplexRule_End**/", string.Empty));

        builder.Append(indent.Repeat(depth));
        builder.AppendLine("/**ComplexRule_End**/");
        return builder.ToString();
    }
}