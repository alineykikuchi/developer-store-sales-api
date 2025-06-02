using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover sale creation, item management, status changes, and business rules validation.
/// </summary>
public class SaleTests
{
    #region Constructor Tests

    /// <summary>
    /// Tests that a Sale can be created with valid parameters.
    /// </summary>
    [Fact(DisplayName = "Sale should be created successfully with valid parameters")]
    public void Given_ValidParameters_When_CreatingSale_Then_SaleShouldBeCreatedSuccessfully()
    {
        // Arrange
        var saleNumber = SaleTestData.GenerateValidSaleNumber();
        var customerId = SaleTestData.GenerateValidCustomerId();
        var branchId = SaleTestData.GenerateValidBranchId();

        // Act
        var sale = new Sale(saleNumber, customerId, branchId);

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.Equal(saleNumber, sale.SaleNumber);
        Assert.Equal(customerId, sale.Customer);
        Assert.Equal(branchId, sale.Branch);
        Assert.Equal(SaleStatus.Active, sale.Status);
        Assert.Equal(0, sale.TotalAmount.Amount);
        Assert.Equal("BRL", sale.TotalAmount.Currency); //TODO: Money não implementa os métodos Equals
        Assert.True(sale.SaleDate <= DateTime.UtcNow);
        Assert.True(sale.CreatedAt <= DateTime.UtcNow);
        Assert.Null(sale.CancelledAt);
        Assert.Empty(sale.Items);
    }

