using MediatR;

namespace HelpDesk.Application.Comments.DeleteComment;

/// <summary>Removes a comment. Admin-only (enforced at the endpoint).</summary>
public sealed record DeleteCommentCommand(Guid CommentId) : IRequest;
