# VAULTMASK: ÜRÜN VİZYONU VE MİMARİ TASARIM DOKÜMANI (SDD)

## 1. ÜRÜNÜN ÖZÜ VE VİZYONU
**Ürün Adı:** VaultMask
**Slogan:** "Verinizi Buluta Çıkarmadan, Kendi Sınırlarınızda Anonimleştirin."
**Vizyon:** Geliştiricilerin ve veri analistlerinin, canlı veritabanı (Production DB) yedeğini kendi bilgisayarlarına alırken KVKK/GDPR ihlali yapmalarını engelleyen, askeri standartlarda güvenli, tamamen çevrimdışı (offline) çalışan bir masaüstü/CLI aracı olmak.

## 2. PAZAR VE HEDEF KİTLE (KİME SATIYORUZ?)
- **Hedef Kitle:** Fintek, Sağlık Teknolojileri (Healthtech) ve B2B SaaS geliştiren şirketlerin CTO'ları, Uyumluluk Yöneticileri (Compliance Officers) ve Kıdemli Geliştiricileri.
- **Çözülen Acı Noktası (Pain Point):** Şirketler yerel testler veya AI eğitimi yapmak için gerçek verilere ihtiyaç duyar. Ancak canlı veritabanında TC Kimlik No, Kredi Kartı, Şifre gibi bilgiler (PII) vardır. VaultMask, bu verileri saniyeler içinde "anlamlı ama sahte" verilerle değiştirerek şirketleri milyonlarca liralık hukuki cezalardan kurtarır.

## 3. KAPSAM VE SINIRLAR (IN-SCOPE vs OUT-OF-SCOPE)
Ajanların odaklarını kaybetmemesi için projenin kesin sınırları şunlardır:

**MVP'de OLACAKLAR (In-Scope):**
- **Çalışma Ortamı:** Tamamen çevrimdışı (İnternet bağlantısı gerektirmez). API veya SaaS DEĞİLDİR.
- **Arayüz:** Şık ve kullanıcı dostu bir Komut Satırı Arayüzü (CLI). Renkli çıktılar, ilerleme çubukları (Progress Bar).
- **Veritabanı Desteği:** Yalnızca MS SQL Server (`Microsoft.Data.SqlClient`).
- **Maskeleme Türleri:** İsim, Email, Telefon, TC Kimlik, Kredi Kartı ve Rastgele Metin/Tarih.

**MVP'de OLMAYACAKLAR (Out-of-Scope - Kesinlikle Kodlanmayacak):**
- PostgreSQL, MySQL, Oracle veya NoSQL desteği (Daha sonra eklenecek).
- Grafiksel Kullanıcı Arayüzü (WPF, MAUI, React vb.).
- Veri buluta yükleme veya Telemetri gönderme.
- Yapay zeka ile otomatik kolon tahmini.

## 4. TEKNİK YIĞIN VE MİMARİ (TECH STACK)
- **Platform:** .NET 10 (C# 14 özelliklerinden faydalanılacak).
- **Mimari:** Clean Architecture (Domain, Application, Infrastructure, Presentation/CLI).
- **Veri Erişimi:** `Dapper` (Yüksek performanslı Micro-ORM). `Entity Framework Core` kesinlikle YASAKTIR.
- **Sahte Veri Üreticisi:** `Bogus` kütüphanesi.
- **Görsel CLI Kütüphanesi:** Terminalde şık bir deneyim için `Spectre.Console` kullanılacaktır.

## 5. GÖRSEL VE UX REFERANSI (CLI İÇİN)
Kullanıcı uygulamayı açtığında terminalde sıkıcı siyah/beyaz yazılar görmemeli. `Spectre.Console` kullanılarak şu deneyim sunulmalıdır:
1. Başlangıçta büyük, renkli bir ASCII Art logo ("VaultMask").
2. Kullanıcıdan SQL Connection String istenirken şifre alanı gizlenmeli (Masked input).
3. Veritabanı tarandığında bir Tablo Ağacı (Tree view) gösterilmeli.
4. Maskeleme işlemi başlarken, hangi tablonun işlendiğini gösteren akıcı bir İlerleme Çubuğu (Progress Bar) bulunmalı.

## 6. PROJE YOL HARİTASI (ROADMAP)
Süreci adım adım yürüteceğiz. Bir adımı tamamlamadan diğerine geçmeyin.
- **Aşama 1 (Şu Anki Aşama):** Domain katmanının ve Repository Interface'lerinin inşası.
- **Aşama 2:** Infrastructure katmanında Dapper ile SQL Server bağlantısının kurulması.
- **Aşama 3:** Application katmanında Bogus ile veri maskeleme (Data Masking) servislerinin yazılması.
- **Aşama 4:** Cli katmanında Spectre.Console ile görsel arayüzün bağlanması.

---

### GÖREV #1 (MICRO-TASK 1): TEMELİ ATMA
Sayın Orkestra Şefi, vizyonumuzu okudun. Şimdi Aşama 1'i başlatıyoruz. Lütfen aşağıdaki adımları sırasıyla uygula ve başka hiçbir dosyaya dokunma:

1. `VaultMask.Domain` projesinde `Interfaces` klasörü oluştur.
2. İçerisine `IDatabaseRepository.cs` adlı dosyayı oluştur.
3. Bu interface, SQL Server'dan veritabanı tablolarını (`GetTablesAsync`), kolon bilgilerini (`GetColumnsAsync`) getirecek ve güncellenmiş maskeli veriyi veritabanına yazacak (`UpdateDataAsync`) metotları barındırsın. Sadece C# 14 standartlarında, asenkron ve temiz bir şekilde bu interface'i kodla.