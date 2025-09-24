using Microsoft.AspNetCore.Mvc;
using RuleEngine.Core.Abstractions;
using RuleEngine.Mvc.ViewModels;
using System.Text.Json;

namespace RuleEngine.Mvc.Controllers
{
    public class RuleBuilderController : Controller
    {
        private readonly IRuleRepository _ruleRepository;

        public RuleBuilderController(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        public IActionResult Index()
        {
            var model = new RuleBuilderViewModel
            {
                RuleItem = new RuleItemViewModel
                {
                    Name = "E-Commerce Rule Builder",
                    Children = new List<object>(),
                    Metadatas = new List<MetadataViewModel>
                    {
                        // 🛒 E-Ticaret Koşul Metadata'ları
                        new MetadataViewModel
                        {
                            Id = 1,
                            Title = "Sepet Tutarı Büyüktür",
                            Name = "CartAmountGreaterThan",
                            Type = "NumericParameter",
                            ParameterTypes = new List<string> { "NumericParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "NumericParameter", Title = "Minimum Tutar", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 2,
                            Title = "Müşteri Tipi",
                            Name = "CustomerType",
                            Type = "StringParameter",
                            ParameterTypes = new List<string> { "StringParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "StringParameter", Title = "Müşteri Tipi", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 3,
                            Title = "VIP Müşteri",
                            Name = "IsVipCustomer",
                            Type = "BooleanParameter",
                            ParameterTypes = new List<string> { "BooleanParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "BooleanParameter", Title = "VIP Müşteri", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 4,
                            Title = "Ürün Kategorisi",
                            Name = "ProductCategory",
                            Type = "StringParameter",
                            ParameterTypes = new List<string> { "StringParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "StringParameter", Title = "Kategori", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 5,
                            Title = "Ürün Sayısı",
                            Name = "ProductQuantity",
                            Type = "NumericParameter",
                            ParameterTypes = new List<string> { "NumericParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "NumericParameter", Title = "Minimum Adet", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 6,
                            Title = "Sipariş Tarihi",
                            Name = "OrderDate",
                            Type = "DateTimeParameter",
                            ParameterTypes = new List<string> { "DateTimeParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "DateTimeParameter", Title = "Tarih", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 7,
                            Title = "Müşteri Yaşı",
                            Name = "CustomerAge",
                            Type = "NumericParameter",
                            ParameterTypes = new List<string> { "NumericParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "NumericParameter", Title = "Minimum Yaş", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 8,
                            Title = "Şehir",
                            Name = "City",
                            Type = "StringParameter",
                            ParameterTypes = new List<string> { "StringParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "StringParameter", Title = "Şehir", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 9,
                            Title = "Önceki Sipariş Sayısı",
                            Name = "PreviousOrderCount",
                            Type = "NumericParameter",
                            ParameterTypes = new List<string> { "NumericParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "NumericParameter", Title = "Minimum Sipariş", Selected = "" }
                            }
                        },
                        new MetadataViewModel
                        {
                            Id = 10,
                            Title = "Kupon Kodu",
                            Name = "CouponCode",
                            Type = "StringParameter",
                            ParameterTypes = new List<string> { "StringParameter" },
                            ParameterDefinations = new List<ParameterDefinitionViewModel>
                            {
                                new ParameterDefinitionViewModel { Type = "StringParameter", Title = "Kupon Kodu", Selected = "" }
                            }
                        }
                    }
                },
                ResultItem = new ResultItemViewModel
                {
                    Metadatas = new List<MetadataViewModel>
                    {
                        // 🎁 E-Ticaret Sonuç Metadata'ları
                        new MetadataViewModel { Id = 1, Title = "İndirim Tutarı", Name = "DiscountAmount", Type = "NumericParameter", ParameterTypes = new List<string> { "NumericParameter" } },
                        new MetadataViewModel { Id = 2, Title = "İndirim Yüzdesi", Name = "DiscountPercentage", Type = "NumericParameter", ParameterTypes = new List<string> { "NumericParameter" } },
                        new MetadataViewModel { Id = 3, Title = "Ücretsiz Kargo", Name = "FreeShipping", Type = "BooleanParameter", ParameterTypes = new List<string> { "BooleanParameter" } },
                        new MetadataViewModel { Id = 4, Title = "Hediye Ürün", Name = "GiftProduct", Type = "StringParameter", ParameterTypes = new List<string> { "StringParameter" } },
                        new MetadataViewModel { Id = 5, Title = "Puan Kazancı", Name = "PointsEarned", Type = "NumericParameter", ParameterTypes = new List<string> { "NumericParameter" } },
                        new MetadataViewModel { Id = 6, Title = "Mesaj", Name = "Message", Type = "StringParameter", ParameterTypes = new List<string> { "StringParameter" } },
                        new MetadataViewModel { Id = 7, Title = "Özel Fiyat", Name = "SpecialPrice", Type = "NumericParameter", ParameterTypes = new List<string> { "NumericParameter" } },
                        new MetadataViewModel { Id = 8, Title = "Hızlı Teslimat", Name = "FastDelivery", Type = "BooleanParameter", ParameterTypes = new List<string> { "BooleanParameter" } }
                    }
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSampleRules()
        {
            try
            {
                var sampleRules = new[]
                {
                    // 🛒 VIP Müşteri İndirimi
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "VIP Müşteri %15 İndirim",
                        Description = "VIP müşterilere %15 indirim uygular",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"IsVipCustomer\",\"Parameters\":[\"true\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"DiscountPercentage\",\"Parameters\":[\"15\"]}]}"
                        }
                    },
                    
                    // 🛒 Büyük Sipariş Ücretsiz Kargo
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "500 TL Üzeri Ücretsiz Kargo",
                        Description = "500 TL üzeri siparişlerde ücretsiz kargo",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"CartAmountGreaterThan\",\"Parameters\":[\"500\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"FreeShipping\",\"Parameters\":[\"true\"]}]}"
                        }
                    },
                    
                    // 🛒 Elektronik Kategorisi İndirimi
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Elektronik %10 İndirim",
                        Description = "Elektronik kategorisindeki ürünlere %10 indirim",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"ProductCategory\",\"Parameters\":[\"Elektronik\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"DiscountPercentage\",\"Parameters\":[\"10\"]}]}"
                        }
                    },
                    
                    // 🛒 Sadık Müşteri Bonusu
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Sadık Müşteri 1000 Puan",
                        Description = "10'dan fazla sipariş veren müşterilere 1000 puan",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"PreviousOrderCount\",\"Parameters\":[\"10\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"PointsEarned\",\"Parameters\":[\"1000\"]}]}"
                        }
                    },
                    
                    // 🛒 İstanbul Müşterileri Hızlı Teslimat
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "İstanbul Hızlı Teslimat",
                        Description = "İstanbul'daki müşterilere hızlı teslimat",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"City\",\"Parameters\":[\"İstanbul\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"FastDelivery\",\"Parameters\":[\"true\"]}]}"
                        }
                    },
                    
                    // 🛒 Karmaşık Rule: VIP + Büyük Sipariş
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "VIP Büyük Sipariş %20 İndirim",
                        Description = "VIP müşteri ve 1000 TL üzeri siparişlerde %20 indirim",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"IsVipCustomer\",\"Parameters\":[\"true\"]},{\"Type\":\"AndOperatorStatement\",\"Parameters\":[\"&&\"]},{\"Type\":\"NamedRuleStatement\",\"Name\":\"CartAmountGreaterThan\",\"Parameters\":[\"1000\"]}]}",
                            ResultExpression = "{\"Type\":\"RuleTreeStatement\",\"Children\":[{\"Type\":\"NamedRuleStatement\",\"Name\":\"DiscountPercentage\",\"Parameters\":[\"20\"]}]}"
                        }
                    }
                };

                var createdRules = new List<object>();
                foreach (var rule in sampleRules)
                {
                    var createdRule = await _ruleRepository.CreateAsync(rule);
                    createdRules.Add(new { Id = createdRule.Id, Name = createdRule.Name });
                }

                return Json(new { 
                    IsSuccess = true, 
                    Message = $"{sampleRules.Length} adet örnek e-ticaret rule'ı oluşturuldu!",
                    Rules = createdRules
                });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = "Error creating sample rules: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveFromBuilder([FromBody] RuleBuilderSaveRequest request)
        {
            try
            {
                var createRequest = new RuleEngine.Core.Models.CreateRuleRequest
                {
                    Name = request.Name,
                    Description = "Generated from Rule Builder",
                    Content = new RuleEngine.Core.Models.RuleContent
                    {
                        PredicateExpression = request.PredicateExpression,
                        ResultExpression = request.ResultExpression
                    }
                };

                var rule = await _ruleRepository.CreateAsync(createRequest);

                return Json(new { IsSuccess = true, Id = rule.Id, Message = "Rule saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = "Error saving rule: " + ex.Message });
            }
        }
    }
}
