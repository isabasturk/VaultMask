using Bogus;
using VaultMask.Application.Interfaces;
using VaultMask.Application.Models;
using VaultMask.Application.Generators;
using VaultMask.Domain.Interfaces;

namespace VaultMask.Application.Services;

/// <summary>
/// Core masking service implementation. 
/// Orchestrates data streaming, fake data generation, and batch updates.
/// </summary>
public sealed class MaskingService(IDatabaseRepository repository, ILicenseManager licenseManager) : IMaskingService
{
    private readonly Faker _faker = new("tr"); // Default to Turkish context for localized strings

    public async Task MaskTableAsync(
        string schema, 
        string tableName, 
        string primaryKeyColumn, 
        IEnumerable<MaskingRule> rules, 
        int batchSize = 1000)
    {
        var isPremium = licenseManager.IsPremium();
        var ruleList = rules.ToList();
        
        // Premium check for T.C. Kimlik
        if (!isPremium && ruleList.Any(r => r.Type == MaskingType.TCKimlik))
        {
            throw new InvalidOperationException("TC Kimlik modül 10 maskeleme algoritması sadece Premium sürümde mevcuttur.");
        }

        var columnsToRead = ruleList.Select(r => r.ColumnName).ToList();
        
        // Ensure Primary Key is included in the read set
        if (!columnsToRead.Any(c => c.Equals(primaryKeyColumn, StringComparison.OrdinalIgnoreCase)))
        {
            columnsToRead.Add(primaryKeyColumn);
        }

        var currentBatch = new List<IDictionary<string, object>>();
        var rowCount = 0;

        await foreach (var row in repository.GetDataAsync(schema, tableName, columnsToRead))
        {
            // Freemium constraint: Limit to first 100 rows per table
            if (!isPremium && rowCount >= 100) break;

            // Create a new dictionary for the masked row 
            // We must preserve the Primary Key value
            var maskedRow = new Dictionary<string, object>
            {
                [primaryKeyColumn] = row[primaryKeyColumn]
            };

            foreach (var rule in ruleList)
            {
                maskedRow[rule.ColumnName] = GenerateFakeValue(rule.Type);
            }

            currentBatch.Add(maskedRow);
            rowCount++;

            if (currentBatch.Count >= batchSize)
            {
                await repository.UpdateDataBatchAsync(schema, tableName, primaryKeyColumn, currentBatch);
                currentBatch.Clear();
            }
        }

        // Process final remaining rows
        if (currentBatch.Count > 0)
        {
            await repository.UpdateDataBatchAsync(schema, tableName, primaryKeyColumn, currentBatch);
        }
    }

    private object GenerateFakeValue(MaskingType type)
    {
        return type switch
        {
            MaskingType.FullName => _faker.Name.FullName(),
            MaskingType.FirstName => _faker.Name.FirstName(),
            MaskingType.LastName => _faker.Name.LastName(),
            MaskingType.Email => _faker.Internet.Email(),
            MaskingType.PhoneNumber => _faker.Phone.PhoneNumber("05#########"),
            MaskingType.TCKimlik => TCKimlikGenerator.Generate(),
            MaskingType.CreditCard => _faker.Finance.CreditCardNumber(),
            MaskingType.RandomText => _faker.Lorem.Sentence(),
            MaskingType.RandomDate => _faker.Date.Past(5),
            MaskingType.Address => $"{_faker.Address.StreetName()} No:{_faker.Address.BuildingNumber()}, {_faker.Address.City()}",
            MaskingType.CompanyName => _faker.Company.CompanyName(),
            _ => _faker.Random.Words(3)
        };
    }
}
