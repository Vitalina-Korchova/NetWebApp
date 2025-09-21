using Microsoft.AspNetCore.Mvc;
using Project.Api.Services;
using Project.Api.Models.DTOs;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
        return Ok(orders);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order == null)
            return NotFound();
        
        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(int customerId, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId, cancellationToken);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateOrderAsync(createOrderDto, cancellationToken);
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto, CancellationToken cancellationToken)
    {
        await _orderService.UpdateOrderAsync(id, updateOrderDto, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateDto, CancellationToken cancellationToken)
    {
        await _orderService.UpdateOrderStatusAsync(id, updateDto.Status, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        await _orderService.DeleteOrderAsync(id, cancellationToken);
        return NoContent();
    }
}