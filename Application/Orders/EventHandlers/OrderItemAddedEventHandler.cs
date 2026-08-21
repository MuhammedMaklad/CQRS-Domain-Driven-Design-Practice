


using Application.Common.Events;
using Domain.Aggregates.OrderAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Orders.EventHandlers;

public sealed class OrderItemAddedEventHandler(ILogger<OrderItemAddedEventHandler> logger)
: INotificationHandler<DomainEventNotification<OrderItemAddedEvent>>
{
  public Task Handle(DomainEventNotification<OrderItemAddedEvent> notification, CancellationToken cancellationToken)
  {
    var domainEvent = notification.DomainEvent;

    logger.LogInformation("Order {OrderId}: added Product {ProductId} x{Quantity} at {OccurredOnUtc}",
      domainEvent.OrderId.Value, domainEvent.ProductId.Value,
      domainEvent.Quantity, domainEvent.OccurredOnUtc);

    return Task.CompletedTask;
  }
}
