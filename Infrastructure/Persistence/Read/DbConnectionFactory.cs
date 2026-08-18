using System.Data;
using Application.Common.interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Read;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
  private readonly string connectionString;

  public DbConnectionFactory(IConfiguration configuration)
  => connectionString = configuration.GetConnectionString("Default")!;
  public IDbConnection CreateConnection()
  => new SqlConnection(connectionString);
}
