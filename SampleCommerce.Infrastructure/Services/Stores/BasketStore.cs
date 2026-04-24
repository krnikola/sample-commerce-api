using SampleCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SampleCommerce.Infrastructure.Services.Stores;

public class BasketStore
{
    private readonly AppDbContext _context;

    public BasketStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddItemAsync(int userId, int productId, int quantity)
    {
        var existing = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        if (existing == null)
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Quantity += quantity;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> RemoveItemAsync(int userId, int productId)
    {
        var item = await _context.CartItems.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        if (item == null) return false;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<(int ProductId, int Quantity)>> GetItemsAsync(int userId)
    {
        var items = await _context.CartItems
            .Where(x => x.UserId == userId)
            .Select(x => new { x.ProductId, x.Quantity })
            .ToListAsync();

        return items.Select(x => (x.ProductId, x.Quantity)).ToList();
    }
}
