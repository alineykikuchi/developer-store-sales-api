using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover item creation, quantity updates, price updates, discount rules, and validation scenarios.
/// </summary>
public class SaleItemTests
{
    #region Constructor Tests

    /// <summary>
    /// Tests that a SaleItem can be created with valid parameters.
    /// </summary>
    [Fact(DisplayName = "SaleItem should be created successfully with valid parameters")]
    public void Given_ValidParameters_When_CreatingSaleItem_Then_SaleItemShouldBeCreatedSuccessfully()
    {
        // Arrange
        var productId = SaleItemTestData.GenerateValidProductId();
        var quantity = SaleItemTestData.GenerateValidQuantity();
        var unitPrice = SaleItemTestData.GenerateValidMoney();

        // Act
        var saleItem = new SaleItem(productId, quantity, unitPrice);

        // Assert
        Assert.Equal(productId, saleItem.Product);
        Assert.Equal(quantity, saleItem.Quantity);
        Assert.Equal(unitPrice, saleItem.UnitPrice);
        Assert.True(saleItem.TotalAmount.Amount > 0);
    }

    /// <summary>
    /// Tests that creating a SaleItem with null product throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Creating SaleItem with null product should throw ArgumentNullException")]
    public void Given_NullProduct_When_CreatingSaleItem_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateValidQuantity();
        var unitPrice = SaleItemTestData.GenerateValidMoney();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SaleItem(null, quantity, unitPrice));
    }

    /// <summary>
    /// Tests that creating a SaleItem with null unit price throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Creating SaleItem with null unit price should throw ArgumentNullException")]
    public void Given_NullUnitPrice_When_CreatingSaleItem_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var productId = SaleItemTestData.GenerateValidProductId();
        var quantity = SaleItemTestData.GenerateValidQuantity();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SaleItem(productId, quantity, null));
    }

    /// <summary>
    /// Tests that creating a SaleItem with invalid quantity throws ArgumentException.
    /// </summary>
    [Fact(DisplayName = "Creating SaleItem with invalid quantity should throw ArgumentException")]
    public void Given_InvalidQuantity_When_CreatingSaleItem_Then_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = SaleItemTestData.GenerateValidProductId();
        var invalidQuantity = SaleItemTestData.GenerateInvalidQuantity();
        var unitPrice = SaleItemTestData.GenerateValidMoney();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SaleItem(productId, invalidQuantity, unitPrice));
    }

    /// <summary>
    /// Tests that creating a SaleItem with excessive quantity throws ArgumentException.
    /// </summary>
    [Fact(DisplayName = "Creating SaleItem with excessive quantity should throw ArgumentException")]
    public void Given_ExcessiveQuantity_When_CreatingSaleItem_Then_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = SaleItemTestData.GenerateValidProductId();
        var excessiveQuantity = SaleItemTestData.GenerateExcessiveQuantity();
        var unitPrice = SaleItemTestData.GenerateValidMoney();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SaleItem(productId, excessiveQuantity, unitPrice));
    }

    #endregion

    #region Discount Rules Tests

    /// <summary>
    /// Tests that no discount is applied for quantities 1-3.
    /// </summary>
    [Fact(DisplayName = "No discount should be applied for quantities 1-3")]
    public void Given_QuantityBetween1And3_When_CreatingSaleItem_Then_NoDiscountShouldBeApplied()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateQuantityWithoutDiscount();
        var unitPrice = new Money(100.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        Assert.Equal(0m, saleItem.DiscountPercentage);
        Assert.Equal(unitPrice.Amount * quantity, saleItem.TotalAmount.Amount);
    }

    /// <summary>
    /// Tests that 10% discount is applied for quantities 4-9.
    /// </summary>
    [Fact(DisplayName = "10% discount should be applied for quantities 4-9")]
    public void Given_QuantityBetween4And9_When_CreatingSaleItem_Then_TenPercentDiscountShouldBeApplied()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateQuantityForTenPercentDiscount();
        var unitPrice = new Money(100.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);

        var expectedSubtotal = unitPrice.Amount * quantity;
        var expectedDiscount = expectedSubtotal * 0.10m;
        var expectedTotal = expectedSubtotal - expectedDiscount;

        Assert.Equal(expectedTotal, saleItem.TotalAmount.Amount);
    }

    /// <summary>
    /// Tests that 20% discount is applied for quantities 10-20.
    /// </summary>
    [Fact(DisplayName = "20% discount should be applied for quantities 10-20")]
    public void Given_QuantityBetween10And20_When_CreatingSaleItem_Then_TwentyPercentDiscountShouldBeApplied()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateQuantityForTwentyPercentDiscount();
        var unitPrice = new Money(100.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        Assert.Equal(20m, saleItem.DiscountPercentage);

        var expectedSubtotal = unitPrice.Amount * quantity;
        var expectedDiscount = expectedSubtotal * 0.20m;
        var expectedTotal = expectedSubtotal - expectedDiscount;

        Assert.Equal(expectedTotal, saleItem.TotalAmount.Amount);
    }

    /// <summary>
    /// Tests specific discount calculation for 4 items.
    /// </summary>
    [Fact(DisplayName = "4 items should have exactly 10% discount")]
    public void Given_ExactlyFourItems_When_CreatingSaleItem_Then_ShouldHaveExactlyTenPercentDiscount()
    {
        // Arrange
        var quantity = 4;
        var unitPrice = new Money(10.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);
        Assert.Equal(36.00m, saleItem.TotalAmount.Amount); // 40 - 10% = 36
    }

    /// <summary>
    /// Tests specific discount calculation for 10 items.
    /// </summary>
    [Fact(DisplayName = "10 items should have exactly 20% discount")]
    public void Given_ExactlyTenItems_When_CreatingSaleItem_Then_ShouldHaveExactlyTwentyPercentDiscount()
    {
        // Arrange
        var quantity = 10;
        var unitPrice = new Money(10.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        Assert.Equal(20m, saleItem.DiscountPercentage);
        Assert.Equal(80.00m, saleItem.TotalAmount.Amount); // 100 - 20% = 80
    }

    #endregion

    #region Update Quantity Tests

    /// <summary>
    /// Tests that quantity can be updated successfully and discounts are recalculated.
    /// </summary>
    [Fact(DisplayName = "Quantity should be updated successfully and discounts recalculated")]
    public void Given_SaleItem_When_UpdatingQuantity_Then_QuantityAndDiscountShouldBeUpdated()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(2, new Money(10.00m));
        var newQuantity = 5;

        // Act
        saleItem.UpdateQuantity(newQuantity);

        // Assert
        Assert.Equal(newQuantity, saleItem.Quantity);
        Assert.Equal(10m, saleItem.DiscountPercentage); // 5 items = 10% discount
        Assert.Equal(45.00m, saleItem.TotalAmount.Amount); // 50 - 10% = 45
    }

    /// <summary>
    /// Tests that updating quantity to invalid value throws ArgumentException.
    /// </summary>
    [Fact(DisplayName = "Updating quantity to invalid value should throw ArgumentException")]
    public void Given_SaleItem_When_UpdatingToInvalidQuantity_Then_ShouldThrowArgumentException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        var invalidQuantity = SaleItemTestData.GenerateInvalidQuantity();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => saleItem.UpdateQuantity(invalidQuantity));
    }

    /// <summary>
    /// Tests that updating quantity to excessive value throws ArgumentException.
    /// </summary>
    [Fact(DisplayName = "Updating quantity to excessive value should throw ArgumentException")]
    public void Given_SaleItem_When_UpdatingToExcessiveQuantity_Then_ShouldThrowArgumentException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        var excessiveQuantity = SaleItemTestData.GenerateExcessiveQuantity();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => saleItem.UpdateQuantity(excessiveQuantity));
    }

    /// <summary>
    /// Tests that updating from discount to no discount recalculates correctly.
    /// </summary>
    [Fact(DisplayName = "Updating from discount to no discount should recalculate correctly")]
    public void Given_SaleItemWithDiscount_When_UpdatingToQuantityWithoutDiscount_Then_ShouldRecalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(5, new Money(10.00m));
        Assert.Equal(10m, saleItem.DiscountPercentage); // Initial 10% discount

        // Act
        saleItem.UpdateQuantity(2);

        // Assert
        Assert.Equal(2, saleItem.Quantity);
        Assert.Equal(0m, saleItem.DiscountPercentage); // No discount for 2 items
        Assert.Equal(20.00m, saleItem.TotalAmount.Amount); // 2 * 10 = 20 (no discount)
    }

    #endregion

    #region Update Unit Price Tests

    /// <summary>
    /// Tests that unit price can be updated successfully and total amount is recalculated.
    /// </summary>
    [Fact(DisplayName = "Unit price should be updated successfully and total recalculated")]
    public void Given_SaleItem_When_UpdatingUnitPrice_Then_UnitPriceAndTotalShouldBeUpdated()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(5, new Money(10.00m));
        var newUnitPrice = new Money(20.00m);

        // Act
        saleItem.UpdateUnitPrice(newUnitPrice);

        // Assert
        Assert.Equal(newUnitPrice, saleItem.UnitPrice);
        Assert.Equal(90.00m, saleItem.TotalAmount.Amount); // 5 * 20 = 100, 100 - 10% = 90
    }

    /// <summary>
    /// Tests that updating unit price to null throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Updating unit price to null should throw ArgumentNullException")]
    public void Given_SaleItem_When_UpdatingToNullUnitPrice_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => saleItem.UpdateUnitPrice(null));
    }

    #endregion

    #region Total Amount Calculation Tests

    /// <summary>
    /// Tests that total amount is calculated correctly without discount.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly without discount")]
    public void Given_SaleItemWithoutDiscount_When_CheckingTotalAmount_Then_ShouldEqualQuantityTimesPrice()
    {
        // Arrange
        var quantity = 3;
        var unitPrice = new Money(15.50m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        var expectedTotal = quantity * unitPrice.Amount;
        Assert.Equal(expectedTotal, saleItem.TotalAmount.Amount);
    }

    /// <summary>
    /// Tests that total amount is calculated correctly with discount.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly with discount")]
    public void Given_SaleItemWithDiscount_When_CheckingTotalAmount_Then_ShouldApplyDiscountCorrectly()
    {
        // Arrange
        var quantity = 6; // Should get 10% discount
        var unitPrice = new Money(25.00m);

        // Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(quantity, unitPrice);

        // Assert
        var subtotal = quantity * unitPrice.Amount; // 6 * 25 = 150
        var discount = subtotal * 0.10m; // 15
        var expectedTotal = subtotal - discount; // 135

        Assert.Equal(expectedTotal, saleItem.TotalAmount.Amount);
    }

    #endregion

    #region Edge Cases Tests

    /// <summary>
    /// Tests boundary condition at quantity 4 (first discount tier).
    /// </summary>
    [Fact(DisplayName = "Quantity 4 should be at discount boundary")]
    public void Given_QuantityFour_When_CreatingSaleItem_Then_ShouldHaveTenPercentDiscount()
    {
        // Arrange & Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(4, new Money(10.00m));

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);
    }

    /// <summary>
    /// Tests boundary condition at quantity 9 (end of first discount tier).
    /// </summary>
    [Fact(DisplayName = "Quantity 9 should still have 10% discount")]
    public void Given_QuantityNine_When_CreatingSaleItem_Then_ShouldHaveTenPercentDiscount()
    {
        // Arrange & Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(9, new Money(10.00m));

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);
    }

    /// <summary>
    /// Tests boundary condition at quantity 20 (maximum allowed).
    /// </summary>
    [Fact(DisplayName = "Quantity 20 should have 20% discount and be maximum allowed")]
    public void Given_QuantityTwenty_When_CreatingSaleItem_Then_ShouldHaveTwentyPercentDiscount()
    {
        // Arrange & Act
        var saleItem = SaleItemTestData.GenerateSaleItemWithQuantityAndPrice(20, new Money(10.00m));

        // Assert
        Assert.Equal(20, saleItem.Quantity);
        Assert.Equal(20m, saleItem.DiscountPercentage);
    }

    #endregion
}