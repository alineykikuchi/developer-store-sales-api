using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications
{

    public class SaleCanBeCancelledSpecificationTests
    {
        [Theory]
        [InlineData(SaleStatus.Active, -15, 30, true)]  // Active sale, 15 days old, 30 days limit
        [InlineData(SaleStatus.Active, -30, 30, true)]  // Active sale, exactly 30 days old
        [InlineData(SaleStatus.Active, -31, 30, false)] // Active sale, 31 days old (too old)
        [InlineData(SaleStatus.Cancelled, -15, 30, false)] // Cancelled sale
        [InlineData(SaleStatus.Active, 0, 30, true)]    // Active sale from today
        public void IsSatisfiedBy_ShouldValidateCancellationRules(
            SaleStatus status,
            int daysOld,
            int maxCancellationDays,
            bool expectedResult)
        {
            // Arrange
            var saleDate = DateTime.UtcNow.AddDays(daysOld);
            var sale = SaleSpecificationTestData.GenerateSaleWithDate(status, saleDate);
            var specification = new SaleCanBeCancelledSpecification(maxCancellationDays);

            // Act
            var result = specification.IsSatisfiedBy(sale);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(15)]
        [InlineData(60)]
        [InlineData(90)]
        public void IsSatisfiedBy_WithDifferentCancellationPeriods_ShouldRespectCustomPeriod(int maxDays)
        {
            // Arrange
            var saleWithinPeriod = SaleSpecificationTestData.GenerateSaleWithDate(
                SaleStatus.Active,
                DateTime.UtcNow.AddDays(-(maxDays - 1)));
            var saleOutsidePeriod = SaleSpecificationTestData.GenerateSaleWithDate(
                SaleStatus.Active,
                DateTime.UtcNow.AddDays(-(maxDays + 1)));
            var specification = new SaleCanBeCancelledSpecification(maxDays);

            // Act
            var resultWithin = specification.IsSatisfiedBy(saleWithinPeriod);
            var resultOutside = specification.IsSatisfiedBy(saleOutsidePeriod);

            // Assert
            resultWithin.Should().BeTrue();
            resultOutside.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithDefaultParameters_ShouldSetMaxCancellationDaysTo30()
        {
            // Arrange
            var saleAt29Days = SaleSpecificationTestData.GenerateSaleWithDate(
                SaleStatus.Active,
                DateTime.UtcNow.AddDays(-29));
            var saleAt31Days = SaleSpecificationTestData.GenerateSaleWithDate(
                SaleStatus.Active,
                DateTime.UtcNow.AddDays(-31));
            var specification = new SaleCanBeCancelledSpecification(); // Using default

            // Act
            var result29Days = specification.IsSatisfiedBy(saleAt29Days);
            var result31Days = specification.IsSatisfiedBy(saleAt31Days);

            // Assert
            result29Days.Should().BeTrue();
            result31Days.Should().BeFalse();
        }
    }
}
