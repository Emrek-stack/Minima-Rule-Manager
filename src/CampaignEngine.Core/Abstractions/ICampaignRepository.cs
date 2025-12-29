using CampaignEngine.Core.Models;

namespace CampaignEngine.Core.Abstractions;

public interface ICampaignRepository
{
    IEnumerable<GeneralCampaign> GetCampaigns(System.DateTime after, int moduleId);
    IDictionary<string, bool> GetAllCampaigns(IDictionary<string, bool> keys);
    bool CheckCampaignQuota(int quota, int campaignId);
}

public interface ITravelProduct
{
    string Key { get; set; }
    Price TotalPrice { get; set; }
    IDictionary<string, CampaignInformation> CampaignInformations { get; set; }
}

public interface ICacheProvider
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, System.TimeSpan expiration);
    T? GetOrCreate<T>(string key, System.Func<T> factory, System.TimeSpan expiration);
    System.Threading.Tasks.Task<T?> GetOrCreateAsync<T>(string key, System.Func<System.Threading.Tasks.Task<T>> factory, System.TimeSpan expiration);
    string GenerateKey(params object[] parts);
    void RemoveByPrefix(string prefix);
}