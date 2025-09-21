using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using Project.Api.Services;
using Project.Dal.UnitOfWork;
using Project.Dal.Repositories.Interfaces;
using Project.Dal.Repositories.Implementations;
using Project.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Налаштування логування (Serilog)
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Додаємо сервіси
builder.Services.AddControllers();

// Додаємо Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Order Management API", 
        Version = "v1",
        Description = "API for managing orders, customers, payments and order items"
    });
});

// Реєстрація Unit of Work
builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new UnitOfWork(connectionString);
});

// ВИПРАВЛЕНА реєстрація репозиторіїв (видалено перші 4 рядки)
builder.Services.AddScoped<ICustomerRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new CustomerRepository(connectionString);
});

builder.Services.AddScoped<IOrderRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new OrderRepository(connectionString);
});

builder.Services.AddScoped<IOrderItemRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new OrderItemRepository(connectionString);
});

builder.Services.AddScoped<IPaymentRepository>(provider =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new PaymentRepository(connectionString);
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Реєстрація фабрики підключень
builder.Services.AddTransient(provider => 
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");
    return new NpgsqlConnection(connectionString);
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1");
        c.RoutePrefix = "swagger"; // Доступ до Swagger UI: /swagger
    });
}

// Глобальна обробка помилок
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Тест підключення до PostgreSQL
app.MapGet("/test-db", async (IConfiguration config) =>
{
    try
    {
        await using var connection = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        return Results.Ok("PostgreSQL connection successful!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// Health check endpoint
app.MapGet("/health", () => 
{
    return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
});

// API info endpoint
app.MapGet("/", () => 
{
    return Results.Ok(new 
    { 
        message = "Order Management API", 
        version = "v1",
        endpoints = new 
        {
            customers = "/api/customers",
            orders = "/api/orders",
            payments = "/api/payments",
            swagger = "/swagger"
        }
    });
});

app.Run();