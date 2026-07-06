namespace HelpDesk.Application.Common.Exceptions;

/// <summary>A request conflicts with current state (e.g. registering an email already in use). Maps to HTTP 409.</summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
