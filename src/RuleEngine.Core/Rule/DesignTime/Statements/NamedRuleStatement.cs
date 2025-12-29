using System.Diagnostics;

namespace RuleEngine.Core.Rule.DesignTime.Statements;

/// <summary>
/// /**/ ile başlayan kural
/// </summary>
[DebuggerDisplay("{" + nameof(Name) + "}")]
public class NamedRuleStatement : Statement
{
    /// <summary>
    /// Kuralın ismi
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Bazı kuralların panel görünütüsü dinamik etkenlere bağlı olabilir.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Kuraldaki parametre değerleri.
    /// </summary>
    public List<string> ParameterValues { get; set; } = new List<string>();

    /// <summary>
    /// Kuraldaki parametre etiketleri.
    /// </summary>
    public Dictionary<string, List<string>>? ParameterLabels { get; set; }
}