


namespace Infrastructure.Persistence.Write.Outbox;


public sealed class OutboxMessage
{
  public Guid Id { get; private set; }
  public string Type { get; private set; } = string.Empty;
  public string Content { get; private set; } = string.Empty;
  public DateTime OccurredOnUtc { get; private set; }
  public DateTime? ProcessedOnUtc { get; private set; }
  public int Attempts { get; private set; } = 0;
  public string? Error { get; private set; }


  private OutboxMessage() { }

  internal OutboxMessage(string type, string content)
  {
    Id = Guid.NewGuid();
    Type = type;
    Content = content;
    OccurredOnUtc = DateTime.UtcNow;
  }
  internal void MarkProcessed(DateTime dateTime) => ProcessedOnUtc = dateTime;
  internal void MarkFailed(string error)
  {
    Attempts++;
    Error = error;
  }
}
