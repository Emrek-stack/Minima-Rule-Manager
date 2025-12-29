using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Cache;
using CampaignEngine.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CampaignEngine.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampaignEngine(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
        services.AddSingleton<InMemoryCampaignRepository>();
        services.AddSingleton<ICampaignRepository>(sp => sp.GetRequiredService<InMemoryCampaignRepository>());
        return services;
    }
}
