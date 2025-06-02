using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for SaleItem entity using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleItemTestData
{
    /// <summary>
    /// Generates a valid SaleItem entity with randomized data.
    /// The generated sale item will have all properties populated with valid values
    /// that meet the system's requirements.
    /// </summary>
    /// <returns>A valid SaleItem entity with randomly generated data.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        var productId = GenerateValidProductId();
        var quantity = GenerateValidQuantity();
        var unitPrice = GenerateValidMoney();

        return new SaleItem(productId, quantity, unitPrice);
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
    /// Generates a valid quantity for sale items (1-20).
    /// </summary>
    /// <returns>A valid quantity between 1 and 20.</returns>
    public static int GenerateValidQuantity()
    {
        return new Faker().Random.Int(1, 20);
    }

    /// <summary>
    /// Generates a quantity that triggers 10% discount (4-9 items).
    /// </summary>
    /// <returns>A quantity between 4 and 9.</returns>
    public static int GenerateQuantityForTenPercentDiscount()
    {
        return new Faker().Random.Int(4, 9);
    }

    /// <summary>
    /// Generates a quantity that triggers 20% discount (10-20 items).
    /// </summary>
    /// <returns>A quantity between 10 and 20.</returns>
    public static int GenerateQuantityForTwentyPercentDiscount()
    {
        return new Faker().Random.Int(10, 20);
    }

    /// <summary>
    /// Generates a quantity that doesn't trigger discount (1-3 items).
    /// </summary>
    /// <returns>A quantity between 1 and 3.</returns>
    public static int GenerateQuantityWithoutDiscount()
    {
        return new Faker().Random.Int(1, 3);
    }

    /// <summary>
    /// Generates a quantity that exceeds the maximum allowed (>20).
    /// </summary>
    /// <returns>A quantity greater than 20.</returns>
    public static int GenerateExcessiveQuantity()
    {
        return new Faker().Random.Int(21, 50);
    }

    /// <summary>
    /// Generates an invalid quantity (zero or negative).
    /// </summary>
    /// <returns>A quantity less than or equal to zero.</returns>
    public static int GenerateInvalidQuantity()
    {
        return new Faker().Random.Int(-10, 0);
    }

    /// <summary>
    /// Generates a SaleItem with specific quantity for testing discount rules.
    /// </summary>
    /// <param name="quantity">The specific quantity to use.</param>
    /// <returns>A SaleItem with the specified quantity.</returns>
    public static SaleItem GenerateSaleItemWithQuantity(int quantity)
    {
        var productId = GenerateValidProductId();
        var unitPrice = GenerateValidMoney();

        return new SaleItem(productId, quantity, unitPrice);
    }

    /// <summary>
    /// Generates a SaleItem with specific unit price for testing calculations.
    /// </summary>
    /// <param name="unitPrice">The specific unit price to use.</param>
    /// <returns>A SaleItem with the specified unit price.</returns>
    public static SaleItem GenerateSaleItemWithPrice(Money unitPrice)
    {
        var productId = GenerateValidProductId();
        var quantity = GenerateValidQuantity();

        return new SaleItem(productId, quantity, unitPrice);
    }

    /// <summary>
    /// Generates a SaleItem with specific quantity and price for precise testing.
    /// </summary>
    /// <param name="quantity">The specific quantity to use.</param>
    /// <param name="unitPrice">The specific unit price to use.</param>
    /// <returns>A SaleItem with the specified quantity and price.</returns>
    public static SaleItem GenerateSaleItemWithQuantityAndPrice(int quantity, Money unitPrice)
    {
        var productId = GenerateValidProductId();
        return new SaleItem(productId, quantity, unitPrice);
    }
}