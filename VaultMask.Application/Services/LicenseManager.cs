using System.Text.RegularExpressions;
using VaultMask.Application.Interfaces;

namespace VaultMask.Application.Services;

public sealed class LicenseManager : ILicenseManager
{
    private readonly string _licenseFilePath;
    private const string LicensePattern = @"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$";

    public LicenseManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var vaultMaskDir = Path.Combine(appData, "VaultMask");
        
        if (!Directory.Exists(vaultMaskDir))
        {
            Directory.CreateDirectory(vaultMaskDir);
        }

        _licenseFilePath = Path.Combine(vaultMaskDir, ".license");
    }

    public bool IsPremium()
    {
        if (!File.Exists(_licenseFilePath)) return false;

        var key = File.ReadAllText(_licenseFilePath).Trim();
        return Regex.IsMatch(key, LicensePattern);
    }

    public void Activate(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Lisans anahtarı boş olamaz.");

        if (!Regex.IsMatch(key.ToUpperInvariant(), LicensePattern))
            throw new ArgumentException("Geçersiz lisans formatı. Beklenen: XXXX-YYYY-ZZZZ");

        File.WriteAllText(_licenseFilePath, key.ToUpperInvariant());
    }
}
