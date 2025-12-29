using RuleEngine.Core.Models;

namespace RuleEngineDemoVue.Server.Models;

public class CampaignRuleInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int ProductCount { get; set; }
    public DateTime OrderTime { get; set; }
    public string City { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}
