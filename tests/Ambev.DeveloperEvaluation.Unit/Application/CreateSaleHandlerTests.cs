using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid create sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid create sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithItems(command.Items.Count);
        createdSale.GetType().GetProperty("Id")!.SetValue(createdSale, Guid.NewGuid());

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            SaleDate = createdSale.SaleDate,
            CustomerName = createdSale.Customer.Name,
            BranchName = createdSale.Branch.Name,
            TotalAmount = createdSale.TotalAmount.Amount,
            Currency = createdSale.TotalAmount.Currency,
            Status = createdSale.Status,
            Items = command.Items.Select(item => new CreateSaleItemResult
            {
                Id = Guid.NewGuid(),
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercentage = item.Quantity >= 10 ? 20m : item.Quantity >= 4 ? 10m : 0m,
                TotalAmount = item.Quantity * item.UnitPrice * (1 - (item.Quantity >= 10 ? 0.2m : item.Quantity >= 4 ? 0.1m : 0m))
            }).ToList()
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null); // Sale number doesn't exist
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        var createResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createResult.Should().NotBeNull();
        createResult.Id.Should().Be(createdSale.Id);
        createResult.SaleNumber.Should().Be(createdSale.SaleNumber);
        createResult.Status.Should().Be(SaleStatus.Active);
        createResult.Items.Should().HaveCount(command.Items.Count);
        await _saleRepository.Received(1).GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid create sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid create sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateInvalidCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating sale with duplicate sale number throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given duplicate sale number When creating sale Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests that creating sale with invalid customer email throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid customer email When creating sale Then throws validation exception")]
    public async Task Handle_InvalidCustomerEmail_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateCommandWithInvalidEmail();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating sale with no items throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given no items When creating sale Then throws validation exception")]
    public async Task Handle_NoItems_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateCommandWithNoItems();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating sale with invalid item quantity throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid item quantity When creating sale Then throws validation exception")]
    public async Task Handle_InvalidItemQuantity_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateCommandWithInvalidItemQuantity();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating sale with excessive item quantity throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given excessive item quantity When creating sale Then throws validation exception")]
    public async Task Handle_ExcessiveItemQuantity_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateCommandWithExcessiveItemQuantity();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        // Create sale with the SAME sale number from command
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetBySaleNumberAsync(
            Arg.Is<string>(sn => sn == command.SaleNumber),
            Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.SaleNumber == command.SaleNumber &&
                             s.Customer.Name == command.Customer.Name &&
                             s.Branch.Name == command.Branch.Name &&
                             s.Items.Count == command.Items.Count),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps objects correctly")]
    public async Task Handle_ValidRequest_MapsObjectsCorrectly()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        // Create sale with the SAME sale number from command
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateSaleResult>(Arg.Is<Sale>(s =>
            s.SaleNumber == command.SaleNumber &&
            s.Status == SaleStatus.Active));
    }

    /// <summary>
    /// Tests that value objects are created correctly from command data.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then creates correct value objects")]
    public async Task Handle_ValidRequest_CreatesCorrectValueObjects()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        // Create sale that matches EXACTLY the command data
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleFromCommand(command);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Customer.Id == command.Customer.Id &&
                s.Customer.Name == command.Customer.Name &&
                s.Customer.Email == command.Customer.Email &&
                s.Branch.Id == command.Branch.Id &&
                s.Branch.Name == command.Branch.Name &&
                s.Branch.Address == command.Branch.Address),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that items are added correctly to the sale.
    /// </summary>
    [Fact(DisplayName = "Given command with items When handling Then adds items correctly to sale")]
    public async Task Handle_ValidRequestWithItems_AddsItemsCorrectlyToSale()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommandWithItems(2);
        // Create sale with the SAME sale number from command
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.Items.Count == 2 &&
                             s.Items.All(item =>
                                 command.Items.Any(cmdItem =>
                                     item.Product.Id == cmdItem.ProductId &&
                                     item.Product.Name == cmdItem.ProductName &&
                                     item.Product.Description == cmdItem.ProductDescription &&
                                     item.Quantity == cmdItem.Quantity &&
                                     item.UnitPrice.Amount == cmdItem.UnitPrice &&
                                     item.UnitPrice.Currency == cmdItem.Currency))),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that discount rules are applied correctly when items are added.
    /// </summary>
    [Fact(DisplayName = "Given items with quantity for discount When creating sale Then applies correct discount")]
    public async Task Handle_ItemsWithDiscountQuantity_AppliesCorrectDiscount()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items = new List<CreateSaleItem>
        {
            CreateSaleHandlerTestData.GenerateValidItemWithQuantity(10) // Should get 20% discount
        };

        // Create sale with the SAME sale number from command
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.Items.Any(item =>
                item.Quantity == 10 &&
                item.DiscountPercentage == 20m)),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that multiple items with different quantities apply correct discounts.
    /// </summary>
    [Fact(DisplayName = "Given multiple items with different quantities When creating sale Then applies correct discounts")]
    public async Task Handle_MultipleItemsWithDifferentQuantities_AppliesCorrectDiscounts()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items = new List<CreateSaleItem>
        {
            CreateSaleHandlerTestData.GenerateValidItemWithQuantity(2),  // No discount (0%)
            CreateSaleHandlerTestData.GenerateValidItemWithQuantity(5),  // 10% discount
            CreateSaleHandlerTestData.GenerateValidItemWithQuantity(15)  // 20% discount
        };

        // Create sale with the SAME sale number from command
        var createdSale = CreateSaleHandlerTestData.GenerateValidSaleWithSaleNumber(command.SaleNumber);

        var result = new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.Items.Count == 3 &&
                             s.Items.Any(item => item.Quantity == 2 && item.DiscountPercentage == 0m) &&
                             s.Items.Any(item => item.Quantity == 5 && item.DiscountPercentage == 10m) &&
                             s.Items.Any(item => item.Quantity == 15 && item.DiscountPercentage == 20m)),
            Arg.Any<CancellationToken>());
    }
}