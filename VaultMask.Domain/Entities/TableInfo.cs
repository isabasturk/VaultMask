namespace VaultMask.Domain.Entities;

/// <summary>
/// Represents metadata for a database table.
/// </summary>
/// <param name="Schema">The database schema (e.g., dbo).</param>
/// <param name="Name">The table name.</param>
public sealed record TableInfo(string Schema, string Name);
