using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for GetSales operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class GetSalesHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetSalesCommand.
    /// The generated commands will have valid:
    /// - Page (between 1-10)
    /// - PageSize (between 1-100)
    /// - Optional filters (CustomerId, BranchId, Status, etc.)
    /// - Valid OrderBy and OrderDirection values
    /// </summary>
    private static readonly Faker<GetSalesCommand> getSalesCommandFaker = new Faker<GetSalesCommand>()
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.PageSize, f => f.Random.Int(5, 50))
        .RuleFor(c => c.CustomerId, f => f.Random.Bool(0.3f) ? f.Random.Guid() : null) // 30% chance
        .RuleFor(c => c.BranchId, f => f.Random.Bool(0.3f) ? f.Random.Guid() : null) // 30% chance
        .RuleFor(c => c.Status, f => f.Random.Bool(0.3f) ? f.PickRandom<SaleStatus>() : null) // 30% chance
        .RuleFor(c => c.StartDate, f => f.Random.Bool(0.3f) ? f.Date.Recent(30) : null) // 30% chance
        .RuleFor(c => c.EndDate, f => f.Random.Bool(0.3f) ? f.Date.Recent(1) : null) // 30% chance
        .RuleFor(c => c.SaleNumber, f => f.Random.Bool(0.2f) ? f.Random.Number(1000, 9999).ToString() : null) // 20% chance
        .RuleFor(c => c.CustomerName, f => f.Random.Bool(0.2f) ? f.Person.FullName : null) // 20% chance
        .RuleFor(c => c.OrderBy, f => f.PickRandom("SaleDate", "TotalAmount", "SaleNumber", "CustomerName"))
        .RuleFor(c => c.OrderDirection, f => f.PickRandom("asc", "desc"));

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - Id (valid GUID)
    /// - SaleNumber (formatted sale number)
    /// - SaleDate (recent date)
    /// - Customer information (CustomerId value object)
    /// - Branch information (BranchId value object)
    /// - Status (Active by default)
    /// - Items collection (empty by default, can be populated)
    /// - TotalAmount (Money value object)
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Number(1000, 99999).ToString(),
            new CustomerId(f.Random.Guid(), f.Person.FullName, f.Internet.Email()),
            new BranchId(f.Random.Guid(), f.Company.CompanyName(), f.Address.FullAddress())
        ))
        .FinishWith((f, sale) =>
        {
            // Set SaleDate to a recent date for realistic data
            var saleDateProperty = typeof(Sale).GetProperty("SaleDate");
            saleDateProperty?.SetValue(sale, f.Date.Recent(30));
        });

    /// <summary>
    /// Generates a valid GetSalesCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid GetSalesCommand with randomly generated data.</returns>
    public static GetSalesCommand GenerateValidCommand()
    {
        return getSalesCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with default pagination.
    /// </summary>
    /// <returns>A valid GetSalesCommand with default pagination settings.</returns>
    public static GetSalesCommand GenerateValidCommandWithDefaultPagination()
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with specific pagination.
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>A valid GetSalesCommand with specified pagination.</returns>
    public static GetSalesCommand GenerateValidCommandWithPagination(int page, int pageSize)
    {
        return new GetSalesCommand
        {
            Page = page,
            PageSize = pageSize,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with filters.
    /// </summary>
    /// <param name="customerId">Customer ID filter</param>
    /// <param name="branchId">Branch ID filter</param>
    /// <param name="status">Status filter</param>
    /// <returns>A valid GetSalesCommand with specified filters.</returns>
    public static GetSalesCommand GenerateValidCommandWithFilters(Guid? customerId = null, Guid? branchId = null, SaleStatus? status = null)
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            CustomerId = customerId,
            BranchId = branchId,
            Status = status,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with date range filter.
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>A valid GetSalesCommand with date range filter.</returns>
    public static GetSalesCommand GenerateValidCommandWithDateRange(DateTime? startDate, DateTime? endDate)
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            StartDate = startDate,
            EndDate = endDate,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with search filters.
    /// </summary>
    /// <param name="saleNumber">Sale number to search</param>
    /// <param name="customerName">Customer name to search</param>
    /// <returns>A valid GetSalesCommand with search filters.</returns>
    public static GetSalesCommand GenerateValidCommandWithSearch(string? saleNumber = null, string? customerName = null)
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            SaleNumber = saleNumber,
            CustomerName = customerName,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates a valid GetSalesCommand with specific ordering.
    /// </summary>
    /// <param name="orderBy">Field to order by</param>
    /// <param name="orderDirection">Order direction</param>
    /// <returns>A valid GetSalesCommand with specified ordering.</returns>
    public static GetSalesCommand GenerateValidCommandWithOrdering(string orderBy, string orderDirection)
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            OrderBy = orderBy,
            OrderDirection = orderDirection
        };
    }

    /// <summary>
    /// Generates an invalid GetSalesCommand with invalid page.
    /// </summary>
    /// <returns>An invalid GetSalesCommand for testing validation.</returns>
    public static GetSalesCommand GenerateInvalidCommandWithInvalidPage()
    {
        return new GetSalesCommand
        {
            Page = 0, // Invalid: must be > 0
            PageSize = 10,
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates an invalid GetSalesCommand with invalid page size.
    /// </summary>
    /// <returns>An invalid GetSalesCommand for testing validation.</returns>
    public static GetSalesCommand GenerateInvalidCommandWithInvalidPageSize()
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 150, // Invalid: must be <= 100
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates an invalid GetSalesCommand with invalid date range.
    /// </summary>
    /// <returns>An invalid GetSalesCommand for testing validation.</returns>
    public static GetSalesCommand GenerateInvalidCommandWithInvalidDateRange()
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(-5), // Invalid: end date before start date
            OrderBy = "SaleDate",
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates an invalid GetSalesCommand with invalid order by.
    /// </summary>
    /// <returns>An invalid GetSalesCommand for testing validation.</returns>
    public static GetSalesCommand GenerateInvalidCommandWithInvalidOrderBy()
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            OrderBy = "InvalidField", // Invalid: not in allowed values
            OrderDirection = "desc"
        };
    }

    /// <summary>
    /// Generates an invalid GetSalesCommand with invalid order direction.
    /// </summary>
    /// <returns>An invalid GetSalesCommand for testing validation.</returns>
    public static GetSalesCommand GenerateInvalidCommandWithInvalidOrderDirection()
    {
        return new GetSalesCommand
        {
            Page = 1,
            PageSize = 10,
            OrderBy = "SaleDate",
            OrderDirection = "invalid" // Invalid: must be 'asc' or 'desc'
        };
    }

    /// <summary>
    /// Generates a list of valid Sale entities.
    /// </summary>
    /// <param name="count">Number of sales to generate</param>
    /// <returns>A list of valid Sale entities.</returns>
    public static List<Sale> GenerateValidSales(int count)
    {
        return saleFaker.Generate(count);
    }

    /// <summary>
    /// Generates a list of valid Sale entities with items.
    /// </summary>
    /// <param name="count">Number of sales to generate</param>
    /// <param name="itemsPerSale">Number of items per sale</param>
    /// <returns>A list of valid Sale entities with items.</returns>
    public static List<Sale> GenerateValidSalesWithItems(int count, int itemsPerSale = 2)
    {
        var sales = saleFaker.Generate(count);

        foreach (var sale in sales)
        {
            for (int i = 0; i < itemsPerSale; i++)
            {
                var productId = new ProductId(
                    Guid.NewGuid(),
                    $"Product {i + 1}",
                    $"Description for product {i + 1}"
                );
                var unitPrice = new Money(10.50m * (i + 1), "BRL");
                sale.AddItem(productId, 2, unitPrice);
            }
        }

        return sales;
    }

    /// <summary>
    /// Generates a paginated result of sales.
    /// Uses reflection to set properties with private setters.
    /// </summary>
    /// <param name="sales">Sales to paginate</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="totalCount">Total count of sales</param>
    /// <returns>A paginated result of sales.</returns>
    public static PaginatedResult<Sale> GeneratePaginatedSales(List<Sale> sales, int page, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var result = new PaginatedResult<Sale>(sales, totalCount, page, pageSize);

        // Use reflection to set the properties since they might have private setters
        //var itemsProperty = typeof(PaginatedResult<Sale>).GetProperty("Items");
        //var pageProperty = typeof(PaginatedResult<Sale>).GetProperty("Page");
        //var pageSizeProperty = typeof(PaginatedResult<Sale>).GetProperty("PageSize");
        //var totalCountProperty = typeof(PaginatedResult<Sale>).GetProperty("TotalCount");
        //var totalPagesProperty = typeof(PaginatedResult<Sale>).GetProperty("TotalPages");

        //itemsProperty?.SetValue(result, sales);
        //pageProperty?.SetValue(result, page);
        //pageSizeProperty?.SetValue(result, pageSize);
        //totalCountProperty?.SetValue(result, totalCount);
        //totalPagesProperty?.SetValue(result, totalPages);

        return result;
    }

    /// <summary>
    /// Generates an empty paginated result.
    /// Uses object initialization that respects private setters.
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>An empty paginated result.</returns>
    public static PaginatedResult<Sale> GenerateEmptyPaginatedResult(int page = 1, int pageSize = 10)
    {
        // Create empty list and use the constructor or factory method if available
        var emptyItems = new List<Sale>();
        var totalCount = 0;
        var totalPages = 0;

        // If PaginatedResult has a constructor that accepts these parameters, use it
        // Otherwise, we'll need to use reflection or a factory method
        var result = new PaginatedResult<Sale>([], totalCount, page, pageSize);

        // Use reflection to set the properties since they might have private setters
        //var itemsProperty = typeof(PaginatedResult<Sale>).GetProperty("Items");
        //var pageProperty = typeof(PaginatedResult<Sale>).GetProperty("Page");
        //var pageSizeProperty = typeof(PaginatedResult<Sale>).GetProperty("PageSize");
        //var totalCountProperty = typeof(PaginatedResult<Sale>).GetProperty("TotalCount");
        //var totalPagesProperty = typeof(PaginatedResult<Sale>).GetProperty("TotalPages");

        //itemsProperty?.SetValue(result, emptyItems);
        //pageProperty?.SetValue(result, page);
        //pageSizeProperty?.SetValue(result, pageSize);
        //totalCountProperty?.SetValue(result, totalCount);
        //totalPagesProperty?.SetValue(result, totalPages);

        return result;
    }

    /// <summary>
    /// Generates a list of GetSalesResult for testing mapping.
    /// </summary>
    /// <param name="count">Number of results to generate</param>
    /// <returns>A list of GetSalesResult objects.</returns>
    public static List<GetSalesResult> GenerateGetSalesResults(int count)
    {
        var faker = new Faker<GetSalesResult>()
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.SaleNumber, f => f.Random.Number(1000, 9999).ToString())
            .RuleFor(r => r.SaleDate, f => f.Date.Recent(30))
            .RuleFor(r => r.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(r => r.CancelledAt, f => f.Random.Bool(0.1f) ? f.Date.Recent(10) : null)
            .RuleFor(r => r.CustomerName, f => f.Person.FullName)
            .RuleFor(r => r.CustomerEmail, f => f.Internet.Email())
            .RuleFor(r => r.CustomerId, f => f.Random.Guid())
            .RuleFor(r => r.BranchName, f => f.Company.CompanyName())
            .RuleFor(r => r.BranchId, f => f.Random.Guid())
            .RuleFor(r => r.TotalAmount, f => f.Random.Decimal(10, 1000))
            .RuleFor(r => r.Currency, f => "BRL")
            .RuleFor(r => r.Status, f => f.PickRandom<SaleStatus>())
            .RuleFor(r => r.TotalItemsCount, f => f.Random.Int(1, 10))
            .RuleFor(r => r.HasDiscountedItems, f => f.Random.Bool());

        return faker.Generate(count);
    }
}