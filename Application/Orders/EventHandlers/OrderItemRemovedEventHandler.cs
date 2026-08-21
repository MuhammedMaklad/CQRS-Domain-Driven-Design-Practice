using Application.Common.Events;
using Domain.Aggregates.OrderAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Orders.EventHandlers;

public sealed class OrderItemRemovedEventHandler(ILogger<OrderItemRemovedEventHandler> logger)
: INotificationHandler<DomainEventNotification<OrderItemRemovedEvent>>
{
  public Task Handle(DomainEventNotification<OrderItemRemovedEvent> notification, CancellationToken cancellationToken)
  {
    var domainEvent = notification.DomainEvent;
    logger.LogInformation(
    "Order {OrderId}: removed Item {OrderItemId} (Product {ProductId}) at {OccurredOnUtc}",
    domainEvent.OrderId.Value, domainEvent.OrderItemId.Value,
    domainEvent.ProductId.Value, domainEvent.OccurredOnUtc);

    return Task.CompletedTask;
  }
}
