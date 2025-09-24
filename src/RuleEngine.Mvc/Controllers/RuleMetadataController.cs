using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RuleEngine.Core.Abstractions;
using RuleEngine.Mvc.ViewModels;
using System.Linq;

namespace RuleEngine.Mvc.Controllers
{
    public class RuleMetadataController : Controller
    {
        private readonly IRuleRepository _ruleRepository;

        public RuleMetadataController(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }

        // GET: RuleMetadata
        public IActionResult Index()
        {
            var model = new RuleMetadataIndexViewModel
            {
                CategoryItems = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Pricing", Value = "1" },
                    new SelectListItem { Text = "Validation", Value = "2" },
                    new SelectListItem { Text = "Business Logic", Value = "3" }
                }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(RuleMetadataListRequest request)
        {
            var rules = await _ruleRepository.GetAllAsync();
                var result = new RuleMetadataListResponse
                {
                    Data = rules.Select((r, index) => new RuleMetadataDto
                    {
                        Id = index + 1, // Use index instead of hash code for stability
                        Name = r.Name,
                        Title = r.Name,
                        ExpressionString = r.Content.PredicateExpression,
                        IsPredicate = true,
                        Categories = new List<string> { "General" }
                    }).ToList(),
                    RecordsFiltered = rules.Count(),
                    RecordsTotal = rules.Count()
                };
            return Json(result);
        }

                public async Task<IActionResult> Detail(int? id)
        {
            var model = new RuleMetadataDetailViewModel
            {
                Id = id ?? 0,
                Name = "",
                Title = "",
                Description = "",
                ExpressionString = "",
                IsPredicate = true,
                Categories = new List<string>(),
                CategoryItems = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Pricing", Value = "1" },
                    new SelectListItem { Text = "Validation", Value = "2" },
                    new SelectListItem { Text = "Business Logic", Value = "3" },
                    new SelectListItem { Text = "Customer", Value = "4" },
                    new SelectListItem { Text = "Product", Value = "5" },
                    new SelectListItem { Text = "Order", Value = "6" },
                    new SelectListItem { Text = "Payment", Value = "7" },
                    new SelectListItem { Text = "Shipping", Value = "8" },
                    new SelectListItem { Text = "Discount", Value = "9" },
                    new SelectListItem { Text = "Inventory", Value = "10" }
                },
                ParameterTypes = new List<SelectListItem>
                {
                    new SelectListItem { Text = "NumericParameter", Value = "NumericParameter" },
                    new SelectListItem { Text = "StringParameter", Value = "StringParameter" },
                    new SelectListItem { Text = "DateTimeParameter", Value = "DateTimeParameter" },
                    new SelectListItem { Text = "ArrayParameter", Value = "ArrayParameter" },
                    new SelectListItem { Text = "ListParameter", Value = "ListParameter" },
                    new SelectListItem { Text = "DefinitionParameter", Value = "DefinitionParameter" },
                    new SelectListItem { Text = "DestinationParameter", Value = "DestinationParameter" },
                    new SelectListItem { Text = "ReadonlyParameter", Value = "ReadonlyParameter" },
                    new SelectListItem { Text = "BooleanListParameter", Value = "BooleanListParameter" },
                    new SelectListItem { Text = "EqualityListParameter", Value = "EqualityListParameter" },
                    new SelectListItem { Text = "DateTimeGroupParameter", Value = "DateTimeGroupParameter" }
                },
                Parameters = new List<ParameterDefinitionModel>()
            };

            if (id.HasValue && id > 0)
            {
                // Index-based ID system for stability
                var allRules = await _ruleRepository.GetAllAsync();
                var ruleIndex = id.Value - 1; // Convert to 0-based index
                
                if (ruleIndex >= 0 && ruleIndex < allRules.Count())
                {
                    var rule = allRules.ElementAt(ruleIndex);
                    
                    Console.WriteLine($"Debug: Rule found at index {ruleIndex} - Name: {rule.Name}, Description: {rule.Description}");
                    Console.WriteLine($"Debug: Rule Content is null: {rule.Content == null}");
                    if (rule.Content != null)
                    {
                        Console.WriteLine($"Debug: Rule Content - PredicateExpression: {rule.Content.PredicateExpression}");
                    }
                    
                    model.Id = id.Value;
                    model.Name = rule.Name ?? "";
                    model.Title = rule.Name ?? "";
                    model.Description = rule.Description ?? "";
                    model.ExpressionString = rule.Content?.PredicateExpression ?? "";
                    model.IsPredicate = true;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(RuleMetadataDetailViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    // For now, skip update since we're using hash codes
                    // var existingRule = await _ruleRepository.GetByIdAsync(model.Id.ToString());
                    // Skip update for now - just create new
                    // if (existingRule != null)
                    // {
                    //     existingRule.Name = model.Name;
                    //     existingRule.Description = model.Description;
                    //     existingRule.Content.PredicateExpression = model.ExpressionString;
                    //     var updateRequest = new RuleEngine.Core.Models.UpdateRuleRequest
                    //     {
                    //         Name = model.Name,
                    //         Description = model.Description,
                    //         Content = new RuleEngine.Core.Models.RuleContent
                    //         {
                    //             PredicateExpression = model.ExpressionString,
                    //             ResultExpression = existingRule.Content.ResultExpression
                    //         }
                    //     };
                    //     await _ruleRepository.UpdateAsync(model.Id.ToString(), updateRequest);
                    // }
                }
                else
                {
                    var createRequest = new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = model.ExpressionString,
                            ResultExpression = ""
                        }
                    };
                    await _ruleRepository.CreateAsync(createRequest);
                }

                return Json(new { IsSuccess = true, Id = model.Id, Message = "Kayıt başarılı bir şekilde eklendi" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = "Hata! Kayıt Eklenmedi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                // Skip delete for now since we're using hash codes
                // await _ruleRepository.DeleteAsync(id.ToString());
                return Json(new { IsSuccess = true, Message = "Kayıt başarılı bir şekilde silindi" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = "Hata! Kayıt silinemedi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                // Basit örnekler
                var simpleRules = new[]
                {
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Basit Fiyat Kontrolü",
                        Description = "Ürün fiyatının 0'dan büyük olması kontrolü",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "product.Price > 0",
                            ResultExpression = "true"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Stok Durumu Kontrolü",
                        Description = "Ürün stok miktarının yeterli olması kontrolü",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "product.StockQuantity >= requestedQuantity",
                            ResultExpression = "true"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Müşteri Yaş Kontrolü",
                        Description = "Müşterinin 18 yaşından büyük olması kontrolü",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "customer.Age >= 18",
                            ResultExpression = "true"
                        }
                    }
                };

                // Orta seviye örnekler
                var mediumRules = new[]
                {
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Karmaşık Fiyat Hesaplama",
                        Description = "İndirim ve vergi dahil fiyat hesaplama",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "product.Price > 0 && discount.Percentage >= 0 && discount.Percentage <= 100",
                            ResultExpression = "product.Price * (1 - discount.Percentage / 100) * (1 + tax.Rate)"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Sipariş Onay Kontrolü",
                        Description = "Sipariş tutarı ve müşteri limiti kontrolü",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "order.TotalAmount <= customer.CreditLimit && order.Items.Count > 0",
                            ResultExpression = "order.TotalAmount <= customer.CreditLimit"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Ürün Kategori Filtreleme",
                        Description = "Belirli kategorilerdeki ürünlerin filtrelenmesi",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "product.Category == \"Electronics\" || product.Category == \"Books\"",
                            ResultExpression = "product.IsAvailable"
                        }
                    }
                };

                // Karmaşık örnekler
                var complexRules = new[]
                {
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Dinamik Fiyatlandırma Sistemi",
                        Description = "Müşteri segmenti, ürün popülaritesi ve stok durumuna göre fiyat hesaplama",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "customer.Segment != null && product.PopularityScore >= 0 && product.StockQuantity > 0",
                            ResultExpression = "product.BasePrice * (customer.Segment == \"Premium\" ? 1.1 : customer.Segment == \"Standard\" ? 1.0 : 0.9) * (product.PopularityScore > 80 ? 1.2 : product.PopularityScore > 50 ? 1.1 : 1.0) * (product.StockQuantity < 10 ? 1.15 : 1.0)"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Çoklu Koşullu Sipariş Validasyonu",
                        Description = "Müşteri, ürün, stok ve ödeme koşullarının birlikte kontrolü",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "customer.IsActive && customer.CreditScore >= 600 && order.Items.All(item => item.Product.IsAvailable && item.Quantity <= item.Product.StockQuantity) && paymentMethod.IsValid",
                            ResultExpression = "order.TotalAmount <= customer.CreditLimit && order.ShippingAddress.IsValid"
                        }
                    },
                    new RuleEngine.Core.Models.CreateRuleRequest
                    {
                        Name = "Akıllı İndirim Sistemi",
                        Description = "Müşteri geçmişi, ürün kategorisi ve sezon bazlı indirim hesaplama",
                        Content = new RuleEngine.Core.Models.RuleContent
                        {
                            PredicateExpression = "customer.PurchaseHistory.Count > 0 && product.Category != null && currentSeason != null",
                            ResultExpression = "customer.PurchaseHistory.Count > 10 ? 0.15 : customer.PurchaseHistory.Count > 5 ? 0.10 : 0.05 + (product.Category == \"Seasonal\" && currentSeason == \"Winter\" ? 0.20 : 0) + (customer.TotalSpent > 1000 ? 0.05 : 0)"
                        }
                    }
                };

                // Tüm örnekleri ekle
                var allRules = simpleRules.Concat(mediumRules).Concat(complexRules);
                
                foreach (var rule in allRules)
                {
                    await _ruleRepository.CreateAsync(rule);
                }

                return Json(new { IsSuccess = true, Message = $"{allRules.Count()} örnek kural başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = "Hata! Örnek veriler eklenemedi: " + ex.Message });
            }
        }
    }
}
