using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using VaultMask.Domain.Entities;
using VaultMask.Domain.Interfaces;
using VaultMask.Domain.Exceptions;
using System.Text;

namespace VaultMask.Infrastructure.Repositories;

/// <summary>
/// SQL Server implementation of the database repository using Dapper.
/// </summary>
/// <param name="connectionString">The connection string to the target SQL Server database.</param>
public sealed class SqlServerRepository(string connectionString) : IDatabaseRepository
{
    private IDbConnection CreateConnection() => new SqlConnection(connectionString);

    public async Task<IEnumerable<TableInfo>> GetTablesAsync()
    {
        const string sql = """
            SELECT TABLE_SCHEMA AS [Schema], TABLE_NAME AS [Name]
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_SCHEMA, TABLE_NAME
            """;

        try
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<TableInfo>(sql);
        }
        catch (SqlException ex)
        {
            throw new DatabaseConnectionException("An error occurred while connecting to the database to fetch tables.", ex);
        }
    }

    public async Task<IEnumerable<ColumnInfo>> GetColumnsAsync(string schema, string tableName)
    {
        const string sql = """
            SELECT 
                c.COLUMN_NAME AS [Name], 
                c.DATA_TYPE AS [DataType], 
                CAST(CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS BIT) AS [IsNullable],
                CAST(CASE WHEN EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                    WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                    AND kcu.TABLE_SCHEMA = c.TABLE_SCHEMA 
                    AND kcu.TABLE_NAME = c.TABLE_NAME 
                    AND kcu.COLUMN_NAME = c.COLUMN_NAME
                ) THEN 1 ELSE 0 END AS BIT) AS [IsPrimaryKey]
            FROM INFORMATION_SCHEMA.COLUMNS c
            WHERE c.TABLE_SCHEMA = @schema AND c.TABLE_NAME = @tableName
            ORDER BY c.ORDINAL_POSITION
            """;

        try
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<ColumnInfo>(sql, new { schema, tableName });
        }
        catch (SqlException ex)
        {
            throw new DatabaseException($"An error occurred while fetching columns for table [{schema}].[{tableName}].", ex);
        }
    }

    public async Task UpdateDataBatchAsync(string schema, string tableName, string primaryKeyColumn, IEnumerable<IDictionary<string, object>> rows)
    {
        var rowList = rows.ToList();
        if (rowList.Count == 0) return;

        // Take keys from the first row to build the SET clause
        // We assume all dictionaries in the batch have the same keys
        var firstRow = rowList[0];
        var columnsToUpdate = firstRow.Keys.Where(k => !k.Equals(primaryKeyColumn, StringComparison.OrdinalIgnoreCase)).ToList();

        if (columnsToUpdate.Count == 0) return;

        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append($"UPDATE [{schema}].[{tableName}] SET ");
        
        for (int i = 0; i < columnsToUpdate.Count; i++)
        {
            var col = columnsToUpdate[i];
            sqlBuilder.Append($"[{col}] = @{col}");
            if (i < columnsToUpdate.Count - 1)
            {
                sqlBuilder.Append(", ");
            }
        }

        sqlBuilder.Append($" WHERE [{primaryKeyColumn}] = @{primaryKeyColumn}");

        try
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(sqlBuilder.ToString(), rowList);
        }
        catch (SqlException ex)
        {
            throw new DatabaseException($"An error occurred during batch update for table [{schema}].[{tableName}].", ex);
        }
    }

    public async IAsyncEnumerable<IDictionary<string, object>> GetDataAsync(string schema, string tableName, IEnumerable<string> columns)
    {
        var columnList = columns.ToList();
        var sql = $"SELECT {string.Join(", ", columnList.Select(c => $"[{c}]"))} FROM [{schema}].[{tableName}]";

        using var connection = CreateConnection();
        // We use ExecuteReaderAsync and manual mapping to support unbuffered streaming
        // and avoid Dapper mapping issues with interfaces (IDictionary).
        var reader = await connection.ExecuteReaderAsync(sql);
        var dbReader = (System.Data.Common.DbDataReader)reader;

        while (await dbReader.ReadAsync())
        {
            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < dbReader.FieldCount; i++)
            {
                row[dbReader.GetName(i)] = dbReader.GetValue(i);
            }
            yield return row;
        }
    }
}
