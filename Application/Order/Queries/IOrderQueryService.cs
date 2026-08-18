using Application.Order.Queries.ReadModels;

namespace Application.Order.Queries;

public interface IOrderQueryService
{
  Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken token);
}
