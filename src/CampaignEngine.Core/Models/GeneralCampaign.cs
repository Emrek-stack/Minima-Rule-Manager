namespace CampaignEngine.Core.Models;

public class GeneralCampaign
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? ModulId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? CouponCodeId { get; set; }
    public string? PromotionCode { get; set; }
    public int? DepartmentId { get; set; }
    public int Priority { get; set; }
    public int? CancelReasonId { get; set; }
    public int? CancelSourceId { get; set; }
    public string? Description { get; set; }
    public string Predicate { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public int? Quota { get; set; }
    public int? CampaignTypes { get; set; }
    public string? Usage { get; set; }
}

public class CampaignInformation
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Price TotalDiscount { get; set; }
    public CampaignTypes CampaignTypes { get; set; }
    public bool Used { get; set; }
}

public class AvailableCampaign
{
    public string CampaignCode { get; set; } = string.Empty;
    public string CampaignName { get; set; } = string.Empty;
    public CampaignIncludes? CampaignType { get; set; }
    public string ProductKey { get; set; } = string.Empty;
    public List<CampaignTargetResult> TargetResults { get; set; } = new();
}