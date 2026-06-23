using LedgerLite.Domain.Enum;

namespace LedgerLite.Api.Validation;

// WHY: Request DTOs decouple the API contract from domain models.
// FluentValidation validates these before hitting handlers.
public record CreateTransactionRequest
{
    public Guid AccountId { get; init; }
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string Category { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
