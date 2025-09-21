using Project.Domain.Models;

namespace Project.Dal.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<Customer> GetByPhoneAsync(string phone);
}