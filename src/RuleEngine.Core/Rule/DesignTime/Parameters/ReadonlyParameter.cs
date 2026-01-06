namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Used for immutable constant parameters.
/// </summary>
public class ReadonlyParameter : ParameterDefinition
{
    /// <summary>
    /// Constant value.
    /// </summary>
    public string ReadonlyValue { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string GenerateExpression(string parameterValue)
    {
        return ReadonlyValue;
    }

    /// <summary>
    /// Creates a readonly parameter.
    /// </summary>
    /// <param name="readonlyValue"></param>
    public ReadonlyParameter(string readonlyValue)
        : base("", "")
    {
        ReadonlyValue = readonlyValue;
    }

    public ReadonlyParameter()
        : base("", "")
    {
    }
}
