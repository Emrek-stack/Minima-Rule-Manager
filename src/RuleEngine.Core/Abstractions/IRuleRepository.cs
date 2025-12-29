using RuleEngine.Core.Models;

namespace RuleEngine.Core.Abstractions;

/// <summary>
/// Repository interface for managing rules
/// </summary>
public interface IRuleRepository
{
    /// <summary>
    /// Gets a rule by its ID
    /// </summary>
    /// <param name="id">Rule ID</param>
    /// <returns>Rule definition or null if not found</returns>
    Task<RuleDefinition?> GetByIdAsync(string id);

    /// <summary>
    /// Gets the active version of a rule
    /// </summary>
    /// <param name="ruleId">Rule ID</param>
    /// <returns>Active rule definition or null if not found</returns>
    Task<RuleDefinition?> GetActiveVersionAsync(string ruleId);

    /// <summary>
    /// Gets all rules
    /// </summary>
    /// <returns>Collection of all rules</returns>
    Task<IEnumerable<RuleDefinition>> GetAllAsync();

    /// <summary>
    /// Creates a new rule
    /// </summary>
    /// <param name="request">Rule creation request</param>
    /// <returns>Created rule definition</returns>
    Task<RuleDefinition> CreateAsync(CreateRuleRequest request);

    /// <summary>
    /// Updates an existing rule
    /// </summary>
    /// <param name="id">Rule ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated rule definition</returns>
    Task<RuleDefinition> UpdateAsync(string id, UpdateRuleRequest request);

    /// <summary>
    /// Deletes a rule
    /// </summary>
    /// <param name="id">Rule ID</param>
    Task DeleteAsync(string id);

    /// <summary>
    /// Creates a new version of a rule
    /// </summary>
    /// <param name="ruleId">Rule ID</param>
    /// <param name="request">Version creation request</param>
    /// <returns>New rule version</returns>
    Task<RuleDefinition> CreateVersionAsync(string ruleId, CreateVersionRequest request);

    /// <summary>
    /// Activates a specific version of a rule
    /// </summary>
    /// <param name="ruleId">Rule ID</param>
    /// <param name="version">Version number to activate</param>
    /// <returns>Activated rule definition</returns>
    Task<RuleDefinition> ActivateVersionAsync(string ruleId, int version);
}