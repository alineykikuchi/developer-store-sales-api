using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem
{
    /// <summary>
    /// Handler for processing RemoveSaleItem commands
    /// </summary>
    public class RemoveSaleItemHandler : IRequestHandler<RemoveSaleItemCommand, RemoveSaleItemResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of RemoveSaleItemHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public RemoveSaleItemHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the RemoveSaleItem command request
        /// </summary>
        /// <param name="command">The RemoveSaleItem command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of removing the sale item</returns>
        public async Task<RemoveSaleItemResult> Handle(RemoveSaleItemCommand command, CancellationToken cancellationToken)
        {
            var validator = new RemoveSaleItemCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Retrieve the sale
            var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {command.SaleId} not found");

            // Business rule: Can only remove items from active sales
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException($"Cannot remove items from cancelled sale {sale.SaleNumber}");

            // Find the item to remove
            var itemToRemove = sale.Items.FirstOrDefault(item => item.Id == command.ItemId);
            if (itemToRemove == null)
                throw new KeyNotFoundException($"Item with ID {command.ItemId} not found in sale {sale.SaleNumber}");

            // Business rule: Cannot remove if it's the last item (would leave sale empty)
            if (sale.Items.Count <= 1)
                throw new InvalidOperationException($"Cannot remove the last item from sale {sale.SaleNumber}. A sale must have at least one item");
            
            sale.RemoveItem(command.ItemId);
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            var removedItemDetails = _mapper.Map<RemoveSaleItemDetails>(itemToRemove);
            removedItemDetails.WasSuccessfullyRemoved = true;

            var result = _mapper.Map<RemoveSaleItemResult>(updatedSale);
            result.RemovedItem = removedItemDetails;

            return result;
        }
    }
}
