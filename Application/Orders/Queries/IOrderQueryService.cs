using Application.Orders.Queries.ReadModels;

namespace Application.Orders.Queries;

public interface IOrderQueryService
{
  Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken token);
  Task<IEnumerable<OrderReadModel>> GetCustomerOrdersAsync(Guid customerId, CancellationToken token);
}
