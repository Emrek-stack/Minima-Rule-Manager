namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Definition-based parameter values.
/// </summary>
public class DefinitionParameter : ArrayParameter
{
    /// <summary>
    /// May contain DefinitionTypeEnum values.
    /// </summary>
    public string DefinitionType { get; set; } = string.Empty;

    /// <summary>
    /// Creates a definition parameter.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="type"></param>
    /// <param name="displayFormat"></param>
    /// <param name="arrayType"></param>
    public DefinitionParameter(string title, string type, string displayFormat = "{0}", string arrayType = "System.Int32")
        : base(title, System.Type.GetType(string.IsNullOrEmpty(arrayType) ? "System.Int32" : arrayType) ?? typeof(int), displayFormat)
    {
        DefinitionType = type;
    }

    public DefinitionParameter()
        : base(string.Empty, typeof(int), "{0}")
    {
    }
}
