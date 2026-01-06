namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// List-typed parameter definition. Selected values are fixed and each has its own format.
/// </summary>
public class ListParameter : ParameterDefinition
{
    /// <summary>
    /// Defines a list-typed parameter.
    /// </summary>
    public ListParameter(string title, string displayFormat = "{0}")
        : base(title, displayFormat)
    {
        Items = new Dictionary<string, ListParameterItem>();
    }

    public ListParameter()
        : base(string.Empty, "{0}")
    {
        Items = new Dictionary<string, ListParameterItem>();
    }

    public override string GetDisplay(string parameterValue)
    {
        if (Items.ContainsKey(parameterValue) &&
            !string.IsNullOrEmpty(Items[parameterValue].Title))
            return base.GetDisplay(Items[parameterValue].Title);
        return base.GetDisplay(parameterValue);
    }

    /// <summary>
    /// List items. The dictionary key represents the item's value.
    /// </summary>
    public Dictionary<string, ListParameterItem> Items { get; set; }

    public override string GenerateExpression(string parameterValue)
    {
        return !Items.ContainsKey(parameterValue)
            ? ""
            : string.Format(Items[parameterValue].ExpressionFormat, parameterValue);
    }
}
