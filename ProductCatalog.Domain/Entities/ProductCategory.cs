namespace ProductCatalog.Domain.Entities;

public class ProductCategory
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual Category Category { get; set; }
}