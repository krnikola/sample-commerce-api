using SampleCommerce.Infrastructure.External.DummyJson;
using SampleCommerce.Infrastructure.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SampleCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoritesStore _favoritesStore;
    private readonly IProductClient _productClient;

    public FavoritesController(FavoritesStore favoritesStore, IProductClient productClient)
    {
        _favoritesStore = favoritesStore;
        _productClient = productClient;
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> Add(int productId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var p = await _productClient.GetProductAsync(productId);
            if (p == null) return NotFound("Product not found.");
        }
        catch
        {
            return NotFound("Product not found.");
        }

        var added = await _favoritesStore.AddAsync(userId, productId);
        return Ok(added ? "Added to favorites." : "Already in favorites.");
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var removed = await _favoritesStore.RemoveAsync(userId, productId);
        return removed ? Ok("Removed from favorites.") : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var productIds = await _favoritesStore.GetProductIdsAsync(userId);

        var tasks = productIds.Select(async id =>
        {
            try { return await _productClient.GetProductAsync(id); }
            catch { return null; }
        });

        var results = await Task.WhenAll(tasks);
        return Ok(results.Where(p => p != null));
    }
}
