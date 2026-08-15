using Domain.Aggregates;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class OrderConfigurations : IEntityTypeConfiguration<Order>
{
  public void Configure(EntityTypeBuilder<Order> builder)
  {
    builder.ToTable("Orders");

    builder.HasKey(order => order.Id);
    builder.Property(order => order.Id)
      .HasConversion(id => id.Value, value => OrderId.From(value));

    builder.Property(order => order.CustomerId)
      .HasConversion(id => id.Value, value => new CustomerId(value));

    builder.Property(order => order.Status)
      .HasConversion<string>();

    builder.ComplexProperty(order => order.TotalPrice, money =>
    {
      money.Property(m => m.Amount);
      money.Property(m => m.Currency);
    });

    builder.ComplexProperty(order => order.ShippingAddress, address =>
    {
      address.Property(a => a.Street);
      address.Property(a => a.City);
      address.Property(a => a.PostalCode);
      address.Property(a => a.Country);
    });

    builder.HasMany(order => order.OrderItems)
      .WithOne()
      .HasForeignKey("OrderId")
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(order => order.OrderItems)
      .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

    builder.Property<int[]>("RowVersion").IsRowVersion();
  }
}
