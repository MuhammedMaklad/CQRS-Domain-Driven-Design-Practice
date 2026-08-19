using Application.Common.interfaces;
using Domain.Aggregates;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Repositories;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using MediatR;

namespace Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
  private readonly IOrderRepository orderRepository;
  private readonly IUnitOfWork unitOfWork;

  public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
  {
    this.orderRepository = orderRepository;
    this.unitOfWork = unitOfWork;
  }
  public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
  {
    var address = Address.Create(request.ShippingAddress.Street, request.ShippingAddress.City, request.ShippingAddress.PostalCode, request.ShippingAddress.Country);
    var customerId = new CustomerId(request.CustomerId);

    var items = new List<OrderItem>();
    foreach (var item in request.ItemRequests)
      items.Add(OrderItem.Create(new ProductId(item.ProductId), Money.Create(item.UnitPrice, item.Currency), item.Quantity));

    var order = Order.Create(customerId, address, items);

    orderRepository.Add(order);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return order.Id.Value;
  }
}