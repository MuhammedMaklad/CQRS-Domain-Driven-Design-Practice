using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;

public readonly record struct OrderId(Guid Value)
{
  public static OrderId New() => new(Guid.NewGuid());
  public static OrderId From(Guid value)
  {
    if (value == Guid.Empty)
      throw new DomainException("OrderId Can't be Empty");
    return new(value);
  }
}
