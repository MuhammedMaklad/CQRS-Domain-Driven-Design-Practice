using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

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
  }
}
