using Application.Orders.Queries.ReadModels;
using MediatR;

namespace Application.Orders.Queries.GetOrders;


public sealed record GetCustomerOrdersQuery(Guid customerId) : IRequest<IEnumerable<OrderReadModel>> { }
