using RuleEngine.Core.Rule.DesignTime.Parameters;

namespace RuleEngine.Core.Rule.DesignTime;

/// <summary>
/// Design-time metadata extracted from rule definitions.
/// </summary>
public sealed class DesignTimeMetadata
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ExpressionFormat { get; set; }
    public string? DisplayFormat { get; set; }
    public bool? IsPredicate { get; set; }
    public List<ParameterDefinition>? Parameters { get; set; }
    public List<RuleCategoryMetadata>? Categories { get; set; }
}

/// <summary>
/// Category metadata used for grouping named rules.
/// </summary>
public sealed class RuleCategoryMetadata
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
}