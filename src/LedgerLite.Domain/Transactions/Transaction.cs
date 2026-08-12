using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;

namespace LedgerLite.Domain.Transactions;

// WHY: Transaction is an entity because it has a unique identity (Id) and represents a distinct event in the ledger.
// WHY: abstract ensures every subclass (Income, Expense) declares its type explicitly. The compiler prevents a Transaction subclass from omitting the type.
public abstract class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public Category Category { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
    public string Description { get; private set; }
    public abstract TransactionType Type { get; }

    // WHY: Parameterless constructor for EF Core. Don't call this directly in domain code.
    protected Transaction()
    {
    }

    public Transaction(
        Guid id,
        Guid accountId,
        Money amount,
        Category category,
        DateTimeOffset timestamp,
        string description)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Transaction ID cannot be empty.", nameof(id));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));
        }

        if (amount.Amount == 0)
        {
            throw new ArgumentException("Transaction amount cannot be zero.", nameof(amount));
        }

        Id = id;
        AccountId = accountId;
        Amount = amount;
        Category = category;
        Timestamp = timestamp;
        Description = description?.Trim() ?? string.Empty;
    }
}
