using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HelpDesk.Api.Realtime;

/// <summary>
/// Real-time channel for ticket updates. Clients subscribe per ticket via a SignalR group, so a
/// status change / assignment / new comment is broadcast only to those viewing that ticket.
/// Requires authentication — the JWT is passed by the client on the WebSocket connection.
/// </summary>
[Authorize]
public sealed class TicketsHub : Hub
{
    public static string GroupFor(Guid ticketId) => $"ticket-{ticketId}";

    public Task JoinTicket(Guid ticketId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, GroupFor(ticketId));

    public Task LeaveTicket(Guid ticketId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupFor(ticketId));
}
