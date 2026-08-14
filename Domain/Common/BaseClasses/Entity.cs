namespace Domain.Common.BaseClasses;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
  public TId Id { get; protected set; } = default!;

  protected Entity() { }
  protected Entity(TId id) => Id = id;
  public bool Equals(Entity<TId>? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    if (GetType() != other.GetType()) return false;
    // EqualityComparer<TId>.Default.Equals(Id, other.Id);
    return EqualityComparer<TId>.Default.Equals(Id, other.Id);
  }

  override public bool Equals(object? obj)
  {
    if (obj is null) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (GetType() != obj.GetType()) return false;

    return Equals((Entity<TId>)obj);
  }
  override public int GetHashCode() => Id.GetHashCode() ^ 31;

  public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => left is null ? right is null : left.Equals(right);
  public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
