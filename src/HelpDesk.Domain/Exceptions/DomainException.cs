namespace HelpDesk.Domain.Exceptions;

/// <summary>
/// Base type for violations of domain invariants (e.g. an illegal status transition).
/// Application/API layers translate these into 4xx responses instead of leaking 500s.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}