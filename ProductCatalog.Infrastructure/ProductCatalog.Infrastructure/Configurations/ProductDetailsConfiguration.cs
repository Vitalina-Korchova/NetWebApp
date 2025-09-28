using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Configurations;

public class ProductDetailsConfiguration : IEntityTypeConfiguration<ProductDetails>
{
    public void Configure(EntityTypeBuilder<ProductDetails> builder)
    {
        builder.ToTable("ProductDetails");

        builder.HasKey(pd => pd.ProductDetailsId);

        builder.Property(pd => pd.ProductId)
            .IsRequired();

        builder.Property(pd => pd.Weight)
            .IsRequired();

        builder.Property(pd => pd.Brand)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(pd => pd.Color)
            .IsRequired()
            .HasMaxLength(45);

        // One-to-one relationship with Product
        builder.HasOne(pd => pd.Product)
            .WithOne(p => p.ProductDetails)
            .HasForeignKey<ProductDetails>(pd => pd.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(pd => pd.ProductId)
            .IsUnique();
    }
}