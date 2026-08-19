using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;

public readonly record struct CustomerId(Guid Value)
{
  public static CustomerId New() => new(Guid.NewGuid());
  public static CustomerId From(Guid value)
  {
    if (Guid.Empty == value)
      throw new DomainException("Invalid GUID for Customer");
    return new(value);
  }
}
