using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    /// <summary>
    /// Validator for AddItemToSaleCommand
    /// </summary>
    public class AddItemToSaleCommandValidator : AbstractValidator<AddItemToSaleCommand>
    {
        public AddItemToSaleCommandValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty()
                .WithMessage("Sale ID is required");

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
