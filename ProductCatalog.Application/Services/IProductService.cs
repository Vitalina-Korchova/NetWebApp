using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Services;

public interface IProductService
{
    Task<ProductDetailDto?> GetProductByIdAsync(int id);
    Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductFilterRequest filter);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task UpdateProductAsync(int id, ProductDto productDto);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
}