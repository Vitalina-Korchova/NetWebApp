using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Data.Seed;

public static class SeedingExtensions
{
    public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            Console.WriteLine("Starting database seeding...");
            
            var context = services.GetRequiredService<ApplicationDbContext>();
            await DataSeeder.SeedAsync(context);
            
            Console.WriteLine("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database seeding failed: {ex.Message}");
        }
    }
}