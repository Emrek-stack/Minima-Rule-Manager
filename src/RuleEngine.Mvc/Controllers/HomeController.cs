using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleEngine.Core.Abstractions;
using RuleEngine.Mvc.Models;
using RuleEngine.Sqlite.Data;

namespace RuleEngine.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRuleRepository _ruleRepository;
    private readonly RuleDbContext _context;

    public HomeController(ILogger<HomeController> logger, IRuleRepository ruleRepository, RuleDbContext context)
    {
        _logger = logger;
        _ruleRepository = ruleRepository;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var rules = await _ruleRepository.GetAllAsync();
        var totalRules = rules.Count();
        var activeRules = rules.Count(r => r.Status == RuleEngine.Core.Models.RuleStatus.Active);
        
        // Get recent executions
        var recentExecutions = await _context.RuleExecutionAudits
            .OrderByDescending(e => e.ExecutedAt)
            .Take(10)
            .Select(e => e.ToDomainModel())
            .ToListAsync();
        
        ViewBag.TotalRules = totalRules;
        ViewBag.ActiveRules = activeRules;
        ViewBag.RecentExecutions = recentExecutions;
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
