using Domain.Common.Abstractions;

namespace Application.Common.interfaces;

public interface IEventDispatcher
{
  Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken token = default);
}
