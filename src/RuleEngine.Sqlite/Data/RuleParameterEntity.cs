using System;
using System.Collections.Generic;
using System.Text.Json;
using RuleEngine.Core.Models;

namespace RuleEngine.Sqlite.Data
{
    /// <summary>
    /// Entity representing a rule parameter in the database
    /// </summary>
    public class RuleParameterEntity
    {
        /// <summary>
        /// Unique identifier for the parameter
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID of the rule this parameter belongs to
        /// </summary>
        public string RuleId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of the parameter
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Value of the parameter as JSON
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Converts entity to domain model parameter
        /// </summary>
        public KeyValuePair<string, object> ToDomainParameter()
        {
            object? value = null;
            if (!string.IsNullOrEmpty(Value))
            {
                try
                {
                    value = JsonSerializer.Deserialize<object>(Value);
                }
                catch
                {
                    // If deserialization fails, use the raw string
                    value = Value;
                }
            }

            return new KeyValuePair<string, object>(Name, value!);
        }

        /// <summary>
        /// Creates entity from domain model parameter
        /// </summary>
        public static RuleParameterEntity FromDomainParameter(string ruleId, KeyValuePair<string, object> parameter)
        {
            return new RuleParameterEntity
            {
                Id = $"{ruleId}_{parameter.Key}",
                RuleId = ruleId,
                Name = parameter.Key,
                Type = parameter.Value?.GetType().Name ?? "object",
                Value = parameter.Value != null ? JsonSerializer.Serialize(parameter.Value) : null
            };
        }
    }
}
