using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.Entities;

/// <summary>
/// The aggregate root of the system. All state changes go through intention-revealing
/// methods (<see cref="ChangeStatus"/>, <see cref="Assign"/>, <see cref="UpdateDetails"/>)
/// so that invariants — the status workflow and "editable only until assignment" — live
/// with the data they protect rather than being re-checked in every handler.
/// </summary>
public class Ticket
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Guid CreatedById { get; private set; }
    public Guid? AssignedToId { get; private set; }

    // Navigation properties.
    public User CreatedBy { get; private set; } = null!;
    public User? AssignedTo { get; private set; }

    private readonly List<Comment> _comments = [];
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private Ticket()
    {
    }

    public Ticket(string title, string description, TicketPriority priority, Guid createdById)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Priority = priority;
        CreatedById = createdById;
        Status = TicketStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>True while the author is still allowed to edit the ticket (before assignment).</summary>
    public bool IsEditableByAuthor => AssignedToId is null;

    /// <summary>Author-driven edit. Blocked once the ticket has an assignee.</summary>
    public void UpdateDetails(string title, string description)
    {
        if (!IsEditableByAuthor)
            throw new TicketAlreadyAssignedException();

        Title = title;
        Description = description;
        Touch();
    }

    /// <summary>Admin action. Enforces the workflow via <see cref="TicketStatusRules"/>.</summary>
    public void ChangeStatus(TicketStatus newStatus)
    {
        if (!TicketStatusRules.CanTransition(Status, newStatus))
            throw new InvalidStatusTransitionException(Status, newStatus);

        Status = newStatus;
        Touch();
    }

    /// <summary>Admin action. Priority has no workflow constraints, so any value is allowed.</summary>
    public void ChangePriority(TicketPriority newPriority)
    {
        Priority = newPriority;
        Touch();
    }

    /// <summary>Admin action. Assigning an agent also locks the ticket from author edits.</summary>
    public void Assign(Guid agentId)
    {
        AssignedToId = agentId;
        Touch();
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
