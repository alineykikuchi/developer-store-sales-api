using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications
{
    public class SaleIsCancelledSpecificationTests
    {
        [Theory]
        [InlineData(SaleStatus.Cancelled, true)]
        [InlineData(SaleStatus.Active, false)]
        public void IsSatisfiedBy_ShouldValidateSaleStatus(SaleStatus status, bool expectedResult)
        {
            // Arrange
            var sale = SaleSpecificationTestData.GenerateSale(status);
            var specification = new SaleIsCancelledSpecification();

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().Be(expectedResult);
        }
    }

}