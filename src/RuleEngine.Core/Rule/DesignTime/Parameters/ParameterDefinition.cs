namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Base class for parameter definitions.
/// </summary>
public abstract class ParameterDefinition
{
    /// <summary>
    /// Creates a parameter definition.
    /// </summary>
    /// <param name="title">Parameter label.</param>
    /// <param name="displayFormat">Display format.</param>
    protected ParameterDefinition(string title, string displayFormat = "{0}")
    {
        DisplayFormat = displayFormat;
        Title = title;
    }

    protected ParameterDefinition()
        : this(string.Empty, "{0}")
    {
    }

    /// <summary>
    /// Used to identify the type during serialization.
    /// </summary>
    public string Type => GetType().Name;

    /// <summary>
    /// Parameter label.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Display format for the parameter.
    /// </summary>
    public string DisplayFormat { get; set; } = "{0}";

    /// <summary>
    /// Builds a display label using <see cref="DisplayFormat"/>.
    /// </summary>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public virtual string GetDisplay(string parameterValue)
    {
        return string.Format(DisplayFormat, parameterValue);
    }

    /// <summary>
    /// Parameter description and purpose.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Used to provide data to the presentation/UI layer.
    /// </summary>
    public Dictionary<string, string>? Data { get; set; }

    /// <summary>
    /// Generates an expression based on the parameter format.
    /// </summary>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public abstract string GenerateExpression(string parameterValue);
}
