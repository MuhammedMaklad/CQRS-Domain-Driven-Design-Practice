using Domain.Aggregates.OrderAggregate.ValueObjects;
using Domain.BaseClasses;

namespace Domain.Aggregates.OrderAggregate.Events;

public sealed record OrderConfirmedEvent(OrderId OrderId) : DomainEvent
{ }
