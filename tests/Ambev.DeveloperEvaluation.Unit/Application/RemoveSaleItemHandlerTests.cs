using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
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
/// Contains unit tests for the <see cref="RemoveSaleItemHandler"/> class.
/// </summary>
public class RemoveSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly RemoveSaleItemHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveSaleItemHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public RemoveSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new RemoveSaleItemHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid remove item request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid remove item data When removing item Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = RemoveSaleItemHandlerTestData.GenerateValidRemovedItemDetails(itemId);

        var result = new RemoveSaleItemResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            RemovedItem = removedItemDetails,
            NewSaleTotalAmount = 40.0m,
            Currency = "BRL",
            TotalItemsCount = 4, // 2 remaining items with 2 quantity each
            HasDiscountedItems = false,
            UpdatedAt = DateTime.UtcNow,
            SaleIsEmpty = false
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.Should().NotBeNull();
        removeResult.SaleId.Should().Be(sale.Id);
        removeResult.SaleNumber.Should().Be(sale.SaleNumber);
        removeResult.RemovedItem.Should().NotBeNull();
        removeResult.RemovedItem.WasSuccessfullyRemoved.Should().BeTrue();
        removeResult.SaleIsEmpty.Should().BeFalse();
        await _saleRepository.Received(1).GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid remove item request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid remove item data When removing item Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = RemoveSaleItemHandlerTestData.GenerateInvalidCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that removing item with empty sale ID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty sale ID When removing item Then throws validation exception")]
    public async Task Handle_EmptySaleId_ThrowsValidationException()
    {
        // Given
        var command = RemoveSaleItemHandlerTestData.GenerateInvalidCommandWithEmptySaleId();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that removing item with empty item ID throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given empty item ID When removing item Then throws validation exception")]
    public async Task Handle_EmptyItemId_ThrowsValidationException()
    {
        // Given
        var command = RemoveSaleItemHandlerTestData.GenerateInvalidCommandWithEmptyItemId();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that removing item from non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale When removing item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.SaleId} not found");
    }

    /// <summary>
    /// Tests that removing item from cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When removing item Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var cancelledSale = RemoveSaleItemHandlerTestData.GenerateCancelledSaleWithItem(itemId);
        cancelledSale.GetType().GetProperty("Id")!.SetValue(cancelledSale, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Cannot remove items from cancelled sale {cancelledSale.SaleNumber}");
    }

    /// <summary>
    /// Tests that removing non-existent item throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent item When removing item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentItem_ThrowsKeyNotFoundException()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithoutItem(itemId);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Item with ID {itemId} not found in sale {sale.SaleNumber}");
    }

    /// <summary>
    /// Tests that removing the last item from sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given sale with single item When removing last item Then throws InvalidOperationException")]
    public async Task Handle_LastItemRemoval_ThrowsInvalidOperationException()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var saleWithSingleItem = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSingleItem(itemId);
        saleWithSingleItem.GetType().GetProperty("Id")!.SetValue(saleWithSingleItem, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(saleWithSingleItem);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Cannot remove the last item from sale {saleWithSingleItem.SaleNumber}. A sale must have at least one item");
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = new RemoveSaleItemDetails();
        var result = new RemoveSaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == command.SaleId),
            Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Id == sale.Id && s.Items.Count == 2), // One item was removed
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps objects correctly")]
    public async Task Handle_ValidRequest_MapsObjectsCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = new RemoveSaleItemDetails();
        var result = new RemoveSaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<RemoveSaleItemDetails>(itemToRemove);
        _mapper.Received(1).Map<RemoveSaleItemResult>(Arg.Is<Sale>(s => s.Id == sale.Id));
    }

    /// <summary>
    /// Tests that the WasSuccessfullyRemoved flag is set correctly.
    /// </summary>
    [Fact(DisplayName = "Given successful removal When handling Then sets success flag correctly")]
    public async Task Handle_SuccessfulRemoval_SetsSuccessFlagCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = new RemoveSaleItemDetails { WasSuccessfullyRemoved = false }; // Initially false
        var result = new RemoveSaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.RemovedItem.WasSuccessfullyRemoved.Should().BeTrue();
    }

    /// <summary>
    /// Tests that removing item from sale with discounts works correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale with discounted items When removing item Then handles discounts correctly")]
    public async Task Handle_SaleWithDiscountedItems_HandlesDiscountsCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithDiscountedItems(itemId);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = new RemoveSaleItemDetails
        {
            Id = itemId,
            RemovedDiscountPercentage = 20m, // 10 items = 20% discount
            WasSuccessfullyRemoved = false
        };

        var result = new RemoveSaleItemResult
        {
            SaleId = sale.Id,
            HasDiscountedItems = true // Still has other discounted items
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.RemovedItem.RemovedDiscountPercentage.Should().Be(20m);
        removeResult.HasDiscountedItems.Should().BeTrue(); // Sale still has other discounted items
    }

    /// <summary>
    /// Tests that removing item updates total amounts correctly.
    /// </summary>
    [Fact(DisplayName = "Given item removal When handling Then updates total amounts correctly")]
    public async Task Handle_ItemRemoval_UpdatesTotalAmountsCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var initialTotalAmount = sale.TotalAmount.Amount;
        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var itemTotalAmount = itemToRemove.TotalAmount.Amount;

        var removedItemDetails = new RemoveSaleItemDetails
        {
            RemovedTotalAmount = itemTotalAmount,
            WasSuccessfullyRemoved = false
        };

        var result = new RemoveSaleItemResult
        {
            SaleId = sale.Id,
            NewSaleTotalAmount = initialTotalAmount - itemTotalAmount
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.RemovedItem.RemovedTotalAmount.Should().Be(itemTotalAmount);
        removeResult.NewSaleTotalAmount.Should().Be(initialTotalAmount - itemTotalAmount);
    }

    /// <summary>
    /// Tests that removing item updates item count correctly.
    /// </summary>
    [Fact(DisplayName = "Given item removal When handling Then updates item count correctly")]
    public async Task Handle_ItemRemoval_UpdatesItemCountCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var initialItemCount = sale.GetTotalItemsCount();
        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedQuantity = itemToRemove.Quantity;

        var removedItemDetails = new RemoveSaleItemDetails
        {
            RemovedQuantity = removedQuantity,
            WasSuccessfullyRemoved = false
        };

        var result = new RemoveSaleItemResult
        {
            SaleId = sale.Id,
            TotalItemsCount = initialItemCount - removedQuantity
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.RemovedItem.RemovedQuantity.Should().Be(removedQuantity);
        removeResult.TotalItemsCount.Should().Be(initialItemCount - removedQuantity);
    }

    /// <summary>
    /// Tests that removed item details contain correct product information.
    /// </summary>
    [Fact(DisplayName = "Given item removal When handling Then captures correct product information")]
    public async Task Handle_ItemRemoval_CapturesCorrectProductInformation()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = RemoveSaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = RemoveSaleItemHandlerTestData.GenerateValidSaleWithSpecificItem(itemId, 3);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var itemToRemove = sale.Items.First(i => i.Id == itemId);
        var removedItemDetails = new RemoveSaleItemDetails
        {
            Id = itemId,
            ProductId = itemToRemove.Product.Id,
            ProductName = itemToRemove.Product.Name,
            ProductDescription = itemToRemove.Product.Description,
            WasSuccessfullyRemoved = false
        };

        var result = new RemoveSaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<RemoveSaleItemDetails>(itemToRemove).Returns(removedItemDetails);
        _mapper.Map<RemoveSaleItemResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.RemovedItem.Id.Should().Be(itemId);
        removeResult.RemovedItem.ProductId.Should().Be(itemToRemove.Product.Id);
        removeResult.RemovedItem.ProductName.Should().Be(itemToRemove.Product.Name);
        removeResult.RemovedItem.ProductDescription.Should().Be(itemToRemove.Product.Description);
    }
}