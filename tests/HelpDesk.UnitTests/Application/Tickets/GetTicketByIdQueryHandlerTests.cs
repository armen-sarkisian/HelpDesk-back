using HelpDesk.Application.Abstractions.Authentication;
using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Application.Common.Exceptions;
using HelpDesk.Application.Tickets;
using HelpDesk.Application.Tickets.GetTicketById;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using MapsterMapper;
using NSubstitute;

namespace HelpDesk.UnitTests.Application.Tickets;

public class GetTicketByIdQueryHandlerTests
{
    private readonly ITicketRepository _tickets = Substitute.For<ITicketRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private GetTicketByIdQueryHandler CreateHandler() => new(_tickets, _currentUser, _mapper);

    [Fact]
    public async Task Handle_throws_not_found_when_ticket_missing()
    {
        var id = Guid.NewGuid();
        _tickets.GetByIdWithCommentsAsync(id, Arg.Any<CancellationToken>()).Returns((Ticket?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => CreateHandler().Handle(new GetTicketByIdQuery(id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_forbids_a_non_owner_non_admin()
    {
        var ownerId = Guid.NewGuid();
        var ticket = new Ticket("t", "d", TicketPriority.Low, ownerId);
        _tickets.GetByIdWithCommentsAsync(ticket.Id, Arg.Any<CancellationToken>()).Returns(ticket);

        _currentUser.Id.Returns(Guid.NewGuid()); // someone else
        _currentUser.IsAdmin.Returns(false);

        await Assert.ThrowsAsync<ForbiddenAccessException>(
            () => CreateHandler().Handle(new GetTicketByIdQuery(ticket.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_allows_an_admin_to_view_any_ticket()
    {
        var ticket = new Ticket("t", "d", TicketPriority.Low, Guid.NewGuid());
        _tickets.GetByIdWithCommentsAsync(ticket.Id, Arg.Any<CancellationToken>()).Returns(ticket);
        _currentUser.Id.Returns(Guid.NewGuid());
        _currentUser.IsAdmin.Returns(true);
        _mapper.Map<TicketDetailsDto>(ticket).Returns(new TicketDetailsDto(
            ticket.Id, ticket.Title, ticket.Description, ticket.Status, ticket.Priority,
            ticket.CreatedAt, ticket.UpdatedAt, ticket.CreatedById, "Owner", null, null, []));

        var result = await CreateHandler().Handle(new GetTicketByIdQuery(ticket.Id), CancellationToken.None);

        Assert.Equal(ticket.Id, result.Id);
    }
}
