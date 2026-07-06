namespace HelpDesk.Application.Common.Exceptions;

/// <summary>The caller is authenticated but not allowed to perform this action. Maps to HTTP 403.</summary>
public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "You are not allowed to perform this action.")
        : base(message)
    {
    }
}
