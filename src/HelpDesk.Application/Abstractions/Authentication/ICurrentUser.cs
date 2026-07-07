namespace HelpDesk.Application.Abstractions.Authentication;

/// <summary>
/// The identity of the caller behind the current request. Implemented in the API layer over
/// <c>HttpContext</c>; handlers depend only on this abstraction so access rules stay testable
/// and free of web types.
/// </summary>
public interface ICurrentUser
{
    Guid Id { get; }

    bool IsAdmin { get; }

    bool IsAuthenticated { get; }
}
