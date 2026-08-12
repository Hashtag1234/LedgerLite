using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace LedgerLite.Api.Validation;

// WHY: Provide a lightweight injectable wrapper type that endpoints can request
// directly as `ValidationFilter<T>.AddEndpointFilter` while still using
// FluentValidation under the hood.
public class ValidationFilter<T>
{
    // Public nested class so callers can request the closed generic type
    // `ValidationFilter<MyRequest>.AddEndpointFilter` from DI.
    public class AddEndpointFilter
    {
        private readonly IValidator<T> _validator;

        public AddEndpointFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        // Forwarding method with the same signature you expect to call
        // from endpoints. Returns FluentValidation's ValidationResult.
        public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default)
        {
            return _validator.ValidateAsync(instance, cancellationToken);
        }

        // Optional synchronous helper
        public ValidationResult Validate(T instance)
        {
            return _validator.Validate(instance);
        }
    }
}
