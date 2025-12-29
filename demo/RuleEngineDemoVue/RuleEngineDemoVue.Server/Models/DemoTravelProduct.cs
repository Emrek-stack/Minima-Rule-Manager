using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;

namespace RuleEngineDemoVue.Server.Models;

public class DemoTravelProduct : ITravelProduct
{
    public string Key { get; set; } = string.Empty;
    public Price TotalPrice { get; set; }
    public IDictionary<string, CampaignInformation> CampaignInformations { get; set; } = new Dictionary<string, CampaignInformation>();
}
