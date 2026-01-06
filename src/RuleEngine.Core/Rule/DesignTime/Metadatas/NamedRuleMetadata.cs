using System.Text;
using System.Text.Json;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Rule.DesignTime.Parameters;
using RuleEngine.Core.Rule.DesignTime.Serialization;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Represents named rules defined externally.
/// </summary>
public sealed class NamedRuleMetadata : Metadata<NamedRuleStatement>
{
    /// <summary>
    /// Creates named rule metadata.
    /// </summary>
    /// <param name="title"></param>
    public NamedRuleMetadata(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Format for this rule. Use placeholders like {N} for parameters.
    /// </summary>
    public string ExpressionFormat { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this rule is a predicate.
    /// </summary>
    public bool IsPredicate { get; set; }

    internal string Id { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string GetDisplay(NamedRuleStatement statement)
    {
        if (!string.IsNullOrEmpty(statement.Label))
            return statement.Label;
        return string.Format(DisplayFormat, statement.ParameterValues
            .Select((parameterValue, index) => ParameterDefinations.Count <= index ? "" : ParameterDefinations[index].GetDisplay(parameterValue))
            .Cast<object>()
            .ToArray());
    }

    /// <summary>
    /// Parameter definitions for this rule.
    /// </summary>
    public List<ParameterDefinition> ParameterDefinations { get; set; } = new List<ParameterDefinition>();

    /// <inheritdoc />
    public override string GenerateExpressionString(NamedRuleStatement statement, int depth, string indent)
    {
        var builder = new StringBuilder();
        builder.Append(indent.Repeat(depth));
        builder.Append("/*");
        builder.Append(statement.Name);
        builder.Append("|");
        if (!string.IsNullOrEmpty(statement.Label))
        {
            builder.Append(statement.Label.Replace("|", ""));
        }
        builder.Append("|");
        builder.Append(JsonSerializer.Serialize(statement.ParameterLabels ?? new Dictionary<string, List<string>>(), DesignTimeJson.Options));
        builder.Append("*/");

        builder.AppendFormat(ExpressionFormat, statement.ParameterValues
            .Select((parameterValue, index) => ParameterDefinations.Count <= index ? "" : ParameterDefinations[index].GenerateExpression(parameterValue))
            .Cast<object>()
            .ToArray());
        return builder.ToString();
    }
}
