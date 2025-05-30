using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.Property(s => s.SaleDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.CancelledAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Configuração do Value Object Customer (External Identity)
        builder.OwnsOne(s => s.Customer, customer =>
        {
            customer.Property(c => c.Id)
                .HasColumnName("CustomerId")
                .HasColumnType("uuid")
                .IsRequired();

            customer.Property(c => c.Name)
                .HasColumnName("CustomerName")
                .HasMaxLength(100)
                .IsRequired();

            customer.Property(c => c.Email)
                .HasColumnName("CustomerEmail")
                .HasMaxLength(150)
                .IsRequired();
        });

        // Configuração do Value Object Branch (External Identity)
        builder.OwnsOne(s => s.Branch, branch =>
        {
            branch.Property(b => b.Id)
                .HasColumnName("BranchId")
                .HasColumnType("uuid")
                .IsRequired();

            branch.Property(b => b.Name)
                .HasColumnName("BranchName")
                .HasMaxLength(100)
                .IsRequired();

            branch.Property(b => b.Address)
                .HasColumnName("BranchAddress")
                .HasMaxLength(200)
                .IsRequired();
        });

        // Configuração do Value Object Money para TotalAmount
        builder.OwnsOne(s => s.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("BRL")
                .IsRequired();
        });

        // Relacionamento com SaleItems
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        // Configuração para ignorar a propriedade privada _items
        builder.Ignore("_items");

    }
}
