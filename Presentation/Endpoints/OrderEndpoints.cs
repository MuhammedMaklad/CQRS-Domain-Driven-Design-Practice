using Application.Orders.Queries.GetOrderById;
using Application.Orders.Queries.GetOrders;
using Application.Orders.Commands.CreateOrder;
using Domain.Common.Exceptions;
using MediatR;

namespace Presentation.Endpoints;

internal static class OrderEndpoints
{
  internal static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/v1/orders");

    group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken token) =>
    {
      var result = await sender.Send(new GetOrderByIdQuery(id), token);
      return result is null ? Results.NotFound() : Results.Ok(new
      {
        Message = "Order Retrieve Successfully",
        data = result
      });
    });
    group.MapGet("/customer/{id:guid}", async (Guid id, ISender sender, CancellationToken token) =>
    {
      var result = await sender.Send(new GetCustomerOrdersQuery(id), token);
      return Results.Ok(new
      {
        Message = "Order Retrieve Successfully",
        data = result
      });
    });

    group.MapPost("/create", async (CreateOrderCommand command, ISender sender, CancellationToken token) =>
    {
      try
      {
        var id = await sender.Send(command, cancellationToken: token);
        return Results.Created($"/api/v1/orders/{id}", new
        {
          Message = "Order Created Successfully",
          data = new
          {
            id
          }
        });
      }
      catch (DomainException ex)
      {
        return Results.BadRequest(new { ex.Message });
      }
    });
    return app;
  }
}