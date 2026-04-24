using VaultMask.Application.Models;

namespace VaultMask.Application.Interfaces;

/// <summary>
/// Orchestrates the data masking process for database tables.
/// </summary>
public interface IMaskingService
{
    /// <summary>
    /// Mask data in a specific table based on the provided rules.
    /// </summary>
    /// <param name="schema">Database schema.</param>
    /// <param name="tableName">Target table name.</param>
    /// <param name="primaryKeyColumn">Primary key column for identifying rows.</param>
    /// <param name="rules">Collection of masking rules.</param>
    /// <param name="batchSize">Number of rows to process in each batch for memory efficiency.</param>
    /// <returns>A task representing the operation.</returns>
    Task MaskTableAsync(
        string schema, 
        string tableName, 
        string primaryKeyColumn, 
        IEnumerable<MaskingRule> rules, 
        int batchSize = 1000);
}
