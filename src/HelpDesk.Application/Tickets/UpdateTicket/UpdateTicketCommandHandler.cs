using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.UpdateTicket;

public sealed class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketDto>
{
    private readonly ITicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public UpdateTicketCommandHandler(
        ITicketRepository tickets,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        // Authorization: only the author may edit their own ticket.
        if (ticket.CreatedById != _currentUser.Id)
            throw new ForbiddenAccessException();

        // Domain invariant: throws TicketAlreadyAssignedException if it's already assigned.
        ticket.UpdateDetails(request.Title.Trim(), request.Description.Trim());

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _tickets.GetByIdWithCommentsAsync(ticket.Id, cancellationToken);
        return _mapper.Map<TicketDto>(updated!);
    }
}
