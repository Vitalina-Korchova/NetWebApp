using Npgsql;
using Project.Dal.Repositories.Interfaces;
using Project.Dal.Repositories.Implementations;
using System.Data;

namespace Project.Dal.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;
    
    public ICustomerRepository Customers { get; private set; }
    public IOrderRepository Orders { get; private set; }
    public IOrderItemRepository OrderItems { get; private set; }
    public IPaymentRepository Payments { get; private set; }

    public UnitOfWork(string connectionString)
    {
        _connectionString = connectionString;
        
        // Ініціалізація репозиторіїв
        Customers = new CustomerRepository(connectionString);
        Orders = new OrderRepository(connectionString);
        OrderItems = new OrderItemRepository(connectionString);
        Payments = new PaymentRepository(connectionString);
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_connection != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _connection = new NpgsqlConnection(_connectionString);
        await _connection.OpenAsync();
        _transaction = await _connection.BeginTransactionAsync(isolationLevel);

        
    }

 

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to commit");
        }

        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            await DisposeAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to rollback");
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await DisposeAsync();
        }
    }

    public Task SaveChangesAsync()
    {
      
        return Task.CompletedTask;
    }

    //правильне закриття з'єднання
    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
        
        // Очищає посилання
        _transaction = null;
        _connection = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        
        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
    
    public NpgsqlConnection GetConnection()
    {
        return _connection;
    }
    
    public NpgsqlTransaction GetTransaction()
    {
        return _transaction;
    }
}