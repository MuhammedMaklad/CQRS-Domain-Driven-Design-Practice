using Application.Common.interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Write.Repositories;

public sealed class EFUnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext context;
  private IDbContextTransaction? dbContextTransaction;
  public EFUnitOfWork(ApplicationDbContext _context) => context = _context;


  public Task<int> SaveChangesAsync(CancellationToken token = default)
  => context.SaveChangesAsync(token);

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
