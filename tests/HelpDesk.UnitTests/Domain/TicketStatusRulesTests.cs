using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;

namespace HelpDesk.UnitTests.Domain;

public class TicketStatusRulesTests
{
    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.InProgress)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed)]
    public void CanTransition_returns_true_for_allowed_forward_steps(TicketStatus from, TicketStatus to)
    {
        Assert.True(TicketStatusRules.CanTransition(from, to));
    }

    [Theory]
    // Skipping a step is not allowed.
    [InlineData(TicketStatus.Open, TicketStatus.Resolved)]
    [InlineData(TicketStatus.Open, TicketStatus.Closed)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Closed)]
    // Moving backwards is not allowed.
    [InlineData(TicketStatus.InProgress, TicketStatus.Open)]
    [InlineData(TicketStatus.Resolved, TicketStatus.InProgress)]
    [InlineData(TicketStatus.Closed, TicketStatus.Resolved)]
    // A no-op transition to the same status is not allowed.
    [InlineData(TicketStatus.Open, TicketStatus.Open)]
    [InlineData(TicketStatus.Closed, TicketStatus.Closed)]
    public void CanTransition_returns_false_for_disallowed_transitions(TicketStatus from, TicketStatus to)
    {
        Assert.False(TicketStatusRules.CanTransition(from, to));
    }

    [Fact]
    public void Closed_is_a_terminal_state_with_no_next_status()
    {
        Assert.Empty(TicketStatusRules.AllowedNext(TicketStatus.Closed));
    }

    [Fact]
    public void AllowedNext_returns_the_single_forward_step()
    {
        Assert.Equal([TicketStatus.InProgress], TicketStatusRules.AllowedNext(TicketStatus.Open));
    }
}
