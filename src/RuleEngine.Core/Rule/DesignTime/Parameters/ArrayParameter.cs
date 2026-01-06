using System.Text.Json;

namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Used for array-typed parameters.
/// </summary>
public class ArrayParameter : ParameterDefinition
{
    /// <summary>
    /// Each array must have a defined element type.
    /// </summary>
    public Type ArrayType { get; set; }

    /// <summary>
    /// Creates an array-typed parameter.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="arrayType">Each array must have a defined element type.</param>
    /// <param name="displayFormat"></param>
    public ArrayParameter(string title, Type arrayType, string displayFormat = "{0}")
        : base(title, displayFormat)
    {
        ArrayType = arrayType;
    }

    public ArrayParameter()
        : base(string.Empty, "{0}")
    {
        ArrayType = typeof(string);
    }

    /// <inheritdoc />
    public override string GenerateExpression(string parameterValue)
    {
        var values = new List<string>();

        if (ArrayType == typeof(string))
            values = !string.IsNullOrEmpty(parameterValue)
                ? parameterValue.Split(',').Select(c => c.ToString()).ToList()
                : new List<string>();
        else
            values = JsonSerializer.Deserialize<List<string>>($"[{parameterValue}]") ?? new List<string>();

        var serializedValues = values
            .Select(v => JsonSerializer.Serialize(Convert.ChangeType(v, ArrayType)));
        return $"new HashSet<object>(new object[]{{ {string.Join(",", serializedValues)} }})";
    }
}
