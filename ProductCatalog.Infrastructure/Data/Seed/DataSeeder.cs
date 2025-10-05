using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Data.Seed;

public class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            await context.Database.MigrateAsync();
            
            // Спочатку створюємо категорії та ЗБЕРІГАЄМО їх
            if (!await context.Categories.AnyAsync())
            {
                await SeedCategories(context);
                await context.SaveChangesAsync(); // ← ДОДАЙТЕ ЦЕ
            }

            // Потім створюємо продукти
            if (!await context.Products.AnyAsync())
            {
                await SeedProducts(context);
                await context.SaveChangesAsync(); // ← ДОДАЙТЕ ЦЕ
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Seeder failed", ex);
        }
    }

    private static async Task SeedCategories(ApplicationDbContext context)
    {
        var categories = new[]
        {
            new Category { Name = "Electronics", Slug = "electronics", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Clothing", Slug = "clothing", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Books", Slug = "books", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Home & Garden", Slug = "home-garden", CreatedAt = DateTime.UtcNow }
        };

        await context.Categories.AddRangeAsync(categories);
        // НЕ викликайте SaveChangesAsync() тут - воно вже в основному методі
    }

    private static async Task SeedProducts(ApplicationDbContext context)
    {
        var electronicsCategory = await context.Categories.FirstAsync(c => c.Slug == "electronics");
        var clothingCategory = await context.Categories.FirstAsync(c => c.Slug == "clothing");

        var products = new[]
        {
            new Product
            {
                Name = "Smartphone X",
                Price = 99900,
                QuantityInStock = 50,
                SKU = "SMX-001",
                CreatedAt = DateTime.UtcNow,
                ProductDetails = new ProductDetails
                {
                    Weight = 0.2,
                    Brand = "TechBrand",
                    Color = "Black"
                },
                ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = electronicsCategory.CategoryId, CreatedAt = DateTime.UtcNow }
                },
                ProductImages = new List<ProductImage>
                {
                    new() { ImageUrl = "https://example.com/images/phone1.jpg" }
                }
            },
            new Product
            {
                Name = "Laptop Pro",
                Price = 199900,
                QuantityInStock = 25,
                SKU = "LP-002",
                CreatedAt = DateTime.UtcNow,
                ProductDetails = new ProductDetails
                {
                    Weight = 1.5,
                    Brand = "TechBrand",
                    Color = "Silver"
                },
                ProductCategories = new List<ProductCategory>
                {
                    new() { CategoryId = electronicsCategory.CategoryId, CreatedAt = DateTime.UtcNow }
                }
            }
        };

        await context.Products.AddRangeAsync(products);
       
    }
}