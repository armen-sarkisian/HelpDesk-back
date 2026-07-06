using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Tickets;

/// <summary>Summary view of a ticket for lists (my tickets / admin dashboard).</summary>
public sealed record TicketDto(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid CreatedById,
    string CreatedByName,
    Guid? AssignedToId,
    string? AssignedToName);
