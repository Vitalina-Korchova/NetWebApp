namespace ProductCatalog.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public long Price { get; set; }
    public long QuantityInStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string SKU { get; set; }

    // Navigation properties
    public virtual ProductDetails ProductDetails { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}