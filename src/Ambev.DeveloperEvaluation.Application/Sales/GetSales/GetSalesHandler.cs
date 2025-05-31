using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    /// <summary>
    /// Handler for processing GetSales commands
    /// </summary>
    public class GetSalesHandler : IRequestHandler<GetSalesCommand, PaginatedResult<GetSalesResult>>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of GetSalesHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the GetSales command request
        /// </summary>
        /// <param name="command">The GetSales command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated result of sales</returns>
        public async Task<PaginatedResult<GetSalesResult>> Handle(GetSalesCommand command, CancellationToken cancellationToken)
        {
            var validator = new GetSalesCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var paginatedSales = await _saleRepository.GetPaginatedAsync(
                page: command.Page,
                pageSize: command.PageSize,
                customerId: command.CustomerId,
                branchId: command.BranchId,
                status: command.Status,
                startDate: command.StartDate,
                endDate: command.EndDate,
                saleNumber: command.SaleNumber,
                customerName: command.CustomerName,
                orderBy: command.OrderBy,
                orderDirection: command.OrderDirection,
                cancellationToken: cancellationToken
            );

            // Map domain entities to result DTOs
            return paginatedSales.Map(sale =>
            {
                var result = _mapper.Map<GetSalesResult>(sale);
                result.TotalItemsCount = sale.GetTotalItemsCount();
                result.HasDiscountedItems = sale.HasDiscountedItems();
                return result;
            });
        }
    }
}
