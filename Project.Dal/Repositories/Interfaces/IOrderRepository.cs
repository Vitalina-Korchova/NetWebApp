using Project.Domain.Models;

namespace Project.Dal.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    Task<IEnumerable<Order>> GetByStatusAsync(string status);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task UpdateStatusAsync(int orderId, string status);
    Task DeleteAsync(int id);
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
    
    Task<int> BulkUpdateStatusAsync(IEnumerable<int> orderIds, string status);
    Task<dynamic> GetOrderStatisticsAsync();
    Task<bool> ValidateOrderBeforeUpdateAsync(int orderId);
}