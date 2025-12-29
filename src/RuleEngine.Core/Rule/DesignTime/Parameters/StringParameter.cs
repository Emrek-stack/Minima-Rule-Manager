namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// String değerleri ifade eden parametre tanımlaması.
/// </summary>
public class StringParameter : ParameterDefinition
{
    /// <summary>
    /// String değerleri ifade eden parametre oluşturur.
    /// </summary>
    public StringParameter(string title)
        : base(title, "\"{0}\"")
    {
    }

    public StringParameter()
        : base(string.Empty, "\"{0}\"")
    {
    }

    public override string GenerateExpression(string parameterValue)
    {
        return string.Format("\"{0}\"", parameterValue);
    }
}