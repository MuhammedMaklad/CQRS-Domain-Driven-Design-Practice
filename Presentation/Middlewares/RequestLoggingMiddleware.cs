
using System.Diagnostics;

namespace Presentation.Middlewares;


public sealed class RequestLoggingMiddleware
{
  private readonly RequestDelegate next;
  private readonly ILogger<RequestLoggingMiddleware> logger;

  public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
  {
    this.next = next;
    this.logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    var sw = Stopwatch.StartNew();
    var request = context.Request;
    try
    {
      await next(context);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Request {Method} {Path} failed", request.Method, request.Path);
      throw;

    }
    finally
    {
      sw.Stop();
      logger.LogInformation("{Method} {Path} {Status} {Elapsed}ms",
          request.Method, request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);

    }
  }
}
