using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale
{
    /// <summary>
    /// Validator for AddItemToSaleRequest
    /// </summary>
    public class AddItemToSaleRequestValidator : AbstractValidator<AddItemToSaleRequest>
    {
        public AddItemToSaleRequestValidator()
        {
            RuleFor(x => x.Product)
                .NotNull()
                .WithMessage("Product information is required");

            RuleFor(x => x.Product.Id)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Product.Name)
                .NotEmpty()
                .WithMessage("Product name is required")
                .MaximumLength(100)
                .WithMessage("Product name cannot exceed 100 characters");

            RuleFor(x => x.Product.Description)
                .NotEmpty()
                .WithMessage("Product description is required")
                .MaximumLength(500)
                .WithMessage("Product description cannot exceed 500 characters");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero")
                .LessThanOrEqualTo(20)
                .WithMessage("Cannot add more than 20 identical items");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency must be 3 characters (e.g., BRL, USD)");
        }
    }
}
