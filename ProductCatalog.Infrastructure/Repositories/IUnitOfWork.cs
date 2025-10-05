namespace ProductCatalog.Infrastructure.Repositories;


public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<int> CompleteAsync();
}