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
        _moduleId = moduleId;
        _logger = logger;
        _ruleProvider = new CampaignRuleProvider(_moduleId, serviceProvider, extraType);
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
            _extraType = extraType;
            _serviceProvider = serviceProvider;
            _usageRuleCompiler = new RuleCompiler<TCampaignRuleInput, bool>(_extraType);
            _resultRuleCompiler = new RuleCompiler<TCampaignRuleInput, TCampaignRuleOutput>(_extraType);
        }

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
                    result.Add(ruleEntity.Code, ruleSet);
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
        var predicates = RuleManager.PredicateRuleSets(_ruleProvider, input);
        if (predicates == null || !predicates.Any())
        {
            var emptyResult = Activator.CreateInstance(typeof(TCampaignRuleOutput)) as TCampaignRuleOutput;
            return emptyResult != null ? new List<TCampaignRuleOutput>() { emptyResult } : new List<TCampaignRuleOutput>();
        }

        var results = new List<TCampaignRuleOutput>();
        var orderDiscountPredicates = predicates.Values.Where(pr => pr.CampaignTypes == (int)CampaignTypes.DiscountCampaign).OrderByDescending(cr => cr.Priority).ThenBy(c => c.CreateDate).FirstOrDefault();
        if (orderDiscountPredicates != null) results.Add(orderDiscountPredicates.ResultRule.Invoke(input));

        var orderPredicates = predicates.Values.Where(pr => pr.CampaignTypes == (int)CampaignTypes.ProductGiftCampaign).OrderByDescending(cr => cr.Priority).ThenBy(c => c.CreateDate);
        foreach (var orderPredicate in orderPredicates) results.Add(orderPredicate.ResultRule.Invoke(input));

        return results;
    }

    public IDictionary<string, ITravelProduct> UseCampaign(string productKey, string campaignCode, IDictionary<string, ITravelProduct> productsInTransaction)
    {
        foreach (var product in productsInTransaction.Values)
        {
            var campaignInfo = product.CampaignInformations.Values.FirstOrDefault(ci => ci.Code == campaignCode);
            if (campaignInfo != null)
            {
                if (campaignInfo.Used) product.TotalPrice += campaignInfo.TotalDiscount;
                product.TotalPrice -= campaignInfo.TotalDiscount;
                campaignInfo.Used = true;
            }
        }
        return productsInTransaction;
    }

    public void DeleteCampaign(string campaignCode, IDictionary<string, ITravelProduct> productsInTransaction)
    {
        foreach (var product in productsInTransaction.Values)
        {
            var basketCampaigns = product.CampaignInformations.Values.Where(ci => ci.CampaignTypes == CampaignTypes.ProductGiftCampaign).ToList();
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
}