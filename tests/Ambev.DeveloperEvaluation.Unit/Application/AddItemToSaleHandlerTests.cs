using Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale;
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
/// Contains unit tests for the <see cref="AddItemToSaleHandler"/> class.
/// </summary>
public class AddItemToSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly AddItemToSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddItemToSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public AddItemToSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new AddItemToSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid add item to sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid add item data When adding item to sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var sale = AddItemToSaleHandlerTestData.GenerateValidSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new AddItemToSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            AddedItem = new AddItemToSaleItemResult
            {
                Id = Guid.NewGuid(),
                ProductId = command.Product.Id,
                ProductName = command.Product.Name,
                ProductDescription = command.Product.Description,
                Quantity = command.Quantity,
                UnitPrice = command.UnitPrice,
                UnitPriceCurrency = command.Currency,
                TotalAmount = command.Quantity * command.UnitPrice,
                TotalAmountCurrency = command.Currency
            },
            NewSaleTotalAmount = command.Quantity * command.UnitPrice,
            Currency = command.Currency,
            TotalItemsCount = command.Quantity,
            HasDiscountedItems = command.Quantity >= 4,
            UpdatedAt = DateTime.UtcNow
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<AddItemToSaleResult>(sale).Returns(result);
        _mapper.Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>()).Returns(result.AddedItem);

        // When
        var addItemResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        addItemResult.Should().NotBeNull();
        addItemResult.SaleId.Should().Be(sale.Id);
        addItemResult.SaleNumber.Should().Be(sale.SaleNumber);
        addItemResult.AddedItem.Should().NotBeNull();
        await _saleRepository.Received(1).GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid add item request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid add item data When adding item to sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new AddItemToSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that adding item to non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale When adding item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.SaleId} not found");
    }

    /// <summary>
    /// Tests that adding item to cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When adding item Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var cancelledSale = AddItemToSaleHandlerTestData.GenerateValidSaleWithStatus(SaleStatus.Cancelled);
        cancelledSale.GetType().GetProperty("Id")!.SetValue(cancelledSale, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Cannot add items to cancelled sale {cancelledSale.SaleNumber}");
    }

    /// <summary>
    /// Tests that adding duplicate product to sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given existing product in sale When adding same product Then throws InvalidOperationException")]
    public async Task Handle_DuplicateProduct_ThrowsInvalidOperationException()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var sale = AddItemToSaleHandlerTestData.GenerateValidSaleWithSpecificProduct(command.Product.Id);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product {command.Product.Name} already exists in sale {sale.SaleNumber}. Use update endpoint to modify quantity");
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var sale = AddItemToSaleHandlerTestData.GenerateValidSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new AddItemToSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            AddedItem = new AddItemToSaleItemResult()
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<AddItemToSaleResult>(sale).Returns(result);
        _mapper.Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>()).Returns(result.AddedItem);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == command.SaleId),
            Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Id == sale.Id && s.Items.Count == 1),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps objects correctly")]
    public async Task Handle_ValidRequest_MapsObjectsCorrectly()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var sale = AddItemToSaleHandlerTestData.GenerateValidSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new AddItemToSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            AddedItem = new AddItemToSaleItemResult()
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<AddItemToSaleResult>(sale).Returns(result);
        _mapper.Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>()).Returns(result.AddedItem);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<AddItemToSaleResult>(Arg.Is<Sale>(s => s.Id == sale.Id));
        _mapper.Received(1).Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>());
    }

    /// <summary>
    /// Tests that value objects are created correctly from command data.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then creates correct value objects")]
    public async Task Handle_ValidRequest_CreatesCorrectValueObjects()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        var sale = AddItemToSaleHandlerTestData.GenerateValidSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new AddItemToSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            AddedItem = new AddItemToSaleItemResult()
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<AddItemToSaleResult>(sale).Returns(result);
        _mapper.Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>()).Returns(result.AddedItem);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Items.Any(item =>
                item.Product.Id == command.Product.Id &&
                item.Product.Name == command.Product.Name &&
                item.Product.Description == command.Product.Description &&
                item.Quantity == command.Quantity &&
                item.UnitPrice.Amount == command.UnitPrice &&
                item.UnitPrice.Currency == command.Currency)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that discount rules are applied correctly based on quantity.
    /// </summary>
    [Fact(DisplayName = "Given command with quantity for discount When handling Then applies correct discount")]
    public async Task Handle_QuantityForDiscount_AppliesCorrectDiscount()
    {
        // Given
        var command = AddItemToSaleHandlerTestData.GenerateValidCommand();
        command.Quantity = 10; // Should get 20% discount

        var sale = AddItemToSaleHandlerTestData.GenerateValidSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new AddItemToSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            AddedItem = new AddItemToSaleItemResult(),
            HasDiscountedItems = true
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<AddItemToSaleResult>(sale).Returns(result);
        _mapper.Map<AddItemToSaleItemResult>(Arg.Any<SaleItem>()).Returns(result.AddedItem);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Items.Any(item =>
                item.Quantity == 10 &&
                item.DiscountPercentage == 20m)),
            Arg.Any<CancellationToken>());
    }
}