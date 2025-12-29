namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Evet, Hayır lı bir liste seçeneği
/// </summary>
public class BooleanListParameter : ListParameter
{
    /// <summary>
    /// Evet, Hayır listesi
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