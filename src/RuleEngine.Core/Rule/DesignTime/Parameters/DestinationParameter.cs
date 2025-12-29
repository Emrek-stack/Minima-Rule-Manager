namespace RuleEngine.Core.Rule.DesignTime.Parameters;

/// <summary>
/// 
/// </summary>
public class DestinationParameter : ArrayParameter
{
    /// <summary>
    /// 
    /// </summary>
    public string Filter { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string[] DestinationTypes { get; set; } = System.Array.Empty<string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="destinationTypes"></param>
    /// <param name="filter"></param>
    /// <param name="displayFormat"></param>
    public DestinationParameter(string title, string[] destinationTypes, string filter, string displayFormat = "{0}")
        : base(title, typeof(int), displayFormat)
    {
        DestinationTypes = destinationTypes;
        Filter = filter;
    }

    public DestinationParameter()
        : base(string.Empty, typeof(int), "{0}")
    {
    }
}