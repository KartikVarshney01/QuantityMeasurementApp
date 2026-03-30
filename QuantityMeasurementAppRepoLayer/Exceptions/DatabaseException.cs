namespace QuantityMeasurementAppRepoLayer.Exceptions;

// New exception for database-layer errors.
public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message) { }

    public DatabaseException(string message, Exception inner)
        : base(message, inner) { }
}
