using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Extensions;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Repositories;

namespace ProductCatalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductFilterRequest filter)
    {
        // Use the new method from repository
        var query = _unitOfWork.Products.GetProductsQueryable();

        // Apply filtering
        query = query.FilterProducts(filter);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting and pagination
        var products = await query
            .Sort(filter.SortBy, filter.SortDirection)
            .Paginate(filter)
            .ToListAsync();

        var productDtos = _mapper.Map<List<ProductDto>>(products);

        return PaginatedResult<ProductDto>.Create(productDtos, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
        return _mapper.Map<ProductDetailDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        product.CreatedAt = DateTime.UtcNow;

        foreach (var categoryId in createProductDto.CategoryIds)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category != null)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    CategoryId = categoryId,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateProductAsync(int id, ProductDto productDto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        _mapper.Map(productDto, product);
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found");

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}