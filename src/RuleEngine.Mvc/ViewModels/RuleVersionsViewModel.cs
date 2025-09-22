using RuleEngine.Sqlite.Data;

namespace RuleEngine.Mvc.ViewModels
{
    public class RuleVersionsViewModel
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public IEnumerable<RuleVersionEntity> Versions { get; set; } = Enumerable.Empty<RuleVersionEntity>();
    }
}
