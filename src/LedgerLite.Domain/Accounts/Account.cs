using LedgerLite.Domain.Common;

namespace LedgerLite.Domain.Accounts;

// WHY: Account is an Entity because it has a distinct identity (Id) and its state changes over time.
public class Account
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public AccountType Type { get; private set; }
    public Money Balance { get; private set; }

    // WHY: Parameterless constructor for EF Core. Don't call this directly in domain code.
    private Account()
    {
        Name = string.Empty;
    }

    public Account(Guid id, string name, AccountType type, Money balance)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Account ID cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
        Type = type;
        Balance = balance;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Account name cannot be empty.", nameof(newName));
        }

        Name = newName.Trim();
    }

    // WHY: Domain methods encapsulate business rules and prevent external code from modifying state in invalid ways.
    public void Deposit(Money amount)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
        }

        // Addition operator on Money handles currency mismatch validation
        Balance += amount;
    }

    public void Withdraw(Money amount)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));
        }

        // Subtraction operator on Money handles currency mismatch validation
        Balance -= amount;
    }
}
