using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    /// <summary>
    /// Handler for processing GetSale commands
    /// </summary>
    public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of GetSaleHandler
        /// </summary>
        /// <param name="saleRepository">The sale repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the GetSale command request
        /// </summary>
        /// <param name="command">The GetSale command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details if found</returns>
        public async Task<GetSaleResult> Handle(GetSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new GetSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

            var result = _mapper.Map<GetSaleResult>(sale);

            // Add calculated properties
            result.TotalItemsCount = sale.GetTotalItemsCount();
            result.HasDiscountedItems = sale.HasDiscountedItems();

            return result;
        }
    }

}
