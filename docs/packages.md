---
title: Packages
layout: default
---

# Paketler ve Suresel Destek

RuleEngine paket ailesi birden fazla NuGet paketi olarak yayinlanir. Her paket .NET 8/9/10 hedefler ve kurumsal senaryolara uygun versiyonlama politikasi ile gelir.

## Paket Listesi

| Paket | Amac | NuGet |
| --- | --- | --- |
| `Minima.RuleEngine.Core` | Kural motoru cekirdegi | https://www.nuget.org/packages/Minima.RuleEngine.Core/ |
| `Minima.RuleEngine.Sqlite` | SQLite kalicilik ve audit | https://www.nuget.org/packages/Minima.RuleEngine.Sqlite/ |
| `Minima.CampaignEngine.Core` | Kampanya motoru | https://www.nuget.org/packages/Minima.CampaignEngine.Core/ |

## Surum ve Uyumluluk

- Mevcut paket surumu: `1.1.11`
- Desteklenen frameworkler: `net8.0`, `net9.0`, `net10.0`
- Minimum bagimliliklar:
  - Microsoft.CodeAnalysis.CSharp.Scripting 4.14.0
  - Microsoft.Extensions.* (8/9/10 uyumlu)

## Versiyonlama Politikasi

- SemVer tabanli: `MAJOR.MINOR.PATCH`
- MINOR surumlerde yeni ozellikler, PATCH surumlerde hata duzeltmeleri yer alir.
- Release otomasyonu `release.sh` ve `release.ps1` ile yapilir.

## Paket Secim Rehberi

- Sadece in-memory kural calistirma: `Minima.RuleEngine.Core`
- SQLite ile kalicilik ve audit: `Minima.RuleEngine.Sqlite`
- Kampanya kurallari: `Minima.CampaignEngine.Core`

## Yukseltme Notlari

- Yeni MINOR surumden once test ortaminda migration ve audit tablolarini dogrulayin.
- Kampanya motorunda yeni alanlar eklendiyse, custom repository implementasyonlarini kontrol edin.

Daha fazla bilgi: `release-process.html`
