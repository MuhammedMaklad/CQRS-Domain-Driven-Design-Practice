

using System.Text.Json;
using Domain.Common.Abstractions;

namespace Infrastructure.Persistence.Write.Outbox;


public static class OutboxSerializer
{
  private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

  public static string Serialize(IDomainEvent domain)
  => JsonSerializer.Serialize(domain);

  public static IDomainEvent Deserialize(string type, string json)
  {
    var domainEventType = Type.GetType(type) ??
      throw new InvalidOperationException($"Unknown domain event type{type}");

    return (IDomainEvent)JsonSerializer.Deserialize(json, domainEventType, options)!;
  }
}
