namespace RuleEngine.Core.Models;

/// <summary>
/// Request model for creating a new version of a rule
/// </summary>
public class CreateVersionRequest
{
    /// <summary>
    /// The actual rule content for the new version
    /// </summary>
    public RuleContent Content { get; set; } = new();

    /// <summary>
    /// Additional parameters for the rule
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Whether to activate this version immediately
    /// </summary>
    public bool Activate { get; set; }
}