using CampaignEngine.Core.Abstractions;
using CampaignEngine.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RuleEngine.Core.Rule;
using RuleEngine.Core.Models;

namespace CampaignEngine.Core;

public class CampaignManager<TCampaignRuleInput, TCampaignRuleOutput>
    where TCampaignRuleInput : RuleInputModel
    where TCampaignRuleOutput : CampaignOutput
{
    private readonly CampaignRuleProvider _ruleProvider;
    private readonly int _moduleId;
    private readonly ILogger<CampaignManager<TCampaignRuleInput, TCampaignRuleOutput>> _logger;

    public int ModuleId => _moduleId;

    public CampaignManager(int moduleId, IServiceProvider serviceProvider, ILogger<CampaignManager<TCampaignRuleInput, TCampaignRuleOutput>> logger, params Type[] extraType)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        _moduleId = moduleId;
        _logger = logger;
        var extraTypes = extraType?.Where(t => t != null).ToArray() ?? Array.Empty<Type>();
        _ruleProvider = new CampaignRuleProvider(_moduleId, serviceProvider, extraTypes);
        _ruleProvider.WaitInitialization();
    }

    private class CampaignRuleProvider : IRuleProvider<CampaignRuleSet, TCampaignRuleInput, TCampaignRuleOutput>
    {
        private readonly int _moduleId;
        private readonly Type[] _extraType;
        private readonly IServiceProvider _serviceProvider;
        private readonly RuleCompiler<TCampaignRuleInput, bool> _usageRuleCompiler;
        private readonly RuleCompiler<TCampaignRuleInput, TCampaignRuleOutput> _resultRuleCompiler;

        public CampaignRuleProvider(int moduleId, IServiceProvider serviceProvider, params Type[] extraType)
        {
            _moduleId = moduleId;
            _extraType = extraType ?? Array.Empty<Type>();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _usageRuleCompiler = new RuleCompiler<TCampaignRuleInput, bool>(_extraType);
            _resultRuleCompiler = new RuleCompiler<TCampaignRuleInput, TCampaignRuleOutput>(_extraType);
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public async Task<IDictionary<string, CampaignRuleSet>> GenerateRuleSetsAsync(DateTime after)
        {
            var repo = _serviceProvider.GetRequiredService<ICampaignRepository>();
            var ruleEntities = repo.GetCampaigns(after, _moduleId);
            var result = new Dictionary<string, CampaignRuleSet>();

            foreach (var ruleEntity in ruleEntities)
            {
                try
                {
                    var predicateCompiler = new RuleCompiler<TCampaignRuleInput, bool>(_extraType);
                    var resultCompiler = new RuleCompiler<TCampaignRuleInput, TCampaignRuleOutput>(_extraType, useExpressionTreeTemplate: false);
                        
                    var predicateRule = await predicateCompiler.CompileAsync(ruleEntity.Code, ruleEntity.Predicate);
                    var resultRule = await resultCompiler.CompileAsync(ruleEntity.Code, ruleEntity.Result);
                    var usageRule = await _usageRuleCompiler.CompileAsync(ruleEntity.Code, ruleEntity.Usage ?? "true");
                        
                    var ruleSet = RuleSet.Create<CampaignRuleSet, TCampaignRuleInput, TCampaignRuleOutput>(
                        ruleEntity.Code,
                        predicateRule,
                        resultRule,
                        ruleEntity.Priority
                    );
                        
                    ruleSet.UsageRule = usageRule;
                    ruleSet.Name = ruleEntity.Name;
                    ruleSet.StartDate = ruleEntity.StartDate;
                    ruleSet.EndDate = ruleEntity.EndDate;
                    ruleSet.Description = ruleEntity.Description;
                    ruleSet.Quota = ruleEntity.Quota ?? 0;
                    ruleSet.CampaignTypes = ruleEntity.CampaignTypes;
                    ruleSet.CancelReasonId = ruleEntity.CancelReasonId;
                    ruleSet.CancelSourceId = ruleEntity.CancelSourceId;
                    ruleSet.CouponCodeId = ruleEntity.CouponCodeId;
                    ruleSet.DepartmentId = ruleEntity.DepartmentId;
                    ruleSet.ModulId = ruleEntity.ModulId;
                    ruleSet.PromotionCode = ruleEntity.PromotionCode;
                    ruleSet.Id = ruleEntity.Id;
                    ruleSet.CreateDate = ruleEntity.CreateDate;
                    result[ruleEntity.Code] = ruleSet;
                }
                catch (Exception e)
                {
                    var logger = _serviceProvider.GetService<ILogger<CampaignRuleProvider>>();
                    logger?.LogError(e, "Error creating rule set for campaign {CampaignCode}", ruleEntity.Code);
                }
            }
            return result;
        }

        public async Task<IDictionary<string, bool>> IsExistsAsync(params string[] keys)
        {
            var result = new Dictionary<string, bool>();
            foreach (var key in keys) result.Add(key, false);
            var campaignRepo = _serviceProvider.GetRequiredService<ICampaignRepository>();
            var founded = campaignRepo.GetAllCampaigns(result);
            foreach (var foundKey in founded.Keys) result[foundKey] = founded[foundKey];
            return await Task.FromResult(result);
        }

        public void WaitInitialization() { }
    }

    public class CampaignRuleSet : RuleSet<TCampaignRuleInput, TCampaignRuleOutput>
    {
        public CampaignRuleSet() { }
            
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ModulId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? CouponCodeId { get; set; }
        public string? PromotionCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? CancelReasonId { get; set; }
        public int? CancelSourceId { get; set; }
        public int? Quota { get; set; }
        public CompiledRule<TCampaignRuleInput, bool>? UsageRule { get; set; }
        public int? CampaignTypes { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public IEnumerable<TCampaignRuleOutput> GetCampaign(TCampaignRuleInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        return GetCampaign(input, out _);
    }

    public IEnumerable<TCampaignRuleOutput> GetCampaign(TCampaignRuleInput input, out List<CampaignRuleSet> ruleSets)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        try
        {
            var predicates = RuleManager.PredicateRuleSets(_ruleProvider, input);
            ruleSets = new List<CampaignRuleSet>();

            if (predicates == null || !predicates.Any())
            {
                var emptyResult = Activator.CreateInstance(typeof(TCampaignRuleOutput)) as TCampaignRuleOutput;
                return emptyResult != null ? new List<TCampaignRuleOutput> { emptyResult } : new List<TCampaignRuleOutput>();
            }

            var results = new List<TCampaignRuleOutput>();
            var orderDiscountPredicate = predicates.Values
                .Where(pr => pr.CampaignTypes == (int)CampaignTypes.DiscountCampaign)
                .OrderByDescending(cr => cr.Priority)
                .ThenBy(c => c.CreateDate)
                .FirstOrDefault();
            if (orderDiscountPredicate != null)
            {
                results.Add(orderDiscountPredicate.ResultRule.Invoke(input));
                ruleSets.Add(orderDiscountPredicate);
            }

            var orderPredicates = predicates.Values
                .Where(pr => pr.CampaignTypes == (int)CampaignTypes.ProductGiftCampaign)
                .OrderByDescending(cr => cr.Priority)
                .ThenBy(c => c.CreateDate);
            foreach (var orderPredicate in orderPredicates)
            {
                results.Add(orderPredicate.ResultRule.Invoke(input));
                ruleSets.Add(orderPredicate);
            }

            return results;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error evaluating campaigns for module {ModuleId}", _moduleId);
            throw;
        }
    }

    public IDictionary<string, ITravelProduct> UseCampaign(string productKey, string campaignCode, IDictionary<string, ITravelProduct> productsInTransaction)
    {
        if (string.IsNullOrWhiteSpace(productKey))
            throw new ArgumentException("Product key cannot be null or empty.", nameof(productKey));
        if (string.IsNullOrWhiteSpace(campaignCode))
            throw new ArgumentException("Campaign code cannot be null or empty.", nameof(campaignCode));
        if (productsInTransaction == null)
            throw new ArgumentNullException(nameof(productsInTransaction));

        if (!productsInTransaction.TryGetValue(productKey, out var product))
            return productsInTransaction;

        if (product.CampaignInformations == null)
            return productsInTransaction;

        var campaignInfo = product.CampaignInformations.Values
            .FirstOrDefault(ci => ci.CampaignTypes == CampaignTypes.ProductGiftCampaign && ci.Code == campaignCode);
        if (campaignInfo == null || campaignInfo.Used)
            return productsInTransaction;

        product.TotalPrice -= campaignInfo.TotalDiscount;
        campaignInfo.Used = true;

        return productsInTransaction;
    }

    public List<AvailableCampaign> GetAvailableCampaigns(string productKey, IDictionary<string, ITravelProduct> productsInTransaction, TCampaignRuleInput input)
    {
        if (string.IsNullOrWhiteSpace(productKey))
            throw new ArgumentException("Product key cannot be null or empty.", nameof(productKey));
        if (productsInTransaction == null)
            throw new ArgumentNullException(nameof(productsInTransaction));
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var availableCampaigns = new List<AvailableCampaign>();
        var repo = _ruleProvider.ServiceProvider.GetRequiredService<ICampaignRepository>();

        var ruleSetOrdered = RuleManager.GetRuleSets(_ruleProvider).Values.OrderByDescending(r => r.Priority);

        foreach (var ruleSet in ruleSetOrdered)
        {
            if (ruleSet.CampaignTypes != (int)CampaignTypes.ProductGiftCampaign)
                continue;
            if (ruleSet.UsageRule == null || !ruleSet.UsageRule.Invoke(input))
                continue;

            foreach (var product in productsInTransaction.Values)
            {
                if (product.CampaignInformations == null)
                    continue;

                var campaignResult = ruleSet.ResultRule.Invoke(input);
                var existingInfo = product.CampaignInformations.Values.FirstOrDefault(ci => ci.Code == ruleSet.Code);
                if (existingInfo == null)
                    continue;

                existingInfo.TotalDiscount = campaignResult.TotalDiscount;

                if (ruleSet.Quota != 0)
                {
                    var quotaState = repo.CheckCampaignQuota(ruleSet.Quota ?? 0, ruleSet.Id);
                    if (!quotaState)
                    {
                        availableCampaigns.Add(new AvailableCampaign());
                        continue;
                    }
                }

                availableCampaigns.Add(BuildAvailableCampaign(productKey, product.Key, ruleSet, campaignResult));
            }
        }

        return availableCampaigns;
    }

    public void DeleteCampaign(string campaignCode, IDictionary<string, ITravelProduct> productsInTransaction)
    {
        if (string.IsNullOrWhiteSpace(campaignCode))
            throw new ArgumentException("Campaign code cannot be null or empty.", nameof(campaignCode));
        if (productsInTransaction == null)
            throw new ArgumentNullException(nameof(productsInTransaction));

        foreach (var product in productsInTransaction.Values)
        {
            if (product.CampaignInformations == null)
                continue;

            var basketCampaigns = product.CampaignInformations.Values
                .Where(ci => ci.CampaignTypes == CampaignTypes.ProductGiftCampaign && ci.Code == campaignCode)
                .ToList();
            foreach (var basketCampaign in basketCampaigns)
            {
                if (basketCampaign.Used)
                {
                    product.TotalPrice += basketCampaign.TotalDiscount;
                    basketCampaign.Used = false;
                }
            }
        }
    }

    private static AvailableCampaign BuildAvailableCampaign(
        string productKey,
        string targetProductKey,
        CampaignRuleSet ruleSet,
        TCampaignRuleOutput campaignResult)
    {
        var availableCampaign = new AvailableCampaign
        {
            CampaignCode = ruleSet.Code,
            CampaignName = ruleSet.Name ?? string.Empty,
            CampaignType = (CampaignIncludes?)ruleSet.CampaignTypes,
            ProductKey = productKey
        };

        var targetResult = new CampaignTargetResult
        {
            ProductKey = targetProductKey,
            Discount = campaignResult.TotalDiscount
        };
        availableCampaign.TargetResults.Add(targetResult);
        return availableCampaign;
    }
}
