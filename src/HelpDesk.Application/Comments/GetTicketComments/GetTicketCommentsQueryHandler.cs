using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Comments.GetTicketComments;

public sealed class GetTicketCommentsQueryHandler : IRequestHandler<GetTicketCommentsQuery, IReadOnlyList<CommentDto>>
{
    private readonly ITicketRepository _tickets;
    private readonly ICommentRepository _comments;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetTicketCommentsQueryHandler(
        ITicketRepository tickets,
        ICommentRepository comments,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _tickets = tickets;
        _comments = comments;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CommentDto>> Handle(GetTicketCommentsQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        if (!_currentUser.IsAdmin && ticket.CreatedById != _currentUser.Id)
            throw new ForbiddenAccessException();

        var comments = await _comments.GetByTicketAsync(ticket.Id, cancellationToken);
        return _mapper.Map<IReadOnlyList<CommentDto>>(comments);
    }
}
