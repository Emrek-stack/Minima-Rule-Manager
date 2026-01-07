# Feature Matrix

This page summarizes all features by package.

## RuleEngine.Core

- Roslyn-based C# expression compilation
- Expression tree and statement body templates
- RuleCompiler compilation and caching
- RuleSet and MultiResultRuleSet flows
- RuleManager + IRuleProvider provider flow
- Design-time metadata catalog (categories, parameters, formats)
- Syntax validation and error reporting (RuleSyntaxError)
- Runtime error wrapping (RuleRuntimeException)
- Rule versioning and status model (RuleStatus)
- DEBUG_RULES PDB output (debug)
- Rule execution result model (RuleExecutionResult)

## RuleEngine.Sqlite

- EF Core persistence
- Rule / RuleVersion / RuleParameter tables
- Rule execution audit logging
- Version activation and rollback
- Parameter serialization (System.Text.Json)
- Migration and seeding flow
- IRuleRepository and IAuditRepository implementations

## CampaignEngine.Core

- Campaign layer built on RuleEngine.Core
- CampaignManager selection flow
- Discount, ProductGift, GiftCoupon types
- Predicate/Result/Usage rules
- Campaign prioritization
- Quota enforcement
- Product-level campaign availability
- UseCampaign / DeleteCampaign basket flow
- Available campaign calculation
- InMemoryCampaignRepository
- Memory cache provider
- Demo seed helper (CampaignSeed)
- Price model (ISO 4217, operator overloads, JSON converter)
