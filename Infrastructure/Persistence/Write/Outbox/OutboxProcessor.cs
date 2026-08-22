using Application.Common.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Write.Outbox;


internal sealed class OutboxProcessor(
  IServiceScopeFactory scopeFactory,
  ILogger<OutboxProcessor> logger
) : BackgroundService
{
  private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
  private const int BatchSize = 20;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await ProcessPendingAsync(stoppingToken);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Outbox processing tick failed");
      }

      try
      {
        await Task.Delay(PollInterval, stoppingToken);
      }
      catch (OperationCanceledException)
      {
        break;
      }
    }
  }

  private async Task ProcessPendingAsync(CancellationToken token)
  {
    using var scope = scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

    var messages = await context.Set<OutboxMessage>()
    .Where(message => message.ProcessedOnUtc == null)
    .OrderBy(message => message.OccurredOnUtc)
    .Take(BatchSize)
    .ToListAsync(token);

    foreach (var message in messages)
    {
      try
      {
        var domainEvent = OutboxSerializer.Deserialize(message.Type, message.Content);
        await eventDispatcher.DispatchAsync([domainEvent], token);
        message.MarkProcessed(DateTime.UtcNow);
      }
      catch (Exception ex)
      {
        message.MarkFailed(ex.Message);
        logger.LogError(ex, "Failed to process outbox message {OutboxMessageId} (attempt {Attempts})", message.Id, message.Attempts);
      }
    }

    await context.SaveChangesAsync(token);
  }
}
