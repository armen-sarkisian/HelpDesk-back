using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.CreateTicket;

public sealed class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly ITicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public CreateTicketCommandHandler(
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

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Priority,
            _currentUser.Id);

        await _tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with the author navigation so the response DTO carries the display name.
        var created = await _tickets.GetByIdWithCommentsAsync(ticket.Id, cancellationToken);
        return _mapper.Map<TicketDto>(created!);
    }
}
