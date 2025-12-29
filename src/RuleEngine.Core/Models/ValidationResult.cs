namespace RuleEngine.Core.Models;

/// <summary>
/// Represents the result of validating a rule
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the validation was successful
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result with errors
    /// </summary>
    public static ValidationResult Failure(params string[] errors) => new()
    {
        IsValid = false,
        Errors = new List<string>(errors)
    };
}