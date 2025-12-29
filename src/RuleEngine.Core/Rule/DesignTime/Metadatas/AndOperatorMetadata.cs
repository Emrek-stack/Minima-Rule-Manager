using System.Text;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// 
/// </summary>
public class AndOperatorMetadata : Metadata<AndOperatorStatement>
{
    public override string GetDisplay(AndOperatorStatement statement)
    {
        return "VE";
    }

    public override string Description => "Arasına geldiği kuralların ya da kural kümelerinin hepsinin geçerli olmasını şart koşar.";

    public override string GenerateExpressionString(AndOperatorStatement statement, int depth, string indent)
    {
        var builder = new StringBuilder();
        builder.Insert(0, indent, depth);
        builder.Append("&&");
        return builder.ToString();
    }
}