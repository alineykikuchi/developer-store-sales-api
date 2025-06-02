using Ambev.DeveloperEvaluation.Application.Sales.AddItemToSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for AddItemToSale operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class AddItemToSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid AddItemToSaleCommand.
    /// The generated commands will have valid:
    /// - SaleId (valid GUID)
    /// - Product (with valid Id, Name, and Description)
    /// - Quantity (positive number between 1-20)
    /// - UnitPrice (positive decimal value)
    /// - Currency (valid currency code, default BRL)
    /// </summary>
    private static readonly Faker<AddItemToSaleCommand> addItemToSaleCommandFaker = new Faker<AddItemToSaleCommand>()
        .RuleFor(c => c.SaleId, f => f.Random.Guid())
        .RuleFor(c => c.Product, f => new AddItemToSaleProduct
        {
            Id = f.Random.Guid(),
            Name = f.Commerce.ProductName(),
            Description = f.Commerce.ProductDescription()
        })
        .RuleFor(c => c.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(c => c.UnitPrice, f => f.Random.Decimal(1, 1000))
        .RuleFor(c => c.Currency, f => f.PickRandom("BRL"));
        //.RuleFor(c => c.Currency, f => f.PickRandom("BRL", "USD", "EUR")); //TODO: no futuro pode colocar outra moeda

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - Id (valid GUID)
    /// - SaleNumber (formatted sale number)
    /// - Customer information
    /// - Branch information  
    /// - Status (Active or Completed)
    /// - Items collection (empty by default)
    /// - TotalAmount (Money value object)
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Number(1000, 9999).ToString(),
            new CustomerId(f.Random.Guid(), f.Person.FullName, f.Internet.Email()),
            new BranchId(f.Random.Guid(), f.Company.CompanyName(), f.Address.FullAddress())
        ));

    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// The generated items will have valid:
    /// - Product information (ProductId value object)
    /// - Quantity (positive number between 1-20)
    /// - UnitPrice (Money value object)
    /// - Automatic discount calculation based on quantity
    /// - Total amount calculation
    /// </summary>
    private static readonly Faker<SaleItem> saleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f =>
        {
            var productId = new ProductId(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                f.Commerce.ProductDescription()
            );
            var quantity = f.Random.Int(1, 20);
            var unitPrice = new Money(f.Random.Decimal(1, 500), "BRL");

            return new SaleItem(productId, quantity, unitPrice);
        });

    /// <summary>
    /// Generates a valid AddItemToSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid AddItemToSaleCommand with randomly generated data.</returns>
    public static AddItemToSaleCommand GenerateValidCommand()
    {
        return addItemToSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// and an empty items collection.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return saleFaker.Generate();
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
    /// Generates a valid Sale entity with existing items.
    /// Uses reflection to access private fields since Items is read-only.
    /// </summary>
    /// <param name="itemCount">Number of items to include in the sale</param>
    /// <returns>A valid Sale entity with the specified number of items.</returns>
    public static Sale GenerateValidSaleWithItems(int itemCount = 1)
    {
        var sale = saleFaker.Generate();
        var items = saleItemFaker.Generate(itemCount);

        // Add items to sale using the AddItem method to maintain domain integrity
        foreach (var item in items)
        {
            sale.AddItem(item.Product, item.Quantity, item.UnitPrice);
        }

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with a specific item that matches the given product ID.
    /// </summary>
    /// <param name="productId">The product ID to include in the sale</param>
    /// <returns>A valid Sale entity with an item containing the specified product.</returns>
    public static Sale GenerateValidSaleWithSpecificProduct(Guid productId)
    {
        var sale = saleFaker.Generate();
        var productIdObj = new ProductId(productId, "Test Product", "Test Description");
        var unitPrice = new Money(10.50m, "BRL");

        sale.AddItem(productIdObj, 2, unitPrice);
        return sale;
    }

    /// <summary>
    /// Generates a valid SaleItem entity with randomized data.
    /// </summary>
    /// <returns>A valid SaleItem entity with randomly generated data.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        return saleItemFaker.Generate();
    }

    /// <summary>
    /// Generates a valid AddItemToSaleCommand for a specific sale and product.
    /// </summary>
    /// <param name="saleId">The sale ID</param>
    /// <param name="productId">The product ID</param>
    /// <returns>A valid AddItemToSaleCommand with specified sale and product IDs.</returns>
    public static AddItemToSaleCommand GenerateValidCommandForSaleAndProduct(Guid saleId, Guid productId)
    {
        var command = addItemToSaleCommandFaker.Generate();
        command.SaleId = saleId;
        command.Product.Id = productId;
        return command;
    }
}