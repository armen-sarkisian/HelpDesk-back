using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.UnitTests.Domain;

public class TicketTests
{
    private static Ticket NewTicket() =>
        new("Printer broken", "The 3rd-floor printer jams.", TicketPriority.Medium, Guid.NewGuid());

    [Fact]
    public void New_ticket_starts_open_and_unassigned()
    {
        var ticket = NewTicket();

        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Null(ticket.AssignedToId);
        Assert.True(ticket.IsEditableByAuthor);
        Assert.Null(ticket.UpdatedAt);
    }

    [Fact]
    public void ChangeStatus_advances_through_the_workflow()
    {
        var ticket = NewTicket();

        ticket.ChangeStatus(TicketStatus.InProgress);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);

        ticket.ChangeStatus(TicketStatus.Resolved);
        Assert.Equal(TicketStatus.Resolved, ticket.Status);

        Assert.NotNull(ticket.UpdatedAt);
    }

    [Fact]
    public void ChangeStatus_throws_on_illegal_transition()
    {
        var ticket = NewTicket();

        var ex = Assert.Throws<InvalidStatusTransitionException>(
            () => ticket.ChangeStatus(TicketStatus.Closed));

        Assert.Equal(TicketStatus.Open, ex.From);
        Assert.Equal(TicketStatus.Closed, ex.To);
        Assert.Equal(TicketStatus.Open, ticket.Status); // unchanged
    }

    [Fact]
    public void UpdateDetails_is_allowed_before_assignment()
    {
        var ticket = NewTicket();

        ticket.UpdateDetails("New title", "New description");

        Assert.Equal("New title", ticket.Title);
        Assert.Equal("New description", ticket.Description);
    }

    [Fact]
    public void Assign_locks_the_ticket_from_author_edits()
    {
        var ticket = NewTicket();

        ticket.Assign(Guid.NewGuid());

        Assert.False(ticket.IsEditableByAuthor);
        Assert.Throws<TicketAlreadyAssignedException>(
            () => ticket.UpdateDetails("x", "y"));
    }
}
