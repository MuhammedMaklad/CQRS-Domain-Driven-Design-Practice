namespace Infrastructure.Persistence.Read.Queries.Orders;

using Application.Common.interfaces;
using Application.Order.Queries;
using Application.Order.Queries.ReadModels;
using Dapper;
using Domain.Aggregates.OrderAggregate.Enums;

internal sealed class OrderQueries : IOrderQueryService
{
  private readonly IDbConnectionFactory _dbConnectionFactory;
  public OrderQueries(IDbConnectionFactory dbConnectionFactory) => _dbConnectionFactory = dbConnectionFactory;

  public async Task<OrderReadModel?> GetOrderByIdAsync(Guid id, CancellationToken token)
  {
    const string sql = """
        SELECT Id, Status, CustomerId, ShippingAddress_City, ShippingAddress_Country,
               ShippingAddress_PostalCode, ShippingAddress_Street,
               TotalPrice_Amount, TotalPrice_Currency
        FROM Orders
        WHERE Id = @Id;

        SELECT Id, ProductId, Quantity, UnitPrice_Amount, UnitPrice_Currency
        FROM OrderItems
        WHERE OrderId = @Id;
        """;

    using var connection = _dbConnectionFactory.CreateConnection();

    var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: token);

    using var multi = await connection.QueryMultipleAsync(command);

    var orderRow = await multi.ReadSingleOrDefaultAsync<OrderRow>();
    if (orderRow is null)
      return null;
    var orderItemsRow = await multi.ReadAsync<OrderItemRow>();

    return Map(orderRow, orderItemsRow);
  }

  private static OrderReadModel Map(OrderRow order, IEnumerable<OrderItemRow> items)
  {
    Enum.TryParse<OrderStatus>(order.Status, ignoreCase: true, out var status);
    return new OrderReadModel
    {
      Id = order.Id,
      CustomerId = order.CustomerId,
      OrderStatus = status,
      City = order.ShippingAddress_City,
      Street = order.ShippingAddress_Street,
      PostalCode = order.ShippingAddress_PostalCode,
      Country = order.ShippingAddress_Country,
      TotalPriceAmount = order.TotalPrice_Amount,
      TotalPriceCurrency = order.TotalPrice_Currency,
      Items = items.Select(i => new OrderItemReadModel
      {
        Id = i.Id,
        ProductId = i.ProductId,
        Quantity = i.Quantity,
        UnitPriceAmount = i.UnitPrice_Amount,
        UnitPriceCurrency = i.UnitPrice_Currency
      }).ToList()
    };
  }

  private sealed record OrderRow(
    Guid Id, string Status, Guid CustomerId,
    string ShippingAddress_City, string ShippingAddress_Country,
    string ShippingAddress_PostalCode, string ShippingAddress_Street,
    decimal TotalPrice_Amount, string TotalPrice_Currency);

  private sealed record OrderItemRow(
    Guid Id, Guid ProductId, int Quantity,
    decimal UnitPrice_Amount, string UnitPrice_Currency);
}
