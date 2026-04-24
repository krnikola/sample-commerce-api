using Microsoft.Extensions.Caching.Memory;

namespace SampleCommerce.Infrastructure.External.DummyJson;

public class CachedProductClient : IProductClient
{
    private readonly DummyJsonProductClient _inner;
    private readonly IMemoryCache _cache;

    public CachedProductClient(DummyJsonProductClient inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public Task<ProductDto?> GetProductAsync(int id)
    {
        return _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await _inner.GetProductAsync(id);
        })!;
    }

    public Task<ProductsResponse?> GetProductsAsync(
        int skip = 0,
        int limit = 30,
        string? sortBy = null,
        string? sortDir = "asc")
    {
        var key = $"products:{skip}:{limit}:{sortBy}:{sortDir}";

        return _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _inner.GetProductsAsync(skip, limit, sortBy, sortDir);
        })!;
    }
}