
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Domain.Common.BaseClasses;
using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
  public const int MaxQuantityPerLine = 100;

  public ProductId ProductId { get; private set; }
  public Money UnitPrice { get; private set; }
  public int Quantity { get; private set; }
  public Money TotalPrice => UnitPrice.Multiply(Quantity);

  public DateTime CreatedAt { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  private OrderItem() { }
  private OrderItem(ProductId productId, Money price, int quantity) : base(OrderItemId.New())
  {
    ProductId = productId;
    UnitPrice = price;
    Quantity = quantity;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  public static OrderItem Create(ProductId productId, Money price, int quantity = 1)
  {
    if (quantity <= 0)
      throw new DomainException("Invalid Quantity for Order Item");

    return new OrderItem(productId, price, quantity);
  }
  public void UpdateQuantity(int value)
  {
    if (value <= 0 || value > MaxQuantityPerLine)
      throw new DomainException($"Quantity must be between 1 and {MaxQuantityPerLine}.");
    Quantity = value;
    UpdatedAt = DateTime.UtcNow;
  }
  public void IncreaseQuantity(int value)
  {
    if (value <= 0 || Quantity + value > MaxQuantityPerLine)
      throw new DomainException($"Resulting quantity must be between 1 and {MaxQuantityPerLine}.");
    Quantity += value;
    UpdatedAt = DateTime.UtcNow;
  }
  public void DecreaseQuantity(int value)
  {
    if (value <= 0 || value > Quantity)
      throw new DomainException("Cannot remove more than the current quantity.");
    Quantity -= value;
    UpdatedAt = DateTime.UtcNow;
  }
}
