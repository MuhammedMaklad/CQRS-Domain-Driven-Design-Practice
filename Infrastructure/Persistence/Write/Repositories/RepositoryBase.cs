using Microsoft.EntityFrameworkCore.Write;

namespace Infrastructure.Persistence.Write.Repositories;


internal abstract class RepositoryBase<T> where T : class
{
  protected readonly ApplicationDbContext Context;
  protected readonly DbSet<T> Set;

  protected RepositoryBase(ApplicationDbContext context)
  {
    Context = context;
    Set = context.Set<T>();
  }

  protected Task<T?> GetByIdAsync<TId>(TId id, CancellationToken token = default)
  => Set.FindAsync([id], token).AsTask();
  public void Add(T entity) => Set.Add(entity);
  public void Remove(T entity) => Set.Remove(entity);
}
