using FluentValidation.TestHelper;
using HelpDesk.Application.Tickets.CreateTicket;
using HelpDesk.Domain.Enums;

namespace HelpDesk.UnitTests.Application.Tickets;

public class CreateTicketCommandValidatorTests
{
    private readonly CreateTicketCommandValidator _validator = new();

    private static CreateTicketCommand Valid() =>
        new("Printer broken", "The 3rd-floor printer jams.", TicketPriority.Medium);

    [Fact]
    public void Valid_command_passes()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_title_fails()
    {
        var command = Valid() with { Title = "" };

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Too_long_title_fails()
    {
        var command = Valid() with { Title = new string('x', 201) };

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Empty_description_fails()
    {
        var command = Valid() with { Description = "" };

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Priority_outside_the_enum_fails()
    {
        var command = Valid() with { Priority = (TicketPriority)99 };

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(x => x.Priority);
    }
}
