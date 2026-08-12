using FluentValidation;

namespace LedgerLite.Api.Validation;

public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name cannot be empty.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Account name cannot be whitespace.");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial balance must be zero or greater.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency cannot be empty.")
            .Length(3)
            .WithMessage("Currency must be a 3-character ISO code.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid account type.");
    }
}
