using HelpDesk.Application.Abstractions.Persistence;

namespace HelpDesk.Infrastructure.Persistence;

/// <summary>
/// EF Core already tracks changes; <see cref="UnitOfWork"/> simply exposes its commit point
/// through the Application-owned abstraction so handlers never depend on <c>DbContext</c>.
/// It shares the same scoped context instance as the repositories, so one call flushes them all.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly HelpDeskDbContext _context;

    public UnitOfWork(HelpDeskDbContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
