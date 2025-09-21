namespace Project.Domain.Models;

public class Payment
{
    public int payment_id { get; set; }
    public int order_id { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string transaction_id { get; set; }
}