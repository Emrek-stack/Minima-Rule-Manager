using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CampaignEngine.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCampaignEngine_ShouldRegisterServices()
    {
        var services = new ServiceCollection();
        services.AddCampaignEngine();
        var provider = services.BuildServiceProvider();
        
        var cacheProvider = provider.GetService<ICacheProvider>();
        cacheProvider.Should().NotBeNull();
        
        var repository = provider.GetService<ICampaignRepository>();
        repository.Should().NotBeNull();
    }
}
