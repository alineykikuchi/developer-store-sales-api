using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
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
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly CancelSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid cancel sale request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid cancel sale data When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateCancellableSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CancelledAt = DateTime.UtcNow,
            CustomerName = sale.Customer.Name,
            BranchName = sale.Branch.Name,
            TotalAmount = sale.TotalAmount.Amount,
            Currency = sale.TotalAmount.Currency,
            Status = SaleStatus.Cancelled,
            CancellationReason = command.CancellationReason,
            WasSuccessfullyCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        var cancelResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelResult.Should().NotBeNull();
        cancelResult.Id.Should().Be(sale.Id);
        cancelResult.SaleNumber.Should().Be(sale.SaleNumber);
        cancelResult.Status.Should().Be(SaleStatus.Cancelled);
        cancelResult.WasSuccessfullyCancelled.Should().BeTrue();
        cancelResult.CancellationReason.Should().Be(command.CancellationReason);
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid cancel sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid cancel sale data When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleCommand(Guid.Empty); // Empty GUID will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that cancelling non-existent sale throws KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that cancelling already cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given already cancelled sale When cancelling Then throws InvalidOperationException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var cancelledSale = CancelSaleHandlerTestData.GenerateValidSaleWithStatus(SaleStatus.Cancelled);
        cancelledSale.GetType().GetProperty("Id")!.SetValue(cancelledSale, command.Id);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(cancelledSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale {cancelledSale.SaleNumber} is already cancelled");
    }

    /// <summary>
    /// Tests that cancelling old sale throws InvalidOperationException due to business rules.
    /// </summary>
    [Fact(DisplayName = "Given sale older than 30 days When cancelling Then throws InvalidOperationException")]
    public async Task Handle_OldSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var oldSale = CancelSaleHandlerTestData.GenerateOldSale();
        oldSale.GetType().GetProperty("Id")!.SetValue(oldSale, command.Id);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(oldSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale {oldSale.SaleNumber} cannot be cancelled. It may already be invoiced or processed");
    }

    /// <summary>
    /// Tests that the sale repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateCancellableSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            WasSuccessfullyCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == command.Id),
            Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Id == sale.Id && s.Status == SaleStatus.Cancelled),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps objects correctly")]
    public async Task Handle_ValidRequest_MapsObjectsCorrectly()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateCancellableSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            WasSuccessfullyCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CancelSaleResult>(Arg.Is<Sale>(s =>
            s.Id == sale.Id && s.Status == SaleStatus.Cancelled));
    }

    /// <summary>
    /// Tests that the domain Cancel method is called correctly.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls domain Cancel method")]
    public async Task Handle_ValidRequest_CallsDomainCancelMethod()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateCancellableSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            WasSuccessfullyCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        // Verify that the sale status was changed to Cancelled
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Status == SaleStatus.Cancelled && s.CancelledAt.HasValue),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that cancellation reason is properly set in result.
    /// </summary>
    [Fact(DisplayName = "Given command with cancellation reason When handling Then sets reason in result")]
    public async Task Handle_CommandWithReason_SetsReasonInResult()
    {
        // Given
        const string expectedReason = "Customer requested cancellation";
        var command = CancelSaleHandlerTestData.GenerateValidCommandWithReason(expectedReason);
        var sale = CancelSaleHandlerTestData.GenerateCancellableSale();
        sale.GetType().GetProperty("Id")!.SetValue(sale, command.Id);

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            WasSuccessfullyCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        var cancelResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelResult.CancellationReason.Should().Be(expectedReason);
        cancelResult.WasSuccessfullyCancelled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that long cancellation reason throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given command with long cancellation reason When handling Then throws validation exception")]
    public async Task Handle_LongCancellationReason_ThrowsValidationException()
    {
        // Given
        var longReason = new string('A', 501); // Exceeds 500 character limit
        var command = CancelSaleHandlerTestData.GenerateValidCommandWithReason(longReason);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}