    /// <summary>
    /// Tests that creating a Sale with null sale number throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Creating sale with null sale number should throw ArgumentNullException")]
    public void Given_NullSaleNumber_When_CreatingSale_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var customerId = SaleTestData.GenerateValidCustomerId();
        var branchId = SaleTestData.GenerateValidBranchId();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Sale(null, customerId, branchId));
    }

    /// <summary>
    /// Tests that creating a Sale with null customer throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Creating sale with null customer should throw ArgumentNullException")]
    public void Given_NullCustomer_When_CreatingSale_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var saleNumber = SaleTestData.GenerateValidSaleNumber();
        var branchId = SaleTestData.GenerateValidBranchId();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Sale(saleNumber, null, branchId));
    }

    /// <summary>
    /// Tests that creating a Sale with null branch throws ArgumentNullException.
    /// </summary>
    [Fact(DisplayName = "Creating sale with null branch should throw ArgumentNullException")]
    public void Given_NullBranch_When_CreatingSale_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var saleNumber = SaleTestData.GenerateValidSaleNumber();
        var customerId = SaleTestData.GenerateValidCustomerId();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Sale(saleNumber, customerId, null));
    }

    #endregion

    #region Add Item Tests

    /// <summary>
    /// Tests that an item can be added to an active sale.
    /// </summary>
    [Fact(DisplayName = "Item should be added successfully to active sale")]
    public void Given_ActiveSale_When_AddingItem_Then_ItemShouldBeAddedSuccessfully()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var unitPrice = SaleTestData.GenerateValidMoney();

        // Act
        var item = sale.AddItem(productId, quantity, unitPrice);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(productId, item.Product);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.True(sale.TotalAmount.Amount > 0);
    }

    /// <summary>
    /// Tests that adding an item to a cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Adding item to cancelled sale should throw InvalidOperationException")]
    public void Given_CancelledSale_When_AddingItem_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var unitPrice = SaleTestData.GenerateValidMoney();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.AddItem(productId, quantity, unitPrice));
    }

    /// <summary>
    /// Tests that adding the same product increases quantity of existing item.
    /// </summary>
    [Fact(DisplayName = "Adding same product should increase quantity of existing item")]
    public void Given_ExistingProduct_When_AddingSameProduct_Then_QuantityShouldIncrease()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var initialQuantity = 5;
        var additionalQuantity = 3;
        var unitPrice = SaleTestData.GenerateValidMoney();

        // Act
        var firstItem = sale.AddItem(productId, initialQuantity, unitPrice);
        var secondItem = sale.AddItem(productId, additionalQuantity, unitPrice);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(firstItem, secondItem);
        Assert.Equal(initialQuantity + additionalQuantity, firstItem.Quantity);
    }

    /// <summary>
    /// Tests that adding more than 20 identical items throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Adding more than 20 identical items should throw InvalidOperationException")]
    public void Given_ExistingProduct_When_AddingMoreThan20Items_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var unitPrice = SaleTestData.GenerateValidMoney();

        sale.AddItem(productId, 15, unitPrice);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.AddItem(productId, 10, unitPrice));
    }

    #endregion

    #region Remove Item Tests

    /// <summary>
    /// Tests that an item can be removed from an active sale.
    /// </summary>
    [Fact(DisplayName = "Item should be removed successfully from active sale")]
    public void Given_SaleWithItem_When_RemovingItem_Then_ItemShouldBeRemovedSuccessfully()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var unitPrice = SaleTestData.GenerateValidMoney();

        var item = sale.AddItem(productId, quantity, unitPrice);

        // Act
        sale.RemoveItem(item.Id);

        // Assert
        Assert.Empty(sale.Items);
        Assert.Equal(0, sale.TotalAmount.Amount);
        Assert.Equal("BRL", sale.TotalAmount.Currency); //TODO: Money não implementa os métodos Equals
    }

    /// <summary>
    /// Tests that removing an item from a cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Removing item from cancelled sale should throw InvalidOperationException")]
    public void Given_CancelledSale_When_RemovingItem_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var unitPrice = SaleTestData.GenerateValidMoney();

        var item = sale.AddItem(productId, quantity, unitPrice);
        sale.Cancel();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.RemoveItem(item.Id));
    }

    /// <summary>
    /// Tests that removing a non-existent item throws ArgumentException.
    /// </summary>
    [Fact(DisplayName = "Removing non-existent item should throw ArgumentException")]
    public void Given_SaleWithoutItem_When_RemovingNonExistentItem_Then_ShouldThrowArgumentException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var nonExistentItemId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => sale.RemoveItem(nonExistentItemId));
    }

    #endregion

    #region Update Item Tests

    /// <summary>
    /// Tests that item quantity can be updated successfully.
    /// </summary>
    [Fact(DisplayName = "Item quantity should be updated successfully")]
    public void Given_SaleWithItem_When_UpdatingQuantity_Then_QuantityShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var initialQuantity = 5;
        var newQuantity = 10;
        var unitPrice = SaleTestData.GenerateValidMoney();

        var item = sale.AddItem(productId, initialQuantity, unitPrice);

        // Act
        sale.UpdateItemQuantity(item.Id, newQuantity);

        // Assert
        Assert.Equal(newQuantity, item.Quantity);
    }

    /// <summary>
    /// Tests that item price can be updated successfully.
    /// </summary>
    [Fact(DisplayName = "Item price should be updated successfully")]
    public void Given_SaleWithItem_When_UpdatingPrice_Then_PriceShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var initialPrice = new Money(10.00m);
        var newPrice = new Money(15.00m);

        var item = sale.AddItem(productId, quantity, initialPrice);

        // Act
        sale.UpdateItemPrice(item.Id, newPrice);

        // Assert
        Assert.Equal(newPrice, item.UnitPrice);
    }

    /// <summary>
    /// Tests that updating quantity in a cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Updating quantity in cancelled sale should throw InvalidOperationException")]
    public void Given_CancelledSale_When_UpdatingQuantity_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = SaleTestData.GenerateValidQuantity();
        var unitPrice = SaleTestData.GenerateValidMoney();

        var item = sale.AddItem(productId, quantity, unitPrice);
        sale.Cancel();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.UpdateItemQuantity(item.Id, 10));
    }

    #endregion

    #region Status Management Tests

    /// <summary>
    /// Tests that an active sale can be cancelled successfully.
    /// </summary>
    [Fact(DisplayName = "Active sale should be cancelled successfully")]
    public void Given_ActiveSale_When_Cancelling_Then_SaleShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.NotNull(sale.CancelledAt);
        Assert.True(sale.CancelledAt <= DateTime.UtcNow);
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Cancelling already cancelled sale should throw InvalidOperationException")]
    public void Given_CancelledSale_When_Cancelling_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.Cancel());
    }

    /// <summary>
    /// Tests that a cancelled sale can be reactivated successfully.
    /// </summary>
    [Fact(DisplayName = "Cancelled sale should be reactivated successfully")]
    public void Given_CancelledSale_When_Reactivating_Then_SaleShouldBeReactivated()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();

        // Act
        sale.Reactivate();

        // Assert
        Assert.Equal(SaleStatus.Active, sale.Status);
        Assert.Null(sale.CancelledAt);
    }

    /// <summary>
    /// Tests that reactivating an already active sale throws InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Reactivating already active sale should throw InvalidOperationException")]
    public void Given_ActiveSale_When_Reactivating_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.Reactivate());
    }

    #endregion

    #region Business Rules Tests

    /// <summary>
    /// Tests that total amount is calculated correctly.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly")]
    public void Given_SaleWithMultipleItems_When_CheckingTotalAmount_Then_TotalShouldBeCorrect()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var product1 = SaleTestData.GenerateValidProductId();
        var product2 = SaleTestData.GenerateValidProductId();

        var item1Price = new Money(10.00m);
        var item2Price = new Money(15.00m);
        var item1Quantity = 2;
        var item2Quantity = 3;

        // Act
        sale.AddItem(product1, item1Quantity, item1Price);
        sale.AddItem(product2, item2Quantity, item2Price);

        // Assert
        var expectedTotal = new Money((item1Price.Amount * item1Quantity) + (item2Price.Amount * item2Quantity));
        Assert.Equal(expectedTotal.Amount, sale.TotalAmount.Amount);
    }

    /// <summary>
    /// Tests that GetTotalItemsCount returns correct count.
    /// </summary>
    [Fact(DisplayName = "GetTotalItemsCount should return correct total")]
    public void Given_SaleWithMultipleItems_When_GettingTotalItemsCount_Then_CountShouldBeCorrect()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var product1 = SaleTestData.GenerateValidProductId();
        var product2 = SaleTestData.GenerateValidProductId();

        var quantity1 = 3;
        var quantity2 = 5;
        var unitPrice = SaleTestData.GenerateValidMoney();

        sale.AddItem(product1, quantity1, unitPrice);
        sale.AddItem(product2, quantity2, unitPrice);

        // Act
        var totalCount = sale.GetTotalItemsCount();

        // Assert
        Assert.Equal(quantity1 + quantity2, totalCount);
    }

    /// <summary>
    /// Tests that IsEligibleForBulkDiscount returns true when an item has 4 or more quantity.
    /// </summary>
    [Fact(DisplayName = "IsEligibleForBulkDiscount should return true when item quantity is 4 or more")]
    public void Given_SaleWithBulkItem_When_CheckingBulkDiscount_Then_ShouldReturnTrue()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = 4;
        var unitPrice = SaleTestData.GenerateValidMoney();

        sale.AddItem(productId, quantity, unitPrice);

        // Act
        var isEligible = sale.IsEligibleForBulkDiscount();

        // Assert
        Assert.True(isEligible);
    }

    /// <summary>
    /// Tests that IsEligibleForBulkDiscount returns false when no item has 4 or more quantity.
    /// </summary>
    [Fact(DisplayName = "IsEligibleForBulkDiscount should return false when no item has bulk quantity")]
    public void Given_SaleWithoutBulkItems_When_CheckingBulkDiscount_Then_ShouldReturnFalse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateValidProductId();
        var quantity = 3;
        var unitPrice = SaleTestData.GenerateValidMoney();

        sale.AddItem(productId, quantity, unitPrice);

        // Act
        var isEligible = sale.IsEligibleForBulkDiscount();

        // Assert
        Assert.False(isEligible);
    }

    #endregion
}