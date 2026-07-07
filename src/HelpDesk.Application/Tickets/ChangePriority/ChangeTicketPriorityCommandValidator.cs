using FluentValidation;

namespace HelpDesk.Application.Tickets.ChangePriority;

public sealed class ChangeTicketPriorityCommandValidator : AbstractValidator<ChangeTicketPriorityCommand>
{
    public ChangeTicketPriorityCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.NewPriority).IsInEnum();
    }
}
