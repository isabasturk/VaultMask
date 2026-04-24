using VaultMask.Domain.Entities;

namespace VaultMask.Domain.Interfaces;

/// <summary>
/// Defines the core repository for interacting with SQL Server schema and data.
/// </summary>
public interface IDatabaseRepository
{
    /// <summary>
    /// Retrieves all user tables available in the current database.
    /// </summary>
    /// <returns>A collection of <see cref="TableInfo"/> results.</returns>
    Task<IEnumerable<TableInfo>> GetTablesAsync();

    /// <summary>
    /// Retrieves metadata for all columns in the specified table.
    /// </summary>
    /// <param name="schema">The table schema.</param>
    /// <param name="tableName">The table name.</param>
    /// <returns>A collection of <see cref="ColumnInfo"/> results.</returns>
    Task<IEnumerable<ColumnInfo>> GetColumnsAsync(string schema, string tableName);

    /// <summary>
    /// Performs a high-performance batch update of masked data.
    /// </summary>
    /// <param name="schema">The table schema.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column used to match rows.</param>
    /// <param name="rows">A collection of dictionaries, where each dictionary contains column-value pairs including the primary key.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateDataBatchAsync(string schema, string tableName, string primaryKeyColumn, IEnumerable<IDictionary<string, object>> rows);

    /// <summary>
    /// Streams data from the specified table for the requested columns.
    /// </summary>
    /// <param name="schema">The table schema.</param>
    /// <param name="tableName">The table name.</param>
    /// <param name="columns">The list of columns to retrieve.</param>
    /// <returns>An async enumerable of dictionaries containing column-value pairs.</returns>
    IAsyncEnumerable<IDictionary<string, object>> GetDataAsync(string schema, string tableName, IEnumerable<string> columns);
}
