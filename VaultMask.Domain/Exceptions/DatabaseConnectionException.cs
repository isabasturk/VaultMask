namespace VaultMask.Domain.Exceptions;

/// <summary>
/// Exception thrown when a connection to the database cannot be established.
/// </summary>
public sealed class DatabaseConnectionException : DatabaseException
{
    public DatabaseConnectionException(string message) : base(message) { }
    public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException) { }
}
