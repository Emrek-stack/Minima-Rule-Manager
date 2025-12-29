using System.Text;
using System.Text.Json;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Rule.DesignTime.Parameters;
using RuleEngine.Core.Rule.DesignTime.Serialization;
using RuleEngine.Core.Rule.DesignTime.Statements;

namespace RuleEngine.Core.Rule.DesignTime.Metadatas;

/// <summary>
/// Ekstra olarak tanımlı kuralları ifade eder.
/// </summary>
public sealed class NamedRuleMetadata : Metadata<NamedRuleStatement>
{
    /// <summary>
    /// Yeni bir kural oluşturur.
    /// </summary>
    /// <param name="title"></param>
    public NamedRuleMetadata(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Bu kuralın yazılış biçimi. parametreler için {N} gibi bir yapı kullanın.
    /// </summary>
    public string ExpressionFormat { get; set; } = string.Empty;

    /// <summary>
    /// 
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
    /// Bu kurala ait parametre tanımlamaları.
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