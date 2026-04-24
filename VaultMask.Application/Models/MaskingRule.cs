namespace VaultMask.Application.Models;

/// <summary>
/// Supported data masking categories.
/// </summary>
public enum MaskingType
{
    FullName,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    TCKimlik,
    CreditCard,
    RandomText,
    RandomDate,
    Address,
    CompanyName
}

/// <summary>
/// Defines which masking logic to apply to a specific column.
/// </summary>
/// <param name="ColumnName">Target column name.</param>
/// <param name="Type">The type of masking to apply.</param>
public sealed record MaskingRule(string ColumnName, MaskingType Type);
