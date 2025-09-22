using System;
using RuleEngine.Core.Models;

namespace RuleEngine.Sqlite.Data
{
    /// <summary>
    /// Entity representing a rule execution audit in the database
    /// </summary>
    public class RuleExecutionAuditEntity
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
        public string? Input { get; set; }

        /// <summary>
        /// JSON serialized output returned by the rule
        /// </summary>
        public string? Output { get; set; }

        /// <summary>
        /// Whether the execution was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if execution failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// How long the execution took (as string for SQLite)
        /// </summary>
        public string Duration { get; set; } = string.Empty;

        /// <summary>
        /// When the execution occurred
        /// </summary>
        public DateTime ExecutedAt { get; set; }

        /// <summary>
        /// Converts entity to domain model
        /// </summary>
        public RuleExecutionAudit ToDomainModel()
        {
            return new RuleExecutionAudit
            {
                Id = Id,
                RuleId = RuleId,
                RuleVersion = RuleVersion,
                Input = Input ?? string.Empty,
                Output = Output ?? string.Empty,
                Success = Success,
                ErrorMessage = ErrorMessage,
                Duration = TimeSpan.Parse(Duration),
                ExecutedAt = ExecutedAt
            };
        }

        /// <summary>
        /// Creates entity from domain model
        /// </summary>
        public static RuleExecutionAuditEntity FromDomainModel(RuleExecutionAudit audit)
        {
            return new RuleExecutionAuditEntity
            {
                Id = audit.Id,
                RuleId = audit.RuleId,
                RuleVersion = audit.RuleVersion,
                Input = audit.Input,
                Output = audit.Output,
                Success = audit.Success,
                ErrorMessage = audit.ErrorMessage,
                Duration = audit.Duration.ToString(),
                ExecutedAt = audit.ExecutedAt
            };
        }
    }
}
