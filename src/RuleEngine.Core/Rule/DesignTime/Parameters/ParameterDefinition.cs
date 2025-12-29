namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Parametre tanımlamaları için taban sınıf
/// </summary>
public abstract class ParameterDefinition
{
    /// <summary>
    /// Parametre oluşturur.
    /// </summary>
    /// <param name="title">Parametrenin etiketi</param>
    /// <param name="displayFormat">Gösterim şekli.</param>
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
    /// Serialize işlemlerinde tipi algılamak için kullanıyoruz.
    /// </summary>
    public string Type => GetType().Name;

    /// <summary>
    /// Parametrenin etiketi.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Parametreyi ifade eden gösterim şekli.
    /// </summary>
    public string DisplayFormat { get; set; } = "{0}";

    /// <summary>
    /// <see cref="DisplayFormat"/>'a göre etiket üretir. Her parametrenin gösterim şekli farklı olabilir.
    /// </summary>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public virtual string GetDisplay(string parameterValue)
    {
        return string.Format(DisplayFormat, parameterValue);
    }

    /// <summary>
    /// Parametrenin açıklaması, ne işe yaradığı
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Presentation kısmına yani UI'a veri taşımak için kullanılır.
    /// </summary>
    public Dictionary<string, string>? Data { get; set; }

    /// <summary>
    /// Verilen değeri, parametrenin yazılış şekline göre çıktı verir.
    /// </summary>
    /// <param name="parameterValue"></param>
    /// <returns></returns>
    public abstract string GenerateExpression(string parameterValue);
}