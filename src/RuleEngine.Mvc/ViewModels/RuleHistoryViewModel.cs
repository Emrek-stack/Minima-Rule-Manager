using RuleEngine.Core.Models;

namespace RuleEngine.Mvc.ViewModels
{
    public class RuleHistoryViewModel
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public IEnumerable<RuleExecutionAudit> Executions { get; set; } = Enumerable.Empty<RuleExecutionAudit>();
    }
}
