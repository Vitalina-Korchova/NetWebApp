using Project.Domain.Models;

namespace Project.Dal.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task<OrderItem> GetByIdAsync(int id);
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
    Task AddAsync(OrderItem orderItem);
    Task UpdateAsync(OrderItem orderItem);
    Task DeleteAsync(int id);
    Task DeleteByOrderIdAsync(int orderId);
}