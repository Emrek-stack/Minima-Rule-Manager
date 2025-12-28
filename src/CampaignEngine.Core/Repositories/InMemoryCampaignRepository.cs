using System;
using System.Collections.Generic;
using System.Linq;
using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;

namespace CampaignEngine.Core.Repositories
{
    public class InMemoryCampaignRepository : ICampaignRepository
    {
        private readonly List<GeneralCampaign> _campaigns = new();

        public IEnumerable<GeneralCampaign> GetCampaigns(DateTime after, int moduleId) => _campaigns.Where(c => c.CreateDate > after && c.ModulId == moduleId);

        public IDictionary<string, bool> GetAllCampaigns(IDictionary<string, bool> keys)
        {
            var result = new Dictionary<string, bool>();
            foreach (var key in keys.Keys)
                result[key] = _campaigns.Any(c => c.Code == key);
            return result;
        }

        public bool CheckCampaignQuota(int quota, int campaignId) => true;

        public void AddCampaign(GeneralCampaign campaign) => _campaigns.Add(campaign);
    }
}
