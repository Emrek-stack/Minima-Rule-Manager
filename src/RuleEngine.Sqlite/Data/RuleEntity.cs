using System;
using RuleEngine.Core.Models;

namespace RuleEngine.Sqlite.Data
{
    /// <summary>
    /// Entity representing a rule in the database
    /// </summary>
    public class RuleEntity
    {
        /// <summary>
        /// Unique identifier for the rule
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable name of the rule
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of what the rule does
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the rule
        /// </summary>
        public RuleStatus Status { get; set; }

        /// <summary>
        /// When the rule was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the rule was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Tags for categorization
        /// </summary>
        public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Converts entity to domain model
        /// </summary>
        public RuleDefinition ToDomainModel()
        {
            return new RuleDefinition
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Status = Status,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                Tags = Tags,
                Content = new RuleContent(), // Will be populated by version
                Parameters = new Dictionary<string, object>() // Will be populated by parameters
            };
        }

        /// <summary>
        /// Creates entity from domain model
        /// </summary>
        public static RuleEntity FromDomainModel(RuleDefinition rule)
        {
            return new RuleEntity
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Status = rule.Status,
                CreatedAt = rule.CreatedAt,
                UpdatedAt = rule.UpdatedAt,
                Tags = rule.Tags
            };
        }
    }
}
