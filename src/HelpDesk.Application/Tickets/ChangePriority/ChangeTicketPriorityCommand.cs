using HelpDesk.Domain.Enums;
using MediatR;

namespace HelpDesk.Application.Tickets.ChangePriority;

public sealed record ChangeTicketPriorityCommand(Guid TicketId, TicketPriority NewPriority) : IRequest<TicketDto>;
