namespace RuleEngine.Core.Models
{
    public class RuleSyntaxError
    {
        public int ChracterAt { get; set; }
        public int Line { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string HelpLink { get; set; }
        public string Category { get; set; }
    }
}
