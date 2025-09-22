using System.Collections.Generic;

namespace RuleEngine.Core.Models
{
    /// <summary>
    /// Request model for creating a new rule
    /// </summary>
    public class CreateRuleRequest
    {
        /// <summary>
        /// Human-readable name of the rule
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of what the rule does
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Tags for categorization
        /// </summary>
        public string[] Tags { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// The actual rule content
        /// </summary>
        public RuleContent Content { get; set; } = new();

        /// <summary>
        /// Additional parameters for the rule
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}
