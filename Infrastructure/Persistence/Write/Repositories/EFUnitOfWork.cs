using Application.Common.interfaces;
using Domain.Common.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Write.Repositories;

public sealed class EFUnitOfWork(ApplicationDbContext context, IEventDispatcher eventDispatcher)
: IUnitOfWork
{
  private IDbContextTransaction? dbContextTransaction;

  public async Task<int> SaveChangesAsync(CancellationToken token = default)
  {
    var domainEvents = CollectDomainEvents();

    var result = await context.SaveChangesAsync(token);

    await eventDispatcher.DispatchAsync(domainEvents, token);

    return result;
  }

  private List<IDomainEvent> CollectDomainEvents()
  {
    var aggregates = context.ChangeTracker.Entries<IAggregateRoot>()
      .Where(entry => entry.Entity.DomainEvents.Count != 0)
      .Select(entry => entry.Entity)
      .ToList();

    var domainEvents = aggregates
      .SelectMany(aggregate => aggregate.DomainEvents)
      .ToList();

    foreach (var aggregate in aggregates)
      aggregate.ClearDomainEvents();

    return domainEvents;
  }

  public async Task BeginTransactionAsync(CancellationToken token = default)
  => dbContextTransaction = await context.Database.BeginTransactionAsync(token);

  public async Task CommitTransactionAsync(CancellationToken token)
  {
    await dbContextTransaction!.CommitAsync(token);
    await dbContextTransaction.DisposeAsync();
    dbContextTransaction = null;
  }

  public async Task RollbackTransactionAsync(CancellationToken token = default)
  {
    await dbContextTransaction!.RollbackAsync(token);
    await dbContextTransaction.DisposeAsync();
    dbContextTransaction = null;
  }
}
