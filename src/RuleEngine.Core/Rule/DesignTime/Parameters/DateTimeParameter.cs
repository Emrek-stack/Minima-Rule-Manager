namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// Parameter definition that represents DateTime values.
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
