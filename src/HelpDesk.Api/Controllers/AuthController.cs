using HelpDesk.Application.Authentication;
using HelpDesk.Application.Authentication.Login;
using HelpDesk.Application.Authentication.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command, CancellationToken ct) =>
        Ok(await Sender.Send(command, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken ct) =>
        Ok(await Sender.Send(command, ct));
}