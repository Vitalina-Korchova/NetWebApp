using Npgsql;
using Project.Domain.Models;
using Project.Dal.Repositories.Interfaces;
using System.Data;

namespace Project.Dal.Repositories.Implementations;


//ADO.NET
public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "SELECT customer_id, name, phone FROM customer WHERE customer_id = @id",
            connection);
        
        command.Parameters.AddWithValue("id", id);
        
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Customer
            {
                customer_id = reader.GetInt32(0),
                name = reader.GetString(1),
                phone = reader.GetString(2)
            };
        }
        
        return null;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = new List<Customer>();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "SELECT customer_id, name, phone FROM customer ORDER BY customer_id",
            connection);
        
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            customers.Add(new Customer
            {
                customer_id = reader.GetInt32(0),
                name = reader.GetString(1),
                phone = reader.GetString(2)
            });
        }
        
        return customers;
    }

    public async Task AddAsync(Customer customer)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "INSERT INTO customer (name, phone) VALUES (@name, @phone) RETURNING customer_id",
            connection);
        
        command.Parameters.AddWithValue("name", customer.name ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("phone", customer.phone ?? (object)DBNull.Value);
        
        customer.customer_id = (int)await command.ExecuteScalarAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "UPDATE customer SET name = @name, phone = @phone WHERE customer_id = @customer_id",
            connection);
        
        command.Parameters.AddWithValue("name", customer.name ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("phone", customer.phone ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("customer_id", customer.customer_id);
        
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "DELETE FROM customer WHERE customer_id = @id",
            connection);
        
        command.Parameters.AddWithValue("id", id);
        
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Customer> GetByPhoneAsync(string phone)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "SELECT customer_id, name, phone FROM customer WHERE phone = @phone",
            connection);
        
        command.Parameters.AddWithValue("phone", phone ?? (object)DBNull.Value);
        
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Customer
            {
                customer_id = reader.GetInt32(0),
                name = reader.GetString(1),
                phone = reader.GetString(2)
            };
        }
        
        return null;
    }
}