using FluentValidation;

namespace HelpDesk.Application.Tickets.ChangeStatus;

public sealed class ChangeTicketStatusCommandValidator : AbstractValidator<ChangeTicketStatusCommand>
{
    public ChangeTicketStatusCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        // Shape check only: whether this transition is legal is a domain rule (TicketStatusRules).
        RuleFor(x => x.NewStatus).IsInEnum();
    }
}
