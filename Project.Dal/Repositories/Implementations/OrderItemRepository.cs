using Dapper;
using Npgsql;
using Project.Domain.Models;
using Project.Dal.Repositories.Interfaces;

namespace Project.Dal.Repositories.Implementations;


//Dapper
public class OrderItemRepository : IOrderItemRepository
{
    private readonly string _connectionString;

    public OrderItemRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<OrderItem> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<OrderItem>(
            "SELECT * FROM order_item WHERE order_item_id = @id",
            new { id });
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<OrderItem>(
            "SELECT * FROM order_item ORDER BY order_item_id");
    }

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<OrderItem>(
            "SELECT * FROM order_item WHERE order_id = @orderId ORDER BY order_item_id",
            new { orderId });
    }

    public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<OrderItem>(
            "SELECT * FROM order_item WHERE product_id = @productId ORDER BY order_id",
            new { productId });
    }

    public async Task AddAsync(OrderItem orderItem)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"INSERT INTO order_item (product_id, quantity, order_id)
                    VALUES (@product_id, @quantity, @order_id)
                    RETURNING order_item_id";
        
        orderItem.order_item_id = await connection.ExecuteScalarAsync<int>(sql, orderItem);
    }

    public async Task AddRangeAsync(IEnumerable<OrderItem> orderItems)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"INSERT INTO order_item (product_id, quantity, order_id)
                    VALUES (@product_id, @quantity, @order_id)";
        
        await connection.ExecuteAsync(sql, orderItems);
    }

    public async Task UpdateAsync(OrderItem orderItem)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var sql = @"UPDATE order_item SET 
                    product_id = @product_id,
                    quantity = @quantity,
                    order_id = @order_id
                    WHERE order_item_id = @order_item_id";
        
        await connection.ExecuteAsync(sql, orderItem);
    }

    public async Task UpdateQuantityAsync(int orderItemId, int quantity)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE order_items SET quantity = @quantity WHERE order_item_id = @orderItemId",
            new { orderItemId, quantity });
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "DELETE FROM order_items WHERE order_item_id = @id",
            new { id });
    }

    public async Task DeleteByOrderIdAsync(int orderId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "DELETE FROM order_items WHERE order_id = @orderId",
            new { orderId });
    }

    public async Task<int> GetTotalQuantityByProductAsync(int productId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COALESCE(SUM(quantity), 0) FROM order_items WHERE product_id = @productId",
            new { productId });
    }
}