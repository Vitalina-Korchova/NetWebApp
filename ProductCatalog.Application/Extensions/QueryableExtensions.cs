using System.Linq.Expressions;
using ProductCatalog.Application.Common;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationRequest request)
    {
        return query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        // Handle nested properties for sorting
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression property = parameter;
        
        foreach (var member in sortBy.Split('.'))
        {
            property = Expression.Property(property, member);
        }

        var lambda = Expression.Lambda(property, parameter);

        var methodName = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) 
            ? "OrderByDescending" 
            : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<Product> FilterProducts(this IQueryable<Product> query, ProductFilterRequest filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(filter.SearchTerm) || 
                                    p.SKU.Contains(filter.SearchTerm));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == filter.CategoryId));
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            query = query.Where(p => p.ProductDetails != null && p.ProductDetails.Brand == filter.Brand);
        }

        return query;
    }
}