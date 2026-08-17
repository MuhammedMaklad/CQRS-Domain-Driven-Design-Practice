using Application.Common.interfaces;
using Domain.Aggregates;
// using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Write;

public class ApplicationDbContext : DbContext
{
  public DbSet<Order> Orders { get; set; }
  public ApplicationDbContext()
  {

  }
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
  }
}
