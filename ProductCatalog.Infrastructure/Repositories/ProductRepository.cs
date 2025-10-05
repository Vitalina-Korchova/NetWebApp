using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        return await _dbSet
            .Include(p => p.ProductDetails)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
            .Include(p => p.ProductImages)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithImagesAsync(int productId)
    {
        var product = await _dbSet.FindAsync(productId);
        if (product != null)
        {
            await _context.Entry(product)
                .Collection(p => p.ProductImages)
                .LoadAsync();
        }
        return product;
    }
    public IQueryable<Product> GetProductsQueryable()
    {
        return _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductDetails)
            .AsQueryable();
    }
}