# VaultMask 🛡️

*High-Performance B2B Database Anonymization CLI Tool for SQL Server*

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net&logoColor=white)](#)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](#)
[![Spectre.Console](https://img.shields.io/badge/CLI-Spectre.Console-blue)](#)

VaultMask is an enterprise-grade Command Line Interface (CLI) tool designed to securely mask Personally Identifiable Information (PII) within your SQL Server databases. Built with a strict adherence to **Clean Architecture** and powered by **Dapper**, it performs in-place anonymization with microscopic resource footprints.

---

## 🌟 Features

- **Blazing Fast In-Place Masking:** Updates records directly in the SQL database using asynchronous chunks, completely eliminating RAM overload and OutOfMemory exceptions.
- **Smart Heuristic Mapping:** Automatically detects columns containing sensitive data like `Name`, `Surname`, `Email`, `Address`, `Phone`, `CompanyName` and flags them for relevant algorithmic masking.
- **T.C. Kimlik Algorithm (Premium):** Cryptographically generates structurally valid, Modulus 10-compliant Turkish National ID numbers.
- **Flawless UX & Memory:** Remembers your last successful connection string and supports dynamic interactive validation loops using `Spectre.Console`.
- **Multi-Language (i18n):** Automatically translates the CLI environment to Turkish or English based on the system locale.
- **Freemium Constraints:** Built-in license engine allowing up to 100 rows per table to be masked for free, with an upgrade path for unlimited enterprise usage.

---

## 🏗️ Architecture

The project strictly follows the **Dependency Rule** of Clean Architecture to ensure absolute separation between database queries, business rules, and the UI layer.

* **Domain:** Core entities (`TableInfo`, `ColumnInfo`), Enums (`MaskingType`), and pure Exceptions. Contains *zero* external dependencies.
* **Application:** Orchestration (`MaskingService`, `TCKimlikGenerator`, `LicenseManager`). Contains business rules.
* **Infrastructure:** Dapper-powered SQL queries (`SqlServerRepository`).
* **CLI (Presentation):** `Spectre.Console` orchestration, interactive prompts, and `HeuristicMapper`.

---

## 🚀 Quick Start / User Manual (Kullanım Kılavuzu)

> [!CAUTION]
> **DO NOT USE ON PRODUCTION DATABASES!**
> VaultMask is designed to modify and anonymize data **in-place**. Running this tool on a live, production database will permanently overwrite genuine customer data with fake equivalents. **ALWAYS** run this tool on a copy, clone, or staging version of your database.

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- SQL Server Database (Local or Remote)

### Installation & Building

Since this is a source-code repository, you need to compile the tool yourself. You have two options:

**Option A: For Developers (Direct Run)**
```bash
git clone https://github.com/yourusername/vaultmask.git
cd VaultMask
dotnet run --project VaultMask.Cli/VaultMask.Cli.csproj -c Release
```

**Option B: For End Users (Create a Standalone `.exe`)**
If you want to create a single `.exe` file that doesn't require .NET SDK installed on the target machine:
```bash
git clone https://github.com/yourusername/vaultmask.git
cd VaultMask
dotnet publish VaultMask.Cli/VaultMask.Cli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
After publishing, your executable will be located at:
`VaultMask.Cli/bin/Release/net10.0/win-x64/publish/VaultMask.Cli.exe`

### Running the App
Once built/published, execute the CLI:

1. **Connection String:** The application will prompt you for a `Connection String`. Enter your SQL Server details (e.g., `Server=.;Database=TestDB;Integrated Security=True;TrustServerCertificate=True;`). The tool will remember this for your next session!
2. **Table Selection:** Use the **Spacebar** to select which tables you want to anonymize, and press **Enter** to confirm.
3. **Column Validation:** For each table, verify the intelligently proposed masking columns.
4. **Execution:** Review the summary and watch the progress bar as your data is safely anonymized chunk by chunk.

### Activating Premium
To unlock row limitations and the T.C. Kimlik generation module, activate your product key via CLI:
```bash
VaultMask.Cli.exe activate XXXX-YYYY-ZZZZ
```

---

## 🇹🇷 Türkçe Açıklama

> [!CAUTION]
> **CANLI VERİTABANINDA (PRODUCTION) KULLANMAYIN!**
> VaultMask verileri **doğrudan hedefin (tablonun) üzerinde** değiştirir ve anonimleştirir. Bu aracı canlı müşteri verilerinizin olduğu sunucuda çalıştırırsanız, tüm gerçek verileriniz kalıcı olarak sahte verilerle değişecektir! **HER ZAMAN** veritabanınızın bir yedeğini alın veya bu aracı staging/test sunucusuna kurduğunuz bir kopyanın üzerinde çalıştırın.

VaultMask, SQL Server veritabanlarınızdaki Kişisel Verileri (KVKK/GDPR uyumlu şekilde) güvenle maskelemek (anonimleştirmek) için geliştirilmiş, kurumsal standartlarda bir .NET 10 CLI (Komut Satırı) aracıdır.

**Öne Çıkan Faydaları:**
- **Mükemmel Mimari:** Entity Framework hantallığı olmadan, sadece Dapper ile saf ve yüksek performanslı SQL iletişimi.
- **Kendi Karar Veren Sistem:** Kolon adlarından yola çıkarak içindeki verinin "E-Posta" mı yoksa "Şirket Adı" mı olduğunu anlar ve en uygun rastgele sahte veri formatını sizin yerinize seçer.
- **Katı Sistem Güvenliği:** Bir hata olduğunda saatlerce C# yığın izi (Stack Trace) okumazsınız; VaultMask sizi neyin yanlış gittiğine dair temiz ve anlaşılır hata mesajlarıyla yönlendirir.

**Nasıl Çalıştırılır? (.exe Oluşturma)**
Bu depo açık kaynak kodları içerdiğinden doğrudan bir `.exe` indiremezsiniz. Projeyi kendi makinenizde derleyip tek bir `.exe` haline getirmek için şu komutu kullanın:
```bash
dotnet publish VaultMask.Cli/VaultMask.Cli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Bu işlem sonrasında `VaultMask.Cli\bin\Release\net10.0\win-x64\publish\` dizini içinde çalışan uygulamanıza (`VaultMask.Cli.exe`) ulaşabilirsiniz.

Açık kaynak dünyasına armağan edilmiş bu araç ile projelerinizdeki canlı veritabanı kopyalarını, güvenlik endişesi yaşamadan test, staging veya geliştirme ortamlarına taşıyabilirsiniz. 

---

## 🤝 Contributing
Contributions, issues, and feature requests are always welcome! Feel free to check the [issues page](#).

## 📄 License
Distributed under the MIT License. See `LICENSE` for more information.
