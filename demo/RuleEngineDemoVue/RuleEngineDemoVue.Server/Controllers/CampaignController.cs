using Microsoft.AspNetCore.Mvc;
using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;
using System.Collections.Concurrent;

namespace RuleEngineDemoVue.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly InMemoryCampaignRepository _repository;
    private static readonly ConcurrentDictionary<int, GeneralCampaign> _campaigns = new();
    private static int _nextId = 1;
    private static bool _initialized = false;

    public CampaignController(InMemoryCampaignRepository repository)
    {
        _repository = repository;
        
        if (!_initialized)
        {
            _initialized = true;
            
            _campaigns[_nextId++] = new GeneralCampaign { Id = 1, Code = "NEWYEAR2025", Name = "Yeni Yıl Kampanyası", Description = "Tüm ürünlerde %20 indirim", StartDate = DateTime.UtcNow.AddDays(-7), EndDate = DateTime.UtcNow.AddDays(30), Priority = 1, Predicate = "Model.TotalAmount > 100", Result = "Model.TotalAmount * 0.8", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 2, Code = "VIP30", Name = "VIP Müşteri İndirimi", Description = "VIP müşterilere özel %30 indirim", StartDate = DateTime.UtcNow.AddDays(-14), EndDate = DateTime.UtcNow.AddDays(60), Priority = 2, Predicate = "Model.CustomerType == \"VIP\"", Result = "Model.TotalAmount * 0.7", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 3, Code = "FREESHIP100", Name = "Kargo Bedava", Description = "100 TL ve üzeri kargo ücretsiz", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(90), Priority = 3, Predicate = "Model.TotalAmount >= 100", Result = "0", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 4, Code = "FIRSTORDER", Name = "İlk Sipariş İndirimi", Description = "Yeni müşterilere %15 indirim", StartDate = DateTime.UtcNow.AddDays(-60), EndDate = DateTime.UtcNow.AddDays(120), Priority = 4, Predicate = "Model.OrderCount == 0", Result = "Model.TotalAmount * 0.85", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 5, Code = "BULK10", Name = "Toplu Alım İndirimi", Description = "3+ ürün alınca %10 indirim", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(40), Priority = 5, Predicate = "Model.ProductCount > 3", Result = "Model.TotalAmount * 0.9", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 6, Code = "WEEKEND15", Name = "Hafta Sonu İndirimi", Description = "Cumartesi-Pazar %15 indirim", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(50), Priority = 6, Predicate = "Model.OrderTime.DayOfWeek == DayOfWeek.Saturday || Model.OrderTime.DayOfWeek == DayOfWeek.Sunday", Result = "Model.TotalAmount * 0.85", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 7, Code = "ISTANBUL", Name = "İstanbul Özel", Description = "İstanbul'a teslimat %5 indirim", StartDate = DateTime.UtcNow.AddDays(-15), EndDate = DateTime.UtcNow.AddDays(45), Priority = 7, Predicate = "Model.City == \"Istanbul\"", Result = "Model.TotalAmount * 0.95", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 8, Code = "ELECTRONICS", Name = "Elektronik İndirimi", Description = "Elektronik kategorisinde %25 indirim", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(35), Priority = 8, Predicate = "Model.Category == \"Electronics\"", Result = "Model.TotalAmount * 0.75", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 9, Code = "PREMIUM500", Name = "Premium Sipariş", Description = "500 TL üzeri %12 indirim", StartDate = DateTime.UtcNow.AddDays(-25), EndDate = DateTime.UtcNow.AddDays(55), Priority = 9, Predicate = "Model.TotalAmount >= 500", Result = "Model.TotalAmount * 0.88", CreateDate = DateTime.UtcNow };
            _campaigns[_nextId++] = new GeneralCampaign { Id = 10, Code = "LOYALTY", Name = "Sadakat İndirimi", Description = "10+ sipariş verenlere %20 indirim", StartDate = DateTime.UtcNow.AddDays(-40), EndDate = DateTime.UtcNow.AddDays(80), Priority = 10, Predicate = "Model.OrderCount >= 10", Result = "Model.TotalAmount * 0.8", CreateDate = DateTime.UtcNow };
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<GeneralCampaign>> GetAll([FromQuery] DateTime? after, [FromQuery] int moduleId = 0)
    {
        var repoCampaigns = _repository.GetCampaigns(after ?? DateTime.MinValue, moduleId);
        var allCampaigns = repoCampaigns.Concat(_campaigns.Values);
        return Ok(allCampaigns);
    }

    [HttpGet("{id}")]
    public ActionResult<GeneralCampaign> Get(int id)
    {
        if (_campaigns.TryGetValue(id, out var campaign))
            return campaign;
        return NotFound();
    }

    [HttpPost]
    public ActionResult<GeneralCampaign> Create(GeneralCampaign campaign)
    {
        campaign.Id = _nextId++;
        campaign.CreateDate = DateTime.UtcNow;
        _campaigns[campaign.Id] = campaign;
        return CreatedAtAction(nameof(Get), new { id = campaign.Id }, campaign);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, GeneralCampaign campaign)
    {
        if (id != campaign.Id) return BadRequest();
        if (!_campaigns.ContainsKey(id)) return NotFound();
        _campaigns[id] = campaign;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_campaigns.TryRemove(id, out _)) return NotFound();
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
}
