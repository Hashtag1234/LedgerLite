namespace LedgerLite.Domain.Enum;

// WHY: Enum acts as a discriminator — tells you at runtime what kind of
// transaction you're dealing with without instanceof checks or reflection.
// Naming it TransactionType (not just "Type") keeps it unambiguous when
// used alongside other enums in the same namespace.
public enum TransactionType
{
    Income,
    Expense
}
