using Microsoft.EntityFrameworkCore;
using CampaignEngine.Core.Extensions;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using CampaignEngine.Core.Abstractions;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Extensions;
using RuleEngine.Core.Managers;
using RuleEngine.Core.Models;
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

    var ruleSeed = new[]
    {
        new { Name = "Minimum Sipariş Tutarı", Description = "Sipariş tutarı 100 TL üzerinde olmalı", Predicate = "Input.TotalAmount > 100m" },
        new { Name = "VIP Müşteri Kontrolü", Description = "Müşteri VIP statüsünde mi?", Predicate = "Input.CustomerType == \"VIP\"" },
        new { Name = "Stok Kontrolü", Description = "Ürün stoğu yeterli mi?", Predicate = "Input.StockQuantity > 0" },
        new { Name = "Çalışma Saatleri", Description = "Sipariş çalışma saatleri içinde mi?", Predicate = "Input.OrderTime.Hour >= 9 && Input.OrderTime.Hour <= 18" },
        new { Name = "Maksimum Sipariş Limiti", Description = "Sipariş tutarı 10000 TL'yi aşmamalı", Predicate = "Input.TotalAmount <= 10000m" },
        new { Name = "Yeni Müşteri", Description = "İlk siparişi mi?", Predicate = "Input.OrderCount == 0" },
        new { Name = "Toplu Alım", Description = "3'ten fazla ürün var mı?", Predicate = "Input.ProductCount > 3" },
        new { Name = "Şehir Kontrolü", Description = "İstanbul'a teslimat mı?", Predicate = "Input.City == \"Istanbul\"" },
        new { Name = "Hafta Sonu İndirimi", Description = "Cumartesi veya Pazar mı?", Predicate = "Input.OrderTime.DayOfWeek == DayOfWeek.Saturday || Input.OrderTime.DayOfWeek == DayOfWeek.Sunday" },
        new { Name = "Kategori Kontrolü", Description = "Elektronik kategorisinde mi?", Predicate = "Input.Category == \"Electronics\"" }
    };

    foreach (var rule in ruleSeed)
    {
        var created = await ruleRepository.CreateAsync(new CreateRuleRequest
        {
            Name = rule.Name,
            Description = rule.Description,
            Tags = new[] { "demo" },
            Content = new RuleContent
            {
                PredicateExpression = rule.Predicate,
                ResultExpression = "true"
            }
        });
        await ruleRepository.ActivateVersionAsync(created.Id, 1);
    }

    var campaignRepository = scope.ServiceProvider.GetRequiredService<ICampaignRepository>();
    if (campaignRepository is InMemoryCampaignRepository memoryRepo)
    {
        memoryRepo.AddCampaign(new GeneralCampaign { Id = 1, Code = "NEWYEAR2025", Name = "Yeni Yıl Kampanyası", Description = "Tüm ürünlerde %20 indirim", StartDate = DateTime.UtcNow.AddDays(-7), EndDate = DateTime.UtcNow.AddDays(30), Priority = 1, Predicate = "Input.TotalAmount > 100m", Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.2m, \"TRY\");", Usage = "Input.UsageCount < 5", CampaignTypes = (int)CampaignTypes.DiscountCampaign, CreateDate = DateTime.UtcNow, ModulId = 1 });
        memoryRepo.AddCampaign(new GeneralCampaign { Id = 2, Code = "VIP30", Name = "VIP Müşteri İndirimi", Description = "VIP müşterilere özel %30 indirim", StartDate = DateTime.UtcNow.AddDays(-14), EndDate = DateTime.UtcNow.AddDays(60), Priority = 2, Predicate = "Input.CustomerType == \"VIP\"", Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.3m, \"TRY\");", Usage = "true", CampaignTypes = (int)CampaignTypes.DiscountCampaign, CreateDate = DateTime.UtcNow, ModulId = 1 });
        memoryRepo.AddCampaign(new GeneralCampaign { Id = 3, Code = "FREESHIP100", Name = "Kargo Bedava", Description = "100 TL ve üzeri kargo ücretsiz", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(90), Priority = 3, Predicate = "Input.TotalAmount >= 100m", Result = "Output.TotalDiscount = new Price(25m, \"TRY\");", Usage = "true", CampaignTypes = (int)CampaignTypes.ProductGiftCampaign, CreateDate = DateTime.UtcNow, ModulId = 1 });
    }
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
