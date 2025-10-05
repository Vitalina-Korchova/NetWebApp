using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Services;

public interface ICategoryService
{
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
}