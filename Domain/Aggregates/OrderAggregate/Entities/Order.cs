using System.Collections.ObjectModel;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.Events;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Domain.Common.BaseClasses;
using Domain.Common.Exceptions;

namespace Domain.Aggregates;

public sealed class Order : AggregateRoot<OrderId>
{

  private readonly List<OrderItem> _orderItems = [];

  public Address ShippingAddress { get; private set; }
  public OrderStatus Status { get; private set; }
  public CustomerId CustomerId { get; private set; }
  public Money TotalPrice { get; private set; } = Money.Zero();
  public ReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();


  private Order() { } // for entity framework
  private Order(CustomerId customerId, Address shippingAddress) : base(OrderId.New())
  {
    CustomerId = customerId;
    ShippingAddress = shippingAddress;
    Status = OrderStatus.PENDING;
  }
  //!--------- public method ---------------- \\
  public static Order Create(CustomerId customerId, Address shippingAddress)
  {
    return new Order(customerId, shippingAddress);
  }
  public void AddItem(ProductId productId, Money price, int quantity)
  {
    EnsureIsPending();

    var existing = _orderItems.Find(item => item.ProductId == productId);
    if (existing is not null)
      existing.IncreaseQuantity(quantity);
    else
      _orderItems.Add(OrderItem.Create(productId, price, quantity));

    ReCalculateTotalPrice();
    AddDomainEvent(new OrderItemAddedEvent(Id, productId, quantity));
  }
  public void RemoveItem(OrderItemId orderItemId)
  {
    EnsureIsPending();
    var orderItem = _orderItems.FirstOrDefault(item => item.Id == orderItemId)
    ?? throw new DomainException($"Order Item with Id: {orderItemId.Value} Not Exist");

    _orderItems.Remove(orderItem);
    ReCalculateTotalPrice();
    AddDomainEvent(new OrderItemRemovedEvent(Id, orderItem.Id, orderItem.ProductId));
  }
  public void Confirm()
  {
    EnsureIsPending();
    Status = OrderStatus.CONFIRMED;
    AddDomainEvent(new OrderConfirmedEvent(Id));
  }
  public void Cancel()
  {
    if (Status == OrderStatus.CANCELLED)
      throw new DomainException($"Order with status: {Status} can't be cancelled.");

    Status = OrderStatus.CANCELLED;
    AddDomainEvent(new OrderCancelledEvent(Id));
  }

  ///!------------- Private Methods -------------\\\
  private void EnsureIsPending()
  {
    if (Status != OrderStatus.PENDING)
      throw new DomainException($"Order with status: {Status} doesn't allow that operation.");
  }
  private void ReCalculateTotalPrice()
  {
    TotalPrice = _orderItems.Aggregate(Money.Zero(),
    (sum, item) => sum.Add(item.TotalPrice));
  }

}
