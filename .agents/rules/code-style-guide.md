---
trigger: always_on
---

# VAULTMASK - MİMARİ VE GELİŞTİRME KURALLARI

Sen kıdemli bir .NET 10 Yazılım Mimarı ve ajanısın. Görevin, Clean Architecture prensiplerine mutlak surette uyarak çevrimdışı bir B2B veritabanı anonimleştirme aracı geliştirmektir.

## 1. TEKNOLOJİ YIĞINI (MUTLAK KURALLAR)
- **Framework:** .NET 8 veya 9 (C#).
- **ORM:** SADECE `Dapper` kullanılacaktır. `Entity Framework Core` KESİNLİKLE YASAKTIR.
- **Veritabanı Sağlayıcısı:** Sadece `Microsoft.Data.SqlClient` (SQL Server).
- **Sahte Veri:** Veri maskeleme işlemleri için `Bogus` kütüphanesi kullanılacaktır.

## 2. CLEAN ARCHITECTURE (BAĞIMLILIK YÖNÜ)
- **Domain:** Sadece Interface'ler (`IRepository`), Entity'ler ve Exception'lar. Dış kütüphane alamaz.
- **Application:** İş kuralları (MaskingService). Sadece Domain'i referans alır.
- **Infrastructure:** Dapper sorguları ve SQL bağlantıları. Domain ve Application'ı referans alır.
- **Cli:** Console arayüzü. Infrastructure ve Application'ı referans alır.
*KURAL: Oklar daima içe dönüktür. Domain katmanı asla Infrastructure'ı bilmeyecek.*

## 3. KODLAMA STANDARTLARI
- Tüm DB işlemleri asenkron (`async/await`) olmalıdır.
- Kullanıcı onay vermeden çalışan mevcut bir metodu veya dosyayı SİLME, üzerine yazma.
- Hataları (Exception) yutma, anlamlı fırlat.