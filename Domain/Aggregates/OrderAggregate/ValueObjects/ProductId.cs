using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;

public readonly record struct ProductId(Guid Value)
{
  public static ProductId New() => new(Guid.NewGuid());
  public static ProductId From(Guid value)
  {
    if (Guid.Empty == value)
      throw new DomainException("Invalid GUID value");
    return new(value);
  }
}
