
namespace SampleCommerce.Infrastructure.External.DummyJson;

public interface IProductClient
{
    Task<ProductsResponse?> GetProductsAsync(
        int skip = 0,
        int limit = 30,
        string? sortBy = null,
        string? sortDir = "asc");

    Task<ProductDto?> GetProductAsync(int id);
}