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

namespace Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale
{
    /// <summary>
    /// Handler for processing AddItemToSale commands
    /// </summary>
    public class AddItemToSaleHandler : IRequestHandler<AddItemToSaleCommand, AddItemToSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of AddItemToSaleHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public AddItemToSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the AddItemToSale command request
        /// </summary>
        /// <param name="command">The AddItemToSale command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of adding item to sale</returns>
        public async Task<AddItemToSaleResult> Handle(AddItemToSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new AddItemToSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Retrieve the sale
            var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {command.SaleId} not found");

            // Business rule: Can only add items to active sales
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException($"Cannot add items to cancelled sale {sale.SaleNumber}");

            // Check if product already exists in the sale
            var existingItem = sale.Items.FirstOrDefault(item => item.Product.Id == command.Product.Id);
            if (existingItem != null)
                throw new InvalidOperationException($"Product {command.Product.Name} already exists in sale {sale.SaleNumber}. Use update endpoint to modify quantity");

            // Create Value Objects
            var productId = new ProductId(
                command.Product.Id,
                command.Product.Name,
                command.Product.Description
            );

            var unitPrice = new Money(command.UnitPrice, command.Currency);

            // Add item to sale using domain logic
            sale.AddItem(productId, command.Quantity, unitPrice);

            // Update the sale in the repository
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            // Find the newly added item
            var addedItem = updatedSale.Items.First(item => item.Product.Id == command.Product.Id);

            // Map to result
            var result = new AddItemToSaleResult
            {
                SaleId = updatedSale.Id,
                SaleNumber = updatedSale.SaleNumber,
                AddedItem = _mapper.Map<AddItemToSaleItemResult>(addedItem),
                NewSaleTotalAmount = updatedSale.TotalAmount.Amount,
                Currency = updatedSale.TotalAmount.Currency,
                TotalItemsCount = updatedSale.GetTotalItemsCount(),
                HasDiscountedItems = updatedSale.HasDiscountedItems(),
                UpdatedAt = DateTime.UtcNow
            };

            return result;
        }
    }
}
