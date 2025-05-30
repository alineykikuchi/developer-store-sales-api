using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    /// <summary>
    /// Handler for processing CreateSale commands
    /// </summary>
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of CreateSaleHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the CreateSale command request
        /// </summary>
        /// <param name="command">The CreateSale command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale details</returns>
        public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Check if sale number already exists
            var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
            if (existingSale != null)
                throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

            // Create External Identity Value Objects
            var customerId = new CustomerId(
                command.Customer.Id,
                command.Customer.Name,
                command.Customer.Email
            );

            var branchId = new BranchId(
                command.Branch.Id,
                command.Branch.Name,
                command.Branch.Address
            );

            // Create the sale aggregate
            var sale = new Sale(command.SaleNumber, customerId, branchId);

            // Add items to the sale
            foreach (var item in command.Items)
            {
                var productId = new ProductId(
                    item.ProductId,
                    item.ProductName,
                    item.ProductDescription
                );

                var unitPrice = new Money(item.UnitPrice, item.Currency);

                sale.AddItem(productId, item.Quantity, unitPrice);
            }

            // Save the sale
            var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

            // Map to result
            var result = _mapper.Map<CreateSaleResult>(createdSale);

            return result;
        }
    }
}
