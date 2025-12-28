namespace RuleEngine.Core.Models
{
    public class RuleSyntaxError
    {
        public int ChracterAt { get; set; }
        public int Line { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HelpLink { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
