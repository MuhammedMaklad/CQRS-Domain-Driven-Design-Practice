
using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;

public sealed record Address
{
  public string Street { get; }
  public string City { get; }
  public string PostalCode { get; }
  public string Country { get; }

  private Address() { } // For EF Core

  internal Address(string street, string city, string postalCode, string country)
  {
    if (string.IsNullOrWhiteSpace(street)) throw new DomainException("Street is required.");
    if (string.IsNullOrWhiteSpace(city)) throw new DomainException("City is required.");
    if (string.IsNullOrWhiteSpace(postalCode)) throw new DomainException("Postal code is required.");
    if (string.IsNullOrWhiteSpace(country)) throw new DomainException("Country is required.");

    Street = street;
    City = city;
    PostalCode = postalCode;
    Country = country;
  }
}
