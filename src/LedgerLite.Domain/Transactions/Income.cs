using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;
namespace LedgerLite.Domain.Transactions;
public sealed class Income : Transaction
{
    public override TransactionType Type => TransactionType.Income;
    
    public Income(
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