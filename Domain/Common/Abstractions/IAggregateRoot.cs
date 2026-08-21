using System.Collections.ObjectModel;

namespace Domain.Common.Abstractions;

public interface IAggregateRoot
{
  ReadOnlyCollection<IDomainEvent> DomainEvents { get; }

  void ClearDomainEvents();
};
