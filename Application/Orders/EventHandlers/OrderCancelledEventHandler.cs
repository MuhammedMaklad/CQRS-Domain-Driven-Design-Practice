using Application.Common.Events;
using Domain.Aggregates.OrderAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Orders.EventHandlers;

public sealed class OrderCancelledEventHandler(ILogger<OrderCancelledEventHandler> logger)
: INotificationHandler<DomainEventNotification<OrderCancelledEvent>>
{
  public Task Handle(DomainEventNotification<OrderCancelledEvent> notification, CancellationToken cancellationToken)
  {
    var domainEvent = notification.DomainEvent;
    logger.LogInformation(
    "Order {OrderId} cancelled at {OccurredOnUtc}",
    domainEvent.OrderId.Value, domainEvent.OccurredOnUtc);

    return Task.CompletedTask;
  }
}
