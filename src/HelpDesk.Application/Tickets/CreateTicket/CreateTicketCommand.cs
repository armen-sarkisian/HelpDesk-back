using HelpDesk.Domain.Enums;
using MediatR;

namespace HelpDesk.Application.Tickets.CreateTicket;

/// <summary>Creates a ticket owned by the current user. The author is taken from the caller's identity, not the payload.</summary>
public sealed record CreateTicketCommand(string Title, string Description, TicketPriority Priority) : IRequest<TicketDto>;
