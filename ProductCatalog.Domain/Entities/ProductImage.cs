namespace ProductCatalog.Domain.Entities;

public class ProductImage
{
    public int ProductImageId { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }

    // Navigation property
    public virtual Product Product { get; set; }
}