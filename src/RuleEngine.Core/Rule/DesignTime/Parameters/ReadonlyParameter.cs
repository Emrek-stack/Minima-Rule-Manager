namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Değişmeyen sabit parametreler için kullanılır.
/// </summary>
public class ReadonlyParameter : ParameterDefinition
{
    /// <summary>
    /// Readonly değer
    /// </summary>
    public string ReadonlyValue { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string GenerateExpression(string parameterValue)
    {
        return ReadonlyValue;
    }

    /// <summary>
    /// 
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