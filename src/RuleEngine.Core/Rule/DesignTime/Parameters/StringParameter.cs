namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Parameter definition that represents string values.
/// </summary>
public class StringParameter : ParameterDefinition
{
    /// <summary>
    /// Creates a parameter definition for string values.
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
