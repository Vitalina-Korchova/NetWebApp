using Microsoft.AspNetCore.Mvc;
using Project.Api.Services;
using Project.Api.Models.DTOs;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllCustomersAsync(cancellationToken);
        return Ok(customers);
    }

    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByPhone(string phone, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByPhoneAsync(phone, cancellationToken);
        if (customer == null)
            return NotFound();
        
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto, CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateCustomerAsync(createCustomerDto, cancellationToken);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateCustomerDto, CancellationToken cancellationToken)
    {
        await _customerService.UpdateCustomerAsync(id, updateCustomerDto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken)
    {
        await _customerService.DeleteCustomerAsync(id, cancellationToken);
        return NoContent();
    }
}