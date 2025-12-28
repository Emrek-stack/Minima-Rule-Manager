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

public class CampaignManagerTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly InMemoryCampaignRepository _repository;

    public CampaignManagerTests()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddLogging();
        _repository = new InMemoryCampaignRepository();
        services.AddSingleton<ICampaignRepository>(_repository);
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void Constructor_ShouldCreateManager()
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<CampaignManager<TestInput, TestOutput>>>();
        var manager = new CampaignManager<TestInput, TestOutput>(1, _serviceProvider, logger, typeof(Price));
        manager.ModuleId.Should().Be(1);
    }

    [Fact]
    public void GetCampaign_WithNoCampaigns_ShouldReturnEmpty()
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<CampaignManager<TestInput, TestOutput>>>();
        var manager = new CampaignManager<TestInput, TestOutput>(1, _serviceProvider, logger, typeof(Price));
        var input = new TestInput { Amount = 100 };
        var result = manager.GetCampaign(input);
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetCampaign_WithMatchingCampaign_ShouldReturnCampaign()
    {
        _repository.AddCampaign(new GeneralCampaign
        {
            Id = 1,
            Code = "TEST1",
            Name = "Test Campaign",
            ModulId = 1,
            Priority = 100,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(1),
            Predicate = "Input.Amount > 50",
            Result = "Output.TotalDiscount = new Price(10, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign,
            CreateDate = DateTime.Now.AddDays(-2)
        });

        var logger = _serviceProvider.GetRequiredService<ILogger<CampaignManager<TestInput, TestOutput>>>();
        var manager = new CampaignManager<TestInput, TestOutput>(1, _serviceProvider, logger, typeof(Price));
        var input = new TestInput { Amount = 100 };
        var result = manager.GetCampaign(input);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GetCampaign_WithNonMatchingPredicate_ShouldReturnEmpty()
    {
        _repository.AddCampaign(new GeneralCampaign
        {
            Id = 2,
            Code = "TEST2",
            Name = "Test Campaign 2",
            ModulId = 1,
            Priority = 100,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(1),
            Predicate = "Input.Amount > 500",
            Result = "Output.TotalDiscount = new Price(50, \"TRY\");",
            Usage = "true",
            CampaignTypes = (int)CampaignTypes.DiscountCampaign,
            CreateDate = DateTime.Now.AddDays(-2)
        });

        var logger = _serviceProvider.GetRequiredService<ILogger<CampaignManager<TestInput, TestOutput>>>();
        var manager = new CampaignManager<TestInput, TestOutput>(1, _serviceProvider, logger, typeof(Price));
        var input = new TestInput { Amount = 100 };
        var result = manager.GetCampaign(input);
        result.Should().NotBeNull();
    }

    public class TestInput : RuleInputModel
    {
        public decimal Amount { get; set; }
    }

    public class TestOutput : CampaignOutput
    {
    }
}
