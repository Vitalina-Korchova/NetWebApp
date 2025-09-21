using Project.Api.Models.DTOs;
using Project.Api.Services;
using Project.Dal.UnitOfWork;
using Project.Domain.Models;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null)
            return null;

        var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(id);
        return MapToDto(order, orderItems);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(customerId);
        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.order_id);
            result.Add(MapToDto(order, orderItems));
        }

        return result;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Просто вказуємо фіксовану суму або 0, оскільки цін продуктів немає
            decimal totalAmount = 0; // Або createOrderDto.Items.Sum(item => item.Quantity * 1) для фіксованої ціни

            var order = new Order
            {
                customer_id = createOrderDto.CustomerId,
                status = "pending",
                total_amount = totalAmount,
                order_date = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            await _unitOfWork.Orders.AddAsync(order);

            // Додаємо товари до замовлення
            foreach (var itemDto in createOrderDto.Items)
            {
                var orderItem = new OrderItem
                {
                    order_id = order.order_id,
                    product_id = itemDto.ProductId,
                    quantity = itemDto.Quantity
                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);
            }

            await _unitOfWork.CommitAsync();

            // Отримуємо додані товари для повернення в DTO
            var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.order_id);
            return MapToDto(order, orderItems);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Orders.UpdateStatusAsync(orderId, status);
    }

    public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            // Спочатку видаляємо товари замовлення
            await _unitOfWork.OrderItems.DeleteByOrderIdAsync(id);
            
            // Потім видаляємо саме замовлення
            await _unitOfWork.Orders.DeleteAsync(id);
            
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private OrderDto MapToDto(Order order, IEnumerable<OrderItem> orderItems = null)
    {
        var dto = new OrderDto
        {
            OrderId = order.order_id,
            CustomerId = order.customer_id,
            Status = order.status,
            TotalAmount = order.total_amount,
            OrderDate = order.order_date,
            UpdatedAt = order.updated_at
        };

        if (orderItems != null)
        {
            dto.Items = orderItems.Select(item => new OrderItemDto
            {
                ProductId = item.product_id,
                Quantity = item.quantity
            }).ToList();
        }

        return dto;
    }
}