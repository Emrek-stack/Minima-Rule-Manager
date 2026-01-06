---
title: Architecture
layout: default
---

# Mimari Genel Bakis

RuleEngine, core rule execution, persistence ve kampanya katmanlarindan olusan moduler bir mimariye sahiptir.

## Bilesenler

- RuleEngine.Core: Kural derleme, calistirma, metadata, versioning
- RuleEngine.Sqlite: EF Core ile kalicilik, audit logging
- CampaignEngine.Core: Kampanya secimi, onceliklendirme, uygulama

## Veri Akisi

```text
Input -> RuleProvider -> RuleManager -> RuleSet -> CompiledRule -> Output
```

Kampanya tarafinda:

```text
CampaignInput -> CampaignManager -> RuleProvider -> RuleSet -> CampaignOutput
```

## Modul Sinirlari

- Core katman, evaluator ve repository arayuzleri ile soyutlanir.
- Sqlite paketi, repository implementasyonlarini ve migrationlarini barindirir.
- CampaignEngine, RuleEngine.Core uzerine insa edilir ve kendi repository soyutlamasina sahiptir.

## Genisletilebilirlik Noktalari

- `IRuleEvaluator` ile farkli expression motorlari
- `IRuleRepository` ve `IAuditRepository` ile custom persistence
- `ICampaignRepository` ile kampanya kaynagi

## Tasarim Zamani Metadata

Kural editörlerinin kural katalogu, parametre tanimlari ve kategorilerle calismasi icin metadata saglanir.

Daha fazla detay: `ruleengine-core.html`
