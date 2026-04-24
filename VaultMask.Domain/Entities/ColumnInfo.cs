namespace VaultMask.Domain.Entities;

/// <summary>
/// Represents metadata for a database column.
/// </summary>
/// <param name="Name">The column name.</param>
/// <param name="DataType">The data type of the column.</param>
/// <param name="IsNullable">Whether the column allows null values.</param>
/// <param name="IsPrimaryKey">Whether the column is part of the primary key.</param>
public sealed record ColumnInfo(string Name, string DataType, bool IsNullable, bool IsPrimaryKey);
