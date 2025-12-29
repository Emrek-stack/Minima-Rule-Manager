using System.Text;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// 
/// </summary>
public class OrOperatorMetadata : Metadata<OrOperatorStatement>
{
    public override string GetDisplay(OrOperatorStatement statement)
    {
        return "VEYA";
    }

    public override string Description => "Arasına geldiği kuralların ya da kural kümelerinin herhangi bir tanesinin geçerli olması yeterlidir.";

    public override string GenerateExpressionString(OrOperatorStatement statement, int depth, string indent)
    {
        var builder = new StringBuilder();
        builder.Insert(0, indent, depth);
        builder.Append("||");
        return builder.ToString();
    }
}