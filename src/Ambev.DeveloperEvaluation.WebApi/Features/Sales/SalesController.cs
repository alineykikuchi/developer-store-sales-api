using Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddItemToSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    /// <summary>
    /// Controller for creating sales
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of SalesController
        /// </summary>
        /// <param name="mediator">The mediator instance</param>
        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new sale
        /// </summary>
        /// <param name="request">The sale creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(response)
            });
        }

        /// <summary>
        /// Retrieves a sale by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSale(Guid id, CancellationToken cancellationToken)
        {
            var command = new GetSaleCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            var response = _mapper.Map<GetSaleResponse>(result);

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = response
            });
        }

        /// <summary>
        /// Retrieves a paginated list of sales with optional filters
        /// </summary>
        /// <param name="request">Filter and pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of sales</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<GetSalesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSales([FromQuery] GetSalesRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<GetSalesCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            // Convert PaginatedResult<GetSalesResult> to PaginatedList<GetSalesResponse>
            var responseItems = result.Items.Select(sale => _mapper.Map<GetSalesResponse>(sale)).ToList();
            var paginatedResponse = new PaginatedList<GetSalesResponse>(
                responseItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize
            );

            return OkPaginated(paginatedResponse);
        }

        /// <summary>
        /// Cancels a sale by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the sale to cancel</param>
        /// <param name="request">The cancellation request with optional reason</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cancellation result</returns>
        [HttpPatch("{id:guid}/cancel")]
        [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelSale(Guid id, [FromBody] CancelSaleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                var requestValidator = new CancelSaleRequestValidator();
                var requestValidationResult = await requestValidator.ValidateAsync(request, cancellationToken);

                if (!requestValidationResult.IsValid)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = requestValidationResult.Errors.Select(e => new ValidationErrorDetail
                        {
                            Error = e.PropertyName,
                            Detail = e.ErrorMessage
                        }).ToList()
                    });

                // Create command
                var command = new CancelSaleCommand(id, request.CancellationReason);
                var result = await _mediator.Send(command, cancellationToken);

                // Map to response
                var response = _mapper.Map<CancelSaleResponse>(result);

                return Ok(new ApiResponseWithData<CancelSaleResponse>
                {
                    Success = true,
                    Message = $"Sale {result.SaleNumber} has been successfully cancelled",
                    Data = response
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"Sale with ID {id} not found"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.PropertyName,
                        Detail = e.ErrorMessage
                    }).ToList()
                });
            }
        }

        /// <summary>
        /// Adds an item to an existing sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="request">The item to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of adding the item</returns>
        [HttpPost("{saleId:guid}/items")]
        [ProducesResponseType(typeof(ApiResponseWithData<AddItemToSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddItemToSale(Guid saleId, [FromBody] AddItemToSaleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                var requestValidator = new AddItemToSaleRequestValidator();
                var requestValidationResult = await requestValidator.ValidateAsync(request, cancellationToken);

                if (!requestValidationResult.IsValid)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = requestValidationResult.Errors.Select(e => new ValidationErrorDetail
                        {
                            Error = e.PropertyName,
                            Detail = e.ErrorMessage
                        }).ToList()
                    });

                // Create command
                var command = _mapper.Map<AddItemToSaleCommand>(request);
                command.SaleId = saleId;

                var result = await _mediator.Send(command, cancellationToken);

                // Map to response
                var response = _mapper.Map<AddItemToSaleResponse>(result);

                return Created(string.Empty, new ApiResponseWithData<AddItemToSaleResponse>
                {
                    Success = true,
                    Message = $"Item {result.AddedItem.ProductName} successfully added to sale {result.SaleNumber}",
                    Data = response
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"Sale with ID {saleId} not found"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.PropertyName,
                        Detail = e.ErrorMessage
                    }).ToList()
                });
            }
        }

        /// <summary>
        /// Modifies an item in an existing sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="itemId">The unique identifier of the item to modify</param>
        /// <param name="request">The modifications to apply</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of modifying the item</returns>
        [HttpPut("{saleId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<ModifySaleItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ModifySaleItem(Guid saleId, Guid itemId, [FromBody] ModifySaleItemRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                var requestValidator = new ModifySaleItemRequestValidator();
                var requestValidationResult = await requestValidator.ValidateAsync(request, cancellationToken);

                if (!requestValidationResult.IsValid)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = requestValidationResult.Errors.Select(e => new ValidationErrorDetail
                        {
                            Error = e.PropertyName,
                            Detail = e.ErrorMessage
                        }).ToList()
                    });

                // Create command
                var command = _mapper.Map<ModifySaleItemCommand>(request);
                command.SaleId = saleId;
                command.ItemId = itemId;

                var result = await _mediator.Send(command, cancellationToken);

                // Map to response
                var response = _mapper.Map<ModifySaleItemResponse>(result);

                return Ok(new ApiResponseWithData<ModifySaleItemResponse>
                {
                    Success = true,
                    Message = $"Item {result.ModifiedItem.ProductName} in sale {result.SaleNumber} has been successfully modified",
                    Data = response
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.PropertyName,
                        Detail = e.ErrorMessage
                    }).ToList()
                });
            }
        }

        /// <summary>
        /// Removes an item from an existing sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="itemId">The unique identifier of the item to remove</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of removing the item</returns>
        [HttpDelete("{saleId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(typeof(ApiResponseWithData<RemoveSaleItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveSaleItem(Guid saleId, Guid itemId, CancellationToken cancellationToken)
        {
            try
            {
                // Create command
                var command = new RemoveSaleItemCommand(saleId, itemId);
                var result = await _mediator.Send(command, cancellationToken);

                // Map to response
                var response = _mapper.Map<RemoveSaleItemResponse>(result);

                return Ok(new ApiResponseWithData<RemoveSaleItemResponse>
                {
                    Success = true,
                    Message = $"Item {result.RemovedItem.ProductName} has been successfully removed from sale {result.SaleNumber}",
                    Data = response
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.PropertyName,
                        Detail = e.ErrorMessage
                    }).ToList()
                });
            }
        }
    }
}
