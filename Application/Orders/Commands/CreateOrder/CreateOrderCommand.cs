using MediatR;

namespace Application.Orders.Commands.CreateOrder;


public sealed record CreateOrderCommand(
  Guid CustomerId,
  ShippingAddress ShippingAddress,
  IReadOnlyList<ItemRequest> ItemRequests
) : IRequest<Guid>
{ }

public sealed record ShippingAddress(string Street, string City, string PostalCode, string Country) { }
public sealed record ItemRequest(Guid ProductId, decimal UnitPrice, string Currency, int Quantity) { }