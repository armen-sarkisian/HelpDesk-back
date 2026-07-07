using FluentValidation.Results;

namespace HelpDesk.Application.Common.Exceptions;

/// <summary>
/// Application-level validation failure. Wrapping FluentValidation's failures in our own type
/// (grouped per property) keeps the API's error-mapping decoupled from the validation library
/// and produces a clean, predictable shape for the 400 response built in the API layer.
/// </summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}
