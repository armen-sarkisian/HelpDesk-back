using HelpDesk.Application.Abstractions.Notifications;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.ChangeStatus;

public sealed class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, TicketDto>
{
    private readonly ITicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITicketNotifier _notifier;

    public ChangeTicketStatusCommandHandler(
        ITicketRepository tickets,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITicketNotifier notifier)
    {
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notifier = notifier;
    }

    public async Task<TicketDto> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        // Domain enforces the workflow; an illegal transition throws InvalidStatusTransitionException.
        ticket.ChangeStatus(request.NewStatus);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _tickets.GetByIdWithCommentsAsync(ticket.Id, cancellationToken);
        var dto = _mapper.Map<TicketDto>(updated!);

        await _notifier.TicketStatusChanged(dto, cancellationToken);
        return dto;
    }
}
