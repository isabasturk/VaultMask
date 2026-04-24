using VaultMask.Application.Models;

namespace VaultMask.Cli.Mappers;

/// <summary>
/// Provides logic to automatically suggest masking types based on column names.
/// </summary>
public static class HeuristicMapper
{
    /// <summary>
    /// Suggests a masking type for a given column name.
    /// </summary>
    /// <param name="columnName">The name of the database column.</param>
    /// <returns>A suggested <see cref="MaskingType"/> or null if no suggestion matches.</returns>
    public static MaskingType? Suggest(string columnName)
    {
        var lowerName = columnName.ToLowerInvariant();

        if (lowerName.Contains("email") || lowerName.Contains("mail"))
            return MaskingType.Email;

        if (lowerName.Contains("phone") || lowerName.Contains("tel") || lowerName.Contains("gsm"))
            return MaskingType.PhoneNumber;

        if (lowerName.Contains("fullname") || lowerName.Contains("full_name") || lowerName.Equals("name"))
            return MaskingType.FullName;

        if (lowerName.Contains("firstname") || lowerName.Contains("first_name") || lowerName.Equals("ad") || lowerName.Equals("adi"))
            return MaskingType.FirstName;

        if (lowerName.Contains("lastname") || lowerName.Contains("last_name") || lowerName.Contains("soyad") || lowerName.Equals("soyadi"))
            return MaskingType.LastName;

        if (lowerName.Contains("company") || lowerName.Contains("şirket") || lowerName.Contains("firm") || lowerName.Contains("kurum"))
            return MaskingType.CompanyName;

        // General catch for names that weren't caught by specific rules
        if (lowerName.Contains("name") || lowerName.Contains("isim") || lowerName.Contains("unvan"))
            return MaskingType.FullName;

        if (lowerName.Contains("tc") || lowerName.Contains("tckn") || lowerName.Contains("identit"))
            return MaskingType.TCKimlik;

        if (lowerName.Contains("card") || lowerName.Contains("kart") || lowerName.Contains("ccv"))
            return MaskingType.CreditCard;

        if (lowerName.Contains("address") || lowerName.Contains("adres"))
            return MaskingType.Address;

        if (lowerName.Contains("date") || lowerName.Contains("tarih"))
            return MaskingType.RandomDate;

        if (lowerName.Contains("description") || lowerName.Contains("metin") || lowerName.Contains("note"))
            return MaskingType.RandomText;

        return null;
    }
}
