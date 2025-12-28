using Microsoft.EntityFrameworkCore;
using RuleEngine.Sqlite.Data;
using CampaignEngine.Core.Extensions;
using CampaignEngine.Core.Repositories;
using RuleEngine.Core.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RuleDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RuleEngine") ?? "Data Source=ruleengine.db"));

builder.Services.AddCampaignEngine();
builder.Services.AddSingleton<InMemoryCampaignRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RuleDbContext>();
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();
    
    var rules = new[]
    {
        new RuleEntity { Id = "RULE001", Name = "Minimum Sipariş Tutarı", Description = "Sipariş tutarı 100 TL üzerinde olmalı", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "order", "validation" } },
        new RuleEntity { Id = "RULE002", Name = "VIP Müşteri Kontrolü", Description = "Müşteri VIP statüsünde mi?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "customer", "vip" } },
        new RuleEntity { Id = "RULE003", Name = "Stok Kontrolü", Description = "Ürün stoğu yeterli mi?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "product", "stock" } },
        new RuleEntity { Id = "RULE004", Name = "Çalışma Saatleri", Description = "Sipariş çalışma saatleri içinde mi?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "time", "business" } },
        new RuleEntity { Id = "RULE005", Name = "Maksimum Sipariş Limiti", Description = "Sipariş tutarı 10000 TL'yi aşmamalı", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "order", "limit" } },
        new RuleEntity { Id = "RULE006", Name = "Yeni Müşteri", Description = "İlk siparişi mi?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "customer", "new" } },
        new RuleEntity { Id = "RULE007", Name = "Toplu Alım", Description = "3'ten fazla ürün var mı?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "product", "bulk" } },
        new RuleEntity { Id = "RULE008", Name = "Şehir Kontrolü", Description = "İstanbul'a teslimat mı?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "location", "city" } },
        new RuleEntity { Id = "RULE009", Name = "Hafta Sonu İndirimi", Description = "Cumartesi veya Pazar mı?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "time", "weekend" } },
        new RuleEntity { Id = "RULE010", Name = "Kategori Kontrolü", Description = "Elektronik kategorisinde mi?", Status = RuleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, Tags = new[] { "product", "category" } }
    };
    await db.Rules.AddRangeAsync(rules);
    
    var versions = new[]
    {
        new RuleVersionEntity { Id = "VER001", RuleId = "RULE001", Version = 1, PredicateExpression = "Model.TotalAmount > 100", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER002", RuleId = "RULE002", Version = 1, PredicateExpression = "Model.CustomerType == \"VIP\"", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER003", RuleId = "RULE003", Version = 1, PredicateExpression = "Model.StockQuantity > 0", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER004", RuleId = "RULE004", Version = 1, PredicateExpression = "Model.OrderTime.Hour >= 9 && Model.OrderTime.Hour <= 18", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER005", RuleId = "RULE005", Version = 1, PredicateExpression = "Model.TotalAmount <= 10000", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER006", RuleId = "RULE006", Version = 1, PredicateExpression = "Model.OrderCount == 0", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER007", RuleId = "RULE007", Version = 1, PredicateExpression = "Model.ProductCount > 3", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER008", RuleId = "RULE008", Version = 1, PredicateExpression = "Model.City == \"Istanbul\"", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER009", RuleId = "RULE009", Version = 1, PredicateExpression = "Model.OrderTime.DayOfWeek == DayOfWeek.Saturday || Model.OrderTime.DayOfWeek == DayOfWeek.Sunday", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow },
        new RuleVersionEntity { Id = "VER010", RuleId = "RULE010", Version = 1, PredicateExpression = "Model.Category == \"Electronics\"", ResultExpression = "true", Language = "csharp", IsActive = true, CreatedAt = DateTime.UtcNow }
    };
    await db.RuleVersions.AddRangeAsync(versions);
    
    var parameters = new[]
    {
        new RuleParameterEntity { Id = "PARAM001", RuleId = "RULE001", Name = "MinAmount", Type = "decimal", Value = "100" },
        new RuleParameterEntity { Id = "PARAM002", RuleId = "RULE005", Name = "MaxAmount", Type = "decimal", Value = "10000" },
        new RuleParameterEntity { Id = "PARAM003", RuleId = "RULE004", Name = "StartHour", Type = "int", Value = "9" },
        new RuleParameterEntity { Id = "PARAM004", RuleId = "RULE004", Name = "EndHour", Type = "int", Value = "18" },
        new RuleParameterEntity { Id = "PARAM005", RuleId = "RULE007", Name = "MinProductCount", Type = "int", Value = "3" }
    };
    await db.RuleParameters.AddRangeAsync(parameters);
    
    await db.SaveChangesAsync();
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
