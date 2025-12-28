using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleEngine.Core.Models;
using RuleEngine.Core.Rule;
using RuleEngine.Sqlite.Data;

namespace RuleEngineDemoVue.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RuleController : ControllerBase
{
    private readonly RuleDbContext _context;

    public RuleController(RuleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RuleEntity>>> GetAll()
    {
        return await _context.Rules.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RuleEntity>> Get(string id)
    {
        var rule = await _context.Rules.FindAsync(id);
        if (rule == null) return NotFound();
        return rule;
    }

    [HttpPost]
    public async Task<ActionResult<RuleEntity>> Create(RuleEntity rule)
    {
        rule.CreatedAt = DateTime.UtcNow;
        rule.UpdatedAt = DateTime.UtcNow;
        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = rule.Id }, rule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, RuleEntity rule)
    {
        if (id != rule.Id) return BadRequest();
        rule.UpdatedAt = DateTime.UtcNow;
        _context.Entry(rule).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var rule = await _context.Rules.FindAsync(id);
        if (rule == null) return NotFound();
        _context.Rules.Remove(rule);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("validate")]
    public ActionResult<List<RuleSyntaxError>> Validate([FromBody] string ruleString)
    {
        var compiler = new RuleCompiler<RuleInputModel, bool>();
        var errors = compiler.CheckSyntax(ruleString);
        return errors;
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
}
