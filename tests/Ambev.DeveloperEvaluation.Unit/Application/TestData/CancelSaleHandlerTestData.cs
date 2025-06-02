using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for CancelSale operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CancelSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CancelSaleCommand.
    /// The generated commands will have valid:
    /// - Id (valid GUID)
    /// - CancellationReason (optional string up to 500 characters)
    /// </summary>
    private static readonly Faker<CancelSaleCommand> cancelSaleCommandFaker = new Faker<CancelSaleCommand>()
        .CustomInstantiator(f => new CancelSaleCommand(
            f.Random.Guid(),
            f.Random.Bool(0.7f) ? f.Lorem.Sentence(10) : null // 70% chance of having a reason
        ));

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - Id (valid GUID)
    /// - SaleNumber (formatted sale number)
    /// - SaleDate (recent date within 30 days)
    /// - Customer information
    /// - Branch information  
    /// - Status (Active by default)
    /// - Items collection (empty by default)
    /// - TotalAmount (Money value object)
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Number(1000, 9999).ToString(),
            new CustomerId(f.Random.Guid(), f.Person.FullName, f.Internet.Email()),
            new BranchId(f.Random.Guid(), f.Company.CompanyName(), f.Address.FullAddress())
        ))
        .FinishWith((f, sale) =>
        {
            // Set SaleDate to a recent date (within 30 days) to ensure it can be cancelled
            var saleDateProperty = typeof(Sale).GetProperty("SaleDate");
            saleDateProperty?.SetValue(sale, f.Date.Recent(25)); // Within cancellation window
        });

    /// <summary>
    /// Generates a valid CancelSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CancelSaleCommand with randomly generated data.</returns>
    public static CancelSaleCommand GenerateValidCommand()
    {
        return cancelSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CancelSaleCommand with a specific sale ID.
    /// </summary>
    /// <param name="saleId">The sale ID to use in the command</param>
    /// <returns>A valid CancelSaleCommand with the specified sale ID.</returns>
    public static CancelSaleCommand GenerateValidCommandForSale(Guid saleId)
    {
        var command = cancelSaleCommandFaker.Generate();
        var newCommand = new CancelSaleCommand(saleId, command.CancellationReason);
        return newCommand;
    }

    /// <summary>
    /// Generates a valid CancelSaleCommand with a specific cancellation reason.
    /// </summary>
    /// <param name="reason">The cancellation reason</param>
    /// <returns>A valid CancelSaleCommand with the specified reason.</returns>
    public static CancelSaleCommand GenerateValidCommandWithReason(string reason)
    {
        return new CancelSaleCommand(Guid.NewGuid(), reason);
    }

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// and will be in Active status with recent sale date (cancellable).
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
    /// Generates a valid Sale entity with a specific sale date.
    /// Uses reflection to set the private SaleDate property.
    /// </summary>
    /// <param name="saleDate">The desired sale date</param>
    /// <returns>A valid Sale entity with the specified sale date.</returns>
    public static Sale GenerateValidSaleWithDate(DateTime saleDate)
    {
        var sale = saleFaker.Generate();

        // Use reflection to set the private SaleDate property
        var saleDateProperty = typeof(Sale).GetProperty("SaleDate");
        saleDateProperty?.SetValue(sale, saleDate);

        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity that is older than 30 days (not cancellable due to age).
    /// </summary>
    /// <returns>A valid Sale entity that is too old to be cancelled.</returns>
    public static Sale GenerateOldSale()
    {
        var oldDate = DateTime.UtcNow.AddDays(-35); // 35 days ago (beyond 30-day limit)
        return GenerateValidSaleWithDate(oldDate);
    }

    /// <summary>
    /// Generates a valid Sale entity that can be cancelled (Active status, recent date).
    /// </summary>
    /// <returns>A valid Sale entity that meets all cancellation criteria.</returns>
    public static Sale GenerateCancellableSale()
    {
        var sale = GenerateValidSale(); // Already generates with recent date and Active status
        return sale;
    }

    /// <summary>
    /// Generates a valid Sale entity with items.
    /// </summary>
    /// <param name="itemCount">Number of items to include in the sale</param>
    /// <returns>A valid Sale entity with the specified number of items.</returns>
    public static Sale GenerateValidSaleWithItems(int itemCount = 1)
    {
        var sale = saleFaker.Generate();

        // Add items to sale using the AddItem method to maintain domain integrity
        for (int i = 0; i < itemCount; i++)
        {
            var productId = new ProductId(
                Guid.NewGuid(),
                $"Product {i + 1}",
                $"Description for product {i + 1}"
            );
            var unitPrice = new Money(10.50m * (i + 1), "BRL");
            sale.AddItem(productId, 2, unitPrice);
        }

        return sale;
    }
}