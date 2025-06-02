using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;

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

            var saleIsActive = new SaleIsCancelledSpecification();
            if (saleIsActive.IsSatisfiedBy(sale))
                throw new InvalidOperationException($"Cannot add items to cancelled sale {sale.SaleNumber}");

            var productExistsInSale = new ProductExistsInSaleSpecification(command.Product.Id);
            if (productExistsInSale.IsSatisfiedBy(sale))
                throw new InvalidOperationException($"Product {command.Product.Name} already exists in sale {sale.SaleNumber}. Use update endpoint to modify.");

            // Create Value Objects
            var productId = new ProductId(
                command.Product.Id,
                command.Product.Name,
                command.Product.Description
            );

            var unitPrice = new Money(command.UnitPrice, command.Currency);

            // Add item to sale using domain logic
            var addedItem = sale.AddItem(productId, command.Quantity, unitPrice);

            // Update the sale in the repository
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            var result = _mapper.Map<AddItemToSaleResult>(updatedSale);
            result.AddedItem = _mapper.Map<AddItemToSaleItemResult>(addedItem);

            return result;
        }
    }
}
