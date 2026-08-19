using Application.Order.Queries.ReadModels;
using MediatR;

namespace Application.Order.Queries.GetOrders;


public sealed record GetCustomerOrdersQuery(Guid customerId) : IRequest<IEnumerable<OrderReadModel>> { }
