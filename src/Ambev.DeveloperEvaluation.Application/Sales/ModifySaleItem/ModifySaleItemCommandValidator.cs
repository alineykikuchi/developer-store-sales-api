using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem
{
    /// <summary>
    /// Validator for ModifySaleItemCommand
    /// </summary>
    public class ModifySaleItemCommandValidator : AbstractValidator<ModifySaleItemCommand>
    {
        public ModifySaleItemCommandValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty()
                .WithMessage("Sale ID is required");

            RuleFor(x => x.ItemId)
                .NotEmpty()
                .WithMessage("Item ID is required");

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
