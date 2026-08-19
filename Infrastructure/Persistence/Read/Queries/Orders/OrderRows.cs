internal sealed record OrderRow(
  Guid Id, string Status, Guid CustomerId,
  string ShippingAddress_City, string ShippingAddress_Country,
  string ShippingAddress_PostalCode, string ShippingAddress_Street,
  decimal TotalPrice_Amount, string TotalPrice_Currency);

internal sealed record OrderItemRow(
  Guid Id, Guid ProductId, int Quantity,
  decimal UnitPrice_Amount, string UnitPrice_Currency);
