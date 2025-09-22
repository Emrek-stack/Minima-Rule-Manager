using System.Collections.Generic;

namespace RuleEngine.Core.Models
{
    /// <summary>
    /// Represents the content of a rule including expressions and metadata
    /// </summary>
    public class RuleContent
    {
        /// <summary>
        /// The predicate expression that determines when the rule applies
        /// </summary>
        public string PredicateExpression { get; set; } = string.Empty;

        /// <summary>
        /// The result expression that defines what the rule returns
        /// </summary>
        public string ResultExpression { get; set; } = string.Empty;

        /// <summary>
        /// The programming language used for expressions (default: csharp)
        /// </summary>
        public string Language { get; set; } = "csharp";

        /// <summary>
        /// Additional metadata for the rule content
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
