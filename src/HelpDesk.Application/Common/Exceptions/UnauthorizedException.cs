namespace HelpDesk.Application.Common.Exceptions;

/// <summary>Authentication failed (e.g. wrong email/password). Maps to HTTP 401.</summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
