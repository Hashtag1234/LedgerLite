using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;
namespace LedgerLite.Domain.Transactions;
public sealed class Expense : Transaction
{
    public override TransactionType Type => TransactionType.Expense;

    // WHY: EF Core requires a parameterless constructor to materialize derived entity types.
    public Expense()
    {
    }

    public Expense(
        Guid id,
        Guid accountId,
        Money amount,
        Category category,
        DateTimeOffset timestamp,
        string description)
        : base(id, accountId, amount, category, timestamp, description)
    {
    }
}