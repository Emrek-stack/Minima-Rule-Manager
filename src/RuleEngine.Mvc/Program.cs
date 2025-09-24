using RuleEngine.Sqlite.Data;
using RuleEngine.Sqlite.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add RuleEngine with SQLite
builder.Services.AddRuleEngineWithSqlite("Data Source=ruleengine.db");

var app = builder.Build();

// Seed sample data
using (var scope = app.Services.CreateScope())
{
    var ruleRepository = scope.ServiceProvider.GetRequiredService<RuleEngine.Core.Abstractions.IRuleRepository>();
    
    // Check if we already have rules
    var existingRules = await ruleRepository.GetAllAsync();
    if (!existingRules.Any())
    {
        // Create sample rules
        var sampleRules = new[]
        {
            new RuleEngine.Core.Models.CreateRuleRequest
            {
                Name = "VIP Customer Discount",
                Description = "Applies 20% discount for VIP customers",
                Tags = new[] { "discount", "vip", "pricing" },
                Content = new RuleEngine.Core.Models.RuleContent
                {
                    PredicateExpression = "Input.IsVip == true",
                    ResultExpression = "Input.Amount * 0.8"
                }
            },
            new RuleEngine.Core.Models.CreateRuleRequest
            {
                Name = "Bulk Order Discount",
                Description = "Applies 10% discount for orders with quantity >= 10",
                Tags = new[] { "discount", "bulk", "quantity" },
                Content = new RuleEngine.Core.Models.RuleContent
                {
                    PredicateExpression = "Input.Quantity >= 10",
                    ResultExpression = "Input.Amount * 0.9"
                }
            },
            new RuleEngine.Core.Models.CreateRuleRequest
            {
                Name = "High Value Order Bonus",
                Description = "Adds bonus points for orders over $500",
                Tags = new[] { "bonus", "points", "high-value" },
                Content = new RuleEngine.Core.Models.RuleContent
                {
                    PredicateExpression = "Input.Amount > 500",
                    ResultExpression = "Math.Floor(Input.Amount / 100) * 10"
                }
            }
        };

        foreach (var ruleRequest in sampleRules)
        {
            var rule = await ruleRepository.CreateAsync(ruleRequest);
            await ruleRepository.ActivateVersionAsync(rule.Id, 1);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RuleBuilder}/{action=Index}/{id?}");

app.Run();
