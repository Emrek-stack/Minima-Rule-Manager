using System.Runtime.Serialization;

namespace CampaignEngine.Core.Models
{
    [DataContract]
    public class CampaignOutput
    {
        [DataMember]
        public Price TotalDiscount { get; set; }
        [DataMember]
        public CampaignProductDiscount? CampaignProductDiscount { get; set; }
    }

    [DataContract]
    public class CampaignProductDiscount
    {
        [DataMember]
        public string ProductKey { get; set; } = string.Empty;
        [DataMember]
        public Price DiscountAmount { get; set; }
        [DataMember]
        public decimal DiscountPercentage { get; set; }
        [DataMember]
        public string DiscountType { get; set; } = string.Empty;
    }

    public class CampaignTargetResult
    {
        public string ProductKey { get; set; } = string.Empty;
        public Price Discount { get; set; }
    }
}
