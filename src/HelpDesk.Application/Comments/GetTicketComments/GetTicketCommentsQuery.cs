using MediatR;

namespace HelpDesk.Application.Comments.GetTicketComments;

public sealed record GetTicketCommentsQuery(Guid TicketId) : IRequest<IReadOnlyList<CommentDto>>;
