using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;

namespace LedgerLite.Domain.Transactions;

// WHY: Transaction is an Entity because it has a unique identity (Id) and represents a distinct transaction event in the ledger.
// WHY: abstract forces every subclass (Income, Expense) to declare their type explicitly. The compiler enforces this — you cannot create a Transaction subclass that "forgets" to say what it is.
public abstract class Transaction
{
    public Guid Id { get; }
    public Guid AccountId { get; }
    public Money Amount { get; }
    public Category Category { get; }
    public DateTimeOffset Timestamp { get; }
    public string Description { get; } 
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
