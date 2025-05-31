using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    /// <summary>
    /// Handler for processing CancelSale commands
    /// </summary>
    public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of CancelSaleHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public CancelSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the CancelSale command request
        /// </summary>
        /// <param name="command">The CancelSale command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cancellation result</returns>
        public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new CancelSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Retrieve the sale
            var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

            // Business rule: Can only cancel if sale is Active (not already cancelled)
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException($"Sale {sale.SaleNumber} is already cancelled");

            // Additional business rule: Check if sale can be cancelled
            // (In a real scenario, you might check if it's invoiced, shipped, etc.)
            if (!CanBeCancelled(sale))
                throw new InvalidOperationException($"Sale {sale.SaleNumber} cannot be cancelled. It may already be invoiced or processed");

            // Cancel the sale using domain logic
            sale.Cancel();

            // Update the sale in the repository
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            // Map to result
            var result = _mapper.Map<CancelSaleResult>(updatedSale);
            result.CancellationReason = command.CancellationReason;
            result.WasSuccessfullyCancelled = true;

            return result;
        }

        /// <summary>
        /// Business logic to determine if a sale can be cancelled
        /// </summary>
        /// <param name="sale">The sale to check</param>
        /// <returns>True if the sale can be cancelled</returns>
        private static bool CanBeCancelled(Sale sale)
        {
            // Business rules for cancellation:
            // 1. Sale must be Active
            // 2. Sale should not be older than 30 days (example business rule)
            // 3. In a real scenario, you might check:
            //    - Not invoiced
            //    - Not shipped
            //    - Not payment processed
            //    - Customer approval, etc.

            if (sale.Status != SaleStatus.Active)
                return false;

            // Example: Cannot cancel sales older than 30 days
            var maxCancellationDays = 30;
            var cancellationCutoff = DateTime.UtcNow.AddDays(-maxCancellationDays);

            if (sale.SaleDate < cancellationCutoff)
                return false;

            // Add more business rules as needed
            // For example:
            // - Check if sale is invoiced (would require additional properties)
            // - Check if payment is processed
            // - Check if items are shipped

            return true;
        }
    }
}
