namespace Project.Domain.Models;

public class OrderItem
{
    public int order_item_id { get; set; }
    public int product_id { get; set; }
    public int quantity { get; set; }
    public int order_id { get; set; }
}