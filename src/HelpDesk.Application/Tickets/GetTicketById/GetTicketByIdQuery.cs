using MediatR;

namespace HelpDesk.Application.Tickets.GetTicketById;

public sealed record GetTicketByIdQuery(Guid TicketId) : IRequest<TicketDetailsDto>;
