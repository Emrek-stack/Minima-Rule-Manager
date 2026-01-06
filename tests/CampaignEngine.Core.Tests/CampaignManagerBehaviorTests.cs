using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Cache;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Models;
using Xunit;

namespace CampaignEngine.Core.Tests;

[Collection("CampaignManagerBehaviorTests")]
public class CampaignManagerBehaviorTests
{
    private static readonly IServiceProvider ServiceProvider;
    private static readonly InMemoryCampaignRepository Repository;

    static CampaignManagerBehaviorTests()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging();
        Repository = new InMemoryCampaignRepository();
        services.AddSingleton<ICampaignRepository>(Repository);
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
        ServiceProvider = services.BuildServiceProvider();
    }

    private static CampaignManager<TestCampaignInput, TestCampaignOutput> CreateManager()
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<CampaignManager<TestCampaignInput, TestCampaignOutput>>>();
        return new CampaignManager<TestCampaignInput, TestCampaignOutput>(1, ServiceProvider, logger, typeof(Price));
    }

    private static void ResetState()
    {
        ClearRepository();
        ClearRuleManagerCache();
    }

    private static void ClearRepository()
    {
        var campaignsField = typeof(InMemoryCampaignRepository)
            .GetField("_campaigns", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (campaignsField?.GetValue(Repository) is IList<GeneralCampaign> campaigns)
            campaigns.Clear();

        var nextIdField = typeof(InMemoryCampaignRepository)
            .GetField("_nextId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        nextIdField?.SetValue(Repository, 1);
    }

    private static void ClearRuleManagerCache()
    {
        var field = typeof(RuleEngine.Core.Rule.RuleManager)
            .GetField("ProviderWorkers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var value = field?.GetValue(null);
        var clearMethod = value?.GetType().GetMethod("Clear");
        clearMethod?.Invoke(value, null);
    }

    [Fact]
    public void GetCampaign_ShouldPickHighestPriorityDiscount()
    {
        ResetState();
        Repository.AddCampaign(new GeneralCampaign
        {
            Code = "LOW",
            ModulId = 1,
            Priority = 10,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(10, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign
        });

        Repository.AddCampaign(new GeneralCampaign
        {
            Code = "HIGH",
            ModulId = 1,
            Priority = 100,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(20, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign
        });

        var manager = CreateManager();

        var input = new TestCampaignInput { TotalAmount = 100 };
        var results = manager.GetCampaign(input).ToList();

        results.Should().HaveCount(1);
        results[0].TotalDiscount.Value.Should().Be(20);
    }

    [Fact]
    public void GetCampaign_ShouldReturnDiscountAndProductGift()
    {
        ResetState();
        Repository.AddCampaign(new GeneralCampaign
        {
            Code = "DISC",
            ModulId = 1,
            Priority = 50,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(10, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign
        });

        Repository.AddCampaign(new GeneralCampaign
        {
            Code = "GIFT1",
            ModulId = 1,
            Priority = 10,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(5, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.ProductGiftCampaign
        });

        var manager = CreateManager();

        var input = new TestCampaignInput { TotalAmount = 100 };
        var results = manager.GetCampaign(input).ToList();

        results.Should().HaveCount(2);
    }

    [Fact]
    public void GetAvailableCampaigns_ShouldReturnCampaignForProductGift()
    {
        ResetState();
        Repository.AddCampaign(new GeneralCampaign
        {
            Id = 1,
            Code = "GIFT1",
            ModulId = 1,
            Priority = 10,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(5, \"TRY\");",
            Usage = "Input.UsageCount < 2",
            CampaignTypes = (int)CampaignTypes.ProductGiftCampaign
        });

        var manager = CreateManager();

        var product = new TestTravelProduct
        {
            Key = "P1",
            TotalPrice = new Price(100, "TRY"),
            CampaignInformations = new Dictionary<string, CampaignInformation>
            {
                ["GIFT1"] = new CampaignInformation
                {
                    Code = "GIFT1",
                    CampaignTypes = CampaignTypes.ProductGiftCampaign,
                    TotalDiscount = new Price(0, "TRY")
                }
            }
        };

        var input = new TestCampaignInput { TotalAmount = 100, UsageCount = 1 };
        var available = manager.GetAvailableCampaigns("P1", new Dictionary<string, ITravelProduct> { ["P1"] = product }, input);

        available.Should().HaveCount(1);
        available[0].CampaignCode.Should().Be("GIFT1");
    }

    [Fact]
    public void GetAvailableCampaigns_ShouldRespectUsageRule()
    {
        ResetState();
        Repository.AddCampaign(new GeneralCampaign
        {
            Id = 1,
            Code = "GIFT1",
            ModulId = 1,
            Priority = 10,
            Predicate = "Input.TotalAmount > 50",
            Result = "Output.TotalDiscount = new Price(5, \"TRY\");",
            Usage = "Input.UsageCount < 2",
            CampaignTypes = (int)CampaignTypes.ProductGiftCampaign
        });

        var manager = CreateManager();

        var product = new TestTravelProduct
        {
            Key = "P1",
            TotalPrice = new Price(100, "TRY"),
            CampaignInformations = new Dictionary<string, CampaignInformation>
            {
                ["GIFT1"] = new CampaignInformation
                {
                    Code = "GIFT1",
                    CampaignTypes = CampaignTypes.ProductGiftCampaign,
                    TotalDiscount = new Price(0, "TRY")
                }
            }
        };

        var input = new TestCampaignInput { TotalAmount = 100, UsageCount = 3 };
        var available = manager.GetAvailableCampaigns("P1", new Dictionary<string, ITravelProduct> { ["P1"] = product }, input);

        available.Should().BeEmpty();
    }

    [Fact]
    public void UseCampaign_ShouldApplyDiscountOnce()
    {
        ResetState();
        var manager = CreateManager();

        var product = new TestTravelProduct
        {
            Key = "P1",
            TotalPrice = new Price(100, "TRY"),
            CampaignInformations = new Dictionary<string, CampaignInformation>
            {
                ["GIFT1"] = new CampaignInformation
                {
                    Code = "GIFT1",
                    CampaignTypes = CampaignTypes.ProductGiftCampaign,
                    TotalDiscount = new Price(10, "TRY"),
                    Used = false
                }
            }
        };

        var products = new Dictionary<string, ITravelProduct> { ["P1"] = product };
        manager.UseCampaign("P1", "GIFT1", products);

        product.TotalPrice.Value.Should().Be(90);
        product.CampaignInformations["GIFT1"].Used.Should().BeTrue();

        manager.UseCampaign("P1", "GIFT1", products);
        product.TotalPrice.Value.Should().Be(90);
    }

    [Fact]
    public void DeleteCampaign_ShouldRestoreDiscount()
    {
        ResetState();
        var manager = CreateManager();

        var product = new TestTravelProduct
        {
            Key = "P1",
            TotalPrice = new Price(90, "TRY"),
            CampaignInformations = new Dictionary<string, CampaignInformation>
            {
                ["GIFT1"] = new CampaignInformation
                {
                    Code = "GIFT1",
                    CampaignTypes = CampaignTypes.ProductGiftCampaign,
                    TotalDiscount = new Price(10, "TRY"),
                    Used = true
                }
            }
        };

        var products = new Dictionary<string, ITravelProduct> { ["P1"] = product };
        manager.DeleteCampaign("GIFT1", products);

        product.TotalPrice.Value.Should().Be(100);
        product.CampaignInformations["GIFT1"].Used.Should().BeFalse();
    }

    // Test models live in TestModels.cs to avoid nested type name issues in RuleCompiler.
}
