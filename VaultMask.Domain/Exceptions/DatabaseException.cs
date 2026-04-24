namespace VaultMask.Domain.Exceptions;

/// <summary>
/// Base class for all database-related exceptions in VaultMask.
/// </summary>
public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}
