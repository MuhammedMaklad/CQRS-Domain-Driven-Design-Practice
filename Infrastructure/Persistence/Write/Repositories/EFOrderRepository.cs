using Domain.Aggregates;
using Domain.Aggregates.OrderAggregate.Repositories;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Write.Repositories;

internal sealed class EFOrderRepository : RepositoryBase<Order>, IOrderRepository
{
  public EFOrderRepository(ApplicationDbContext context) : base(context)
  {
  }

  public Task<Order?> GetByIdAsync(OrderId id, CancellationToken token = default)
    => GetByIdAsync<OrderId>(id, token);

  public Task<bool> ExistsAsync(OrderId id, CancellationToken token = default)
    => Set.AnyAsync(order => order.Id == id, token);
}
