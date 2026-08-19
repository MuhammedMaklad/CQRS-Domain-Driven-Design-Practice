

using Domain.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Presentation.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    if (exception is DomainException domainException)
    {
      httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
      await httpContext.Response.WriteAsJsonAsync(new { Message = domainException.Message }, cancellationToken);
      return true;
    }
    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await httpContext.Response.WriteAsJsonAsync(new
    {
      Message = "Internal Server Error",
      Inspect = exception.Message
    }, cancellationToken);
    return true;
  }
}
