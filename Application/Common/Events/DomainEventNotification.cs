using Domain.Common.Abstractions;
using MediatR;

namespace Application.Common.Events;

public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
: INotification where TDomainEvent : IDomainEvent
{
  public TDomainEvent DomainEvent { get; } = DomainEvent;
}
