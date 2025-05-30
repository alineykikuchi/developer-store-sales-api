using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.DiscountPercentage)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        // Foreign Key para Sale
        builder.Property<Guid>("SaleId")
            .HasColumnType("uuid")
            .IsRequired();

        // Configuração do Value Object Product (External Identity)
        builder.OwnsOne(si => si.Product, product =>
        {
            product.Property(p => p.Id)
                .HasColumnName("ProductId")
                .HasColumnType("uuid")
                .IsRequired();

            product.Property(p => p.Name)
                .HasColumnName("ProductName")
                .HasMaxLength(100)
                .IsRequired();

            product.Property(p => p.Description)
                .HasColumnName("ProductDescription")
                .HasMaxLength(500)
                .IsRequired();
        });

        // Configuração do Value Object Money para UnitPrice
        builder.OwnsOne(si => si.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("BRL")
                .IsRequired();
        });

        // Configuração do Value Object Money para TotalAmount
        builder.OwnsOne(si => si.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalAmountCurrency")
                .HasMaxLength(3)
                .HasDefaultValue("BRL")
                .IsRequired();
        });

    }
}
