namespace Application.Order.Queries.ReadModels;

public sealed class OrderItemReadModel
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPriceAmount { get; set; }
  public string UnitPriceCurrency { get; set; } = string.Empty;
}
