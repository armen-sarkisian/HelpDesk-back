using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Entities;

/// <summary>
/// Single source of truth for the ticket workflow: <c>Open → InProgress → Resolved → Closed</c>.
/// Kept as a small, self-contained rule object so both the domain (<see cref="Ticket.ChangeStatus"/>)
/// and higher layers (e.g. exposing the next valid statuses to the UI) can consult it without
/// duplicating <c>if</c>-chains. Transitions are strictly forward and one step at a time; a
/// no-op transition to the same status is intentionally *not* allowed.
/// </summary>
public static class TicketStatusRules
{
    private static readonly IReadOnlyDictionary<TicketStatus, TicketStatus[]> Allowed =
        new Dictionary<TicketStatus, TicketStatus[]>
        {
            [TicketStatus.Open] = [TicketStatus.InProgress],
            [TicketStatus.InProgress] = [TicketStatus.Resolved],
            [TicketStatus.Resolved] = [TicketStatus.Closed],
            [TicketStatus.Closed] = []
        };

    public static bool CanTransition(TicketStatus from, TicketStatus to) =>
        Allowed.TryGetValue(from, out var next) && Array.IndexOf(next, to) >= 0;

    /// <summary>Statuses reachable in one step from <paramref name="from"/> (empty for terminal states).</summary>
    public static IReadOnlyCollection<TicketStatus> AllowedNext(TicketStatus from) =>
        Allowed.TryGetValue(from, out var next) ? next : [];
}
