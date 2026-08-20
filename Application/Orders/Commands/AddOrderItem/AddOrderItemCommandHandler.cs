


using Application.Common.interfaces;
using Application.Orders.Exceptions;
using Domain.Aggregates;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Repositories;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using MediatR;

namespace Application.Orders.Commands.AddOrderItem;

public sealed class AddOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
: IRequestHandler<AddOrderItemCommand, Guid>
{
  public async Task<Guid> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
  {
    var order = await orderRepository.GetByIdAsync(OrderId.From(request.OrderId), cancellationToken) ??
      throw new OrderNotFoundException($"invalid Order id: {request.OrderId}");

    order.AddItem(ProductId.From(request.ProductId), Money.Create(request.UnitPrice, request.Currency), request.Quantity);

    await unitOfWork.SaveChangesAsync();

    return order.Id.Value;
  }
}
