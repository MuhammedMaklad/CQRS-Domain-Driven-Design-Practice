


namespace Infrastructure.Persistence.Write.Outbox;


public sealed class OutboxMessage
{
  public Guid Id { get; private set; }
  public string Type { get; private set; } = string.Empty;
  public string Content { get; private set; } = string.Empty;
  public DateTime OccurredOnUTC { get; private set; }
  public DateTime? ProceedInUTC { get; private set; }
  public int Attempts { get; private set; } = 0;
  public string? Error { get; private set; }


  private OutboxMessage() { }

  internal OutboxMessage(string type, string content)
  {
    Type = type;
    Content = content;
    OccurredOnUTC = DateTime.UtcNow;
  }
  internal void MarkProceed(DateTime dateTime) => ProceedInUTC = dateTime;
  internal void MarkField(string error)
  {
    Attempts++;
    Error = error;
  }
}
