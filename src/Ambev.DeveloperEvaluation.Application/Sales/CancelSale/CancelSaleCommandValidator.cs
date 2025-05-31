using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    /// <summary>
    /// Validator for CancelSaleCommand
    /// </summary>
    public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
    {
        public CancelSaleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Sale ID is required");

            RuleFor(x => x.CancellationReason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.CancellationReason))
                .WithMessage("Cancellation reason cannot exceed 500 characters");
        }
    }
}
