using LedgerLite.Domain.Enum;
using LedgerLite.Domain.Accounts;

namespace LedgerLite.Api.Validation;

public record CreateAccountRequest
{
    public string Name { get; init; } = string.Empty;
    public AccountType Type { get; init; }
    public decimal InitialBalance { get; init; }
    public string Currency { get; init; } = "USD";
}
