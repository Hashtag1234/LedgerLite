namespace LedgerLite.Domain.Enum;

// WHY: The enum acts as a discriminator — it indicates the transaction kind at runtime without explicit type checks or reflection.
// Naming it TransactionType (not just "Type") keeps it unambiguous when
// used alongside other enums in the same namespace.
public enum TransactionType
{
    Income,
    Expense
}
