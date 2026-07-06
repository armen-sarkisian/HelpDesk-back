using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.ChangePriority;

public sealed class ChangeTicketPriorityCommandHandler : IRequestHandler<ChangeTicketPriorityCommand, TicketDto>
{
    private readonly ITicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChangeTicketPriorityCommandHandler(ITicketRepository tickets, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(ChangeTicketPriorityCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        ticket.ChangePriority(request.NewPriority);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _tickets.GetByIdWithCommentsAsync(ticket.Id, cancellationToken);
        return _mapper.Map<TicketDto>(updated!);
    }
}
