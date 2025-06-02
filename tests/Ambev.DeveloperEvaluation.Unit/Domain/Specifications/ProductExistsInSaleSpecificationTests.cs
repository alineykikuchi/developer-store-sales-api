using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications
{
    public class ProductExistsInSaleSpecificationTests
    {
        [Fact]
        public void IsSatisfiedBy_WhenProductExistsInSale_ShouldReturnTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithSpecificProduct(productId);
            var specification = new ProductExistsInSaleSpecification(productId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeTrue("Product should exist in the sale");
        }

        [Fact]
        public void IsSatisfiedBy_WhenProductDoesNotExistInSale_ShouldReturnFalse()
        {
            // Arrange
            var existingProductId = Guid.NewGuid();
            var nonExistentProductId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithSpecificProduct(existingProductId);
            var specification = new ProductExistsInSaleSpecification(nonExistentProductId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse("Product should not exist in the sale");
        }

        [Fact]
        public void IsSatisfiedBy_WhenSaleHasNoItems_ShouldReturnFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithoutItems();
            var specification = new ProductExistsInSaleSpecification(productId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse("Sale with no items should not contain any product");
        }

        [Fact]
        public void IsSatisfiedBy_WhenSaleIsNull_ShouldReturnFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var specification = new ProductExistsInSaleSpecification(productId);

            // Act
            var result = specification.IsSatisfiedBy(null);

            // Assert
            result.Should().BeFalse("Null sale should not contain any product");
        }

        [Fact]
        public void IsSatisfiedBy_WhenSaleHasMultipleItems_AndProductExists_ShouldReturnTrue()
        {
            // Arrange
            var targetProductId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithMultipleProducts(
                new[] { Guid.NewGuid(), targetProductId, Guid.NewGuid() });
            var specification = new ProductExistsInSaleSpecification(targetProductId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeTrue("Product should be found among multiple items");
        }

        [Fact]
        public void IsSatisfiedBy_WhenSaleHasMultipleItems_AndProductDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithMultipleProducts(
                new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
            var specification = new ProductExistsInSaleSpecification(nonExistentProductId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse("Product should not be found among multiple items");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void IsSatisfiedBy_WhenProductExistsInSaleWithVariousItemCounts_ShouldReturnTrue(int itemCount)
        {
            // Arrange
            var targetProductId = Guid.NewGuid();
            var sale = SaleSpecificationTestData.GenerateSaleWithItemsIncludingProduct(itemCount, targetProductId);
            var specification = new ProductExistsInSaleSpecification(targetProductId);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeTrue($"Product should be found in sale with {itemCount} items");
        }

        [Fact]
        public void Constructor_ShouldAcceptValidProductId()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var specification = new ProductExistsInSaleSpecification(productId);

            // Assert
            specification.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldAcceptEmptyGuid()
        {
            // Arrange & Act
            var specification = new ProductExistsInSaleSpecification(Guid.Empty);

            // Assert
            specification.Should().NotBeNull();
        }

        [Fact]
        public void IsSatisfiedBy_WithEmptyGuidProductId_ShouldReturnFalseWhenNoMatch()
        {
            // Arrange
            var sale = SaleSpecificationTestData.GenerateSaleWithSpecificProduct(Guid.NewGuid());
            var specification = new ProductExistsInSaleSpecification(Guid.Empty);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().BeFalse("Empty GUID should not match any real product ID");
        }
    }
}