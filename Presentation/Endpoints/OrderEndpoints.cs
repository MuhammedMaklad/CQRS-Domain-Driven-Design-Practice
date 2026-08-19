using Application.Order.Queries.GetOrderById;
using Application.Order.Queries.GetOrders;
using MediatR;

namespace Presentation.Endpoints;

internal static class OrderEndpoints
{
  internal static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/v1/orders");

    app.MapGet("/{id:guid}", async (Guid id, ISender Sender, CancellationToken token) =>
    {
      var result = await Sender.Send(new GetOrderByIdQuery(id), token);
      return result is null ? Results.NotFound() : Results.Ok(new
      {
        Message = "Order Retrieve Successfully",
        data = result
      });
    });
    app.MapGet("/customer/{id:guid}", async (Guid id, ISender Sender, CancellationToken token) =>
    {
      var result = await Sender.Send(new GetCustomerOrdersQuery(id), token);
      return result is null ? Results.NotFound() : Results.Ok(new
      {
        Message = "Order Retrieve Successfully",
        data = result
      });
    });
    return app;
  }
}
