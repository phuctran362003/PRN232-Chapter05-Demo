using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Repositories;

public class WatercolorsPaintingRepo : DataAccessObject<WatercolorsPainting>
{
    public async Task<List<WatercolorsPainting>> GetAllAsync()
    {
        var items = await _context.WatercolorsPaintings.Include(i => i.Style).ToListAsync();
        return items;
    }

    public async Task<WatercolorsPainting> GetByIdAsync(string id)
    {
        var item = await _context.WatercolorsPaintings.Include(i => i.Style)
            .FirstOrDefaultAsync(t => t.PaintingId == id);
        if (item == null) _context.Entry(item).State = EntityState.Detached;
        return item;
    }

    public async Task<List<WatercolorsPainting>> Search(int? item1, string? item2)
    {
        return await _context.WatercolorsPaintings
            .Include(i => i.Style)
            .Where(u => (string.IsNullOrEmpty(item2) || u.PaintingAuthor.ToLower().Contains(item2.ToLower()))
                        && (!item1.HasValue || u.PublishYear == item1.Value))
            .ToListAsync();
    }
}