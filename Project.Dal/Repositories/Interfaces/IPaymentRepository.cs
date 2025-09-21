using Project.Domain.Models;

namespace Project.Dal.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> GetByIdAsync(int id);
    Task<Payment> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(int id);
    Task<Payment> GetByTransactionIdAsync(string transactionId);
    Task UpdateStatusAsync(int paymentId, string status);
}