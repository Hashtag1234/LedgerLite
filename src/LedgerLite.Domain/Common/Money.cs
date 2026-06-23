namespace LedgerLite.Domain.Common;

// WHY: Money is a Value Object where equality is based on value rather than identity. A readonly record struct guarantees immutability and value-based equality.
public readonly record struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if( amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        }

        if (currency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-character ISO code.", nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    // WHY: Operator overloading allows natural mathematical syntax for value objects while enforcing domain invariants (matching currencies).
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot add money with different currencies: {left.Currency} and {right.Currency}");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {left.Currency} and {right.Currency}");
        }

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }
}
