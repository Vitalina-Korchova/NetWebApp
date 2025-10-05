namespace ProductCatalog.Infrastructure.Repositories;

using ProductCatalog.Domain.Entities;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetProductWithDetailsAsync(int productId);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithImagesAsync(int productId);
    IQueryable<Product> GetProductsQueryable();
}