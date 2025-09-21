using Microsoft.AspNetCore.Mvc;
using Project.Api.Services;
using Project.Api.Models.DTOs;

namespace Project.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int id, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id, cancellationToken);
        if (payment == null)
            return NotFound();
        
        return Ok(payment);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(CancellationToken cancellationToken)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(cancellationToken);
        return Ok(payments);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentByOrder(int orderId, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId, cancellationToken);
        if (payment == null)
            return NotFound();
        
        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto, CancellationToken cancellationToken)
    {
        var payment = await _paymentService.CreatePaymentAsync(createPaymentDto, cancellationToken);
        return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto updateDto, CancellationToken cancellationToken)
    {
        await _paymentService.UpdatePaymentStatusAsync(id, updateDto.Status, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id, CancellationToken cancellationToken)
    {
        await _paymentService.DeletePaymentAsync(id, cancellationToken);
        return NoContent();
    }
}