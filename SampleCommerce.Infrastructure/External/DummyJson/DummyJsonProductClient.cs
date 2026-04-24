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
        // Ako nema sortiranja, zadrži jednostavan i jeftin put
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return await _http.GetFromJsonAsync<ProductsResponse>($"products?skip={skip}&limit={limit}");
        }

        // 1) Prvo dohvatimo mali payload da saznamo ukupan broj proizvoda
        var metadata = await _http.GetFromJsonAsync<ProductsResponse>("products?skip=0&limit=1");
        if (metadata?.Products is null)
            return metadata;

        var total = metadata.Total;

        // 2) Dohvatimo sve proizvode
        var allProductsResponse = await _http.GetFromJsonAsync<ProductsResponse>($"products?skip=0&limit={total}");
        if (allProductsResponse?.Products is null)
            return allProductsResponse;

        var products = allProductsResponse.Products;
        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        // 3) Globalno sortiranje
        products = sortBy.ToLower() switch
        {
            "title" => desc
                ? products.OrderByDescending(p => p.Title).ToList()
                : products.OrderBy(p => p.Title).ToList(),

            "price" => desc
                ? products.OrderByDescending(p => p.Price).ToList()
                : products.OrderBy(p => p.Price).ToList(),

            "rating" => desc
                ? products.OrderByDescending(p => p.Rating).ToList()
                : products.OrderBy(p => p.Rating).ToList(),

            "stock" => desc
                ? products.OrderByDescending(p => p.Stock).ToList()
                : products.OrderBy(p => p.Stock).ToList(),

            _ => products
        };

        // 4) Tek nakon sortiranja radimo paginaciju
        var pagedProducts = products
            .Skip(skip)
            .Take(limit)
            .ToList();

        return new ProductsResponse
        {
            Products = pagedProducts,
            Total = total,
            Skip = skip,
            Limit = limit
        };
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
