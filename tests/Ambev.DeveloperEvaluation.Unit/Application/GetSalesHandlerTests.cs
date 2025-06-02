using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Common;
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
/// Contains unit tests for the <see cref="GetSalesHandler"/> class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get sales request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid get sales data When getting sales Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommandWithDefaultPagination();
        var sales = GetSalesHandlerTestData.GenerateValidSalesWithItems(3, 2);
        var paginatedSales = GetSalesHandlerTestData.GeneratePaginatedSales(sales, command.Page, command.PageSize, 10);
        var mappedResults = GetSalesHandlerTestData.GenerateGetSalesResults(3);

        _saleRepository.GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // Setup mapper for each sale
        for (int i = 0; i < sales.Count; i++)
        {
            _mapper.Map<GetSalesResult>(sales[i]).Returns(mappedResults[i]);
        }

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(10);
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid get sales request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid page When getting sales Then throws validation exception")]
    public async Task Handle_InvalidPage_ThrowsValidationException()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateInvalidCommandWithInvalidPage();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that invalid page size throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid page size When getting sales Then throws validation exception")]
    public async Task Handle_InvalidPageSize_ThrowsValidationException()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateInvalidCommandWithInvalidPageSize();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that invalid date range throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid date range When getting sales Then throws validation exception")]
    public async Task Handle_InvalidDateRange_ThrowsValidationException()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateInvalidCommandWithInvalidDateRange();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that invalid order by field throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid order by When getting sales Then throws validation exception")]
    public async Task Handle_InvalidOrderBy_ThrowsValidationException()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateInvalidCommandWithInvalidOrderBy();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that invalid order direction throws validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid order direction When getting sales Then throws validation exception")]
    public async Task Handle_InvalidOrderDirection_ThrowsValidationException()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateInvalidCommandWithInvalidOrderDirection();

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
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var status = SaleStatus.Active;
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var saleNumber = "12345";
        var customerName = "John Doe";

        var command = new GetSalesCommand
        {
            Page = 2,
            PageSize = 20,
            CustomerId = customerId,
            BranchId = branchId,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            SaleNumber = saleNumber,
            CustomerName = customerName,
            OrderBy = "TotalAmount",
            OrderDirection = "asc"
        };

        var paginatedSales = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            Arg.Is<int>(p => p == 2),
            Arg.Is<int>(ps => ps == 20),
            Arg.Is<Guid?>(cid => cid == customerId),
            Arg.Is<Guid?>(bid => bid == branchId),
            Arg.Is<SaleStatus?>(s => s == status),
            Arg.Is<DateTime?>(sd => sd == startDate),
            Arg.Is<DateTime?>(ed => ed == endDate),
            Arg.Is<string>(sn => sn == saleNumber),
            Arg.Is<string>(cn => cn == customerName),
            Arg.Is<string>(ob => ob == "TotalAmount"),
            Arg.Is<string>(od => od == "asc"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that empty result is handled correctly.
    /// </summary>
    [Fact(DisplayName = "Given no sales found When getting sales Then returns empty result")]
    public async Task Handle_NoSalesFound_ReturnsEmptyResult()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommandWithDefaultPagination();
        var emptyResult = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>())
            .Returns(emptyResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination works correctly.
    /// </summary>
    [Fact(DisplayName = "Given specific pagination When getting sales Then returns correct pagination")]
    public async Task Handle_SpecificPagination_ReturnsCorrectPagination()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommandWithPagination(3, 5);
        var sales = GetSalesHandlerTestData.GenerateValidSales(5);
        var paginatedSales = GetSalesHandlerTestData.GeneratePaginatedSales(sales, 3, 5, 25);
        var mappedResults = GetSalesHandlerTestData.GenerateGetSalesResults(5);

        _saleRepository.GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        for (int i = 0; i < sales.Count; i++)
        {
            _mapper.Map<GetSalesResult>(sales[i]).Returns(mappedResults[i]);
        }

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.CurrentPage.Should().Be(3);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(5);
        result.Items.Should().HaveCount(5);
    }

    /// <summary>
    /// Tests that filters are passed correctly to repository.
    /// </summary>
    [Fact(DisplayName = "Given command with filters When getting sales Then passes filters to repository")]
    public async Task Handle_CommandWithFilters_PassesFiltersToRepository()
    {
        // Given
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var status = SaleStatus.Cancelled;
        var command = GetSalesHandlerTestData.GenerateValidCommandWithFilters(customerId, branchId, status);

        var paginatedSales = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize,
            Arg.Is<Guid?>(cid => cid == customerId),
            Arg.Is<Guid?>(bid => bid == branchId),
            Arg.Is<SaleStatus?>(s => s == status),
            Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<string>(),
            command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that date range filter is passed correctly to repository.
    /// </summary>
    [Fact(DisplayName = "Given command with date range When getting sales Then passes date range to repository")]
    public async Task Handle_CommandWithDateRange_PassesDateRangeToRepository()
    {
        // Given
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var command = GetSalesHandlerTestData.GenerateValidCommandWithDateRange(startDate, endDate);

        var paginatedSales = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize, Arg.Any<Guid?>(), Arg.Any<Guid?>(), Arg.Any<SaleStatus?>(),
            Arg.Is<DateTime?>(sd => sd == startDate),
            Arg.Is<DateTime?>(ed => ed == endDate),
            Arg.Any<string>(), Arg.Any<string>(), command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that search filters are passed correctly to repository.
    /// </summary>
    [Fact(DisplayName = "Given command with search filters When getting sales Then passes search to repository")]
    public async Task Handle_CommandWithSearchFilters_PassesSearchToRepository()
    {
        // Given
        var saleNumber = "12345";
        var customerName = "John Doe";
        var command = GetSalesHandlerTestData.GenerateValidCommandWithSearch(saleNumber, customerName);

        var paginatedSales = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize, Arg.Any<Guid?>(), Arg.Any<Guid?>(), Arg.Any<SaleStatus?>(),
            Arg.Any<DateTime?>(), Arg.Any<DateTime?>(),
            Arg.Is<string>(sn => sn == saleNumber),
            Arg.Is<string>(cn => cn == customerName),
            command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that ordering parameters are passed correctly to repository.
    /// </summary>
    [Fact(DisplayName = "Given command with ordering When getting sales Then passes ordering to repository")]
    public async Task Handle_CommandWithOrdering_PassesOrderingToRepository()
    {
        // Given
        var orderBy = "TotalAmount";
        var orderDirection = "asc";
        var command = GetSalesHandlerTestData.GenerateValidCommandWithOrdering(orderBy, orderDirection);

        var paginatedSales = GetSalesHandlerTestData.GenerateEmptyPaginatedResult(command.Page, command.PageSize);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(), Arg.Any<Guid?>(),
            Arg.Any<SaleStatus?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize, Arg.Any<Guid?>(), Arg.Any<Guid?>(), Arg.Any<SaleStatus?>(),
            Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Is<string>(ob => ob == orderBy),
            Arg.Is<string>(od => od == orderDirection),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that calculated properties are set correctly for each mapped result.
    /// </summary>
    [Fact(DisplayName = "Given sales with items When mapping Then sets calculated properties correctly")]
    public async Task Handle_SalesWithItems_SetsCalculatedPropertiesCorrectly()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommandWithDefaultPagination();
        var sales = GetSalesHandlerTestData.GenerateValidSalesWithItems(2, 3);
        var paginatedSales = GetSalesHandlerTestData.GeneratePaginatedSales(sales, command.Page, command.PageSize, 2);

        var mappedResults = new List<GetSalesResult>
        {
            new GetSalesResult { Id = sales[0].Id },
            new GetSalesResult { Id = sales[1].Id }
        };

        _saleRepository.GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        _mapper.Map<GetSalesResult>(sales[0]).Returns(mappedResults[0]);
        _mapper.Map<GetSalesResult>(sales[1]).Returns(mappedResults[1]);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        var resultItems = result.Items.ToList();
        resultItems[0].TotalItemsCount.Should().Be(sales[0].GetTotalItemsCount());
        resultItems[0].HasDiscountedItems.Should().Be(sales[0].HasDiscountedItems());
        resultItems[1].TotalItemsCount.Should().Be(sales[1].GetTotalItemsCount());
        resultItems[1].HasDiscountedItems.Should().Be(sales[1].HasDiscountedItems());
    }

    /// <summary>
    /// Tests that the handler processes multiple sales correctly.
    /// </summary>
    [Fact(DisplayName = "Given multiple sales When handling Then processes all sales correctly")]
    public async Task Handle_MultipleSales_ProcessesAllSalesCorrectly()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommandWithDefaultPagination();
        var sales = GetSalesHandlerTestData.GenerateValidSales(3);
        var paginatedSales = GetSalesHandlerTestData.GeneratePaginatedSales(sales, command.Page, command.PageSize, 3);

        // Create specific mapped results that we can verify
        var mappedResults = new List<GetSalesResult>
        {
            new GetSalesResult { Id = sales[0].Id, SaleNumber = "SALE001" },
            new GetSalesResult { Id = sales[1].Id, SaleNumber = "SALE002" },
            new GetSalesResult { Id = sales[2].Id, SaleNumber = "SALE003" }
        };

        _saleRepository.GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>())
            .Returns(paginatedSales);

        // Setup mapper to return specific results for each sale
        for (int i = 0; i < sales.Count; i++)
        {
            _mapper.Map<GetSalesResult>(sales[i]).Returns(mappedResults[i]);
        }

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);

        // Verify that the repository was called correctly
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.Page, command.PageSize, command.CustomerId, command.BranchId,
            command.Status, command.StartDate, command.EndDate, command.SaleNumber,
            command.CustomerName, command.OrderBy, command.OrderDirection,
            Arg.Any<CancellationToken>());

        // Verify that mapper was called (the exact number depends on the implementation)
        _mapper.Received().Map<GetSalesResult>(Arg.Any<Sale>());
    }
}