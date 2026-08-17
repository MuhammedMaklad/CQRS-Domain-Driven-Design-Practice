using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Write.Configurations;

public sealed class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
{
  public void Configure(EntityTypeBuilder<OrderItem> builder)
  {
    builder.ToTable("OrderItems");

    builder.HasKey(item => item.Id);
    builder.Property(item => item.Id)
      .HasConversion(id => id.Value, value => OrderItemId.From(value));

    builder.Property(item => item.ProductId)
      .HasConversion(id => id.Value, value => new ProductId(value));

    builder.ComplexProperty(item => item.UnitPrice, money =>
    {
      money.Property(m => m.Amount);
      money.Property(m => m.Currency);
    });

    builder.Ignore(item => item.TotalPrice);
  }
}
