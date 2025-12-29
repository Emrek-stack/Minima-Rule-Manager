using CampaignEngine.Core.Models;
using CampaignEngine.Core.Repositories;

namespace CampaignEngine.Core.Demo;

public static class CampaignSeed
{
    public static List<GeneralCampaign> BuildDemoCampaigns(int moduleId)
    {
        var now = DateTime.UtcNow;

        return new List<GeneralCampaign>
        {
            new GeneralCampaign
            {
                Id = 1,
                Code = "NEWYEAR2025",
                Name = "Yeni Yil Kampanyasi",
                Description = "Yilbasi kampanyasi: toplam tutar uzerinden indirim.",
                StartDate = now.AddDays(-7),
                EndDate = now.AddDays(30),
                Priority = 4,
                Predicate = "Input.TotalAmount > 250m",
                Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.2m, \"TRY\");",
                Usage = "Input.UsageCount < 5",
                CampaignTypes = (int)CampaignTypes.DiscountCampaign,
                CreateDate = now,
                ModulId = moduleId,
                PromotionCode = "NY2025",
                Quota = 500
            },
            new GeneralCampaign
            {
                Id = 2,
                Code = "VIP30",
                Name = "VIP Musteri Indirimi",
                Description = "VIP musterilere ozel %30 indirim.",
                StartDate = now.AddDays(-14),
                EndDate = now.AddDays(60),
                Priority = 5,
                Predicate = "Input.CustomerType == \"VIP\"",
                Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.3m, \"TRY\");",
                Usage = "Input.UsageCount < 3",
                CampaignTypes = (int)CampaignTypes.DiscountCampaign,
                CreateDate = now,
                ModulId = moduleId,
                PromotionCode = "VIP30"
            },
            new GeneralCampaign
            {
                Id = 3,
                Code = "BULK15",
                Name = "Toplu Alim Indirimi",
                Description = "3+ urun icin %15 indirim.",
                StartDate = now.AddDays(-3),
                EndDate = now.AddDays(90),
                Priority = 3,
                Predicate = "Input.ProductCount >= 3",
                Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.15m, \"TRY\");",
                Usage = "true",
                CampaignTypes = (int)CampaignTypes.DiscountCampaign,
                CreateDate = now,
                ModulId = moduleId
            },
            new GeneralCampaign
            {
                Id = 4,
                Code = "NIGHTOWL10",
                Name = "Gece Indirimi",
                Description = "22:00-06:00 arasinda %10 indirim.",
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(120),
                Priority = 2,
                Predicate = "Input.OrderTime.Hour >= 22 || Input.OrderTime.Hour <= 6",
                Result = "Output.TotalDiscount = new Price(Input.TotalAmount * 0.1m, \"TRY\");",
                Usage = "true",
                CampaignTypes = (int)CampaignTypes.DiscountCampaign,
                CreateDate = now,
                ModulId = moduleId
            },
            new GeneralCampaign
            {
                Id = 5,
                Code = "CITYGIFT50",
                Name = "Sehir Bazli Hediye",
                Description = "Istanbul icin 50 TL hediye.",
                StartDate = now.AddDays(-20),
                EndDate = now.AddDays(45),
                Priority = 1,
                Predicate = "Input.City == \"Istanbul\"",
                Result = "Output.TotalDiscount = new Price(50m, \"TRY\");" +
                         "Output.CampaignProductDiscount = new CampaignProductDiscount {" +
                         " ProductKey = \"FLIGHT-100\"," +
                         " DiscountAmount = new Price(50m, \"TRY\")," +
                         " DiscountPercentage = 0m," +
                         " DiscountType = \"flat\" };",
                Usage = "true",
                CampaignTypes = (int)CampaignTypes.ProductGiftCampaign,
                CreateDate = now,
                ModulId = moduleId,
                Quota = 100
            },
            new GeneralCampaign
            {
                Id = 6,
                Code = "FREESHIP100",
                Name = "Kargo Bedava",
                Description = "100 TL ve uzeri siparislerde kargo bedava.",
                StartDate = now.AddDays(-30),
                EndDate = now.AddDays(90),
                Priority = 2,
                Predicate = "Input.TotalAmount >= 100m",
                Result = "Output.TotalDiscount = new Price(25m, \"TRY\");",
                Usage = "true",
                CampaignTypes = (int)CampaignTypes.ProductGiftCampaign,
                CreateDate = now,
                ModulId = moduleId
            }
        };
    }

    public static void SeedToRepository(InMemoryCampaignRepository repository, int moduleId)
    {
        foreach (var campaign in BuildDemoCampaigns(moduleId))
        {
            repository.AddCampaign(campaign);
        }
    }
}
