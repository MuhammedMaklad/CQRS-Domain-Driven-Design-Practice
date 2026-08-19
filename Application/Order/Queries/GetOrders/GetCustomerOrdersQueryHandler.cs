using Application.Order.Queries.ReadModels;
using MediatR;

namespace Application.Order.Queries.GetOrders;


public sealed class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, IEnumerable<OrderReadModel>>
{
  private readonly IOrderQueryService orderQueryService;

  public GetCustomerOrdersQueryHandler(IOrderQueryService orderQueryService)
  {
    this.orderQueryService = orderQueryService;
  }
  public async Task<IEnumerable<OrderReadModel>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
  {
    return await orderQueryService.GetCustomerOrdersAsync(request.customerId, cancellationToken);
  }
}
