using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "MKD")
    {
        if (amount < 0)
            throw new OrderDomainException("A monetary amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new OrderDomainException("A currency code is required.");

        Amount = amount;
        Currency = currency;
    }

    public static Money Zero { get; } = new(0);

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public static Money operator *(Money money, int factor) => new(money.Amount * factor, money.Currency);

    public static Money operator *(Money money, decimal factor) => new(money.Amount * factor, money.Currency);

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new OrderDomainException($"Cannot combine {left.Currency} with {right.Currency}.");
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
