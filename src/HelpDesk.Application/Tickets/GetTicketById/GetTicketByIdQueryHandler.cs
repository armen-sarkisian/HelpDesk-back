using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.GetTicketById;

public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailsDto>
{
    private readonly ITicketRepository _tickets;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetTicketByIdQueryHandler(ITicketRepository tickets, ICurrentUser currentUser, IMapper mapper)
    {
        _tickets = tickets;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<TicketDetailsDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdWithCommentsAsync(request.TicketId, cancellationToken)
            ?? throw NotFoundException.For("Ticket", request.TicketId);

        // Owners see their own tickets; admins see everything.
        if (!_currentUser.IsAdmin && ticket.CreatedById != _currentUser.Id)
            throw new ForbiddenAccessException();

        return _mapper.Map<TicketDetailsDto>(ticket);
    }
}
