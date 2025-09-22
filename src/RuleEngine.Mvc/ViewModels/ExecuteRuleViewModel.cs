using System.ComponentModel.DataAnnotations;
using RuleEngine.Core.Models;

namespace RuleEngine.Mvc.ViewModels
{
    public class ExecuteRuleViewModel
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Sample Input (JSON)")]
        public string SampleInput { get; set; } = string.Empty;

        public RuleExecutionResult? ExecutionResult { get; set; }
        public bool Success { get; set; }
    }
}
