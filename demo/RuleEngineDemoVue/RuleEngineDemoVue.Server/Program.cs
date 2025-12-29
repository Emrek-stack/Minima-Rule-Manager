using Microsoft.EntityFrameworkCore;
using CampaignEngine.Core.Extensions;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Demo;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Managers;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule.DesignTime;
using RuleEngine.Core.Rule.DesignTime.Parameters;
using RuleEngine.Sqlite.Data;
using RuleEngine.Sqlite.Extensions;
using RuleEngineDemoVue.Server.Models;
using RuleEngineDemoVue.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddRuleEngineWithSqlite(builder.Configuration.GetConnectionString("RuleEngine") ?? "Data Source=ruleengine.db");
builder.Services.AddRuleEngineDesignTime();
builder.Services.AddSingleton<IRuleEvaluator, DemoRuleEvaluator>();
builder.Services.AddScoped<IRuleEngine, DemoRuleEngine>();
builder.Services.AddScoped<IRuleManager, RuleManager>();

builder.Services.AddCampaignEngine();
builder.Services.AddSingleton(sp =>
    new CampaignEngine.Core.CampaignManager<CampaignRuleInput, CampaignOutput>(
        moduleId: 1,
        serviceProvider: sp,
        logger: sp.GetRequiredService<ILogger<CampaignEngine.Core.CampaignManager<CampaignRuleInput, CampaignOutput>>>(),
        typeof(Price)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RuleDbContext>();
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();

    var ruleRepository = scope.ServiceProvider.GetRequiredService<IRuleRepository>();

    var ruleSeed = BuildDemoRuleSeed();
    foreach (var rule in ruleSeed)
    {
        var created = await ruleRepository.CreateAsync(rule);
        await ruleRepository.ActivateVersionAsync(created.Id, 1);
        await ruleRepository.UpdateAsync(created.Id, new UpdateRuleRequest { Status = RuleStatus.Active });
    }

    var campaignRepository = scope.ServiceProvider.GetRequiredService<ICampaignRepository>();
    if (campaignRepository is InMemoryCampaignRepository memoryRepo)
    {
        foreach (var campaign in CampaignSeed.BuildDemoCampaigns(moduleId: 1))
        {
            memoryRepo.AddCampaign(campaign);
        }
    }
}

static List<CreateRuleRequest> BuildDemoRuleSeed()
{
    var orderCategory = new RuleCategoryMetadata { Id = 1, Title = "Order" };
    var customerCategory = new RuleCategoryMetadata { Id = 2, Title = "Customer" };
    var timeCategory = new RuleCategoryMetadata { Id = 3, Title = "Time" };
    var inventoryCategory = new RuleCategoryMetadata { Id = 4, Title = "Inventory" };
    var geoCategory = new RuleCategoryMetadata { Id = 5, Title = "Geo" };
    var outputCategory = new RuleCategoryMetadata { Id = 6, Title = "Output" };

    var customerTypeList = new ListParameter("Customer Type");
    customerTypeList.Items = new Dictionary<string, ListParameterItem>
    {
        { "VIP", new ListParameterItem("VIP", "\"{0}\"") },
        { "STANDARD", new ListParameterItem("Standard", "\"{0}\"") },
        { "CORP", new ListParameterItem("Corporate", "\"{0}\"") }
    };

    var cityList = new ArrayParameter("Cities", typeof(string));

    return new List<CreateRuleRequest>
    {
        new CreateRuleRequest
        {
            Name = "Minimum Order Total",
            Description = "Order total must exceed a minimum threshold.",
            Tags = new[] { "demo", "order" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.TotalAmount > 100m",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Order.MinTotal",
                    "Minimum Order Total",
                    "Checks if order total is above a threshold.",
                    "Input.TotalAmount > {0}",
                    "Order total > {0} TL",
                    true,
                    new List<ParameterDefinition> { new NumericParameter("Minimum Total") },
                    orderCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "Parameterized Threshold",
            Description = "Stores parameter values to drive rule configuration.",
            Tags = new[] { "demo", "parameters" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.TotalAmount >= 200m",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Order.ParameterizedThreshold",
                    "Parameterized Threshold",
                    "Uses stored parameters for UI/configuration.",
                    "Input.TotalAmount >= {0}",
                    "Order total >= {0} TL",
                    true,
                    new List<ParameterDefinition>
                    {
                        new NumericParameter("Threshold"),
                        new ArrayParameter("Allowed Cities", typeof(string))
                    },
                    orderCategory)
            },
            Parameters = new Dictionary<string, object>
            {
                ["Threshold"] = 200m,
                ["AllowedCities"] = new[] { "Istanbul", "Ankara" },
                ["Segment"] = "VIP"
            }
        },
        new CreateRuleRequest
        {
            Name = "Customer Type Gate",
            Description = "Allow specific customer segments.",
            Tags = new[] { "demo", "customer" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.CustomerType == \"VIP\"",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Customer.Type",
                    "Customer Type",
                    "Matches a customer type.",
                    "Input.CustomerType == {0}",
                    "Customer type is {0}",
                    true,
                    new List<ParameterDefinition> { customerTypeList },
                    customerCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "Stock Level Check",
            Description = "Ensure stock is above a minimum.",
            Tags = new[] { "demo", "inventory" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.StockQuantity >= 5",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Inventory.MinStock",
                    "Minimum Stock",
                    "Checks if stock quantity is sufficient.",
                    "Input.StockQuantity >= {0}",
                    "Stock >= {0}",
                    true,
                    new List<ParameterDefinition> { new NumericParameter("Minimum Stock") },
                    inventoryCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "City Targeting",
            Description = "Allow only selected cities.",
            Tags = new[] { "demo", "geo" },
            Content = new RuleContent
            {
                PredicateExpression = "new HashSet<object>(new object[] { \"Istanbul\", \"Ankara\" }).Contains(Input.City)",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Geo.CityIn",
                    "City In List",
                    "Checks if the city is inside the selected list.",
                    "new HashSet<object>(new object[]{{ {0} }}).Contains(Input.City)",
                    "City in {0}",
                    true,
                    new List<ParameterDefinition> { cityList },
                    geoCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "Weekend Orders",
            Description = "Orders only on weekends.",
            Tags = new[] { "demo", "time" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.OrderTime.DayOfWeek == DayOfWeek.Saturday || Input.OrderTime.DayOfWeek == DayOfWeek.Sunday",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Time.Weekend",
                    "Weekend Order",
                    "Checks if order time is on weekend.",
                    "Input.OrderTime.DayOfWeek == DayOfWeek.Saturday || Input.OrderTime.DayOfWeek == DayOfWeek.Sunday",
                    "Weekend order",
                    true,
                    new List<ParameterDefinition>(),
                    timeCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "Bulk Purchase",
            Description = "Product count higher than threshold.",
            Tags = new[] { "demo", "order" },
            Content = new RuleContent
            {
                PredicateExpression = "Input.ProductCount >= 3",
                ResultExpression = "true",
                Metadata = BuildDesignTimeMetadata(
                    "Order.ProductCount",
                    "Minimum Product Count",
                    "Checks if product count is above a threshold.",
                    "Input.ProductCount >= {0}",
                    "Products >= {0}",
                    true,
                    new List<ParameterDefinition> { new NumericParameter("Minimum Products") },
                    orderCategory)
            }
        },
        new CreateRuleRequest
        {
            Name = "Dynamic Discount Output",
            Description = "Returns a dynamic discount amount.",
            Tags = new[] { "demo", "output" },
            Content = new RuleContent
            {
                PredicateExpression = "true",
                ResultExpression = "new { Discount = Input.TotalAmount * 0.15m, Reason = \"HighValue\" }",
                Metadata = BuildDesignTimeMetadata(
                    "Output.Discount",
                    "Discount Output",
                    "Creates an output payload for discount.",
                    "new { Discount = {0}, Reason = \"{1}\" }",
                    "Discount output",
                    false,
                    new List<ParameterDefinition>
                    {
                        new NumericParameter("Discount Amount"),
                        new StringParameter("Reason")
                    },
                    outputCategory)
            }
        }
    };
}

static Dictionary<string, object> BuildDesignTimeMetadata(
    string name,
    string title,
    string description,
    string expressionFormat,
    string displayFormat,
    bool isPredicate,
    List<ParameterDefinition> parameters,
    params RuleCategoryMetadata[] categories)
{
    return new Dictionary<string, object>
    {
        ["DesignTime"] = new DesignTimeMetadata
        {
            Name = name,
            Title = title,
            Description = description,
            ExpressionFormat = expressionFormat,
            DisplayFormat = displayFormat,
            IsPredicate = isPredicate,
            Parameters = parameters,
            Categories = categories.ToList()
        }
    };
}

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
