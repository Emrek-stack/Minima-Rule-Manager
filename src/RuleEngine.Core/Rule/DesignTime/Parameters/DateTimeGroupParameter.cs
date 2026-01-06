using System.Text.Json;

namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Parameter definition that represents DateTime values.
/// </summary>
public class DateTimeGroupParameter : ArrayParameter
{
    public DateTimeGroupParameter(string title, string displayFormat = "{0}")
        : base(title, typeof(DateTime), displayFormat)
    {
    }

    public DateTimeGroupParameter()
        : base(string.Empty, typeof(DateTime), "{0}")
    {
    }

    public override string GenerateExpression(string parameterValue)
    {
        var values = parameterValue.Split(',').ToList();
        var serializedValues = values
            .Select(v => JsonSerializer.Serialize(Convert.ChangeType(v, ArrayType)));
        return $"new HashSet<object>(new object[]{{ {string.Join(",", serializedValues)} }})";
    }
}
