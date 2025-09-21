namespace Project.Api.Services;
using Project.Api.Models.DTOs;
public interface ICustomerService
{
    Task<CustomerDto> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto, CancellationToken cancellationToken = default);
    Task UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto, CancellationToken cancellationToken = default);
    Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto> GetCustomerByPhoneAsync(string phone, CancellationToken cancellationToken = default);
}