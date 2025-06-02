using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid get sale data When getting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithId(command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CreatedAt = sale.CreatedAt,
            CancelledAt = sale.CancelledAt,
            CustomerName = sale.Customer.Name,
            CustomerEmail = sale.Customer.Email,
            CustomerId = sale.Customer.Id,
            BranchName = sale.Branch.Name,
            BranchAddress = sale.Branch.Address,
            BranchId = sale.Branch.Id,
            TotalAmount = sale.TotalAmount.Amount,
            Currency = sale.TotalAmount.Currency,
            Status = sale.Status,
            Items = new List<GetSaleItemResult>(),
            TotalItemsCount = 0,
            HasDiscountedItems = false
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.Should().NotBeNull();
        getResult.Id.Should().Be(sale.Id);
        getResult.SaleNumber.Should().Be(sale.SaleNumber);
        getResult.CustomerName.Should().Be(sale.Customer.Name);
        getResult.BranchName.Should().Be(sale.Branch.Name);
        getResult.Status.Should().Be(sale.Status);
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid get sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid get sale data When getting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateInvalidCommand(); // Empty GUID will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that getting non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale When getting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithId(command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == command.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps objects correctly")]
    public async Task Handle_ValidRequest_MapsObjectsCorrectly()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithId(command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<GetSaleResult>(Arg.Is<Sale>(s => s.Id == command.Id));
    }

    /// <summary>
    /// Tests that calculated properties are set correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale with items When handling Then sets calculated properties correctly")]
    public async Task Handle_SaleWithItems_SetsCalculatedPropertiesCorrectly()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithItems(2);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Items = new List<GetSaleItemResult>
            {
                new GetSaleItemResult { Id = Guid.NewGuid() },
                new GetSaleItemResult { Id = Guid.NewGuid() }
            }
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.TotalItemsCount.Should().Be(sale.GetTotalItemsCount());
        getResult.HasDiscountedItems.Should().Be(sale.HasDiscountedItems());
    }

    /// <summary>
    /// Tests that cancelled sale data is returned correctly.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When getting sale Then returns cancellation data")]
    public async Task Handle_CancelledSale_ReturnsCancellationData()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateCancelledSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Status = SaleStatus.Cancelled,
            CancelledAt = sale.CancelledAt
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.Status.Should().Be(SaleStatus.Cancelled);
        getResult.CancelledAt.Should().NotBeNull();
        getResult.CancelledAt.Should().Be(sale.CancelledAt);
    }

    /// <summary>
    /// Tests that sale with discounted items shows correct discount information.
    /// </summary>
    [Fact(DisplayName = "Given sale with discounted items When getting sale Then shows discount information")]
    public async Task Handle_SaleWithDiscountedItems_ShowsDiscountInformation()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithDiscountedItems(true);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            HasDiscountedItems = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.HasDiscountedItems.Should().BeTrue();
        sale.HasDiscountedItems().Should().BeTrue();
    }

    /// <summary>
    /// Tests that sale without discounted items shows correct discount information.
    /// </summary>
    [Fact(DisplayName = "Given sale without discounted items When getting sale Then shows no discount information")]
    public async Task Handle_SaleWithoutDiscountedItems_ShowsNoDiscountInformation()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithDiscountedItems(false);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            HasDiscountedItems = false
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.HasDiscountedItems.Should().BeFalse();
        sale.HasDiscountedItems().Should().BeFalse();
    }

    /// <summary>
    /// Tests that total items count is calculated correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale with specific item count When getting sale Then calculates total items count correctly")]
    public async Task Handle_SaleWithSpecificItemCount_CalculatesTotalItemsCountCorrectly()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var expectedTotalItems = 15;
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithTotalItemsCount(expectedTotalItems);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.TotalItemsCount.Should().Be(expectedTotalItems);
        sale.GetTotalItemsCount().Should().Be(expectedTotalItems);
    }

    /// <summary>
    /// Tests that customer and branch information is returned correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale with customer and branch When getting sale Then returns customer and branch information")]
    public async Task Handle_SaleWithCustomerAndBranch_ReturnsCustomerAndBranchInformation()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var customerId = Guid.NewGuid();
        var customerName = "John Doe";
        var customerEmail = "john.doe@example.com";
        var branchId = Guid.NewGuid();
        var branchName = "Main Branch";
        var branchAddress = "123 Main St";

        var sale = GetSaleHandlerTestData.GenerateValidSaleWithCustomerAndBranch(
            customerId, customerName, customerEmail,
            branchId, branchName, branchAddress);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            BranchId = branchId,
            BranchName = branchName,
            BranchAddress = branchAddress
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.CustomerId.Should().Be(customerId);
        getResult.CustomerName.Should().Be(customerName);
        getResult.CustomerEmail.Should().Be(customerEmail);
        getResult.BranchId.Should().Be(branchId);
        getResult.BranchName.Should().Be(branchName);
        getResult.BranchAddress.Should().Be(branchAddress);
    }

    /// <summary>
    /// Tests that all sale properties are mapped correctly.
    /// </summary>
    [Fact(DisplayName = "Given complete sale data When getting sale Then maps all properties correctly")]
    public async Task Handle_CompleteSaleData_MapsAllPropertiesCorrectly()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSaleWithItems(3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CreatedAt = sale.CreatedAt,
            CancelledAt = sale.CancelledAt,
            CustomerName = sale.Customer.Name,
            CustomerEmail = sale.Customer.Email,
            CustomerId = sale.Customer.Id,
            BranchName = sale.Branch.Name,
            BranchAddress = sale.Branch.Address,
            BranchId = sale.Branch.Id,
            TotalAmount = sale.TotalAmount.Amount,
            Currency = sale.TotalAmount.Currency,
            Status = sale.Status,
            Items = new List<GetSaleItemResult>
            {
                new GetSaleItemResult { Id = Guid.NewGuid() },
                new GetSaleItemResult { Id = Guid.NewGuid() },
                new GetSaleItemResult { Id = Guid.NewGuid() }
            }
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getResult.Id.Should().Be(sale.Id);
        getResult.SaleNumber.Should().Be(sale.SaleNumber);
        getResult.SaleDate.Should().Be(sale.SaleDate);
        getResult.CreatedAt.Should().Be(sale.CreatedAt);
        getResult.CustomerName.Should().Be(sale.Customer.Name);
        getResult.CustomerEmail.Should().Be(sale.Customer.Email);
        getResult.CustomerId.Should().Be(sale.Customer.Id);
        getResult.BranchName.Should().Be(sale.Branch.Name);
        getResult.BranchAddress.Should().Be(sale.Branch.Address);
        getResult.BranchId.Should().Be(sale.Branch.Id);
        getResult.TotalAmount.Should().Be(sale.TotalAmount.Amount);
        getResult.Currency.Should().Be(sale.TotalAmount.Currency);
        getResult.Status.Should().Be(sale.Status);
        getResult.Items.Should().HaveCount(3);
        getResult.TotalItemsCount.Should().Be(sale.GetTotalItemsCount());
        getResult.HasDiscountedItems.Should().Be(sale.HasDiscountedItems());
    }
}