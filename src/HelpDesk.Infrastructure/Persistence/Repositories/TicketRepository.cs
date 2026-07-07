using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository : ITicketRepository
{
    private readonly HelpDeskDbContext _context;

    public TicketRepository(HelpDeskDbContext context) => _context = context;

    // Tracked: the caller intends to mutate the ticket and then SaveChanges.
    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Ticket?> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Ticket>> GetByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default) =>
        await _context.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedById == creatorId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default) =>
        await _context.Tickets.AddAsync(ticket, cancellationToken);
}
