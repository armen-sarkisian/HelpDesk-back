using HelpDesk.Application.Abstractions.Persistence;
using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository : ICommentRepository
{
    private readonly HelpDeskDbContext _context;

    public CommentRepository(HelpDeskDbContext context) => _context = context;

    public Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Comment?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Comment>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken = default) =>
        await _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Comment comment, CancellationToken cancellationToken = default) =>
        await _context.Comments.AddAsync(comment, cancellationToken);

    public void Remove(Comment comment) => _context.Comments.Remove(comment);
}
