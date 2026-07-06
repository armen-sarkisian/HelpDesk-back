using HelpDesk.Application.Comments;
using HelpDesk.Application.Tickets;

namespace HelpDesk.Application.Abstractions.Notifications;

/// <summary>
/// Pushes real-time ticket events to interested clients. Implemented with SignalR in the API
/// layer; handlers depend only on this contract so the transport stays out of the Application layer.
/// </summary>
public interface ITicketNotifier
{
    Task TicketStatusChanged(TicketDto ticket, CancellationToken cancellationToken = default);

    Task TicketAssigned(TicketDto ticket, CancellationToken cancellationToken = default);

    Task CommentAdded(Guid ticketId, CommentDto comment, CancellationToken cancellationToken = default);
}
