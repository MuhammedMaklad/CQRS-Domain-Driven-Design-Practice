


using Infrastructure.Persistence.Write.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Write.Configurations;

public sealed class OutboxMessageConfigurations : IEntityTypeConfiguration<OutboxMessage>
{
  public void Configure(EntityTypeBuilder<OutboxMessage> builder)
  {
    builder.ToTable("OutBoxMessages");
    builder.HasKey(prop => prop.Id);
    builder.Property(x => x.Type).HasMaxLength(400).IsRequired();
    builder.Property(x => x.Content).IsRequired();
    builder.HasIndex(x => x.ProcessedOnUtc);
  }
}
