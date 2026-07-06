using HelpDesk.Application.Comments;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Tickets;

/// <summary>Full view of a single ticket, including its comment thread.</summary>
public sealed record TicketDetailsDto(
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
    string? AssignedToName,
    IReadOnlyList<CommentDto> Comments);
