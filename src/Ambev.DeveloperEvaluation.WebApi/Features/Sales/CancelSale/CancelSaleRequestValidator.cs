using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale
{
    /// <summary>
    /// Validator for CancelSaleRequest
    /// </summary>
    public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
    {
        public CancelSaleRequestValidator()
        {
            RuleFor(x => x.CancellationReason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.CancellationReason))
                .WithMessage("Cancellation reason cannot exceed 500 characters");
        }
    }
}
