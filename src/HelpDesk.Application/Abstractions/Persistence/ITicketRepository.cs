using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Abstractions.Persistence;

public interface ITicketRepository
{
    /// <summary>Tracked load for mutations (edit / status / priority / assignment).</summary>
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Read-only load including comments and their authors, for the details view.</summary>
    Task<Ticket?> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Read-only list of a user's own tickets (newest first).</summary>
    Task<IReadOnlyList<Ticket>> GetByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default);

    /// <summary>Read-only list of every ticket (admin dashboard, newest first).</summary>
    Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
}
