using Application.Orders.Queries;
using Application.Orders.Queries.ReadModels;
using MediatR;

namespace Application.Orders.Queries.GetOrders;


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
