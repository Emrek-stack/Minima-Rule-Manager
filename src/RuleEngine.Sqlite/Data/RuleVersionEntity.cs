using System;
using System.Collections.Generic;
using System.Text.Json;
using RuleEngine.Core.Models;

namespace RuleEngine.Sqlite.Data
{
    /// <summary>
    /// Entity representing a rule version in the database
    /// </summary>
    public class RuleVersionEntity
    {
        /// <summary>
        /// Unique identifier for the version
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID of the rule this version belongs to
        /// </summary>
        public string RuleId { get; set; } = string.Empty;

        /// <summary>
        /// Version number
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The predicate expression
        /// </summary>
        public string PredicateExpression { get; set; } = string.Empty;

        /// <summary>
        /// The result expression
        /// </summary>
        public string ResultExpression { get; set; } = string.Empty;

        /// <summary>
        /// The programming language used
        /// </summary>
        public string Language { get; set; } = "csharp";

        /// <summary>
        /// Additional metadata as JSON
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// When this version was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Whether this version is currently active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Converts entity to domain model content
        /// </summary>
        public RuleContent ToDomainContent()
        {
            var metadata = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(Metadata))
            {
                try
                {
                    metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(Metadata) ?? new Dictionary<string, object>();
                }
                catch
                {
                    // If deserialization fails, use empty dictionary
                }
            }

            return new RuleContent
            {
                PredicateExpression = PredicateExpression,
                ResultExpression = ResultExpression,
                Language = Language,
                Metadata = metadata
            };
        }

        /// <summary>
        /// Creates entity from domain model content
        /// </summary>
        public static RuleVersionEntity FromDomainContent(string ruleId, int version, RuleContent content)
        {
            return new RuleVersionEntity
            {
                Id = $"{ruleId}_v{version}",
                RuleId = ruleId,
                Version = version,
                PredicateExpression = content.PredicateExpression,
                ResultExpression = content.ResultExpression,
                Language = content.Language,
                Metadata = content.Metadata.Count > 0 ? JsonSerializer.Serialize(content.Metadata) : null,
                CreatedAt = DateTime.UtcNow,
                IsActive = false
            };
        }
    }
}
