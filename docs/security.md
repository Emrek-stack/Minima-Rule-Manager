---
title: Security
layout: default
---

# Guvenlik

RuleEngine, kurumsal kullanim icin guvenlik ve kontrol noktalarini saglar. Bu sayfa, guvenlik yaklasimini ve operasyonel onerileri ozetler.

## Guvenlik Politikasi

Guvenlik aciklari icin `SECURITY.md` dokumanina bakin. Sorumlu bildirim surecini takip edin.

## Expression Guvenligi

- Expression validasyonu derleme oncesinde yapilir.
- Kurallar yalnizca tanimlanan Input/Output modelleri ile calisir.
- Custom evaluator yazarken izolasyon ve whitelist yaklasimi tavsiye edilir.

## Audit ve Izlenebilirlik

- Rule execution audit tablosu, kim/ne zaman/sonuc bilgisi icerecek sekilde tasarlanmistir.
- Log retention politikasini kurumunuzun reguleasyonuna gore belirleyin.

## Tavsiyeler

- Uretimde DEBUG_RULES kapali tutun.
- Kural degisikliklerinde versiyonlama kullanin ve rollback planlayin.
- Kritik kampanyalar icin quota ve usage kontrollerini aktif edin.
