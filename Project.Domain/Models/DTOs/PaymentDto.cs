namespace Project.Api.Models.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string TransactionId { get; set; }
}

public class CreatePaymentDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionId { get; set; }
}

public class UpdatePaymentStatusDto
{
    public string Status { get; set; }
}