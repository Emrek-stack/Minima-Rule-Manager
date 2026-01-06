namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Yes/No list option.
/// </summary>
public class BooleanListParameter : ListParameter
{
    /// <summary>
    /// Yes/No list.
    /// </summary>
    public BooleanListParameter(string title)
        : base(title)
    {
        Items = new Dictionary<string, ListParameterItem>
        {
            { "true", new ListParameterItem("Evet") },
            { "false", new ListParameterItem("Hayır") }
        };
    }

    public BooleanListParameter()
        : base(string.Empty)
    {
    }
}
