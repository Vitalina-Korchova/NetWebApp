using Project.Api.Models.DTOs;

namespace Project.Api.Services;

public interface IPaymentService
{
    Task<PaymentDto> GetPaymentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(CancellationToken cancellationToken = default);
    Task<PaymentDto> GetPaymentByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, CancellationToken cancellationToken = default);
    Task UpdatePaymentStatusAsync(int paymentId, string status, CancellationToken cancellationToken = default);
    Task DeletePaymentAsync(int id, CancellationToken cancellationToken = default);
}