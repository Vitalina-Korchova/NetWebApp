namespace Project.Domain.Models;

public class Order
{
    public int order_id { get; set; }
    public int customer_id { get; set; }
    public string status { get; set; }
    public decimal total_amount { get; set; }
    public DateTime order_date { get; set; }
    public DateTime updated_at { get; set; }
}