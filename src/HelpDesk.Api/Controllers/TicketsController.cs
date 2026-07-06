using HelpDesk.Api.Authorization;
using HelpDesk.Application.Tickets;
using HelpDesk.Application.Tickets.Assign;
using HelpDesk.Application.Tickets.ChangePriority;
using HelpDesk.Application.Tickets.ChangeStatus;
using HelpDesk.Application.Tickets.CreateTicket;
using HelpDesk.Application.Tickets.GetAllTickets;
using HelpDesk.Application.Tickets.GetMyTickets;
using HelpDesk.Application.Tickets.GetTicketById;
using HelpDesk.Application.Tickets.UpdateTicket;
using HelpDesk.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[Authorize]
[Route("api/tickets")]
public sealed class TicketsController : ApiControllerBase
{
    // Request bodies for actions that combine a route id with a payload.
    public sealed record UpdateTicketRequest(string Title, string Description);
    public sealed record ChangeStatusRequest(TicketStatus Status);
    public sealed record ChangePriorityRequest(TicketPriority Priority);
    public sealed record AssignRequest(Guid AssigneeId);

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create(CreateTicketCommand command, CancellationToken ct)
    {
        var ticket = await Sender.Send(command, ct);
        return Created($"/api/tickets/{ticket.Id}", ticket);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetMine(CancellationToken ct) =>
        Ok(await Sender.Send(new GetMyTicketsQuery(), ct));

    [HttpGet("all")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetAll(CancellationToken ct) =>
        Ok(await Sender.Send(new GetAllTicketsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailsDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await Sender.Send(new GetTicketByIdQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, UpdateTicketRequest body, CancellationToken ct) =>
        Ok(await Sender.Send(new UpdateTicketCommand(id, body.Title, body.Description), ct));

    [HttpPut("{id:guid}/status")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<TicketDto>> ChangeStatus(Guid id, ChangeStatusRequest body, CancellationToken ct) =>
        Ok(await Sender.Send(new ChangeTicketStatusCommand(id, body.Status), ct));

    [HttpPut("{id:guid}/priority")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<TicketDto>> ChangePriority(Guid id, ChangePriorityRequest body, CancellationToken ct) =>
        Ok(await Sender.Send(new ChangeTicketPriorityCommand(id, body.Priority), ct));

    [HttpPut("{id:guid}/assign")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<TicketDto>> Assign(Guid id, AssignRequest body, CancellationToken ct) =>
        Ok(await Sender.Send(new AssignTicketCommand(id, body.AssigneeId), ct));
}