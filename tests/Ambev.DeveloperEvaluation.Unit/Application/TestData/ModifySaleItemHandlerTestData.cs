using Ambev.DeveloperEvaluation.Application.Sales.ModifySaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for ModifySaleItem operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class ModifySaleItemHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid ModifySaleItemCommand.
    /// The generated commands will have valid:
    /// - SaleId (valid GUID)
    /// - ItemId (valid GUID)
    /// - Quantity (optional, between 1-20)
    /// - UnitPrice (optional, positive decimal)
    /// - Currency (BRL by default)
    /// </summary>
    private static readonly Faker<ModifySaleItemCommand> modifySaleItemCommandFaker = new Faker<ModifySaleItemCommand>()
        .RuleFor(c => c.SaleId, f => f.Random.Guid())
        .RuleFor(c => c.ItemId, f => f.Random.Guid())
        .RuleFor(c => c.Quantity, f => f.Random.Bool(0.7f) ? f.Random.Int(1, 20) : null) // 70% chance
        .RuleFor(c => c.UnitPrice, f => f.Random.Bool(0.7f) ? f.Random.Decimal(1, 1000) : null) // 70% chance
        .RuleFor(c => c.Currency, f => "BRL");

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
    /// Generates a valid ModifySaleItemCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid ModifySaleItemCommand with randomly generated data.</returns>
    public static ModifySaleItemCommand GenerateValidCommand()
    {
        return modifySaleItemCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ModifySaleItemCommand for specific sale and item.
    /// </summary>
    /// <param name="saleId">The sale ID</param>
    /// <param name="itemId">The item ID</param>
    /// <returns>A valid ModifySaleItemCommand with specified IDs.</returns>
    public static ModifySaleItemCommand GenerateValidCommandForSaleAndItem(Guid saleId, Guid itemId)
    {
        var command = modifySaleItemCommandFaker.Generate();
        command.SaleId = saleId;
        command.ItemId = itemId;
        return command;
    }

    /// <summary>
    /// Generates a valid ModifySaleItemCommand with only quantity modification.
    /// </summary>
    /// <param name="quantity">The new quantity</param>
    /// <returns>A valid ModifySaleItemCommand with only quantity set.</returns>
    public static ModifySaleItemCommand GenerateValidCommandWithQuantityOnly(int quantity)
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = quantity,
            UnitPrice = null,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates a valid ModifySaleItemCommand with only price modification.
    /// </summary>
    /// <param name="unitPrice">The new unit price</param>
    /// <returns>A valid ModifySaleItemCommand with only price set.</returns>
    public static ModifySaleItemCommand GenerateValidCommandWithPriceOnly(decimal unitPrice)
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = null,
            UnitPrice = unitPrice,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates a valid ModifySaleItemCommand with both quantity and price modifications.
    /// </summary>
    /// <param name="quantity">The new quantity</param>
    /// <param name="unitPrice">The new unit price</param>
    /// <returns>A valid ModifySaleItemCommand with both fields set.</returns>
    public static ModifySaleItemCommand GenerateValidCommandWithQuantityAndPrice(int quantity, decimal unitPrice)
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates an invalid ModifySaleItemCommand with empty GUIDs.
    /// </summary>
    /// <returns>An invalid ModifySaleItemCommand for testing validation.</returns>
    public static ModifySaleItemCommand GenerateInvalidCommand()
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.Empty,
            ItemId = Guid.Empty,
            Quantity = null,
            UnitPrice = null,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates an invalid ModifySaleItemCommand with no modifications.
    /// </summary>
    /// <returns>An invalid ModifySaleItemCommand with no quantity or price.</returns>
    public static ModifySaleItemCommand GenerateInvalidCommandWithNoModifications()
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = null,
            UnitPrice = null,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates an invalid ModifySaleItemCommand with invalid quantity.
    /// </summary>
    /// <returns>An invalid ModifySaleItemCommand with zero quantity.</returns>
    public static ModifySaleItemCommand GenerateInvalidCommandWithInvalidQuantity()
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = 0, // Invalid
            UnitPrice = null,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates an invalid ModifySaleItemCommand with excessive quantity.
    /// </summary>
    /// <returns>An invalid ModifySaleItemCommand with quantity > 20.</returns>
    public static ModifySaleItemCommand GenerateInvalidCommandWithExcessiveQuantity()
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = 25, // Invalid: > 20
            UnitPrice = null,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates an invalid ModifySaleItemCommand with invalid price.
    /// </summary>
    /// <returns>An invalid ModifySaleItemCommand with zero price.</returns>
    public static ModifySaleItemCommand GenerateInvalidCommandWithInvalidPrice()
    {
        return new ModifySaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            Quantity = null,
            UnitPrice = 0, // Invalid
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific item.
    /// </summary>
    /// <param name="itemId">The item ID to include</param>
    /// <param name="quantity">Initial quantity</param>
    /// <param name="unitPrice">Initial unit price</param>
    /// <returns>A valid Sale entity with the specified item.</returns>
    public static Sale GenerateValidSaleWithItem(Guid itemId, int quantity = 5, decimal unitPrice = 10.0m)
    {
        var sale = saleFaker.Generate();

        var productId = new ProductId(Guid.NewGuid(), "Test Product", "Test Description");
        var money = new Money(unitPrice, "BRL");
        sale.AddItem(productId, quantity, money);

        // Set the item ID using reflection since it's generated internally
        var item = sale.Items.First();
        item.GetType().GetProperty("Id")!.SetValue(item, itemId);

        return sale;
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
    /// Generates a valid Sale entity with a specific status.
    /// Uses reflection to set the private Status property.
    /// </summary>
    /// <param name="status">The desired status</param>
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
    /// Generates a cancelled Sale entity with an item.
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <returns>A cancelled Sale entity with the specified item.</returns>
    public static Sale GenerateCancelledSaleWithItem(Guid itemId)
    {
        var sale = GenerateValidSaleWithItem(itemId);

        // Cancel the sale
        var statusProperty = typeof(Sale).GetProperty("Status");
        statusProperty?.SetValue(sale, SaleStatus.Cancelled);

        var cancelledAtProperty = typeof(Sale).GetProperty("CancelledAt");
        cancelledAtProperty?.SetValue(sale, DateTime.UtcNow);

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
    /// Generates a valid ModifySaleItemResult for testing.
    /// </summary>
    /// <returns>A valid ModifySaleItemResult with sample data.</returns>
    public static ModifySaleItemResult GenerateValidResult()
    {
        var faker = new Faker();
        return new ModifySaleItemResult
        {
            SaleId = faker.Random.Guid(),
            SaleNumber = faker.Random.Number(1000, 9999).ToString(),
            ModifiedItem = new ModifySaleItemDetails
            {
                Id = faker.Random.Guid(),
                ProductId = faker.Random.Guid(),
                ProductName = faker.Commerce.ProductName(),
                ProductDescription = faker.Commerce.ProductDescription(),
                PreviousQuantity = 5,
                NewQuantity = 8,
                PreviousUnitPrice = 10.0m,
                NewUnitPrice = 12.0m,
                UnitPriceCurrency = "BRL",
                PreviousDiscountPercentage = 0m,
                NewDiscountPercentage = 10m,
                PreviousTotalAmount = 50.0m,
                NewTotalAmount = 86.4m, // 8 * 12 * 0.9 (10% discount)
                TotalAmountCurrency = "BRL",
                QuantityChanged = true,
                PriceChanged = true,
                DiscountChanged = true
            },
            NewSaleTotalAmount = 86.4m,
            Currency = "BRL",
            TotalItemsCount = 8,
            HasDiscountedItems = true,
            UpdatedAt = DateTime.UtcNow
        };
    }
}