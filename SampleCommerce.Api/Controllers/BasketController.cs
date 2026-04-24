using SampleCommerce.Infrastructure.Services.Stores;
using SampleCommerce.Api.Features.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SampleCommerce.Infrastructure.External.DummyJson;

namespace SampleCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly BasketStore _basketStore;
    private readonly IProductClient _productClient;

    public BasketController(BasketStore basketStore, IProductClient productClient)
    {
        _basketStore = basketStore;
        _productClient = productClient;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToBasketRequest request)
    {
        if (request.Quantity <= 0) return BadRequest("Quantity must be > 0.");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var product = await _productClient.GetProductAsync(request.ProductId);
            if (product is null)
                return NotFound("Product not found.");
        }
        catch
        {
            return NotFound("Product not found.");
        }

        await _basketStore.AddItemAsync(userId, request.ProductId, request.Quantity);
        return Ok("Item added.");
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var removed = await _basketStore.RemoveItemAsync(userId, productId);
        return removed ? Ok("Item removed.") : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetBasket()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var items = await _basketStore.GetItemsAsync(userId);

        var tasks = items.Select(async i =>
        {
            try
            {
                var product = await _productClient.GetProductAsync(i.ProductId);
                return new { product, quantity = i.Quantity };
            }
            catch
            {
                return new { product = (ProductDto?)null, quantity = i.Quantity };
            }
        });

        var result = await Task.WhenAll(tasks);

        return Ok(result.Where(x => x.product != null));
    }
}
