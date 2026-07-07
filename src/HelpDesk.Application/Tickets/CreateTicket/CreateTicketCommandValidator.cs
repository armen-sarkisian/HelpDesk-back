using FluentValidation;

namespace HelpDesk.Application.Tickets.CreateTicket;

public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        // Reject values outside the enum (e.g. a bogus number bound from JSON).
        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
