using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(75);

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.QuantityInStock)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.CreatedAt);

        // Relationships
        builder.HasMany(p => p.ProductImages)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ProductDetails)
            .WithOne(pd => pd.Product)
            .HasForeignKey<ProductDetails>(pd => pd.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}