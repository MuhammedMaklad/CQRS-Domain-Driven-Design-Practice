using Domain.Aggregates.OrderAggregate.ValueObjects;

namespace Domain.Aggregates.OrderAggregate.Repositories;

public interface IOrderRepository
{
  Task<Order?> GetByIdAsync(OrderId id, CancellationToken token = default);
  Task<bool> ExistsAsync(OrderId id, CancellationToken token = default);
  void Add(Order order);
  void Remove(Order order);
}
