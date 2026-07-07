using HelpDesk.Api.Authorization;
using HelpDesk.Application.Users;
using HelpDesk.Application.Users.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

// Listing users is an admin concern (e.g. choosing an assignee).
[Authorize(Policy = Policies.Admin)]
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct) =>
        Ok(await Sender.Send(new GetUsersQuery(), ct));
}