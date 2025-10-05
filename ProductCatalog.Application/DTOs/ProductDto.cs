namespace ProductCatalog.Application.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Price { get; set; }
    public long QuantityInStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string SKU { get; set; } = string.Empty;
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public long Price { get; set; }
    public long QuantityInStock { get; set; }
    public string SKU { get; set; } = string.Empty;
    public List<int> CategoryIds { get; set; } = new();
}

public class ProductDetailDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Price { get; set; }
    public long QuantityInStock { get; set; }
    public string SKU { get; set; } = string.Empty;
    public ProductDetailsDto? ProductDetails { get; set; }
    public List<ProductImageDto> ProductImages { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
}

public class ProductDetailsDto
{
    public double Weight { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class ProductImageDto
{
    public string ImageUrl { get; set; } = string.Empty;
}