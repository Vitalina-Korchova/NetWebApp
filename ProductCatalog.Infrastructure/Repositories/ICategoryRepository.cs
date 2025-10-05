using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int categoryId);
}