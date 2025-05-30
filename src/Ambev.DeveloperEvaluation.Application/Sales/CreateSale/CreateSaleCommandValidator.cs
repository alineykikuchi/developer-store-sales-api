using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Validator for CreateSaleCommand
    /// </summary>
    public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty()
                .WithMessage("Sale number is required")
                .MaximumLength(50)
                .WithMessage("Sale number cannot exceed 50 characters");

            RuleFor(x => x.Customer)
                .NotNull()
                .WithMessage("Customer information is required");

            RuleFor(x => x.Customer.Id)
                .NotEmpty()
                .WithMessage("Customer ID is required");

            RuleFor(x => x.Customer.Name)
                .NotEmpty()
                .WithMessage("Customer name is required")
                .MaximumLength(100)
                .WithMessage("Customer name cannot exceed 100 characters");

            RuleFor(x => x.Customer.Email)
                .NotEmpty()
                .WithMessage("Customer email is required")
                .EmailAddress()
                .WithMessage("Customer email must be valid");

            RuleFor(x => x.Branch)
                .NotNull()
                .WithMessage("Branch information is required");

            RuleFor(x => x.Branch.Id)
                .NotEmpty()
                .WithMessage("Branch ID is required");

            RuleFor(x => x.Branch.Name)
                .NotEmpty()
                .WithMessage("Branch name is required");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required for the sale");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("Product ID is required");

                item.RuleFor(x => x.ProductName)
                    .NotEmpty()
                    .WithMessage("Product name is required");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than zero")
                    .LessThanOrEqualTo(20)
                    .WithMessage("Cannot sell more than 20 identical items");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Unit price must be greater than zero");
            });
        }
    }
}
