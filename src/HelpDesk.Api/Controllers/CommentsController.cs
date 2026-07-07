using HelpDesk.Api.Authorization;
using HelpDesk.Application.Comments;
using HelpDesk.Application.Comments.AddComment;
using HelpDesk.Application.Comments.DeleteComment;
using HelpDesk.Application.Comments.GetTicketComments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[Authorize]
[Route("api/comments")]
public sealed class CommentsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Add(AddCommentCommand command, CancellationToken ct)
    {
        var comment = await Sender.Send(command, ct);
        return Created($"/api/comments/{comment.Id}", comment);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetForTicket([FromQuery] Guid ticketId, CancellationToken ct) =>
        Ok(await Sender.Send(new GetTicketCommentsQuery(ticketId), ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteCommentCommand(id), ct);
        return NoContent();
    }
}