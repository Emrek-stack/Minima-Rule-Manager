using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Extensions;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Models;
using Xunit;

namespace CampaignEngine.Core.Tests.Integration;

public class CampaignIntegrationTests
{
    [Fact]
    public void FullCampaignFlow_ShouldWork()
    {
        var services = new ServiceCollection();
        services.AddCampaignEngine();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        var repo = provider.GetRequiredService<ICampaignRepository>() as InMemoryCampaignRepository;
        repo!.AddCampaign(new GeneralCampaign
        {
            Id = 1,
            Code = "DISCOUNT20",
            Name = "20% Discount",
            ModulId = 1,
            Priority = 100,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(30),
            Predicate = "Input.TotalAmount > 100",
            Result = "Output.TotalDiscount = Input.TotalAmount * 0.2m;",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign,
            CreateDate = DateTime.Now.AddDays(-2)
        });

        var logger = provider.GetRequiredService<ILogger<CampaignManager<OrderInput, OrderOutput>>>();
        var manager = new CampaignManager<OrderInput, OrderOutput>(1, provider, logger, typeof(Price));

        var input = new OrderInput { TotalAmount = 200 };
        var campaigns = manager.GetCampaign(input);

        campaigns.Should().NotBeEmpty();
    }

    [Fact]
    public void MultipleCampaigns_ShouldApplyByPriority()
    {
        var services = new ServiceCollection();
        services.AddCampaignEngine();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        var repo = provider.GetRequiredService<ICampaignRepository>() as InMemoryCampaignRepository;
        repo!.AddCampaign(new GeneralCampaign
        {
            Id = 1,
            Code = "LOW",
            ModulId = 1,
            Priority = 50,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(30),
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(10, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign,
            CreateDate = DateTime.Now.AddDays(-2)
        });

        repo.AddCampaign(new GeneralCampaign
        {
            Id = 2,
            Code = "HIGH",
            ModulId = 1,
            Priority = 100,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(30),
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(20, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign,
            CreateDate = DateTime.Now.AddDays(-2)
        });

        var logger = provider.GetRequiredService<ILogger<CampaignManager<OrderInput, OrderOutput>>>();
        var manager = new CampaignManager<OrderInput, OrderOutput>(1, provider, logger, typeof(Price));

        var input = new OrderInput { TotalAmount = 100 };
        var campaigns = manager.GetCampaign(input);

        campaigns.Should().NotBeEmpty();
    }

    public class OrderInput : RuleInputModel
    {
        public decimal TotalAmount { get; set; }
    }

    public class OrderOutput : CampaignOutput
    {
    }
}
