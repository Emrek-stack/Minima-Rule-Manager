namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Represents an item within a list parameter.
/// </summary>
public class ListParameterItem
{
    /// <summary>
    /// Creates a list parameter item.
    /// </summary>
    /// <param name="title">Label (required).</param>
    /// <param name="expressionFormat">Format used for the value. If empty, the raw value is used.</param>
    /// <param name="description">Item description.</param>
    public ListParameterItem(string title, string expressionFormat = "{0}", string description = "")
    {
        Title = title;
        ExpressionFormat = expressionFormat;
        Description = description;
    }

    public ListParameterItem()
    {
        Title = string.Empty;
        ExpressionFormat = "{0}";
        Description = string.Empty;
    }

    /// <summary>
    /// Item title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Item description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Item format.
    /// </summary>
    public string ExpressionFormat { get; set; }
}
