using System;
using System.Collections.Generic;

namespace RuleEngine.Core.Models
{
    /// <summary>
    /// Represents a rule definition with metadata and content
    /// </summary>
    public class RuleDefinition
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
        /// Version number of the rule
        /// </summary>
        public int Version { get; set; }

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
        /// Description of what the rule does
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The actual rule content (predicate and result expressions)
        /// </summary>
        public RuleContent Content { get; set; } = new();

        /// <summary>
        /// Additional parameters for the rule
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
