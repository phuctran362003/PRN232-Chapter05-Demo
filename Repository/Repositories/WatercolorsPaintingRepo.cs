using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Repositories;

public class WatercolorsPaintingRepo : DataAccessObject<WatercolorsPainting>
{
    public new async Task<List<WatercolorsPainting>> GetAllAsync()
    {
        Console.WriteLine("💾 REPOSITORY: Fetching all WatercolorsPaintings with included Style");
        var items = await _context.WatercolorsPaintings.Include(i => i.Style).ToListAsync();
        Console.WriteLine($"💾 REPOSITORY: Retrieved {items.Count} WatercolorsPaintings");
        return items;
    }

    public new async Task<WatercolorsPainting?> GetByIdAsync(string id)
    {
        Console.WriteLine($"💾 REPOSITORY: Fetching WatercolorsPainting with ID '{id}'");
        var item = await _context.WatercolorsPaintings.Include(i => i.Style)
            .FirstOrDefaultAsync(t => t.PaintingId == id);

        if (item != null)
            Console.WriteLine($"💾 REPOSITORY: Found WatercolorsPainting '{item.PaintingName}' with ID '{id}'");
        else
            Console.WriteLine($"💾 REPOSITORY: WatercolorsPainting with ID '{id}' not found");

        return item;
    }

    public async Task<List<WatercolorsPainting>> Search(int? item1, string? item2)
    {
        Console.WriteLine(
            $"💾 REPOSITORY: Searching for WatercolorsPaintings with PublishYear={item1}, Author='{item2}'");

        var query = _context.WatercolorsPaintings.Include(i => i.Style).AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(item2))
        {
            Console.WriteLine($"💾 REPOSITORY: Applying author filter: '{item2}'");
            query = query.Where(u => u.PaintingAuthor != null && u.PaintingAuthor.ToLower().Contains(item2.ToLower()));
        }

        if (item1.HasValue)
        {
            Console.WriteLine($"💾 REPOSITORY: Applying year filter: {item1.Value}");
            query = query.Where(u => u.PublishYear == item1.Value);
        }

        var results = await query.ToListAsync();
        Console.WriteLine($"💾 REPOSITORY: Search returned {results.Count} results");
        return results;
    }
}