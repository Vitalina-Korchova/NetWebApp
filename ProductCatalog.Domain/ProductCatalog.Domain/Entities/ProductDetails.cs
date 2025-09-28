namespace ProductCatalog.Domain.Entities;

public class ProductDetails
{
    public int ProductDetailsId { get; set; }
    public int ProductId { get; set; }
    public double Weight { get; set; }
    public string Brand { get; set; }
    public string Color { get; set; }

    // Navigation property
    public virtual Product Product { get; set; }
}