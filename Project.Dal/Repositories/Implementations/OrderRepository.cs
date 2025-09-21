using Dapper;
using Npgsql;
using Project.Domain.Models;
using Project.Dal.Repositories.Interfaces;
using System.Data;

namespace Project.Dal.Repositories.Implementations;

// Комбінований ADO.NET + Dapper
public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Dapper для читання
    public async Task<Order> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Order>(
            "SELECT * FROM order_technique WHERE order_id = @id",
            new { id });
    }

    // Dapper для читання
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Order>(
            "SELECT * FROM order_technique ORDER BY order_id");
    }

    // Dapper для читання
    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Order>(
            "SELECT * FROM order_technique WHERE customer_id = @customerId ORDER BY order_date DESC",
            new { customerId });
    }

    // Dapper для читання
    public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Order>(
            "SELECT * FROM order_technique WHERE status = @status ORDER BY order_date DESC",
            new { status });
    }

    // ADO.NET для запису
    public async Task AddAsync(Order order)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            @"INSERT INTO order_technique (customer_id, status, total_amount, order_date, updated_at)
              VALUES (@customer_id, @status, @total_amount, @order_date, @updated_at)
              RETURNING order_id",
            connection);
        
        command.Parameters.AddWithValue("customer_id", order.customer_id);
        command.Parameters.AddWithValue("status", order.status ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("total_amount", order.total_amount);
        command.Parameters.AddWithValue("order_date", order.order_date);
        command.Parameters.AddWithValue("updated_at", order.updated_at);
        
        order.order_id = (int)await command.ExecuteScalarAsync();
    }

    // ADO.NET для запису
    public async Task UpdateAsync(Order order)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            @"UPDATE order_technique SET 
                customer_id = @customer_id,
                status = @status, 
                total_amount = @total_amount,
                order_date = @order_date,
                updated_at = @updated_at
              WHERE order_id = @order_id",
            connection);
        
        command.Parameters.AddWithValue("customer_id", order.customer_id);
        command.Parameters.AddWithValue("status", order.status ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("total_amount", order.total_amount);
        command.Parameters.AddWithValue("order_date", order.order_date);
        command.Parameters.AddWithValue("updated_at", order.updated_at);
        command.Parameters.AddWithValue("order_id", order.order_id);
        
        await command.ExecuteNonQueryAsync();
    }

    // Dapper для запису (проста операція)
    public async Task UpdateStatusAsync(int orderId, string status)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"UPDATE order_technique SET 
                    status = @status,
                    updated_at = @updated_at
                    WHERE order_id = @order_id";
        
        await connection.ExecuteAsync(sql, new 
        { 
            order_id = orderId, 
            status = status,
            updated_at = DateTime.UtcNow
        });
    }

    // ADO.NET для видалення
    public async Task DeleteAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "DELETE FROM order_technique WHERE order_id = @id",
            connection);
        
        command.Parameters.AddWithValue("id", id);
        await command.ExecuteNonQueryAsync();
    }

    // Dapper для агрегаційних функцій
    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<decimal>(
            "SELECT COALESCE(SUM(total_amount), 0) FROM order_technique WHERE order_date BETWEEN @startDate AND @endDate AND status = 'completed'",
            new { startDate, endDate });
    }

    // ADO.NET для складних операцій
    public async Task<int> BulkUpdateStatusAsync(IEnumerable<int> orderIds, string status)
    {
        if (orderIds == null || !orderIds.Any())
            return 0;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var idList = string.Join(",", orderIds);
        var updatedAt = DateTime.UtcNow;
        
        await using var command = new NpgsqlCommand(
            $@"UPDATE order_technique SET 
                status = @status,
                updated_at = @updated_at
              WHERE order_id IN ({idList})",
            connection);
        
        command.Parameters.AddWithValue("status", status);
        command.Parameters.AddWithValue("updated_at", updatedAt);
        
        return await command.ExecuteNonQueryAsync();
    }

    // Dapper для отримання статистики
    public async Task<dynamic> GetOrderStatisticsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<dynamic>(
            @"SELECT 
                COUNT(*) as total_orders,
                COUNT(CASE WHEN status = 'completed' THEN 1 END) as completed_orders,
                COUNT(CASE WHEN status = 'pending' THEN 1 END) as pending_orders,
                COUNT(CASE WHEN status = 'cancelled' THEN 1 END) as cancelled_orders,
                COALESCE(SUM(total_amount), 0) as total_revenue
              FROM order_technique");
    }

    // Комбінований підхід - Dapper для читання з ADO.NET для додаткових операцій
    public async Task<bool> ValidateOrderBeforeUpdateAsync(int orderId)
    {
        // Dapper для читання
        await using var connection = new NpgsqlConnection(_connectionString);
        var order = await connection.QueryFirstOrDefaultAsync<Order>(
            "SELECT * FROM order_technique WHERE order_id = @orderId FOR UPDATE",
            new { orderId });

        if (order == null)
            return false;

        // Додаткова логіка перевірки з ADO.NET
        await connection.OpenAsync();
        await using var checkCommand = new NpgsqlCommand(
            "SELECT COUNT(*) FROM payment WHERE order_id = @orderId AND payment_status = 'completed'",
            connection);
        
        checkCommand.Parameters.AddWithValue("orderId", orderId);
        var paymentCount = (long)await checkCommand.ExecuteScalarAsync();
        
        return paymentCount > 0;
    }
}