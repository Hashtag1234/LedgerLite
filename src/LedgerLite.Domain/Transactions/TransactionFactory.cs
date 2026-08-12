using System;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Enum;

namespace LedgerLite.Domain.Transactions;

// WHY: This factory centralizes construction logic for transaction subtypes,
// keeps calling code simple, and enforces invariants when creating transactions.
public static class TransactionFactory
{
    public static Transaction Create(
        TransactionType type,
        Guid id,
        Guid accountId,
        Money amount,
        Category category,
        DateTimeOffset timestamp,
        string description)
    {
        if (type == TransactionType.Income)
        {
            return new Income(id, accountId, amount, category, timestamp, description);
        }

        return new Expense(id, accountId, amount, category, timestamp, description);
    }
}
