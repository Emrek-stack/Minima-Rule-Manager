using System.Collections.Generic;
using System.Threading.Tasks;
using RuleEngine.Core.Models;

namespace RuleEngine.Core.Abstractions
{
    /// <summary>
    /// Repository interface for managing rule execution audits
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Logs a rule execution
        /// </summary>
        /// <param name="audit">Audit information</param>
        Task LogExecutionAsync(RuleExecutionAudit audit);

        /// <summary>
        /// Gets execution history for a rule
        /// </summary>
        /// <param name="ruleId">Rule ID</param>
        /// <param name="limit">Maximum number of records to return</param>
        /// <returns>Collection of execution audits</returns>
        Task<IEnumerable<RuleExecutionAudit>> GetExecutionHistoryAsync(string ruleId, int limit = 100);
    }
}
