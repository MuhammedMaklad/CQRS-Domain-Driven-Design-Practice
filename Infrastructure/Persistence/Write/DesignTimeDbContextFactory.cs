using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Write;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  public ApplicationDbContext CreateDbContext(string[] args)
  {

    var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation");
    var config = new ConfigurationBuilder()
      .SetBasePath(path)
      .AddJsonFile("appsettings.json", optional: true)
      .AddJsonFile("appsettings.Development.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

    var connectionString = config.GetConnectionString("Default");
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

    optionsBuilder.UseSqlServer(connectionString);

    return new ApplicationDbContext(optionsBuilder.Options);
  }
}
