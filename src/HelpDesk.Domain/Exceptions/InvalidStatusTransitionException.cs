using HelpDesk.Domain.Enums;

namespace HelpDesk.Domain.Exceptions;

/// <summary>
/// Thrown when a ticket is asked to move to a status that is not reachable
/// from its current one according to the allowed workflow.
/// </summary>
public sealed class InvalidStatusTransitionException : DomainException
{
    public TicketStatus From { get; }
    public TicketStatus To { get; }

    public InvalidStatusTransitionException(TicketStatus from, TicketStatus to)
        : base($"Cannot change ticket status from '{from}' to '{to}'.")
    {
        From = from;
        To = to;
    }
}