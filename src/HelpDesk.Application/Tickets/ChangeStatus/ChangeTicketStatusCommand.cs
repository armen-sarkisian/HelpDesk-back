using HelpDesk.Domain.Enums;
using MediatR;

namespace HelpDesk.Application.Tickets.ChangeStatus;

public sealed record ChangeTicketStatusCommand(Guid TicketId, TicketStatus NewStatus) : IRequest<TicketDto>;
