using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;

namespace CampaignEngine.Core.Repositories;

public class InMemoryCampaignRepository : ICampaignRepository
{
    private readonly List<GeneralCampaign> _campaigns = new();
    private int _nextId = 1;

    public IEnumerable<GeneralCampaign> GetCampaigns(DateTime after, int moduleId)
    {
        return _campaigns.Where(c =>
            c.CreateDate > after &&
            (moduleId <= 0 || c.ModulId == moduleId));
    }

    public IDictionary<string, bool> GetAllCampaigns(IDictionary<string, bool> keys)
    {
        var result = new Dictionary<string, bool>();
        foreach (var key in keys.Keys)
            result[key] = _campaigns.Any(c => c.Code == key);
        return result;
    }

    public bool CheckCampaignQuota(int quota, int campaignId) => true;

    public void AddCampaign(GeneralCampaign campaign)
    {
        if (campaign.Id == 0)
            campaign.Id = _nextId++;
        campaign.CreateDate = campaign.CreateDate == default ? DateTime.UtcNow : campaign.CreateDate;
        _campaigns.Add(campaign);
    }

    public GeneralCampaign? GetById(int id) => _campaigns.FirstOrDefault(c => c.Id == id);

    public bool UpdateCampaign(GeneralCampaign campaign)
    {
        var index = _campaigns.FindIndex(c => c.Id == campaign.Id);
        if (index < 0)
            return false;
        _campaigns[index] = campaign;
        return true;
    }

    public bool DeleteCampaign(int id)
    {
        var campaign = _campaigns.FirstOrDefault(c => c.Id == id);
        if (campaign == null)
            return false;
        _campaigns.Remove(campaign);
        return true;
    }
}
