namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Integer, double, boolean veya sabit yazılı parametreler için kullanılır.
/// </summary>
public class NumericParameter : ParameterDefinition
{
    public NumericParameter(string title, string displayFormat = "{0}")
        : base(title, displayFormat)
    {
    }

    public NumericParameter()
        : base(string.Empty, "{0}")
    {
    }

    public override string GenerateExpression(string parameterValue)
    {
        var trimmed = parameterValue.Trim();
        var parts = trimmed.Split(',', '.', ' ');
        if (parts.Length > 1)
            return string.Format("{0}.{1}", string.Join("", parts.Take(parts.Length - 1)), parts.Last());
        return string.Format("{0}", trimmed);
    }
}