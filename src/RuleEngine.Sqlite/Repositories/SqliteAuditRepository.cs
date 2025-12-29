using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Sqlite.Data;

namespace RuleEngine.Sqlite.Repositories;

/// <summary>
/// SQLite implementation of the audit repository
/// </summary>
public class SqliteAuditRepository : IAuditRepository
{
    private readonly RuleDbContext _context;
    private readonly ILogger<SqliteAuditRepository> _logger;

    public SqliteAuditRepository(RuleDbContext context, ILogger<SqliteAuditRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task LogExecutionAsync(RuleExecutionAudit audit)
    {
        if (audit == null)
            throw new ArgumentNullException(nameof(audit));

        var entity = RuleExecutionAuditEntity.FromDomainModel(audit);
        _context.RuleExecutionAudits.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogDebug("Logged execution audit for rule {RuleId}", audit.RuleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log execution audit for rule {RuleId}", audit.RuleId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RuleExecutionAudit>> GetExecutionHistoryAsync(string ruleId, int limit = 100)
    {
        if (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Rule ID cannot be null or empty", nameof(ruleId));

        if (limit <= 0)
            throw new ArgumentException("Limit must be greater than 0", nameof(limit));

        var entities = await _context.RuleExecutionAudits
            .Where(a => a.RuleId == ruleId)
            .OrderByDescending(a => a.ExecutedAt)
            .Take(limit)
            .ToListAsync();

        return entities.Select(e => e.ToDomainModel());
    }
}