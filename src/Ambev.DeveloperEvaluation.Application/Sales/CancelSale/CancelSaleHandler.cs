using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using AutoMapper;
using FluentValidation;
using MediatR;

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

            var saleIsCancelled = new SaleIsCancelledSpecification();
            if (saleIsCancelled.IsSatisfiedBy(sale))
                throw new InvalidOperationException($"Sale {sale.SaleNumber} is already cancelled");

            var saleCanBeCancelled = new SaleCanBeCancelledSpecification();
            if (!saleCanBeCancelled.IsSatisfiedBy(sale))
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
    }
}
