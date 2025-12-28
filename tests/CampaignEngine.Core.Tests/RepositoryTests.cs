using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using FluentAssertions;
using Xunit;

namespace CampaignEngine.Core.Tests.Repositories;

public class InMemoryCampaignRepositoryTests
{
    [Fact]
    public void AddCampaign_ShouldAddCampaign()
    {
        var repo = new InMemoryCampaignRepository();
        var campaign = new GeneralCampaign { Code = "TEST1", ModulId = 1, CreateDate = DateTime.Now };
        repo.AddCampaign(campaign);
        var campaigns = repo.GetCampaigns(DateTime.MinValue, 1);
        campaigns.Should().Contain(c => c.Code == "TEST1");
    }

    [Fact]
    public void GetCampaigns_ShouldFilterByDate()
    {
        var repo = new InMemoryCampaignRepository();
        repo.AddCampaign(new GeneralCampaign { Code = "OLD", ModulId = 1, CreateDate = DateTime.Now.AddDays(-10) });
        repo.AddCampaign(new GeneralCampaign { Code = "NEW", ModulId = 1, CreateDate = DateTime.Now });
        var campaigns = repo.GetCampaigns(DateTime.Now.AddDays(-5), 1);
        campaigns.Should().Contain(c => c.Code == "NEW");
        campaigns.Should().NotContain(c => c.Code == "OLD");
    }

    [Fact]
    public void GetCampaigns_ShouldFilterByModule()
    {
        var repo = new InMemoryCampaignRepository();
        repo.AddCampaign(new GeneralCampaign { Code = "M1", ModulId = 1, CreateDate = DateTime.Now });
        repo.AddCampaign(new GeneralCampaign { Code = "M2", ModulId = 2, CreateDate = DateTime.Now });
        var campaigns = repo.GetCampaigns(DateTime.MinValue, 1);
        campaigns.Should().Contain(c => c.Code == "M1");
        campaigns.Should().NotContain(c => c.Code == "M2");
    }

    [Fact]
    public void GetAllCampaigns_ShouldReturnExistence()
    {
        var repo = new InMemoryCampaignRepository();
        repo.AddCampaign(new GeneralCampaign { Code = "EXISTS", ModulId = 1, CreateDate = DateTime.Now });
        var keys = new Dictionary<string, bool> { { "EXISTS", false }, { "NOTEXISTS", false } };
        var result = repo.GetAllCampaigns(keys);
        result["EXISTS"].Should().BeTrue();
        result["NOTEXISTS"].Should().BeFalse();
    }

    [Fact]
    public void CheckCampaignQuota_ShouldReturnTrue()
    {
        var repo = new InMemoryCampaignRepository();
        var result = repo.CheckCampaignQuota(100, 1);
        result.Should().BeTrue();
    }
}
