using SampleCommerce.Infrastructure.External.DummyJson;
using Microsoft.AspNetCore.Mvc;

namespace SampleCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductClient _client;

    public ProductController(IProductClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int skip = 0,
        [FromQuery] int limit = 30,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = "asc")
    {
        var result = await _client.GetProductsAsync(skip, limit, sortBy, sortDir);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _client.GetProductAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}