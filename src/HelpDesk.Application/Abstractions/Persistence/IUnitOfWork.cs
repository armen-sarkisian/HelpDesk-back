namespace HelpDesk.Application.Abstractions.Persistence;

/// <summary>
/// Commits all changes tracked across the repositories as a single transaction.
/// Repositories only stage changes (Add/Remove/mutations); nothing is persisted until
/// <see cref="SaveChangesAsync"/> is called, keeping the "one business operation = one save"
/// boundary explicit in the handlers.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
