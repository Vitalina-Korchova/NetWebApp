using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImage");

        builder.HasKey(pi => pi.ProductImageId);

        builder.Property(pi => pi.ProductId)
            .IsRequired();

        builder.Property(pi => pi.ImageUrl)
            .IsRequired()
            .HasMaxLength(170);

        // Many-to-one relationship with Product
        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(pi => pi.ProductId);
    }
}