namespace HelpDesk.Application.Common.Exceptions;

/// <summary>A requested entity does not exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, object key) =>
        new($"{entity} '{key}' was not found.");
}
