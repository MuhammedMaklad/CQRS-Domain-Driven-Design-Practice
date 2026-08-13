using Domain.Common;
using Domain.Common.BaseClasses;
using Domain.Common.Exceptions;

namespace Domain.Aggregates.OrderAggregate.ValueObjects;


public sealed class Money : ValueObject
{
  public decimal Amount { get; private set; }
  public string Currency { get; private set; } = string.Empty;

  private Money(decimal amount, string currency)
  {
    Amount = amount;
    Currency = currency;
  }

  public static Money Create(decimal amount, string currency)
  {
    if (amount < 0) throw new DomainException("Amount cannot be negative.");
    if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency cannot be null or empty.");

    return new Money(amount, currency);
  }
  public static Money Zero(string currency = "USD")
  {
    if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency cannot be null or empty.");

    return new Money(0, currency);
  }

  public Money Add(Money other)
  {
    if (Currency != other.Currency) throw new DomainException("Cannot add amounts with different currencies.");
    if (other.Amount < 0) throw new DomainException("Cannot add a negative amount.");
    return new Money(Amount + other.Amount, Currency);
  }

  public Money Subtract(Money other)
  {
    if (Currency != other.Currency) throw new DomainException("Cannot subtract amounts with different currencies.");
    if (other.Amount < 0) throw new DomainException("Cannot subtract a negative amount.");
    if (Amount - other.Amount < 0) throw new DomainException("Resulting amount cannot be negative.");
    return new Money(Amount - other.Amount, Currency);
  }

  public Money Multiply(decimal factor)
  {
    if (factor < 0) throw new DomainException("Factor cannot be negative.");
    return new Money(Amount * factor, Currency);
  }

  public Money ApplyDiscount(decimal percentage)
  {
    if (percentage < 0 || percentage > 100) throw new DomainException("Percentage must be between 0 and 100.");
    var discountAmount = Amount * (percentage / 100);
    return new Money(Amount - discountAmount, Currency);
  }
  protected override IEnumerable<object> GetObjectValues()
  {
    yield return Amount;
    yield return Currency;
  }
}
