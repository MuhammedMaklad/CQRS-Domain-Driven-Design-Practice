using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;


public readonly record struct OrderItemId(Guid Value)
{
  public static OrderItemId New() => new(Guid.NewGuid());
  public static OrderItemId From(Guid value)
  {
    if (value == Guid.Empty)
      throw new DomainException("OrderItemId Can't be Null");
    return new(value);
  }
}
