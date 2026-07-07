using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

/// <summary>
/// Base for API controllers. Exposes a lazily-resolved <see cref="ISender"/> so actions stay thin —
/// they translate HTTP into MediatR requests and let handlers do the work.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;

    protected ISender Sender =>
        _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}