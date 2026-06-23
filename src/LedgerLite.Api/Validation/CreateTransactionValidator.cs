using FluentValidation;
using LedgerLite.Api.Validation;

namespace LedgerLite.Api.Validation;

// WHY: Validators encapsulate business rules for API inputs. FluentValidation
// integrates with endpoint filters to reject invalid requests early.
public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID cannot be empty.");

        RuleFor(x => x.Amount)
            .NotEqual(0)
            .WithMessage("Transaction amount cannot be zero.")
            .GreaterThan(0)
            .WithMessage("Transaction amount must be positive (sign is handled by Type).");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency cannot be empty.")
            .Length(3)
            .WithMessage("Currency must be a 3-character ISO code.");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category cannot be empty.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid transaction type.");
    }
}
