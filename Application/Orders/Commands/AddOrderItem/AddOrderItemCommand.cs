


using MediatR;

namespace Application.Orders.Commands.AddOrderItem;

public sealed record AddOrderItemCommand(
  Guid OrderId,
  Guid ProductId,
  decimal UnitPrice,
  string Currency,
  int Quantity
) : IRequest<Guid>
{ }
