using Application.Orders.Queries.ReadModels;
using MediatR;

namespace Application.Orders.Queries.GetOrderById;


public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderReadModel?> { }
