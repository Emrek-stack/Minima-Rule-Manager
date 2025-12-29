using Microsoft.AspNetCore.Mvc;
using RuleEngine.Core.Rule.DesignTime;
using RuleEngine.Core.Rule.DesignTime.Parameters;

namespace RuleEngineDemoVue.Server.Controllers;

[ApiController]
[Route("api/rule-metadata")]
public class RuleMetadataController : ControllerBase
{
    public RuleMetadataController(MetadataManagerInitializer initializer)
    {
        _ = initializer;
    }

    [HttpGet]
    public ActionResult<RuleMetadataResponse> GetMetadatas([FromQuery] bool isPredicate = true, [FromQuery] string? categoryIds = null)
    {
        MetadataManager.RefreshAsync().GetAwaiter().GetResult();

        var categories = ParseCategories(categoryIds);
        var metadatas = categories.Count == 0
            ? MetadataManager.GetMetadaByCategory(isPredicate)
            : MetadataManager.GetMetadaByCategory(isPredicate, categories.ToArray());

        var response = new RuleMetadataResponse
        {
            Metadatas = metadatas.ToDictionary(k => k.Key, v => RuleMetadataDto.From(v.Value))
        };

        return Ok(response);
    }

    [HttpGet("categories")]
    public ActionResult<IEnumerable<RuleCategoryDto>> GetCategories()
    {
        MetadataManager.RefreshAsync().GetAwaiter().GetResult();

        var categories = MetadataManager.Categories
            .Select(kv => new RuleCategoryDto
            {
                Id = kv.Key,
                Name = kv.Value,
                RuleNames = MetadataManager.RulesByCategory.TryGetValue(kv.Key.ToString(), out var rules) ? rules : new List<string>()
            })
            .OrderBy(c => c.Id)
            .ToList();

        return Ok(categories);
    }

    private static List<int> ParseCategories(string? categoryIds)
    {
        if (string.IsNullOrWhiteSpace(categoryIds))
            return new List<int>();

        return categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => int.TryParse(id, out var value) ? value : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
    }
}

public sealed class RuleMetadataResponse
{
    public Dictionary<string, RuleMetadataDto> Metadatas { get; set; } = new();
}

public sealed class RuleMetadataDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayFormat { get; set; } = string.Empty;
    public string ExpressionFormat { get; set; } = string.Empty;
    public bool IsPredicate { get; set; }
    public List<ParameterDefinition> ParameterDefinations { get; set; } = new();

    public static RuleMetadataDto From(RuleEngine.Core.Rule.DesignTime.Metadatas.NamedRuleMetadata metadata)
    {
        return new RuleMetadataDto
        {
            Title = metadata.Title ?? string.Empty,
            Description = metadata.Description ?? string.Empty,
            DisplayFormat = metadata.DisplayFormat ?? string.Empty,
            ExpressionFormat = metadata.ExpressionFormat ?? string.Empty,
            IsPredicate = metadata.IsPredicate,
            ParameterDefinations = metadata.ParameterDefinations ?? new List<ParameterDefinition>()
        };
    }
}

public sealed class RuleCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> RuleNames { get; set; } = new();
}
