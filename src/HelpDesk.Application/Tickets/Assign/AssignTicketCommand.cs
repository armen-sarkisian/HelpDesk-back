using MediatR;

namespace HelpDesk.Application.Tickets.Assign;

public sealed record AssignTicketCommand(Guid TicketId, Guid AssigneeId) : IRequest<TicketDto>;
