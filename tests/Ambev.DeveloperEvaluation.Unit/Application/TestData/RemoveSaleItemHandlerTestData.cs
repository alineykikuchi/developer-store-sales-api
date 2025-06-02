using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for RemoveSaleItem operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class RemoveSaleItemHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid RemoveSaleItemCommand.
    /// The generated commands will have valid:
    /// - SaleId (valid GUID)
    /// - ItemId (valid GUID)
    /// </summary>
    private static readonly Faker<RemoveSaleItemCommand> removeSaleItemCommandFaker = new Faker<RemoveSaleItemCommand>()
        .CustomInstantiator(f => new RemoveSaleItemCommand(f.Random.Guid(), f.Random.Guid()));

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - Id (valid GUID)
    /// - SaleNumber (formatted sale number)
    /// - Customer information (CustomerId value object)
    /// - Branch information (BranchId value object)
    /// - Status (Active by default)
    /// - Items collection (can be populated)
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Number(1000, 99999).ToString(),
            new CustomerId(f.Random.Guid(), f.Person.FullName, f.Internet.Email()),
            new BranchId(f.Random.Guid(), f.Company.CompanyName(), f.Address.FullAddress())
        ));

    /// <summary>
    /// Generates a valid RemoveSaleItemCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid RemoveSaleItemCommand with randomly generated data.</returns>
    public static RemoveSaleItemCommand GenerateValidCommand()
    {
        return removeSaleItemCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid RemoveSaleItemCommand for specific sale and item.
    /// </summary>
    /// <param name="saleId">The sale ID</param>
    /// <param name="itemId">The item ID</param>
    /// <returns>A valid RemoveSaleItemCommand with specified IDs.</returns>
    public static RemoveSaleItemCommand GenerateValidCommandForSaleAndItem(Guid saleId, Guid itemId)
    {
        return new RemoveSaleItemCommand(saleId, itemId);
    }

    /// <summary>
    /// Generates an invalid RemoveSaleItemCommand with empty GUIDs.
    /// </summary>
    /// <returns>An invalid RemoveSaleItemCommand for testing validation.</returns>
    public static RemoveSaleItemCommand GenerateInvalidCommand()
    {
        return new RemoveSaleItemCommand(Guid.Empty, Guid.Empty);
    }

    /// <summary>
    /// Generates an invalid RemoveSaleItemCommand with empty sale ID.
    /// </summary>
    /// <returns>An invalid RemoveSaleItemCommand with empty sale ID.</returns>
    public static RemoveSaleItemCommand GenerateInvalidCommandWithEmptySaleId()
    {
        return new RemoveSaleItemCommand(Guid.Empty, Guid.NewGuid());
    }

    /// <summary>
    /// Generates an invalid RemoveSaleItemCommand with empty item ID.
    /// </summary>
    /// <returns>An invalid RemoveSaleItemCommand with empty item ID.</returns>
    public static RemoveSaleItemCommand GenerateInvalidCommandWithEmptyItemId()
    {
        return new RemoveSaleItemCommand(Guid.NewGuid(), Guid.Empty);
    }

    /// <summary>
    /// Generates a valid Sale entity with multiple items.
    /// </summary>
    /// <param name="itemCount">Number of items to add</param>
    /// <returns>A valid Sale entity with multiple items.</returns>
    public static Sale GenerateValidSaleWithMultipleItems(int itemCount = 3)
    {
        var sale = saleFaker.Generate();

        for (int i = 0; i < itemCount; i++)
        {
            var productId = new ProductId(Guid.NewGuid(), $"Product {i + 1}", $"Description {i + 1}");
            var unitPrice = new Money(10.0m * (i + 1), "BRL");
            sale.AddItem(productId, 2, unitPrice);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific item.
    /// </summary>
    /// <param name="itemId">The item ID to include</param>
    /// <param name="totalItems">Total number of items in the sale</param>
    /// <returns>A valid Sale entity with the specified item and total items.</returns>
    public static Sale GenerateValidSaleWithSpecificItem(Guid itemId, int totalItems = 3)
    {
        var sale = saleFaker.Generate();

        // Add the first item with the specified ID
        var firstProductId = new ProductId(Guid.NewGuid(), "First Product", "First Description");
        var firstUnitPrice = new Money(10.0m, "BRL");
        sale.AddItem(firstProductId, 2, firstUnitPrice);

        // Set the first item ID to the specified ID
        var firstItem = sale.Items.First();
        firstItem.GetType().GetProperty("Id")!.SetValue(firstItem, itemId);

        // Add remaining items if needed
        for (int i = 1; i < totalItems; i++)
        {
            var productId = new ProductId(Guid.NewGuid(), $"Product {i + 1}", $"Description {i + 1}");
            var unitPrice = new Money(10.0m * (i + 1), "BRL");
            sale.AddItem(productId, 2, unitPrice);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with only one item.
    /// This sale cannot have items removed due to business rules.
    /// </summary>
    /// <param name="itemId">The single item ID</param>
    /// <returns>A valid Sale entity with only one item.</returns>
    public static Sale GenerateValidSaleWithSingleItem(Guid itemId)
    {
        var sale = saleFaker.Generate();

        var productId = new ProductId(Guid.NewGuid(), "Single Product", "Single Description");
        var unitPrice = new Money(15.0m, "BRL");
        sale.AddItem(productId, 5, unitPrice);

        // Set the item ID
        var item = sale.Items.First();
        item.GetType().GetProperty("Id")!.SetValue(item, itemId);

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity without the specified item.
    /// </summary>
    /// <param name="itemId">The item ID that should NOT be in the sale</param>
    /// <returns>A valid Sale entity without the specified item.</returns>
    public static Sale GenerateValidSaleWithoutItem(Guid itemId)
    {
        var sale = GenerateValidSaleWithMultipleItems(2);

        // Ensure none of the items have the specified ID
        foreach (var item in sale.Items)
        {
            var newId = Guid.NewGuid();
            while (newId == itemId) // Ensure it's different
            {
                newId = Guid.NewGuid();
            }
            item.GetType().GetProperty("Id")!.SetValue(item, newId);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific status.
    /// Uses reflection to set the private Status property.
    /// </summary>
    /// <param name="status">The desired status</param>
    /// <param name="itemCount">Number of items in the sale</param>
    /// <returns>A valid Sale entity with the specified status.</returns>
    public static Sale GenerateValidSaleWithStatus(SaleStatus status, int itemCount = 3)
    {
        var sale = GenerateValidSaleWithMultipleItems(itemCount);

        // Use reflection to set the private Status property
        var statusProperty = typeof(Sale).GetProperty("Status");
        statusProperty?.SetValue(sale, status);

        // If status is Cancelled, also set the CancelledAt property
        if (status == SaleStatus.Cancelled)
        {
            var cancelledAtProperty = typeof(Sale).GetProperty("CancelledAt");
            cancelledAtProperty?.SetValue(cancelledAtProperty, DateTime.UtcNow);
        }

        return sale;
    }

    /// <summary>
    /// Generates a cancelled Sale entity with a specific item.
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <returns>A cancelled Sale entity with the specified item.</returns>
    public static Sale GenerateCancelledSaleWithItem(Guid itemId)
    {
        var sale = GenerateValidSaleWithSpecificItem(itemId, 3);

        // Cancel the sale
        var statusProperty = typeof(Sale).GetProperty("Status");
        statusProperty?.SetValue(sale, SaleStatus.Cancelled);

        var cancelledAtProperty = typeof(Sale).GetProperty("CancelledAt");
        cancelledAtProperty?.SetValue(sale, DateTime.UtcNow);

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with items that have discounts.
    /// </summary>
    /// <param name="itemId">The item ID to include</param>
    /// <returns>A valid Sale entity with discounted items.</returns>
    public static Sale GenerateValidSaleWithDiscountedItems(Guid itemId)
    {
        var sale = saleFaker.Generate();

        // Add the first item with the specified ID and discount-triggering quantity
        var firstProductId = new ProductId(Guid.NewGuid(), "Discounted Product", "Product with discount");
        sale.AddItem(firstProductId, 10, new Money(10.0m, "BRL")); // 20% discount

        // Set the first item ID to the specified ID
        var firstItem = sale.Items.First();
        firstItem.GetType().GetProperty("Id")!.SetValue(firstItem, itemId);

        // Add another item with discount
        var secondProductId = new ProductId(Guid.NewGuid(), "Another Product", "Another description");
        sale.AddItem(secondProductId, 5, new Money(15.0m, "BRL"));  // 10% discount

        return sale;
    }

    /// <summary>
    /// Generates a valid RemoveSaleItemResult for testing.
    /// </summary>
    /// <returns>A valid RemoveSaleItemResult with sample data.</returns>
    public static RemoveSaleItemResult GenerateValidResult()
    {
        var faker = new Faker();
        return new RemoveSaleItemResult
        {
            SaleId = faker.Random.Guid(),
            SaleNumber = faker.Random.Number(1000, 9999).ToString(),
            RemovedItem = new RemoveSaleItemDetails
            {
                Id = faker.Random.Guid(),
                ProductId = faker.Random.Guid(),
                ProductName = faker.Commerce.ProductName(),
                ProductDescription = faker.Commerce.ProductDescription(),
                RemovedQuantity = 5,
                RemovedUnitPrice = 15.0m,
                UnitPriceCurrency = "BRL",
                RemovedDiscountPercentage = 10m,
                RemovedTotalAmount = 67.5m, // 5 * 15 * 0.9
                TotalAmountCurrency = "BRL",
                WasSuccessfullyRemoved = true
            },
            NewSaleTotalAmount = 150.0m,
            Currency = "BRL",
            TotalItemsCount = 10,
            HasDiscountedItems = true,
            UpdatedAt = DateTime.UtcNow,
            SaleIsEmpty = false
        };
    }

    /// <summary>
    /// Generates a valid RemoveSaleItemDetails for testing.
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <returns>A valid RemoveSaleItemDetails with sample data.</returns>
    public static RemoveSaleItemDetails GenerateValidRemovedItemDetails(Guid itemId)
    {
        var faker = new Faker();
        return new RemoveSaleItemDetails
        {
            Id = itemId,
            ProductId = faker.Random.Guid(),
            ProductName = faker.Commerce.ProductName(),
            ProductDescription = faker.Commerce.ProductDescription(),
            RemovedQuantity = 3,
            RemovedUnitPrice = 20.0m,
            UnitPriceCurrency = "BRL",
            RemovedDiscountPercentage = 0m,
            RemovedTotalAmount = 60.0m,
            TotalAmountCurrency = "BRL",
            WasSuccessfullyRemoved = true
        };
    }

    /// <summary>
    /// Generates a Sale entity that becomes empty after removing an item.
    /// Note: This is used for testing the SaleIsEmpty flag, even though 
    /// the business rule prevents removing the last item.
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <returns>A Sale entity that would be empty after removal.</returns>
    public static Sale GenerateSaleForEmptyTest(Guid itemId)
    {
        // This is for testing the mapping logic, not the business rule
        var sale = saleFaker.Generate();

        var productId = new ProductId(Guid.NewGuid(), "Last Product", "Last Description");
        var unitPrice = new Money(10.0m, "BRL");
        sale.AddItem(productId, 1, unitPrice);

        // Remove the item to simulate empty state for mapping test
        sale.RemoveItem(sale.Items.First().Id);

        return sale;
    }
}