using Project.Dal.UnitOfWork;
using Project.Domain.Models;
using Project.Api.Models.DTOs;

namespace Project.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null)
            return null;

        return MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            name = createCustomerDto.Name,
            phone = createCustomerDto.Phone
        };

        await _unitOfWork.Customers.AddAsync(customer);
        return MapToDto(customer);
    }

    public async Task UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer == null)
            throw new Exception("Customer not found");

        customer.name = updateCustomerDto.Name;
        customer.phone = updateCustomerDto.Phone;

        await _unitOfWork.Customers.UpdateAsync(customer);
    }

    public async Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Customers.DeleteAsync(id);
    }

    public async Task<CustomerDto> GetCustomerByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByPhoneAsync(phone);
        if (customer == null)
            return null;

        return MapToDto(customer);
    }

    private CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.customer_id,
            Name = customer.name,
            Phone = customer.phone
        };
    }
}