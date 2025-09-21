using Project.Dal.UnitOfWork;
using Project.Domain.Models;
using Project.Api.Models.DTOs;

namespace Project.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentDto> GetPaymentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(CancellationToken cancellationToken = default)
    {
        var payments = await _unitOfWork.Payments.GetAllAsync();
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto> GetPaymentByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByOrderIdAsync(orderId);
        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, CancellationToken cancellationToken = default)
    {
        var payment = new Payment
        {
            order_id = createPaymentDto.OrderId,
            amount = createPaymentDto.Amount,
            payment_status = "pending",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            transaction_id = createPaymentDto.TransactionId
        };

        await _unitOfWork.Payments.AddAsync(payment);
        return MapToDto(payment);
    }

    public async Task UpdatePaymentStatusAsync(int paymentId, string status, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Payments.UpdateStatusAsync(paymentId, status);
    }

    public async Task UpdatePaymentAsync(int paymentId, UpdatePaymentDto updatePaymentDto, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

        payment.order_id = updatePaymentDto.OrderId;
        payment.amount = updatePaymentDto.Amount;
        payment.payment_status = updatePaymentDto.PaymentStatus;
        payment.transaction_id = updatePaymentDto.TransactionId;
        payment.updated_at = DateTime.UtcNow;

        await _unitOfWork.Payments.UpdateAsync(payment);
    }
    
    public async Task DeletePaymentAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Payments.DeleteAsync(id);
    }

    private PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            PaymentId = payment.payment_id,
            OrderId = payment.order_id,
            Amount = payment.amount,
            PaymentStatus = payment.payment_status,
            CreatedAt = payment.created_at,
            UpdatedAt = payment.updated_at,
            TransactionId = payment.transaction_id
        };
    }
}