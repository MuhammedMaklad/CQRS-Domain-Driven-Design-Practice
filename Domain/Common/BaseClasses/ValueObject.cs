
namespace Domain.Common.BaseClasses;

public abstract class ValueObject : IEquatable<ValueObject>
{
  protected abstract IEnumerable<object> GetObjectValues();
  public bool Equals(ValueObject? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    if (GetType() != other.GetType()) return false;

    return GetObjectValues().SequenceEqual(other.GetObjectValues());
  }

  override public bool Equals(object? obj)
  {
    if (obj is null) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (GetType() != obj.GetType()) return false;

    return Equals((ValueObject)obj);
  }
  override public int GetHashCode()
  {
    return GetObjectValues()
      .Select(x => x?.GetHashCode() ?? 0)
      .Aggregate((x, y) => x ^ y);
  }
  public static bool operator ==(ValueObject? left, ValueObject? right) => left is not null && left.Equals(right);
  public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
