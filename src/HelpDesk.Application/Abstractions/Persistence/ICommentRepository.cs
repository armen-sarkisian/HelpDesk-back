using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Abstractions.Persistence;

public interface ICommentRepository
{
    /// <summary>Tracked load, used by the admin "delete comment" operation.</summary>
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Read-only load including the author, for building a comment DTO after creation.</summary>
    Task<Comment?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Read-only list of a ticket's comments with their authors (oldest first).</summary>
    Task<IReadOnlyList<Comment>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);

    Task AddAsync(Comment comment, CancellationToken cancellationToken = default);

    void Remove(Comment comment);
}
