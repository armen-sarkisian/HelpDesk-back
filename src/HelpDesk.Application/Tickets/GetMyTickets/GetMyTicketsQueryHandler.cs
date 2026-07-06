using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.GetMyTickets;

public sealed class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, IReadOnlyList<TicketDto>>
{
    private readonly ITicketRepository _tickets;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public GetMyTicketsQueryHandler(ITicketRepository tickets, ICurrentUser currentUser, IMapper mapper)
    {
        _tickets = tickets;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _tickets.GetByCreatorAsync(_currentUser.Id, cancellationToken);
        return _mapper.Map<IReadOnlyList<TicketDto>>(tickets);
    }
}
