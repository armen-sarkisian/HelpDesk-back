using HelpDesk.Application.Abstractions.Notifications;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.Assign;

public sealed class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
{
    private readonly ITicketRepository _tickets;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITicketNotifier _notifier;

    public AssignTicketCommandHandler(
        ITicketRepository tickets,
        IUserRepository users,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITicketNotifier notifier)
    {
        _tickets = tickets;
        _users = users;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notifier = notifier;
    }

    public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        // The assignee must be a real user, otherwise we'd create a dangling foreign key.
        var assignee = await _users.GetByIdAsync(request.AssigneeId, cancellationToken)
            ?? throw NotFoundException.For("User", request.AssigneeId);

        ticket.Assign(assignee.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _tickets.GetByIdWithCommentsAsync(ticket.Id, cancellationToken);
        var dto = _mapper.Map<TicketDto>(updated!);

        await _notifier.TicketAssigned(dto, cancellationToken);
        return dto;
    }
}
