namespace Project.Api.Models.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}


public class UpdateOrderStatusDto
{
    public string Status { get; set; }
}

public class UpdateOrderDto
{
    public int CustomerId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

