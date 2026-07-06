namespace HelpDesk.Domain.Enums;

/// <summary>
/// Lifecycle state of a ticket. Allowed transitions are enforced by
/// <see cref="Entities.TicketStatusRules"/> — the raw enum carries no rules by itself.
/// </summary>
public enum TicketStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}