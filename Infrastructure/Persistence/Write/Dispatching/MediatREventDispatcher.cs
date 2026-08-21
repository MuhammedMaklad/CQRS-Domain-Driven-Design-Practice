using Application.Common.Events;
using Application.Common.interfaces;
using Domain.Common.Abstractions;
using MediatR;

namespace Infrastructure.Persistence.Write.Dispatching;

internal sealed class MediatREventDispatcher(IPublisher publisher) : IEventDispatcher
{
  public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken token = default)
  {
    foreach (var domainEvent in domainEvents)
    {
      var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
      var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;

      await publisher.Publish(notification, token);
    }
  }
}
