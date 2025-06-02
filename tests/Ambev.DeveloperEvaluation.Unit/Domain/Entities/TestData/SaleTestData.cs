using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Sale entity using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// that meet the system's requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        var saleNumber = GenerateValidSaleNumber();
        var customerId = GenerateValidCustomerId();
        var branchId = GenerateValidBranchId();

        return new Sale(saleNumber, customerId, branchId);
    }

    /// <summary>
    /// Generates a valid sale number.
    /// </summary>
    /// <returns>A valid sale number.</returns>
    public static string GenerateValidSaleNumber()
    {
        return new Faker().Random.AlphaNumeric(10);
    }

    /// <summary>
    /// Generates a valid CustomerId.
    /// </summary>
    /// <returns>A valid CustomerId.</returns>
    public static CustomerId GenerateValidCustomerId()
    {
        var faker = new Faker();
        return new CustomerId(
            Guid.NewGuid(),
            faker.Person.FullName,
            faker.Internet.Email()
        );
    }

    /// <summary>
    /// Generates a valid BranchId.
    /// </summary>
    /// <returns>A valid BranchId.</returns>
    public static BranchId GenerateValidBranchId()
    {
        var faker = new Faker();
        return new BranchId(
            Guid.NewGuid(),
            faker.Company.CompanyName(),
            faker.Address.FullAddress()
        );
    }

    /// <summary>
    /// Generates a valid ProductId.
    /// </summary>
    /// <returns>A valid ProductId.</returns>
    public static ProductId GenerateValidProductId()
    {
        var faker = new Faker();
        return new ProductId(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription()
        );
    }

    /// <summary>
    /// Generates a valid Money value.
    /// </summary>
    /// <returns>A valid Money value.</returns>
    public static Money GenerateValidMoney()
    {
        var faker = new Faker();
        var amount = faker.Random.Decimal(1, 1000);
        return new Money(amount);
    }

    /// <summary>
    /// Generates a valid quantity for sale items.
    /// </summary>
    /// <returns>A valid quantity between 1 and 10.</returns>
    public static int GenerateValidQuantity()
    {
        return new Faker().Random.Int(1, 10);
    }

    /// <summary>
    /// Generates a quantity that exceeds the maximum allowed (20).
    /// </summary>
    /// <returns>A quantity greater than 20.</returns>
    public static int GenerateExcessiveQuantity()
    {
        return new Faker().Random.Int(21, 50);
    }

    /// <summary>
    /// Generates a Sale with multiple items for testing purposes.
    /// </summary>
    /// <param name="itemCount">Number of items to add to the sale.</param>
    /// <returns>A Sale with the specified number of items.</returns>
    public static Sale GenerateSaleWithItems(int itemCount = 3)
    {
        var sale = GenerateValidSale();

        for (int i = 0; i < itemCount; i++)
        {
            var productId = GenerateValidProductId();
            var quantity = GenerateValidQuantity();
            var unitPrice = GenerateValidMoney();

            sale.AddItem(productId, quantity, unitPrice);
        }

        return sale;
    }

    /// <summary>
    /// Generates a cancelled Sale for testing purposes.
    /// </summary>
    /// <returns>A cancelled Sale.</returns>
    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateValidSale();
        sale.Cancel();
        return sale;
    }
}