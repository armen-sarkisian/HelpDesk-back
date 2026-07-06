namespace HelpDesk.Domain.Exceptions;

/// <summary>
/// Thrown when the ticket author tries to edit a ticket that has already been
/// assigned to an agent. Encodes the rule "editable only until assignment".
/// </summary>
public sealed class TicketAlreadyAssignedException : DomainException
{
    public TicketAlreadyAssignedException()
        : base("The ticket has already been assigned and can no longer be edited by its author.")
    {
    }
}