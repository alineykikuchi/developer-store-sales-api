using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for GetSale operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class GetSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetSaleCommand.
    /// The generated commands will have valid:
    /// - Id (valid GUID)
    /// </summary>
    private static readonly Faker<GetSaleCommand> getSaleCommandFaker = new Faker<GetSaleCommand>()
        .CustomInstantiator(f => new GetSaleCommand(f.Random.Guid()));

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
    /// Generates a valid GetSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid GetSaleCommand with randomly generated data.</returns>
    public static GetSaleCommand GenerateValidCommand()
    {
        return getSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid GetSaleCommand with a specific sale ID.
    /// </summary>
    /// <param name="saleId">The sale ID to use in the command</param>
    /// <returns>A valid GetSaleCommand with the specified sale ID.</returns>
    public static GetSaleCommand GenerateValidCommandForSale(Guid saleId)
    {
        return new GetSaleCommand(saleId);
    }

    /// <summary>
    /// Generates an invalid GetSaleCommand with empty GUID.
    /// </summary>
    /// <returns>An invalid GetSaleCommand for testing validation.</returns>
    public static GetSaleCommand GenerateInvalidCommand()
    {
        return new GetSaleCommand(Guid.Empty);
    }

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// and will be in Active status.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return saleFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific ID.
    /// </summary>
    /// <param name="saleId">The sale ID to use</param>
    /// <returns>A valid Sale entity with the specified ID.</returns>
    public static Sale GenerateValidSaleWithId(Guid saleId)
    {
        var sale = saleFaker.Generate();
        sale.GetType().GetProperty("Id")!.SetValue(sale, saleId);
        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific status.
    /// Uses reflection to set the private Status property since it doesn't have a public setter.
    /// </summary>
    /// <param name="status">The desired status for the sale</param>
    /// <returns>A valid Sale entity with the specified status.</returns>
    public static Sale GenerateValidSaleWithStatus(SaleStatus status)
    {
        var sale = saleFaker.Generate();

        // Use reflection to set the private Status property
        var statusProperty = typeof(Sale).GetProperty("Status");
        statusProperty?.SetValue(sale, status);

        // If status is Cancelled, also set the CancelledAt property
        if (status == SaleStatus.Cancelled)
        {
            var cancelledAtProperty = typeof(Sale).GetProperty("CancelledAt");
            cancelledAtProperty?.SetValue(sale, DateTime.UtcNow);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with items.
    /// </summary>
    /// <param name="itemCount">Number of items to include in the sale</param>
    /// <returns>A valid Sale entity with the specified number of items.</returns>
    public static Sale GenerateValidSaleWithItems(int itemCount = 1)
    {
        var sale = saleFaker.Generate();

        // Add items to sale using the AddItem method to maintain domain integrity
        for (int i = 0; i < itemCount; i++)
        {
            var productId = new ProductId(
                Guid.NewGuid(),
                $"Product {i + 1}",
                $"Description for product {i + 1}"
            );
            var unitPrice = new Money(10.50m * (i + 1), "BRL");
            sale.AddItem(productId, 2, unitPrice);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with items that have discounts.
    /// </summary>
    /// <param name="withDiscountedItems">Whether to include items with discounts</param>
    /// <returns>A valid Sale entity with discounted items if specified.</returns>
    public static Sale GenerateValidSaleWithDiscountedItems(bool withDiscountedItems = true)
    {
        var sale = saleFaker.Generate();

        if (withDiscountedItems)
        {
            // Add items with quantities that trigger discounts
            var productId1 = new ProductId(Guid.NewGuid(), "Product 1", "Description 1");
            var productId2 = new ProductId(Guid.NewGuid(), "Product 2", "Description 2");

            sale.AddItem(productId1, 5, new Money(10.0m, "BRL"));  // 10% discount
            sale.AddItem(productId2, 12, new Money(15.0m, "BRL")); // 20% discount
        }
        else
        {
            // Add items without discounts
            var productId = new ProductId(Guid.NewGuid(), "Product 1", "Description 1");
            sale.AddItem(productId, 2, new Money(10.0m, "BRL"));  // No discount
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific number of total items (considering quantities).
    /// </summary>
    /// <param name="totalItemsCount">The total number of items considering quantities</param>
    /// <returns>A valid Sale entity with the specified total items count.</returns>
    public static Sale GenerateValidSaleWithTotalItemsCount(int totalItemsCount)
    {
        var sale = saleFaker.Generate();

        // Add a single item with the specified quantity
        var productId = new ProductId(Guid.NewGuid(), "Test Product", "Test Description");
        var unitPrice = new Money(10.0m, "BRL");
        sale.AddItem(productId, totalItemsCount, unitPrice);

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with specific customer and branch data.
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="customerName">The customer name</param>
    /// <param name="customerEmail">The customer email</param>
    /// <param name="branchId">The branch ID</param>
    /// <param name="branchName">The branch name</param>
    /// <param name="branchAddress">The branch address</param>
    /// <returns>A valid Sale entity with the specified customer and branch data.</returns>
    public static Sale GenerateValidSaleWithCustomerAndBranch(
        Guid customerId, string customerName, string customerEmail,
        Guid branchId, string branchName, string branchAddress)
    {
        var customValue = new CustomerId(customerId, customerName, customerEmail);
        var branchValue = new BranchId(branchId, branchName, branchAddress);

        var faker = new Faker();
        return new Sale(faker.Random.Number(1000, 99999).ToString(), customValue, branchValue);
    }

    /// <summary>
    /// Generates a cancelled Sale entity with cancellation date.
    /// </summary>
    /// <returns>A valid cancelled Sale entity.</returns>
    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateValidSaleWithStatus(SaleStatus.Cancelled);

        // Ensure CancelledAt is set
        var cancelledAtProperty = typeof(Sale).GetProperty("CancelledAt");
        if (cancelledAtProperty?.GetValue(sale) == null)
        {
            cancelledAtProperty?.SetValue(sale, DateTime.UtcNow.AddDays(-1));
        }

        return sale;
    }
}