namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Liste tipindeki parametre tanımlaması. Seçilen parametre değerleri sabit olup, her birisinin kendi formatı vardır.
/// </summary>
public class ListParameter : ParameterDefinition
{
    /// <summary>
    /// Liste tipinde parametre tanımlar.
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
    /// Listede bulunan elemanlar. Dictionary'nin key'i elemanın değerini ifade eder.
    /// </summary>
    public Dictionary<string, ListParameterItem> Items { get; set; }

    public override string GenerateExpression(string parameterValue)
    {
        return !Items.ContainsKey(parameterValue)
            ? ""
            : string.Format(Items[parameterValue].ExpressionFormat, parameterValue);
    }
}