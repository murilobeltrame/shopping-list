namespace ShoppingList.Domain.Exceptions;

/// <summary>
/// Base exception for domain-layer invariant violations.
/// Thrown when business rules are violated (e.g., invalid entity state, invariant constraints).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
