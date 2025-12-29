using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Sqlite.Data;

namespace RuleEngine.Sqlite.Repositories;

/// <summary>
/// SQLite implementation of the rule repository
/// </summary>
public class SqliteRuleRepository : IRuleRepository
{
    private readonly RuleDbContext _context;
    private readonly ILogger<SqliteRuleRepository> _logger;

    public SqliteRuleRepository(RuleDbContext context, ILogger<SqliteRuleRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RuleDefinition?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        var ruleEntity = await _context.Rules
            .FirstOrDefaultAsync(r => r.Id == id);

        if (ruleEntity == null)
            return null;

        return await BuildRuleDefinitionAsync(ruleEntity);
    }

    /// <inheritdoc />
    public async Task<RuleDefinition?> GetActiveVersionAsync(string ruleId)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Rule ID cannot be null or empty", nameof(ruleId));

        var ruleEntity = await _context.Rules
            .FirstOrDefaultAsync(r => r.Id == ruleId);

        if (ruleEntity == null)
            return null;

        return await BuildRuleDefinitionAsync(ruleEntity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RuleDefinition>> GetAllAsync()
    {
        var ruleEntities = await _context.Rules.ToListAsync();
        var rules = new List<RuleDefinition>();

        foreach (var ruleEntity in ruleEntities)
        {
            var rule = await BuildRuleDefinitionAsync(ruleEntity);
            if (rule != null)
                rules.Add(rule);
        }

        return rules;
    }

    /// <inheritdoc />
    public async Task<RuleDefinition> CreateAsync(CreateRuleRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var ruleId = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;

        var ruleEntity = new RuleEntity
        {
            Id = ruleId,
            Name = request.Name,
            Description = request.Description,
            Status = RuleStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now,
            Tags = request.Tags
        };

        _context.Rules.Add(ruleEntity);

        // Create initial version
        var versionEntity = RuleVersionEntity.FromDomainContent(ruleId, 1, request.Content);
        versionEntity.IsActive = true;
        _context.RuleVersions.Add(versionEntity);

        // Add parameters
        foreach (var parameter in request.Parameters)
        {
            var parameterEntity = RuleParameterEntity.FromDomainParameter(ruleId, parameter);
            _context.RuleParameters.Add(parameterEntity);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Created rule {RuleId} with name {Name}", ruleId, request.Name);

        return await BuildRuleDefinitionAsync(ruleEntity) ?? throw new InvalidOperationException("Failed to build rule definition after creation");
    }

    /// <inheritdoc />
    public async Task<RuleDefinition> UpdateAsync(string id, UpdateRuleRequest request)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var ruleEntity = await _context.Rules.FindAsync(id);
        if (ruleEntity == null)
            throw new ArgumentException($"Rule with ID '{id}' not found", nameof(id));

        // Update basic properties
        if (request.Name != null)
            ruleEntity.Name = request.Name;
        if (request.Description != null)
            ruleEntity.Description = request.Description;
        if (request.Tags != null)
            ruleEntity.Tags = request.Tags;
        if (request.Status.HasValue)
            ruleEntity.Status = request.Status.Value;

        ruleEntity.UpdatedAt = DateTime.UtcNow;

        // Update content if provided
        if (request.Content != null)
        {
            // Deactivate current version
            var currentVersion = await _context.RuleVersions
                .FirstOrDefaultAsync(v => v.RuleId == id && v.IsActive);
            if (currentVersion != null)
            {
                currentVersion.IsActive = false;
            }

            // Create new version
            var newVersionNumber = await GetNextVersionNumberAsync(id);
            var newVersion = RuleVersionEntity.FromDomainContent(id, newVersionNumber, request.Content);
            newVersion.IsActive = true;
            _context.RuleVersions.Add(newVersion);
        }

        // Update parameters if provided
        if (request.Parameters != null)
        {
            // Remove existing parameters
            var existingParameters = await _context.RuleParameters
                .Where(p => p.RuleId == id)
                .ToListAsync();
            _context.RuleParameters.RemoveRange(existingParameters);

            // Add new parameters
            foreach (var parameter in request.Parameters)
            {
                var parameterEntity = RuleParameterEntity.FromDomainParameter(id, parameter);
                _context.RuleParameters.Add(parameterEntity);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated rule {RuleId}", id);

        return await BuildRuleDefinitionAsync(ruleEntity) ?? throw new InvalidOperationException("Failed to build rule definition after update");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        var ruleEntity = await _context.Rules.FindAsync(id);
        if (ruleEntity == null)
            throw new ArgumentException($"Rule with ID '{id}' not found", nameof(id));

        _context.Rules.Remove(ruleEntity);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted rule {RuleId}", id);
    }

    /// <inheritdoc />
    public async Task<RuleDefinition> CreateVersionAsync(string ruleId, CreateVersionRequest request)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Rule ID cannot be null or empty", nameof(ruleId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var ruleEntity = await _context.Rules.FindAsync(ruleId);
        if (ruleEntity == null)
            throw new ArgumentException($"Rule with ID '{ruleId}' not found", nameof(ruleId));

        // Deactivate current version if activating new one
        if (request.Activate)
        {
            var currentVersion = await _context.RuleVersions
                .FirstOrDefaultAsync(v => v.RuleId == ruleId && v.IsActive);
            if (currentVersion != null)
            {
                currentVersion.IsActive = false;
            }
        }

        // Create new version
        var newVersionNumber = await GetNextVersionNumberAsync(ruleId);
        var newVersion = RuleVersionEntity.FromDomainContent(ruleId, newVersionNumber, request.Content);
        newVersion.IsActive = request.Activate;
        _context.RuleVersions.Add(newVersion);

        // Update parameters if provided
        if (request.Parameters.Count > 0)
        {
            // Remove existing parameters
            var existingParameters = await _context.RuleParameters
                .Where(p => p.RuleId == ruleId)
                .ToListAsync();
            _context.RuleParameters.RemoveRange(existingParameters);

            // Add new parameters
            foreach (var parameter in request.Parameters)
            {
                var parameterEntity = RuleParameterEntity.FromDomainParameter(ruleId, parameter);
                _context.RuleParameters.Add(parameterEntity);
            }
        }

        ruleEntity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created version {Version} for rule {RuleId}", newVersionNumber, ruleId);

        return await BuildRuleDefinitionAsync(ruleEntity) ?? throw new InvalidOperationException("Failed to build rule definition after version creation");
    }

    /// <inheritdoc />
    public async Task<RuleDefinition> ActivateVersionAsync(string ruleId, int version)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Rule ID cannot be null or empty", nameof(ruleId));

        var ruleEntity = await _context.Rules.FindAsync(ruleId);
        if (ruleEntity == null)
            throw new ArgumentException($"Rule with ID '{ruleId}' not found", nameof(ruleId));

        // Deactivate current version
        var currentVersion = await _context.RuleVersions
            .FirstOrDefaultAsync(v => v.RuleId == ruleId && v.IsActive);
        if (currentVersion != null)
        {
            currentVersion.IsActive = false;
        }

        // Activate specified version
        var targetVersion = await _context.RuleVersions
            .FirstOrDefaultAsync(v => v.RuleId == ruleId && v.Version == version);
        if (targetVersion == null)
            throw new ArgumentException($"Version {version} not found for rule '{ruleId}'", nameof(version));

        targetVersion.IsActive = true;
        ruleEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Activated version {Version} for rule {RuleId}", version, ruleId);

        return await BuildRuleDefinitionAsync(ruleEntity) ?? throw new InvalidOperationException("Failed to build rule definition after version activation");
    }

    private async Task<RuleDefinition?> BuildRuleDefinitionAsync(RuleEntity ruleEntity)
    {
        // Get active version
        var activeVersion = await _context.RuleVersions
            .FirstOrDefaultAsync(v => v.RuleId == ruleEntity.Id && v.IsActive);

        if (activeVersion == null)
            return null;

        // Get parameters
        var parameters = await _context.RuleParameters
            .Where(p => p.RuleId == ruleEntity.Id)
            .ToDictionaryAsync(p => p.Name, p => p.ToDomainParameter().Value);

        var rule = ruleEntity.ToDomainModel();
        rule.Content = activeVersion.ToDomainContent();
        rule.Parameters = parameters;
        rule.Version = activeVersion.Version;

        return rule;
    }

    private async Task<int> GetNextVersionNumberAsync(string ruleId)
    {
        var maxVersion = await _context.RuleVersions
            .Where(v => v.RuleId == ruleId)
            .MaxAsync(v => (int?)v.Version) ?? 0;

        return maxVersion + 1;
    }
}