namespace ProductCatalog.Domain.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}