using HelpDesk.Application.Abstractions.Notifications;
using HelpDesk.Application.Comments;
using HelpDesk.Application.Tickets;
using Microsoft.AspNetCore.SignalR;

namespace HelpDesk.Api.Realtime;

/// <summary>SignalR implementation of <see cref="ITicketNotifier"/>: broadcasts to the ticket's group.</summary>
public sealed class TicketNotifier : ITicketNotifier
{
    private readonly IHubContext<TicketsHub> _hub;

    public TicketNotifier(IHubContext<TicketsHub> hub) => _hub = hub;

    public Task TicketStatusChanged(TicketDto ticket, CancellationToken cancellationToken = default) =>
        _hub.Clients.Group(TicketsHub.GroupFor(ticket.Id))
            .SendAsync("TicketStatusChanged", ticket, cancellationToken);

    public Task TicketAssigned(TicketDto ticket, CancellationToken cancellationToken = default) =>
        _hub.Clients.Group(TicketsHub.GroupFor(ticket.Id))
            .SendAsync("TicketAssigned", ticket, cancellationToken);

    public Task CommentAdded(Guid ticketId, CommentDto comment, CancellationToken cancellationToken = default) =>
        _hub.Clients.Group(TicketsHub.GroupFor(ticketId))
            .SendAsync("CommentAdded", comment, cancellationToken);
}
