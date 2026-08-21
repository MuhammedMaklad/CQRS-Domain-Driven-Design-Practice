using Application.Common.Events;
using Domain.Aggregates.OrderAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Orders.EventHandlers;

public sealed class OrderConfirmedEventHandler(ILogger<OrderConfirmedEventHandler> logger)
: INotificationHandler<DomainEventNotification<OrderConfirmedEvent>>
{
  public Task Handle(DomainEventNotification<OrderConfirmedEvent> notification, CancellationToken cancellationToken)
  {
    var domainEvent = notification.DomainEvent;
    logger.LogInformation(
    "Order {OrderId} confirmed at {OccurredOnUtc}",
    domainEvent.OrderId.Value, domainEvent.OccurredOnUtc);

    return Task.CompletedTask;
  }
}
