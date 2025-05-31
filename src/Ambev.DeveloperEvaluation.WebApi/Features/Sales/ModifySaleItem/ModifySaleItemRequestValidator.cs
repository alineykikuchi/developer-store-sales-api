using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem
{
    /// <summary>
    /// Validator for ModifySaleItemRequest
    /// </summary>
    public class ModifySaleItemRequestValidator : AbstractValidator<ModifySaleItemRequest>
    {
        public ModifySaleItemRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .When(x => x.Quantity.HasValue)
                .WithMessage("Quantity must be greater than zero");

            RuleFor(x => x.Quantity)
                .LessThanOrEqualTo(20)
                .When(x => x.Quantity.HasValue)
                .WithMessage("Cannot have more than 20 identical items");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .When(x => x.UnitPrice.HasValue)
                .WithMessage("Unit price must be greater than zero");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency must be 3 characters (e.g., BRL, USD)");

            // At least one field must be provided for modification
            RuleFor(x => x)
                .Must(x => x.Quantity.HasValue || x.UnitPrice.HasValue)
                .WithMessage("At least quantity or unit price must be provided for modification");
        }
    }
}
