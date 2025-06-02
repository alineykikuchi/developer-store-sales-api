using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData
{
    /// <summary>
    /// Provides methods for generating test data using the Bogus library.
    /// This class centralizes all test data generation for Sale Specification tests
    /// to ensure consistency across test cases.
    /// </summary>
    public static class SaleSpecificationTestData
    {
        /// <summary>
        /// Configures the Faker to generate valid Sale entities.
        /// The generated sales will have valid:
        /// - SaleNumber (formatted as SALE-{number})
        /// - Customer information
        /// - Branch information
        /// Status is not set here as it's the main test parameter
        /// </summary>
        private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
            .CustomInstantiator(f =>
            {
                var customerId = new CustomerId(
                    f.Random.Guid(),
                    f.Company.CompanyName(),
                    f.Internet.Email()
                );

                var branchId = new BranchId(
                    f.Random.Guid(),
                    f.Address.City(),
                    f.Address.FullAddress()
                );

                var saleNumber = $"SALE-{f.Random.Number(1000, 9999)}";

                return new Sale(saleNumber, customerId, branchId);
            });

        /// <summary>
        /// Generates a valid Sale entity with the specified status.
        /// </summary>
        /// <param name="status">The SaleStatus to set for the generated sale.</param>
        /// <returns>A valid Sale entity with randomly generated data and specified status.</returns>
        public static Sale GenerateSale(SaleStatus status)
        {
            var sale = saleFaker.Generate();

            // Apply status change if needed
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Generates a valid Sale entity with the specified status and sale date.
        /// </summary>
        /// <param name="status">The SaleStatus to set for the generated sale.</param>
        /// <param name="saleDate">The sale date to set for the generated sale.</param>
        /// <returns>A valid Sale entity with randomly generated data, specified status and date.</returns>
        public static Sale GenerateSaleWithDate(SaleStatus status, DateTime saleDate)
        {
            var sale = saleFaker.Generate();

            // Set the sale date (assuming there's a way to modify it - adjust based on your Sale implementation)
            // If SaleDate is immutable, you might need to use reflection or create a test-specific constructor
            SetSaleDate(sale, saleDate);

            // Apply status change if needed
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Generates a Sale entity with specific customer and branch information.
        /// Useful for more controlled test scenarios.
        /// </summary>
        /// <param name="saleNumber">The sale number.</param>
        /// <param name="customerName">The customer name.</param>
        /// <param name="branchName">The branch name.</param>
        /// <param name="status">The sale status.</param>
        /// <returns>A Sale entity with specified parameters.</returns>
        public static Sale GenerateSaleWithSpecificData(
            string saleNumber,
            string customerName,
            string branchName,
            SaleStatus status)
        {
            var faker = new Faker();

            var customerId = new CustomerId(
                faker.Random.Guid(),
                customerName,
                faker.Internet.Email()
            );

            var branchId = new BranchId(
                faker.Random.Guid(),
                branchName,
                faker.Address.FullAddress()
            );

            var sale = new Sale(saleNumber, customerId, branchId);

            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Helper method to set sale date for testing purposes.
        /// This method should be adjusted based on your Sale entity implementation.
        /// </summary>
        /// <param name="sale">The sale entity.</param>
        /// <param name="saleDate">The date to set.</param>
        private static void SetSaleDate(Sale sale, DateTime saleDate)
        {
            // Option 1: If Sale has a public setter for SaleDate
            // sale.SaleDate = saleDate;

            // Option 2: If you need to use reflection (adjust property name as needed)
            var saleProperty = typeof(Sale).GetProperty("SaleDate");
            if (saleProperty != null && saleProperty.CanWrite)
            {
                saleProperty.SetValue(sale, saleDate);
            }
            else
            {
                // Option 3: Use reflection on private fields if needed
                var saleField = typeof(Sale).GetField("_saleDate",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                saleField?.SetValue(sale, saleDate);
            }

            // Option 4: If none of the above work, you might need to create a test-specific constructor
            // or factory method in your Sale entity for testing purposes
        }

        /// <summary>
        /// Generates a Sale with items for more comprehensive testing.
        /// </summary>
        /// <param name="status">The sale status.</param>
        /// <param name="itemCount">Number of items to add to the sale.</param>
        /// <returns>A Sale entity with the specified number of items.</returns>
        public static Sale GenerateSaleWithItems(SaleStatus status, int itemCount = 3)
        {
            var sale = GenerateSale(SaleStatus.Active); // Start with active to add items
            var faker = new Faker();

            for (int i = 0; i < itemCount; i++)
            {
                var productId = new ProductId(
                    faker.Random.Guid(),
                    faker.Commerce.ProductName(),
                    faker.Commerce.ProductDescription()
                );

                var unitPrice = new Money(
                    faker.Random.Decimal(10, 1000),
                    "BRL"
                );

                sale.AddItem(productId, faker.Random.Int(1, 10), unitPrice);
            }

            // Apply final status
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Generates a Sale with a specific product ID.
        /// </summary>
        /// <param name="productId">The specific product ID to include in the sale.</param>
        /// <param name="status">The sale status.</param>
        /// <returns>A Sale entity containing the specified product.</returns>
        public static Sale GenerateSaleWithSpecificProduct(Guid productId, SaleStatus status = SaleStatus.Active)
        {
            var sale = GenerateSale(SaleStatus.Active);
            var faker = new Faker();

            var product = new ProductId(
                productId,
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription()
            );

            var unitPrice = new Money(
                faker.Random.Decimal(10, 1000),
                "BRL"
            );

            sale.AddItem(product, faker.Random.Int(1, 10), unitPrice);

            // Apply final status
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Generates a Sale without any items.
        /// </summary>
        /// <param name="status">The sale status.</param>
        /// <returns>A Sale entity without items.</returns>
        public static Sale GenerateSaleWithoutItems(SaleStatus status = SaleStatus.Active)
        {
            var sale = GenerateSale(status);
            // No items added intentionally
            return sale;
        }

        /// <summary>
        /// Generates a Sale with multiple specific product IDs.
        /// </summary>
        /// <param name="productIds">Array of product IDs to include.</param>
        /// <param name="status">The sale status.</param>
        /// <returns>A Sale entity containing all specified products.</returns>
        public static Sale GenerateSaleWithMultipleProducts(Guid[] productIds, SaleStatus status = SaleStatus.Active)
        {
            var sale = GenerateSale(SaleStatus.Active);
            var faker = new Faker();

            foreach (var productId in productIds)
            {
                var product = new ProductId(
                    productId,
                    faker.Commerce.ProductName(),
                    faker.Commerce.ProductDescription()
                );

                var unitPrice = new Money(
                    faker.Random.Decimal(10, 1000),
                    "BRL"
                );

                sale.AddItem(product, faker.Random.Int(1, 10), unitPrice);
            }

            // Apply final status
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }

        /// <summary>
        /// Generates a Sale with a specified number of items, ensuring one specific product is included.
        /// </summary>
        /// <param name="totalItemCount">Total number of items in the sale.</param>
        /// <param name="targetProductId">The specific product ID that must be included.</param>
        /// <param name="status">The sale status.</param>
        /// <returns>A Sale entity with the specified item count including the target product.</returns>
        public static Sale GenerateSaleWithItemsIncludingProduct(int totalItemCount, Guid targetProductId, SaleStatus status = SaleStatus.Active)
        {
            if (totalItemCount < 1)
                throw new ArgumentException("Total item count must be at least 1", nameof(totalItemCount));

            var sale = GenerateSale(SaleStatus.Active);
            var faker = new Faker();

            // Add the target product first
            var targetProduct = new ProductId(
                targetProductId,
                faker.Commerce.ProductName(),
                faker.Commerce.ProductDescription()
            );

            var targetUnitPrice = new Money(
                faker.Random.Decimal(10, 1000),
                "BRL"
            );

            sale.AddItem(targetProduct, faker.Random.Int(1, 10), targetUnitPrice);

            // Add remaining items with random products
            for (int i = 1; i < totalItemCount; i++)
            {
                var randomProduct = new ProductId(
                    faker.Random.Guid(),
                    faker.Commerce.ProductName(),
                    faker.Commerce.ProductDescription()
                );

                var unitPrice = new Money(
                    faker.Random.Decimal(10, 1000),
                    "BRL"
                );

                sale.AddItem(randomProduct, faker.Random.Int(1, 10), unitPrice);
            }

            // Apply final status
            if (status == SaleStatus.Cancelled)
            {
                sale.Cancel();
            }

            return sale;
        }
    }
}