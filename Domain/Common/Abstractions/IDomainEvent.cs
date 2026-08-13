namespace Domain.Common.Abstractions;

public interface IDomainEvent
{
  Guid EventId { get; }
  DateTime OccurredOnUtc { get; }
}
