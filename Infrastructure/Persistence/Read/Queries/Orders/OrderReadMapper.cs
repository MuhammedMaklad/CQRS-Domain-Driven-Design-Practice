using Application.Order.Queries.ReadModels;
using Domain.Aggregates.OrderAggregate.Enums;

internal static class OrderReadMapper
{
  internal static OrderReadModel Map(OrderRow order, IEnumerable<OrderItemRow> items)
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
}
