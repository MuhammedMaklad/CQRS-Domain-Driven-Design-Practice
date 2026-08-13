using System.Collections.ObjectModel;
using Domain.Common.Abstractions;

namespace Domain.Common.BaseClasses;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : notnull
{
  private readonly List<IDomainEvent> _domainEvents = [];
  public ReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  protected AggregateRoot() { } // for entity framework
  protected AggregateRoot(TId id) : base(id) { }
  /// ! Methods
  public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
  public void DeleteDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
  public void ClearDomainEvent() => _domainEvents.Clear();
}
