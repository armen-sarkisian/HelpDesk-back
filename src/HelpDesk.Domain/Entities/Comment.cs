namespace HelpDesk.Domain.Entities;

/// <summary>
/// A message posted by a user on a ticket. Immutable once created — edits are out of scope;
/// only admins may delete comments (an operation modelled at the application layer).
/// </summary>
public class Comment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    // Navigation properties.
    public Ticket Ticket { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private Comment()
    {
    }

    public Comment(Guid ticketId, Guid userId, string message)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        UserId = userId;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }
}
