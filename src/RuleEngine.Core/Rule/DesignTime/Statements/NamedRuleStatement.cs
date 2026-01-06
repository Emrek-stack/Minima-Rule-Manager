using System.Diagnostics;

namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// Rule statement that starts with /**/.
/// </summary>
[DebuggerDisplay("{" + nameof(Name) + "}")]
public class NamedRuleStatement : Statement
{
    /// <summary>
    /// Rule name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// UI label, can be dynamic for some rules.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Parameter values within the rule.
    /// </summary>
    public List<string> ParameterValues { get; set; } = new List<string>();

    /// <summary>
    /// Parameter labels within the rule.
    /// </summary>
    public Dictionary<string, List<string>>? ParameterLabels { get; set; }
}
