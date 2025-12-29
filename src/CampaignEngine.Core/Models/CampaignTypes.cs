using System.Runtime.Serialization;

namespace CampaignEngine.Core.Models;

public enum CampaignTypes
{
    [EnumMember(Value = "discount")]
    DiscountCampaign = 0,
    [EnumMember(Value = "product_gift")]
    ProductGiftCampaign = 1,
    [EnumMember(Value = "gift_coupon")]
    GiftCoupon = 2
}

public enum CampaignIncludes
{
    [EnumMember(Value = "included")]
    Included = 0,
    [EnumMember(Value = "optional")]
    Optional = 1
}