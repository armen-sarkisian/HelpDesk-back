using HelpDesk.Application.Abstractions.Persistence;
using MapsterMapper;
using MediatR;

namespace HelpDesk.Application.Tickets.GetAllTickets;

public sealed class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, IReadOnlyList<TicketDto>>
{
    private readonly ITicketRepository _tickets;
    private readonly IMapper _mapper;

    public GetAllTicketsQueryHandler(ITicketRepository tickets, IMapper mapper)
    {
        _tickets = tickets;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _tickets.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TicketDto>>(tickets);
    }
}
