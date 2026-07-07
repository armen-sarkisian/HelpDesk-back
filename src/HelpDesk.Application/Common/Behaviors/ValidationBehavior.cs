using FluentValidation;
using MediatR;
using ValidationException = HelpDesk.Application.Common.Exceptions.ValidationException;

namespace HelpDesk.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline step that runs every <see cref="IValidator{T}"/> registered for the incoming
/// request before it reaches its handler. Validation therefore lives in one cross-cutting place
/// instead of being repeated at the top of each handler. Requests without a validator pass through.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
