using Application.Orders.Queries;
using Application.Orders.Queries.ReadModels;
using MediatR;

namespace Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderReadModel?>
{

  private readonly IOrderQueryService orderQueryService;
  public GetOrderByIdQueryHandler(IOrderQueryService orderQueryService)
  => this.orderQueryService = orderQueryService;
  public async Task<OrderReadModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
  {
    var result = await orderQueryService.GetOrderByIdAsync(request.Id, cancellationToken);
    return result;
  }
}
