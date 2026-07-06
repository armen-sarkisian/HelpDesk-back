using MediatR;

namespace HelpDesk.Application.Comments.AddComment;

public sealed record AddCommentCommand(Guid TicketId, string Message) : IRequest<CommentDto>;
