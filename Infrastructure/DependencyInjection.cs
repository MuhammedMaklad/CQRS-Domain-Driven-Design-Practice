using Application.Common.interfaces;
using Application.Order.Queries;
using Domain.Aggregates.OrderAggregate.Repositories;
using Infrastructure.Persistence.Read;
using Infrastructure.Persistence.Read.Queries.Orders;
using Infrastructure.Persistence.Write;
using Infrastructure.Persistence.Write.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<ApplicationDbContext>(cfg =>
    cfg.UseSqlServer(configuration.GetConnectionString("Default")));

    services.AddScoped<IUnitOfWork, EFUnitOfWork>();
    services.AddScoped<IOrderRepository, EFOrderRepository>();
    services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
    services.AddScoped<IOrderQueryService, OrderQueries>();

    return services;
  }
}
