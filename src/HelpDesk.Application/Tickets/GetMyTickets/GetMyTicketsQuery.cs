using MediatR;

namespace HelpDesk.Application.Tickets.GetMyTickets;

/// <summary>Lists the current user's own tickets. Scope comes from the caller's identity.</summary>
public sealed record GetMyTicketsQuery : IRequest<IReadOnlyList<TicketDto>>;
