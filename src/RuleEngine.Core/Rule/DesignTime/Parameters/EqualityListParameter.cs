namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Builds a selectable list of equality operators.
/// </summary>
public class EqualityListParameter : ListParameter
{
    public bool Symbol { get; set; }

    /// <summary>
    /// Builds a selectable list of equality operators.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="symbol">Show symbols instead of labels.</param>
    /// <param name="includeOnly">Operators to include. If empty, all are shown.</param>
    public EqualityListParameter(string title, bool symbol = true, params string[] includeOnly)
        : base(title)
    {
        Symbol = symbol;
        Items = new Dictionary<string, ListParameterItem>
        {
            { "==", new ListParameterItem(Symbol ? "=" : "Eşittir") },
            { "!=", new ListParameterItem(Symbol ? "!=" : "Eşit Değildir") },
            { "<", new ListParameterItem(Symbol ? "<" : "Küçüktür") },
            { ">", new ListParameterItem(Symbol ? ">" : "Büyüktür") },
            { "<=", new ListParameterItem(Symbol ? "<=" : "Küçük Eşittir") },
            { ">=", new ListParameterItem(Symbol ? ">=" : "Büyük Eşittir") }
        };

        if (includeOnly != null && includeOnly.Length > 0)
            Items = includeOnly.ToDictionary(o => o, o => Items.ContainsKey(o) ? Items[o] : new ListParameterItem(o));
    }

    public EqualityListParameter()
        : base(string.Empty)
    {
    }
}
