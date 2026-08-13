

using Domain.Common.Abstractions;

namespace Domain.BaseClasses;

public abstract record DomainEvent : IDomainEvent
{
  public Guid EventId { get; init; } = Guid.NewGuid();

  public DateTime OccurredOnUtc { get; init; }
}
