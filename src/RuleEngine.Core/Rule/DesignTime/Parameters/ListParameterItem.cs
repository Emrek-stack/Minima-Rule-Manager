namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Liste parametresindeki her bir elemanı ifade eder.
/// </summary>
public class ListParameterItem
{
    /// <summary>
    /// Liste parametre elemanı oluşturur.
    /// </summary>
    /// <param name="title">Etiket, girilmesi zorunludur.</param>
    /// <param name="expressionFormat">Değerin hangi formatta yazılacağı. Girilmezse değeri olduğu gibi yazar.</param>
    /// <param name="description">Bu elemananın açıklaması.</param>
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
    /// Liste elemanının başlığı.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Liste elemanının açıklaması.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Liste elemanının formatı.
    /// </summary>
    public string ExpressionFormat { get; set; }
}