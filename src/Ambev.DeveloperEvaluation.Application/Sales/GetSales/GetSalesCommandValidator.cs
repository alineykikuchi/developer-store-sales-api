using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    /// <summary>
    /// Validator for GetSalesCommand
    /// </summary>
    public class GetSalesCommandValidator : AbstractValidator<GetSalesCommand>
    {
        public GetSalesCommandValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date must be before or equal to end date");

            RuleFor(x => x.OrderBy)
                .Must(value => new[] { "SaleDate", "TotalAmount", "SaleNumber", "CustomerName" }.Contains(value))
                .WithMessage("OrderBy must be one of: SaleDate, TotalAmount, SaleNumber, CustomerName");

            RuleFor(x => x.OrderDirection)
                .Must(value => new[] { "asc", "desc" }.Contains(value.ToLower()))
                .WithMessage("OrderDirection must be 'asc' or 'desc'");
        }
    }
}
