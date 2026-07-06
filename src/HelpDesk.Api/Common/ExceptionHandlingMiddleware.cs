using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = HelpDesk.Application.Common.Exceptions.ValidationException;

namespace HelpDesk.Api.Common;

/// <summary>
/// Translates exceptions thrown anywhere below into RFC 7807 <see cref="ProblemDetails"/> responses,
/// so handlers can just throw meaningful exceptions and never build error payloads themselves.
/// Known application/domain exceptions become 4xx; everything else is a logged 500 that never
/// leaks internal details to the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException validation:
                await WriteAsync(context, new ValidationProblemDetails(
                    validation.Errors.ToDictionary(e => e.Key, e => e.Value))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                });
                break;

            case NotFoundException:
                await WriteAsync(context, Problem(StatusCodes.Status404NotFound, "Not Found", exception.Message));
                break;

            case ConflictException:
                await WriteAsync(context, Problem(StatusCodes.Status409Conflict, "Conflict", exception.Message));
                break;

            case ForbiddenAccessException:
                await WriteAsync(context, Problem(StatusCodes.Status403Forbidden, "Forbidden", exception.Message));
                break;

            case UnauthorizedException:
                await WriteAsync(context, Problem(StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message));
                break;

            // Domain-rule violations (illegal status transition, editing an assigned ticket) are
            // conflicts with the current state.
            case DomainException:
                await WriteAsync(context, Problem(StatusCodes.Status409Conflict, "Conflict", exception.Message));
                break;

            default:
                _logger.LogError(exception, "Unhandled exception processing {Path}", context.Request.Path);
                await WriteAsync(context, Problem(
                    StatusCodes.Status500InternalServerError,
                    "Server Error",
                    "An unexpected error occurred."));
                break;
        }
    }

    private static ProblemDetails Problem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };

    private static async Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, problem.GetType());
    }
}
