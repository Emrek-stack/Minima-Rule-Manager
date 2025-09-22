using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;
using RuleEngine.Mvc.ViewModels;
using RuleEngine.Sqlite.Data;

namespace RuleEngine.Mvc.Controllers
{
    public class RulesController : Controller
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly RuleDbContext _context;

        public RulesController(
            IRuleRepository ruleRepository,
            IAuditRepository auditRepository,
            RuleDbContext context)
        {
            _ruleRepository = ruleRepository;
            _auditRepository = auditRepository;
            _context = context;
        }

        // GET: Rules
        public async Task<IActionResult> Index()
        {
            var rules = await _ruleRepository.GetAllAsync();
            return View(rules);
        }

        // GET: Rules/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            return View(rule);
        }

        // GET: Rules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = new CreateRuleRequest
                {
                    Name = model.Name,
                    Description = model.Description,
                    Tags = model.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    Content = new RuleContent
                    {
                        PredicateExpression = model.PredicateExpression,
                        ResultExpression = model.ResultExpression,
                        Language = "csharp"
                    }
                };

                var rule = await _ruleRepository.CreateAsync(request);
                await _ruleRepository.ActivateVersionAsync(rule.Id, 1);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Rules/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            var model = new EditRuleViewModel
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Tags = string.Join(", ", rule.Tags),
                Status = rule.Status,
                PredicateExpression = rule.Content.PredicateExpression,
                ResultExpression = rule.Content.ResultExpression
            };

            return View(model);
        }

        // POST: Rules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditRuleViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var request = new UpdateRuleRequest
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Tags = model.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries),
                        Status = model.Status,
                        Content = new RuleContent
                        {
                            PredicateExpression = model.PredicateExpression,
                            ResultExpression = model.ResultExpression,
                            Language = "csharp"
                        }
                    };

                    await _ruleRepository.UpdateAsync(id, request);
                }
                catch (Exception)
                {
                    if (await _ruleRepository.GetByIdAsync(id) == null)
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Rules/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            return View(rule);
        }

        // POST: Rules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _ruleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Rules/Execute/5
        public async Task<IActionResult> Execute(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            var model = new ExecuteRuleViewModel
            {
                RuleId = id,
                RuleName = rule.Name,
                SampleInput = "{\n  \"Amount\": 150,\n  \"IsVip\": true\n}"
            };

            return View(model);
        }

        // POST: Rules/Execute/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(string id, ExecuteRuleViewModel model)
        {
            if (id != model.RuleId)
                return NotFound();

            try
            {
                // Parse JSON input
                var input = System.Text.Json.JsonSerializer.Deserialize<object>(model.SampleInput);
                
                // TODO: Implement rule execution with Gordios RuleManager
                // For now, return a mock result
                model.ExecutionResult = new RuleExecutionResult
                {
                    Success = true,
                    Result = "Rule execution not yet implemented with Gordios RuleManager",
                    Duration = TimeSpan.FromMilliseconds(1)
                };
                model.Success = true;
            }
            catch (Exception ex)
            {
                model.ExecutionResult = new RuleExecutionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Duration = TimeSpan.Zero
                };
                model.Success = false;
            }

            return View(model);
        }

        // GET: Rules/History/5
        public async Task<IActionResult> History(string id, int limit = 50)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            var history = await _auditRepository.GetExecutionHistoryAsync(id, limit);
            
            var model = new RuleHistoryViewModel
            {
                RuleId = id,
                RuleName = rule.Name,
                Executions = history
            };

            return View(model);
        }

        // GET: Rules/Versions/5
        public async Task<IActionResult> Versions(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            // Get all versions for this rule
            var versions = await _context.RuleVersions
                .Where(v => v.RuleId == id)
                .OrderByDescending(v => v.Version)
                .ToListAsync();

            var model = new RuleVersionsViewModel
            {
                RuleId = id,
                RuleName = rule.Name,
                Versions = versions
            };

            return View(model);
        }

        // POST: Rules/ActivateVersion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateVersion(string id, int version)
        {
            await _ruleRepository.ActivateVersionAsync(id, version);
            return RedirectToAction(nameof(Versions), new { id });
        }

        // GET: Rules/RuleBuilder
        public IActionResult RuleBuilder()
        {
            var model = new RuleBuilderViewModel();
            return View(model);
        }

        // POST: Rules/SaveFromBuilder
        [HttpPost]
        public async Task<IActionResult> SaveFromBuilder([FromBody] SaveFromBuilderRequest request)
        {
            try
            {
                var createRequest = new CreateRuleRequest
                {
                    Name = request.Name,
                    Description = "Generated from Rule Builder",
                    Tags = new[] { "builder", "generated" },
                    Content = new RuleContent
                    {
                        PredicateExpression = request.PredicateExpression,
                        ResultExpression = request.ResultExpression,
                        Language = request.Language
                    }
                };

                var rule = await _ruleRepository.CreateAsync(createRequest);
                await _ruleRepository.ActivateVersionAsync(rule.Id, 1);

                return Json(new { success = true, id = rule.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }

    public class SaveFromBuilderRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PredicateExpression { get; set; } = string.Empty;
        public string ResultExpression { get; set; } = string.Empty;
        public string Language { get; set; } = "json";
    }
}
