

using System.Data;
using Domain.Aggregates.OrderAggregate.Entities;
using FluentValidation;

namespace Application.Orders.Commands.AddOrderItem;

public sealed class AddOrderItemValidation
: AbstractValidator<AddOrderItemCommand>
{
  public AddOrderItemValidation()
  {
    RuleFor(prop => prop.OrderId).NotEmpty();

    RuleFor(prop => prop.ProductId).NotEmpty();

    RuleFor(prop => prop.UnitPrice)
    .GreaterThan(0);

    RuleFor(prop => prop.Quantity).GreaterThan(0).LessThan(OrderItem.MaxQuantityPerLine);

    RuleFor(prop => prop.Currency).NotEmpty();
  }
}
