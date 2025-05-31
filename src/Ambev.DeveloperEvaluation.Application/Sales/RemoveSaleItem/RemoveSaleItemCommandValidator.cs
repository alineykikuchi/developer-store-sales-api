using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    /// <summary>
    /// Validator for RemoveSaleItemCommand
    /// </summary>
    public class RemoveSaleItemCommandValidator : AbstractValidator<RemoveSaleItemCommand>
    {
        public RemoveSaleItemCommandValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty()
                .WithMessage("Sale ID is required");

            RuleFor(x => x.ItemId)
                .NotEmpty()
                .WithMessage("Item ID is required");
        }
    }
}
