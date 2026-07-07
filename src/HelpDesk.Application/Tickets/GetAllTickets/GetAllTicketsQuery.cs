using MediatR;

namespace HelpDesk.Application.Tickets.GetAllTickets;

/// <summary>Admin dashboard: every ticket, newest first. Admin-only (enforced at the endpoint).</summary>
public sealed record GetAllTicketsQuery : IRequest<IReadOnlyList<TicketDto>>;
