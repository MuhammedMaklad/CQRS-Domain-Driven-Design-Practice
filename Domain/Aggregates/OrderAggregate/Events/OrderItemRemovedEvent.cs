using Domain.Aggregates.OrderAggregate.ValueObjects;
using Domain.BaseClasses;

namespace Domain.Aggregates.OrderAggregate.Events;

public sealed record OrderItemRemovedEvent
(OrderId OrderId, OrderItemId OrderItemId, ProductId ProductId) : DomainEvent
{ }
