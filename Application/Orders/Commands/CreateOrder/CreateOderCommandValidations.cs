

using Application.Orders.Commands.CreateOrder;
using Domain.Aggregates.OrderAggregate.Entities;
using FluentValidation;

namespace Application.Orders.Commands;


public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
  public CreateOrderCommandValidator()
  {
    RuleFor(prop => prop.CustomerId).NotEmpty();
    RuleFor(prop => prop.ShippingAddress).NotEmpty().SetValidator(new ShippingAddressValidator());
    RuleFor(prop => prop.ItemRequests).NotEmpty().WithMessage("Order must contain at least one item.");
    RuleForEach(prop => prop.ItemRequests).SetValidator(new ItemRequestValidator());
  }
}

public sealed class ShippingAddressValidator : AbstractValidator<ShippingAddress>
{
  public ShippingAddressValidator()
  {
    RuleFor(x => x.Street).NotEmpty();
    RuleFor(x => x.City).NotEmpty();
    RuleFor(x => x.PostalCode).NotEmpty();
    RuleFor(x => x.Country).NotEmpty();
  }
}

public sealed class ItemRequestValidator : AbstractValidator<ItemRequest>
{
  public ItemRequestValidator()
  {
    RuleFor(x => x.ProductId).NotEmpty();
    RuleFor(x => x.UnitPrice).GreaterThan(0);
    RuleFor(x => x.Currency).NotEmpty();
    RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(OrderItem.MaxQuantityPerLine);
  }
}
