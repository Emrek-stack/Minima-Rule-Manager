namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// DateTime değerleri ifade eden parametre tanımlaması.
/// </summary>
public class DateTimeParameter : StringParameter
{
    public DateTimeParameter(string title)
        : base(title)
    {
    }

    public DateTimeParameter()
        : base(string.Empty)
    {
    }
}