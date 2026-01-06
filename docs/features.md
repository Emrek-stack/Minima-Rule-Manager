---
title: Feature Matrix
layout: default
---

# Feature Matrix

Bu sayfa, paketlerin sundugu tum ozellikleri tek yerde toplar.

## RuleEngine.Core

- Roslyn tabanli C# expression derleme
- Expression tree ve statement body templating
- RuleCompiler ile cache edilebilir derleme modeli
- RuleSet ve MultiResultRuleSet ile predicate/result akisi
- RuleManager + IRuleProvider provider akisi
- Design-time metadata katalogu (kategori, parametre, format)
- Syntax dogrulama ve hata raporlama (RuleSyntaxError)
- Rule runtime hata sarmalama (RuleRuntimeException)
- Rule versioning ve durum modeli (RuleStatus)
- DEBUG_RULES ile PDB cikisi (debug)
- Rule execution sonuc modeli (RuleExecutionResult)

## RuleEngine.Sqlite

- EF Core tabanli kalicilik
- Rule / RuleVersion / RuleParameter tabloları
- Rule execution audit logging
- Version activation ve rollback destekleri
- Parameter serilestirme (System.Text.Json)
- Migration ve seeding akisi
- IRuleRepository ve IAuditRepository implementasyonlari

## CampaignEngine.Core

- RuleEngine.Core uzerine kampanya katmani
- CampaignManager ile kampanya secimi
- Discount, ProductGift, GiftCoupon kampanya tipleri
- Predicate/Result/Usage kurallari
- Kampanya onceliklendirme (priority)
- Kota kontrolu (quota)
- Product-level kampanya uygulanabilirligi
- UseCampaign / DeleteCampaign sepet akisi
- Available campaign hesaplama
- InMemoryCampaignRepository
- Memory cache provider
- Demo seed helper (CampaignSeed)
- Price model (ISO 4217, operator overloads, JSON converter)
