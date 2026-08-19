namespace Infrastructure.Persistence.Read.Queries.Orders;

using Application.Common.interfaces;
using Application.Orders.Queries;
using Application.Orders.Queries.ReadModels;
using Dapper;

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

    return OrderReadMapper.Map(orderRow, orderItemsRow);
  }

  public async Task<IEnumerable<OrderReadModel>> GetCustomerOrdersAsync(Guid customerId, CancellationToken token)
  {
    const string sql = """
      SELECT o.Id, o.Status, o.CustomerId,
             o.ShippingAddress_City, o.ShippingAddress_Country,
             o.ShippingAddress_PostalCode, o.ShippingAddress_Street,
             o.TotalPrice_Amount, o.TotalPrice_Currency,
             i.Id, i.ProductId, i.Quantity, i.UnitPrice_Amount, i.UnitPrice_Currency
      FROM Orders o
      LEFT JOIN OrderItems i ON o.Id = i.OrderId
      WHERE o.CustomerId = @CustomerId;
      """;

    using var connection = _dbConnectionFactory.CreateConnection();
    var command = new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: token);

    var rows = await connection.QueryAsync<OrderRow, OrderItemRow?, (OrderRow Order, OrderItemRow? Item)>(
      command, (order, item) => (order, item), splitOn: "Id");

    var grouped = new Dictionary<Guid, (OrderRow Order, List<OrderItemRow> Items)>();
    foreach (var (order, item) in rows)
    {
      if (!grouped.TryGetValue(order.Id, out var entry))
      {
        entry = (order, []);
        grouped[order.Id] = entry;
      }
      if (item is not null)
        entry.Items.Add(item);
    }

    return grouped.Values.Select(group => OrderReadMapper.Map(group.Order, group.Items));
  }
}
