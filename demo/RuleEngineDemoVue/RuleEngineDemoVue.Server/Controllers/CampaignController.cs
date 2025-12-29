using Microsoft.AspNetCore.Mvc;
using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using RuleEngineDemoVue.Server.Models;

namespace RuleEngineDemoVue.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly InMemoryCampaignRepository _repository;
    private readonly CampaignEngine.Core.CampaignManager<CampaignRuleInput, CampaignOutput> _campaignManager;

    public CampaignController(
        InMemoryCampaignRepository repository,
        CampaignEngine.Core.CampaignManager<CampaignRuleInput, CampaignOutput> campaignManager)
    {
        _repository = repository;
        _campaignManager = campaignManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<GeneralCampaign>> GetAll([FromQuery] DateTime? after, [FromQuery] int moduleId = 1)
    {
        var repoCampaigns = _repository.GetCampaigns(after ?? DateTime.MinValue, moduleId);
        return Ok(repoCampaigns);
    }

    [HttpGet("{id}")]
    public ActionResult<GeneralCampaign> Get(int id)
    {
        var campaign = _repository.GetById(id);
        return campaign == null ? NotFound() : Ok(campaign);
    }

    [HttpPost]
    public ActionResult<GeneralCampaign> Create(GeneralCampaign campaign)
    {
        _repository.AddCampaign(campaign);
        return CreatedAtAction(nameof(Get), new { id = campaign.Id }, campaign);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, GeneralCampaign campaign)
    {
        if (id != campaign.Id) return BadRequest();
        if (!_repository.UpdateCampaign(campaign)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_repository.DeleteCampaign(id)) return NotFound();
        return NoContent();
    }

    [HttpPost("check")]
    public ActionResult<IDictionary<string, bool>> CheckCampaigns([FromBody] Dictionary<string, bool> keys)
    {
        return Ok(_repository.GetAllCampaigns(keys));
    }

    [HttpGet("quota/{campaignId}")]
    public ActionResult<bool> CheckQuota(int campaignId, [FromQuery] int quota)
    {
        return Ok(_repository.CheckCampaignQuota(quota, campaignId));
    }

    [HttpPost("evaluate")]
    public ActionResult<IEnumerable<CampaignOutput>> Evaluate([FromBody] CampaignEvaluationRequest request)
    {
        var results = _campaignManager.GetCampaign(request.Input);
        return Ok(results);
    }

    [HttpPost("available")]
    public ActionResult<IEnumerable<AvailableCampaign>> Available([FromBody] CampaignAvailabilityRequest request)
    {
        var campaigns = _repository.GetCampaigns(DateTime.MinValue, request.ModuleId).ToList();
        foreach (var product in request.Products)
        {
            if (product.CampaignInformations.Count > 0)
                continue;

            foreach (var campaign in campaigns)
            {
                product.CampaignInformations[campaign.Code] = new CampaignInformation
                {
                    Code = campaign.Code,
                    Name = campaign.Name,
                    CampaignTypes = (CampaignTypes)(campaign.CampaignTypes ?? (int)CampaignTypes.DiscountCampaign),
                    TotalDiscount = new Price(0m, "TRY")
                };
            }
        }

        var products = request.Products.ToDictionary(p => p.Key, p => (ITravelProduct)p);
        var results = _campaignManager.GetAvailableCampaigns(request.ProductKey, products, request.Input);
        return Ok(results);
    }
}

public sealed class CampaignEvaluationRequest
{
    public CampaignRuleInput Input { get; set; } = new();
}

public sealed class CampaignAvailabilityRequest
{
    public string ProductKey { get; set; } = string.Empty;
    public CampaignRuleInput Input { get; set; } = new();
    public List<DemoTravelProduct> Products { get; set; } = new();
    public int ModuleId { get; set; } = 1;
}
