namespace Application.Common.interfaces;


public interface IUnitOfWork
{
  Task<int> SaveChangesAsync(CancellationToken token = default);
  Task BeginTransactionAsync(CancellationToken token = default);
  Task CommitTransactionAsync(CancellationToken token);
  Task RollbackTransactionAsync(CancellationToken token = default);
}
