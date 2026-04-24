namespace VaultMask.Application.Interfaces;

public interface ILicenseManager
{
    bool IsPremium();
    void Activate(string key);
}
