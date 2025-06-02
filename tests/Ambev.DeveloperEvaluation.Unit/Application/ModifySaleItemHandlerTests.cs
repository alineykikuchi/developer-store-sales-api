using Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem;
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
/// Contains unit tests for the <see cref="ModifySaleItemHandler"/> class.
/// </summary>
public class ModifySaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ModifySaleItemHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifySaleItemHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public ModifySaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ModifySaleItemHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid modify item request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid modify item data When modifying item Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithQuantityAndPrice(8, 15.0m);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 5, 10.0m);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails
        {
            Id = itemId,
            ProductId = sale.Items.First().Product.Id,
            ProductName = sale.Items.First().Product.Name,
            NewQuantity = 8,
            NewUnitPrice = 15.0m,
            QuantityChanged = true,
            PriceChanged = true
        };

        var result = new ModifySaleItemResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            ModifiedItem = modifiedItemDetails,
            NewSaleTotalAmount = 108.0m, // 8 * 15 * 0.9 (10% discount for 8 items)
            Currency = "BRL",
            TotalItemsCount = 8,
            HasDiscountedItems = true,
            UpdatedAt = DateTime.UtcNow
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.Should().NotBeNull();
        modifyResult.SaleId.Should().Be(sale.Id);
        modifyResult.SaleNumber.Should().Be(sale.SaleNumber);
        modifyResult.ModifiedItem.Should().NotBeNull();
        modifyResult.ModifiedItem.QuantityChanged.Should().BeTrue();
        modifyResult.ModifiedItem.PriceChanged.Should().BeTrue();
        await _saleRepository.Received(1).GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid modify item request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid modify item data When modifying item Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateInvalidCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that modifying item with no modifications throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given no modifications When modifying item Then throws validation exception")]
    public async Task Handle_NoModifications_ThrowsValidationException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateInvalidCommandWithNoModifications();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that modifying item with invalid quantity throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid quantity When modifying item Then throws validation exception")]
    public async Task Handle_InvalidQuantity_ThrowsValidationException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateInvalidCommandWithInvalidQuantity();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that modifying item with excessive quantity throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given excessive quantity When modifying item Then throws validation exception")]
    public async Task Handle_ExcessiveQuantity_ThrowsValidationException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateInvalidCommandWithExcessiveQuantity();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that modifying item with invalid price throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid price When modifying item Then throws validation exception")]
    public async Task Handle_InvalidPrice_ThrowsValidationException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateInvalidCommandWithInvalidPrice();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that modifying item in non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale When modifying item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = ModifySaleItemHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.SaleId} not found");
    }

    /// <summary>
    /// Tests that modifying item in cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When modifying item Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var cancelledSale = ModifySaleItemHandlerTestData.GenerateCancelledSaleWithItem(itemId);
        cancelledSale.GetType().GetProperty("Id")!.SetValue(cancelledSale, command.SaleId);

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Cannot modify items in cancelled sale {cancelledSale.SaleNumber}");
    }

    /// <summary>
    /// Tests that modifying non-existent item throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent item When modifying item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentItem_ThrowsKeyNotFoundException()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithoutItem(itemId);
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
    /// Tests that modifying only quantity works correctly.
    /// </summary>
    [Fact(DisplayName = "Given quantity only modification When modifying item Then updates quantity only")]
    public async Task Handle_QuantityOnlyModification_UpdatesQuantityOnly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithQuantityOnly(10);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 5, 15.0m);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails
        {
            Id = itemId,
            QuantityChanged = true,
            PriceChanged = false
        };

        var result = new ModifySaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.ModifiedItem.QuantityChanged.Should().BeTrue();
        modifyResult.ModifiedItem.PriceChanged.Should().BeFalse();

        // Verify that only UpdateItemQuantity was called
        var updatedItem = sale.Items.First(i => i.Id == itemId);
        updatedItem.Quantity.Should().Be(10);
    }

    /// <summary>
    /// Tests that modifying only price works correctly.
    /// </summary>
    [Fact(DisplayName = "Given price only modification When modifying item Then updates price only")]
    public async Task Handle_PriceOnlyModification_UpdatesPriceOnly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithPriceOnly(20.0m);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 5, 15.0m);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails
        {
            Id = itemId,
            QuantityChanged = false,
            PriceChanged = true
        };

        var result = new ModifySaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.ModifiedItem.QuantityChanged.Should().BeFalse();
        modifyResult.ModifiedItem.PriceChanged.Should().BeTrue();

        // Verify that the price was updated
        var updatedItem = sale.Items.First(i => i.Id == itemId);
        updatedItem.UnitPrice.Amount.Should().Be(20.0m);
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new ModifySaleItemResult { SaleId = sale.Id };
        var itemDetails = new ModifySaleItemDetails();

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(itemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == command.SaleId),
            Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Id == sale.Id),
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
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandForSaleAndItem(Guid.NewGuid(), itemId);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);

        var result = new ModifySaleItemResult { SaleId = sale.Id };
        var itemDetails = new ModifySaleItemDetails();

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(itemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<ModifySaleItemDetails>(Arg.Any<SaleItem>());
        _mapper.Received(1).Map<ModifySaleItemResult>(Arg.Is<Sale>(s => s.Id == sale.Id));
    }

    /// <summary>
    /// Tests that previous values are correctly stored for comparison.
    /// </summary>
    [Fact(DisplayName = "Given item modification When handling Then stores previous values correctly")]
    public async Task Handle_ItemModification_StoresPreviousValuesCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithQuantityAndPrice(8, 20.0m);
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 5, 15.0m);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails { Id = itemId };
        var result = new ModifySaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.ModifiedItem.PreviousQuantity.Should().Be(5);
        modifyResult.ModifiedItem.PreviousUnitPrice.Should().Be(15.0m);
    }

    /// <summary>
    /// Tests that change flags are set correctly.
    /// </summary>
    [Fact(DisplayName = "Given modifications When handling Then sets change flags correctly")]
    public async Task Handle_Modifications_SetsChangeFlagsCorrectly()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithQuantityAndPrice(10, 15.0m); // Same price, different quantity
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 5, 15.0m);
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails { Id = itemId };
        var result = new ModifySaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.ModifiedItem.QuantityChanged.Should().BeTrue(); // 5 -> 10
        modifyResult.ModifiedItem.PriceChanged.Should().BeFalse(); // 15.0 -> 15.0 (same)
        modifyResult.ModifiedItem.DiscountChanged.Should().BeTrue(); // 0% -> 10% (due to quantity change)
    }

    /// <summary>
    /// Tests that discount changes are detected correctly.
    /// </summary>
    [Fact(DisplayName = "Given quantity change affecting discount When handling Then detects discount change")]
    public async Task Handle_QuantityChangeAffectingDiscount_DetectsDiscountChange()
    {
        // Given
        var itemId = Guid.NewGuid();
        var command = ModifySaleItemHandlerTestData.GenerateValidCommandWithQuantityOnly(10); // Should trigger 20% discount
        var sale = ModifySaleItemHandlerTestData.GenerateValidSaleWithItem(itemId, 3, 15.0m); // No discount initially
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.SaleId);
        command.ItemId = itemId;

        var modifiedItemDetails = new ModifySaleItemDetails { Id = itemId };
        var result = new ModifySaleItemResult { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<ModifySaleItemDetails>(Arg.Any<SaleItem>()).Returns(modifiedItemDetails);
        _mapper.Map<ModifySaleItemResult>(sale).Returns(result);

        // When
        var modifyResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        modifyResult.ModifiedItem.DiscountChanged.Should().BeTrue(); // 0% -> 20%
        modifyResult.ModifiedItem.PreviousDiscountPercentage.Should().Be(0m);
    }
}