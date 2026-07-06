using FluentValidation;

namespace HelpDesk.Application.Tickets.Assign;

public sealed class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    public AssignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.AssigneeId).NotEmpty();
    }
}
