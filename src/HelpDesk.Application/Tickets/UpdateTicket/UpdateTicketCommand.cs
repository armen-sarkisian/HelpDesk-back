using MediatR;

namespace HelpDesk.Application.Tickets.UpdateTicket;

public sealed record UpdateTicketCommand(Guid TicketId, string Title, string Description) : IRequest<TicketDto>;
