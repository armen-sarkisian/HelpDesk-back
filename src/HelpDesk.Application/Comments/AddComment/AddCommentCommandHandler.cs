using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Notifications;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Comments.AddComment;

public sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly ITicketRepository _tickets;
    private readonly ICommentRepository _comments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly ITicketNotifier _notifier;

    public AddCommentCommandHandler(
        ITicketRepository tickets,
        ICommentRepository comments,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMapper mapper,
        ITicketNotifier notifier)
    {
        _tickets = tickets;
        _comments = comments;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
        _notifier = notifier;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        // You can comment on a ticket only if you can see it: your own, or anything for admins.
        if (!_currentUser.IsAdmin && ticket.CreatedById != _currentUser.Id)
            throw new ForbiddenAccessException();

        var comment = new Comment(ticket.Id, _currentUser.Id, request.Message.Trim());
        await _comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _comments.GetByIdWithUserAsync(comment.Id, cancellationToken);
        var dto = _mapper.Map<CommentDto>(created!);

        await _notifier.CommentAdded(ticket.Id, dto, cancellationToken);
        return dto;
    }
}
