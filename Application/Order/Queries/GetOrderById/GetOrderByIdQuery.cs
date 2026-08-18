using Application.Order.Queries.ReadModels;
using MediatR;

namespace Application.Order.Queries.GetOrderById;


public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderReadModel?> { }
