using System.Text;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Kural kümesini ifade eder.
/// </summary>
public sealed class RuleTreeMetadata : Metadata<RuleTreeStatement>
{
    /// <summary>
    /// Kural kümesi
    /// </summary>
    public RuleTreeMetadata()
    {
        DisplayFormat = "{0} ({1} adet kural)";
    }

    public override string GetDisplay(RuleTreeStatement statement)
    {
        return statement.Statements.Count > 0
            ? string.Format(DisplayFormat, statement.Name ?? "Küme", statement.Statements.Count)
            : "Küme";
    }

    public override string Description => "Birden fazla kuralı barındıran kümedir.";

    public override string GenerateExpressionString(RuleTreeStatement statement, int depth, string indent)
    {
        return GenerateExpressionString(statement, depth, indent, null);
    }

    public string GenerateExpressionString(
        RuleTreeStatement statement,
        int depth,
        string indent,
        IDictionary<string, NamedRuleMetadata>? namedRules)
    {
        var builder = new StringBuilder();
        builder.Append(indent.Repeat(depth));
        if (depth > 0)
        {
            builder.AppendFormat("(/*{0}*/", statement.Name);
            builder.Append(RuleGenerator.StatementSeperator);
        }

        if (statement.Statements != null)
        {
            foreach (var child in statement.Statements)
            {
                builder.Append(RuleGenerator.Generate(child, depth + 1, indent, namedRules));
                builder.Append(RuleGenerator.StatementSeperator);
            }
        }

        builder.Append(indent.Repeat(depth));
        if (depth > 0)
        {
            builder.Append(")");
            builder.Append(RuleGenerator.StatementSeperator);
        }

        return builder.ToString();
    }
}