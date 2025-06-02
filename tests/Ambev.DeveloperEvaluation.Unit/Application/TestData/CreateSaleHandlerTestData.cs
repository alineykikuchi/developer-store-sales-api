using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data for CreateSale operations using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateSaleCommand.
    /// The generated commands will have valid:
    /// - SaleNumber (unique formatted number)
    /// - Customer (with valid Id, Name, and Email)
    /// - Branch (with valid Id, Name, and Address)
    /// - Items (list of valid items with products)
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.SaleNumber, f => f.Random.Number(1000, 99999).ToString())
        .RuleFor(c => c.Customer, f => new CreateSaleCustomer
        {
            Id = f.Random.Guid(),
            Name = f.Person.FullName,
            Email = f.Internet.Email()
        })
        .RuleFor(c => c.Branch, f => new CreateSaleBranch
        {
            Id = f.Random.Guid(),
            Name = f.Company.CompanyName(),
            Address = f.Address.FullAddress()
        })
        .RuleFor(c => c.Items, f => GenerateValidItems(f.Random.Int(1, 3))); // 1 to 3 items

    /// <summary>
    /// Configures the Faker to generate valid CreateSaleItem.
    /// The generated items will have valid:
    /// - ProductId (valid GUID)
    /// - ProductName and Description
    /// - Quantity (between 1-20)
    /// - UnitPrice (positive decimal)
    /// - Currency (BRL by default)
    /// </summary>
    private static readonly Faker<CreateSaleItem> createSaleItemFaker = new Faker<CreateSaleItem>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductDescription())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 1000))
        .RuleFor(i => i.Currency, f => "BRL");

    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - SaleNumber (formatted sale number)
    /// - Customer information (CustomerId value object)
    /// - Branch information (BranchId value object)
    /// - Status (Active by default)
    /// - Items collection (empty by default, can be populated)
    /// </summary>
    private static readonly Faker<Sale> saleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Number(1000, 99999).ToString(),
            new CustomerId(f.Random.Guid(), f.Person.FullName, f.Internet.Email()),
            new BranchId(f.Random.Guid(), f.Company.CompanyName(), f.Address.FullAddress())
        ));

    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CreateSaleCommand with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateSaleCommand with a specific sale number.
    /// </summary>
    /// <param name="saleNumber">The sale number to use</param>
    /// <returns>A valid CreateSaleCommand with the specified sale number.</returns>
    public static CreateSaleCommand GenerateValidCommandWithSaleNumber(string saleNumber)
    {
        var command = createSaleCommandFaker.Generate();
        command.SaleNumber = saleNumber;
        return command;
    }

    /// <summary>
    /// Generates a valid CreateSaleCommand with a specific number of items.
    /// </summary>
    /// <param name="itemCount">Number of items to include</param>
    /// <returns>A valid CreateSaleCommand with the specified number of items.</returns>
    public static CreateSaleCommand GenerateValidCommandWithItems(int itemCount)
    {
        var command = createSaleCommandFaker.Generate();
        command.Items = GenerateValidItems(itemCount);
        return command;
    }

    /// <summary>
    /// Generates a valid CreateSaleCommand with a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="customerName">The customer name</param>
    /// <param name="customerEmail">The customer email</param>
    /// <returns>A valid CreateSaleCommand with the specified customer.</returns>
    public static CreateSaleCommand GenerateValidCommandWithCustomer(Guid customerId, string customerName, string customerEmail)
    {
        var command = createSaleCommandFaker.Generate();
        command.Customer = new CreateSaleCustomer
        {
            Id = customerId,
            Name = customerName,
            Email = customerEmail
        };
        return command;
    }

    /// <summary>
    /// Generates a list of valid CreateSaleItem objects.
    /// </summary>
    /// <param name="count">Number of items to generate</param>
    /// <returns>A list of valid CreateSaleItem objects.</returns>
    public static List<CreateSaleItem> GenerateValidItems(int count)
    {
        return createSaleItemFaker.Generate(count);
    }

    /// <summary>
    /// Generates a single valid CreateSaleItem.
    /// </summary>
    /// <returns>A valid CreateSaleItem with randomly generated data.</returns>
    public static CreateSaleItem GenerateValidItem()
    {
        return createSaleItemFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateSaleItem with specific quantity (for discount testing).
    /// </summary>
    /// <param name="quantity">The quantity to set</param>
    /// <returns>A valid CreateSaleItem with the specified quantity.</returns>
    public static CreateSaleItem GenerateValidItemWithQuantity(int quantity)
    {
        var item = createSaleItemFaker.Generate();
        item.Quantity = quantity;
        return item;
    }

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return saleFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity with specific sale number.
    /// Uses the same Faker patterns for Customer and Branch to maintain consistency.
    /// </summary>
    /// <param name="saleNumber">The sale number to use</param>
    /// <returns>A valid Sale entity with the specified sale number.</returns>
    public static Sale GenerateValidSaleWithSaleNumber(string saleNumber)
    {
        var faker = new Faker();
        var customerId = new CustomerId(
            faker.Random.Guid(),
            faker.Person.FullName,
            faker.Internet.Email()
        );
        var branchId = new BranchId(
            faker.Random.Guid(),
            faker.Company.CompanyName(),
            faker.Address.FullAddress()
        );
        return new Sale(saleNumber, customerId, branchId);
    }

    /// <summary>
    /// Generates a valid Sale entity that matches the command data.
    /// This ensures the Sale entity has the same SaleNumber, Customer, and Branch as the command.
    /// </summary>
    /// <param name="command">The command to match</param>
    /// <returns>A valid Sale entity matching the command data.</returns>
    public static Sale GenerateValidSaleFromCommand(CreateSaleCommand command)
    {
        var customerId = new CustomerId(
            command.Customer.Id,
            command.Customer.Name,
            command.Customer.Email
        );
        var branchId = new BranchId(
            command.Branch.Id,
            command.Branch.Name,
            command.Branch.Address
        );
        return new Sale(command.SaleNumber, customerId, branchId);
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

    /// <summary>
    /// Generates an invalid CreateSaleCommand (empty sale number).
    /// </summary>
    /// <returns>An invalid CreateSaleCommand for testing validation.</returns>
    public static CreateSaleCommand GenerateInvalidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = string.Empty, // Invalid: empty sale number
            Customer = new CreateSaleCustomer(),
            Branch = new CreateSaleBranch(),
            Items = new List<CreateSaleItem>()
        };
    }

    /// <summary>
    /// Generates a CreateSaleCommand with invalid customer email.
    /// </summary>
    /// <returns>A CreateSaleCommand with invalid customer data.</returns>
    public static CreateSaleCommand GenerateCommandWithInvalidEmail()
    {
        var command = createSaleCommandFaker.Generate();
        command.Customer.Email = "invalid-email"; // Invalid email format
        return command;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with no items.
    /// </summary>
    /// <returns>A CreateSaleCommand with empty items list.</returns>
    public static CreateSaleCommand GenerateCommandWithNoItems()
    {
        var command = createSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItem>(); // No items
        return command;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with invalid item quantity.
    /// </summary>
    /// <returns>A CreateSaleCommand with item having invalid quantity.</returns>
    public static CreateSaleCommand GenerateCommandWithInvalidItemQuantity()
    {
        var command = createSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItem>
        {
            new CreateSaleItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                Quantity = 0, // Invalid: zero quantity
                UnitPrice = 10.0m,
                Currency = "BRL"
            }
        };
        return command;
    }

    /// <summary>
    /// Generates a CreateSaleCommand with item quantity exceeding limit.
    /// </summary>
    /// <returns>A CreateSaleCommand with item having excessive quantity.</returns>
    public static CreateSaleCommand GenerateCommandWithExcessiveItemQuantity()
    {
        var command = createSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItem>
        {
            new CreateSaleItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                Quantity = 25, // Invalid: exceeds 20 limit
                UnitPrice = 10.0m,
                Currency = "BRL"
            }
        };
        return command;
    }
}