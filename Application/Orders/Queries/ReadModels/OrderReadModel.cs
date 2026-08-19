using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValueObjects;

namespace Application.Orders.Queries.ReadModels;

public sealed class OrderReadModel
{
  public Guid Id { get; set; }
  public Guid CustomerId { get; set; }
  public OrderStatus OrderStatus { get; set; }

  public string City { get; set; } = string.Empty;
  public string Street { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
  public string Country { get; set; } = string.Empty;

  public string TotalPriceCurrency { get; set; } = string.Empty;
  public decimal TotalPriceAmount { get; set; }

  public IReadOnlyList<OrderItemReadModel> Items { get; set; } = [];
}
