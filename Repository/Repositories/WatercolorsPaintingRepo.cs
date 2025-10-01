using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Repositories;

public class WatercolorsPaintingRepo : DataAccessObject<WatercolorsPainting>
{
    public new async Task<List<WatercolorsPainting>> GetAllAsync()
    {
        var items = await _context.WatercolorsPaintings.Include(i => i.Style).ToListAsync();
        return items;
    }

    public new async Task<WatercolorsPainting?> GetByIdAsync(string id)
    {
        var item = await _context.WatercolorsPaintings.Include(i => i.Style)
            .FirstOrDefaultAsync(t => t.PaintingId == id);
        return item;
    }

    public async Task<List<WatercolorsPainting>> Search(int? item1, string? item2)
    {

        var query = _context.WatercolorsPaintings.Include(i => i.Style).AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(item2))
        {
            query = query.Where(u => u.PaintingAuthor != null && u.PaintingAuthor.ToLower().Contains(item2.ToLower()));
        }

        if (item1.HasValue)
        {
            query = query.Where(u => u.PublishYear == item1.Value);
        }

        var results = await query.ToListAsync();
        return results;
    }
}