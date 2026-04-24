using SampleCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SampleCommerce.Infrastructure.Services.Stores;

public class FavoritesStore
{
    private readonly AppDbContext _context;

    public FavoritesStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(int userId, int productId)
    {
        var exists = await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId);
        if (exists) return false;

        _context.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(int userId, int productId)
    {
        var fav = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        if (fav == null) return false;

        _context.Favorites.Remove(fav);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<int>> GetProductIdsAsync(int userId)
    {
        return _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.ProductId)
            .ToListAsync();
    }
}
