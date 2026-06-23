namespace LedgerLite.Domain.Accounts;

// WHY: AccountType is an enum to restrict the type of financial accounts to a known set of options, enforcing type-safety across the application.
public enum AccountType
{
    Checking,
    Savings,
    CreditCard,
    Investment
}
