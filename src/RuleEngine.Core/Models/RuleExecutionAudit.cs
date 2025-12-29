namespace RuleEngine.Core.Models;

/// <summary>
/// Represents an audit log entry for rule execution
/// </summary>
public class RuleExecutionAudit
{
    /// <summary>
    /// Unique identifier for the audit entry
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID of the rule that was executed
    /// </summary>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// Version of the rule that was executed
    /// </summary>
    public int RuleVersion { get; set; }

    /// <summary>
    /// JSON serialized input that was passed to the rule
    /// </summary>
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// JSON serialized output returned by the rule
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Whether the execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// How long the execution took
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// When the execution occurred
    /// </summary>
    public DateTime ExecutedAt { get; set; }
}