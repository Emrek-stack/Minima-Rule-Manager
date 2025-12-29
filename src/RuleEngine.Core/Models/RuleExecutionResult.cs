namespace RuleEngine.Core.Models;

/// <summary>
/// Represents the result of executing a rule
/// </summary>
public class RuleExecutionResult
{
    /// <summary>
    /// Whether the rule execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The result value returned by the rule
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// How long the execution took
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Additional metadata about the execution
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}