using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;
using RuleEngine.Core.Models;

namespace CampaignEngine.Core.Tests;

public class TestCampaignInput : RuleInputModel
{
    public decimal TotalAmount { get; set; }
    public int UsageCount { get; set; }
}

public class TestCampaignOutput : CampaignOutput
{
}

public sealed class TestTravelProduct : ITravelProduct
{
    public string Key { get; set; } = string.Empty;
    public Price TotalPrice { get; set; }
    public IDictionary<string, CampaignInformation> CampaignInformations { get; set; } = new Dictionary<string, CampaignInformation>();
}
