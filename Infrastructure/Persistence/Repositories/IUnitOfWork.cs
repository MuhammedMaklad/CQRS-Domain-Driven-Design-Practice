namespace Infrastructure.Persistence.Repositories;

public interface IUnitOfWork
{
  Task<int> SaveChangesAsync(CancellationToken token = default);
}
