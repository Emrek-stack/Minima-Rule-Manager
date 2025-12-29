namespace RuleEngine.Core.Models;

/// <summary>
/// Request model for updating an existing rule
/// </summary>
public class UpdateRuleRequest
{
    /// <summary>
    /// Human-readable name of the rule
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Description of what the rule does
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tags for categorization
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// The actual rule content
    /// </summary>
    public RuleContent? Content { get; set; }

    /// <summary>
    /// Additional parameters for the rule
    /// </summary>
    public Dictionary<string, object>? Parameters { get; set; }

    /// <summary>
    /// New status for the rule
    /// </summary>
    public RuleStatus? Status { get; set; }
}