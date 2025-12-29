using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleEngine.Core.Abstractions;
using RuleEngine.Core.Models;
using RuleEngine.Sqlite.Data;
using RuleEngineDemoVue.Server.Models;

namespace RuleEngineDemoVue.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RuleController : ControllerBase
{
    private readonly RuleDbContext _context;
    private readonly IRuleRepository _ruleRepository;
    private readonly IRuleEngine _ruleEngine;
    private readonly IRuleEvaluator _ruleEvaluator;
    private readonly IAuditRepository _auditRepository;

    public RuleController(
        RuleDbContext context,
        IRuleRepository ruleRepository,
        IRuleEngine ruleEngine,
        IRuleEvaluator ruleEvaluator,
        IAuditRepository auditRepository)
    {
        _context = context;
        _ruleRepository = ruleRepository;
        _ruleEngine = ruleEngine;
        _ruleEvaluator = ruleEvaluator;
        _auditRepository = auditRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RuleDefinition>>> GetAll()
    {
        var rules = await _ruleRepository.GetAllAsync();
        return Ok(rules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RuleDefinition>> Get(string id)
    {
        var rule = await _ruleRepository.GetByIdAsync(id);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    [HttpPost]
    public async Task<ActionResult<RuleDefinition>> Create(CreateRuleRequest request)
    {
        var created = await _ruleRepository.CreateAsync(request);
        await _ruleRepository.ActivateVersionAsync(created.Id, 1);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RuleDefinition>> Update(string id, UpdateRuleRequest request)
    {
        var updated = await _ruleRepository.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _ruleRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidationResult>> Validate([FromBody] RuleValidationRequest request)
    {
        var rule = await _ruleRepository.GetActiveVersionAsync(request.RuleId);
        if (rule == null)
            return NotFound();

        var validation = await _ruleEvaluator.ValidateAsync(rule, request.Input);
        return Ok(validation);
    }

    [HttpGet("versions/{ruleId}")]
    public async Task<ActionResult<IEnumerable<RuleVersionEntity>>> GetVersions(string ruleId)
    {
        return await _context.RuleVersions.Where(v => v.RuleId == ruleId).ToListAsync();
    }

    [HttpGet("parameters/{ruleId}")]
    public async Task<ActionResult<IEnumerable<RuleParameterEntity>>> GetParameters(string ruleId)
    {
        return await _context.RuleParameters.Where(p => p.RuleId == ruleId).ToListAsync();
    }

    [HttpPost("evaluate/{ruleId}")]
    public async Task<ActionResult<RuleExecutionResult>> Evaluate(string ruleId, [FromBody] OrderRuleInput input)
    {
        var result = await _ruleEngine.EvaluateAsync(ruleId, input);
        return Ok(result);
    }

    [HttpPost("{ruleId}/versions")]
    public async Task<ActionResult<RuleDefinition>> CreateVersion(string ruleId, CreateVersionRequest request)
    {
        var rule = await _ruleRepository.CreateVersionAsync(ruleId, request);
        return Ok(rule);
    }

    [HttpPost("{ruleId}/versions/{version}/activate")]
    public async Task<ActionResult<RuleDefinition>> ActivateVersion(string ruleId, int version)
    {
        var rule = await _ruleRepository.ActivateVersionAsync(ruleId, version);
        return Ok(rule);
    }

    [HttpGet("audit/{ruleId}")]
    public async Task<ActionResult<IEnumerable<RuleExecutionAudit>>> GetAuditHistory(string ruleId, [FromQuery] int limit = 50)
    {
        var history = await _auditRepository.GetExecutionHistoryAsync(ruleId, limit);
        return Ok(history);
    }
}

public sealed class RuleValidationRequest
{
    public string RuleId { get; set; } = string.Empty;
    public OrderRuleInput Input { get; set; } = new();
}
