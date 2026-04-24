using System.Globalization;
using System.Net.Http.Json;

namespace SampleCommerce.Infrastructure.External.DummyJson;

public class DummyJsonProductClient : IProductClient
{
    private readonly HttpClient _http;

    public DummyJsonProductClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProductsResponse?> GetProductsAsync(
        int skip = 0,
        int limit = 30,
        string? sortBy = null,
        string? sortDir = "asc")
    {

        var result = await _http.GetFromJsonAsync<ProductsResponse>($"products?skip={skip}&limit={limit}");

        if (result?.Products is null || string.IsNullOrWhiteSpace(sortBy))
            return result;

        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        result.Products = sortBy.ToLower() switch
        {
            "title" => desc
                ? result.Products.OrderByDescending(p => p.Title).ToList()
                : result.Products.OrderBy(p => p.Title).ToList(),

            "price" => desc
                ? result.Products.OrderByDescending(p => p.Price).ToList()
                : result.Products.OrderBy(p => p.Price).ToList(),

            "rating" => desc
                ? result.Products.OrderByDescending(p => p.Rating).ToList()
                : result.Products.OrderBy(p => p.Rating).ToList(),

            "stock" => desc
                ? result.Products.OrderByDescending(p => p.Stock).ToList()
                : result.Products.OrderBy(p => p.Stock).ToList(),

            _ => result.Products
        };

        return result;
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"products/{id}");
    }
}

// DTO koji odgovara DummyJSON strukturi
public class ProductsResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Rating { get; set; }
    public int Stock { get; set; }
    public string Brand { get; set; } = "";
    public string Category { get; set; } = "";
    public string Thumbnail { get; set; } = "";
}
