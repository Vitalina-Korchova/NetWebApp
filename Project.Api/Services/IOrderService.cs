
using Project.Api.Models.DTOs;

namespace Project.Api.Services;

public interface IOrderService
{
    Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);
    
    Task UpdateOrderStatusAsync(int orderId, string status, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default);
}

