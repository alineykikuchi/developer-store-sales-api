using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem
{
    /// <summary>
    /// Handler for processing ModifySaleItem commands
    /// </summary>
    public class ModifySaleItemHandler : IRequestHandler<ModifySaleItemCommand, ModifySaleItemResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of ModifySaleItemHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public ModifySaleItemHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the ModifySaleItem command request
        /// </summary>
        /// <param name="command">The ModifySaleItem command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of modifying the sale item</returns>
        public async Task<ModifySaleItemResult> Handle(ModifySaleItemCommand command, CancellationToken cancellationToken)
        {
            var validator = new ModifySaleItemCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Retrieve the sale
            var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {command.SaleId} not found");

            // Business rule: Can only modify items in active sales
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException($"Cannot modify items in cancelled sale {sale.SaleNumber}");

            // Find the item to modify
            var itemToModify = sale.Items.FirstOrDefault(item => item.Id == command.ItemId);
            if (itemToModify == null)
                throw new KeyNotFoundException($"Item with ID {command.ItemId} not found in sale {sale.SaleNumber}");

            // Store previous values for comparison
            var previousQuantity = itemToModify.Quantity;
            var previousUnitPrice = itemToModify.UnitPrice.Amount;
            var previousDiscountPercentage = itemToModify.DiscountPercentage;
            var previousTotalAmount = itemToModify.TotalAmount.Amount;

            // Update quantity if provided
            if (command.Quantity.HasValue)
            {
                sale.UpdateItemQuantity(command.ItemId, command.Quantity.Value);
            }

            // Update unit price if provided
            if (command.UnitPrice.HasValue)
            {
                var newUnitPrice = new Money(command.UnitPrice.Value, command.Currency);
                sale.UpdateItemPrice(command.ItemId, newUnitPrice);
            }

            // Update the sale in the repository
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            var modifiedItemDetails = _mapper.Map<ModifySaleItemDetails>(itemToModify);

            // Set the previous values for comparison
            modifiedItemDetails.PreviousQuantity = previousQuantity;
            modifiedItemDetails.PreviousUnitPrice = previousUnitPrice;
            modifiedItemDetails.PreviousDiscountPercentage = previousDiscountPercentage;
            modifiedItemDetails.PreviousTotalAmount = previousTotalAmount;

            // Set the change flags
            modifiedItemDetails.QuantityChanged = command.Quantity.HasValue && previousQuantity != itemToModify.Quantity;
            modifiedItemDetails.PriceChanged = command.UnitPrice.HasValue && Math.Abs(previousUnitPrice - itemToModify.UnitPrice.Amount) > 0.01m;
            modifiedItemDetails.DiscountChanged = Math.Abs(previousDiscountPercentage - itemToModify.DiscountPercentage) > 0.01m;
            
            var result = _mapper.Map<ModifySaleItemResult>(updatedSale);
            result.ModifiedItem = modifiedItemDetails;

            return result;
        }
    }
}
