using Dapper;
using Npgsql;
using Project.Domain.Models;
using Project.Dal.Repositories.Interfaces;
using System.Data;

namespace Project.Dal.Repositories.Implementations;


//ADO.NET + Dapper комбінований
public class PaymentRepository : IPaymentRepository
{
    private readonly string _connectionString;

    public PaymentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Payment> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Payment>(
            "SELECT * FROM payment WHERE payment_id = @id",
            new { id });
    }

    public async Task<Payment> GetByOrderIdAsync(int orderId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Payment>(
            "SELECT * FROM payment WHERE order_id = @orderId ORDER BY created_at DESC LIMIT 1",
            new { orderId });
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Payment>(
            "SELECT * FROM payment ORDER BY created_at DESC");
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Payment>(
            "SELECT * FROM payment WHERE payment_status = @status ORDER BY created_at DESC",
            new { status });
    }

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Payment>(
            "SELECT * FROM payment WHERE created_at BETWEEN @startDate AND @endDate ORDER BY created_at DESC",
            new { startDate, endDate });
    }

    public async Task AddAsync(Payment payment)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            @"INSERT INTO payment (order_id, amount, payment_status, created_at, updated_at, transaction_id)
              VALUES (@order_id, @amount, @payment_status, @created_at, @updated_at, @transaction_id)
              RETURNING payment_id",
            connection);
        
        command.Parameters.AddWithValue("order_id", payment.order_id);
        command.Parameters.AddWithValue("amount", payment.amount);
        command.Parameters.AddWithValue("payment_status", payment.payment_status ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("created_at", payment.created_at);
        command.Parameters.AddWithValue("updated_at", payment.updated_at);
        command.Parameters.AddWithValue("transaction_id", payment.transaction_id ?? (object)DBNull.Value);
        
        payment.payment_id = (int)await command.ExecuteScalarAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            @"UPDATE payment SET 
                order_id = @order_id,
                amount = @amount,
                payment_status = @payment_status,
                created_at = @created_at,
                updated_at = @updated_at,
                transaction_id = @transaction_id
              WHERE payment_id = @payment_id",
            connection);
        
        command.Parameters.AddWithValue("order_id", payment.order_id);
        command.Parameters.AddWithValue("amount", payment.amount);
        command.Parameters.AddWithValue("payment_status", payment.payment_status ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("created_at", payment.created_at);
        command.Parameters.AddWithValue("updated_at", payment.updated_at);
        command.Parameters.AddWithValue("transaction_id", payment.transaction_id ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("payment_id", payment.payment_id);
        
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateStatusAsync(int paymentId, string status)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"UPDATE payment SET 
                    payment_status = @status,
                    updated_at = @updated_at
                    WHERE payment_id = @payment_id";
        
        await connection.ExecuteAsync(sql, new 
        { 
            payment_id = paymentId, 
            status = status,
            updated_at = DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "DELETE FROM payment WHERE payment_id = @id",
            new { id });
    }

    public async Task<Payment> GetByTransactionIdAsync(string transactionId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Payment>(
            "SELECT * FROM payment WHERE transaction_id = @transactionId",
            new { transactionId });
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<decimal>(
            "SELECT COALESCE(SUM(amount), 0) FROM payment WHERE created_at BETWEEN @startDate AND @endDate AND payment_status = 'completed'",
            new { startDate, endDate });
    }

    public async Task<int> GetPaymentCountByStatusAsync(string status)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM payment WHERE payment_status = @status",
            new { status });
    }
   
}