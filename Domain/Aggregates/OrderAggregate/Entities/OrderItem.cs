
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Domain.Common.BaseClasses;
using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.Entities;

public sealed class OrderItem : Entity<int>
{
  public OrderId OrderId { get; private set; }
  public ProductId ProductId { get; private set; }
  public Money UnitPrice { get; private set; }
  public int Quantity { get; private set; }
  public Money TotalPrice => UnitPrice.Multiply(Quantity);

  public DateTime CreatedAt { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  private OrderItem(OrderId orderId, ProductId productId, int price, string currency, int quantity)
  {
    OrderId = orderId;
    ProductId = productId;
    UnitPrice = Money.Create(price, currency);
    Quantity = quantity;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  public static OrderItem Create(OrderId orderId, ProductId productId, int price, string currency = "USD", int quantity = 1)
  {
    if (quantity <= 0)
      throw new DomainException("Invalid Quantity for Order Item");

    return new OrderItem(orderId, productId, price, currency, quantity);
  }
}
