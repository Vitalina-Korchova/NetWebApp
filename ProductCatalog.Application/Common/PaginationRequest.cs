namespace ProductCatalog.Application.Common;

public class PaginationRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public string? SearchTerm { get; set; }
}

public class ProductFilterRequest : PaginationRequest
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? CategoryId { get; set; }
    public string? Brand { get; set; }
}